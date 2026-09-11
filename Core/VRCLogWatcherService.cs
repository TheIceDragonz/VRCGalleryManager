using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace VRCGalleryManager.Core
{
    public class PhotoPopupModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FilePath { get; set; } = "";
        public string FileName { get; set; } = "";
        public string Resolution { get; set; } = "";
        public string? WorldName { get; set; }
        public string? WorldId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string? ThumbnailBase64 { get; set; }
        public bool IsLeaving { get; set; }
        public bool IsCollapsed { get; set; }
        public CancellationTokenSource? DismissCts { get; set; }
        public List<QrCodeItem> DetectedQrCodes { get; set; } = new();
    }

    public class VRCLogWatcherService : IDisposable
    {
        private static readonly Regex ScreenshotRegex = new(
            @"\[VRC Camera\]\s+Took screenshot to:\s*(?<path>.+?\.(?:png|jpg|jpeg))",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex ResolutionRegex = new(
            @"_(?<res>\d+x\d+)\.(?:png|jpg|jpeg)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly List<PhotoPopupModel> _activePopups = new();
        private readonly HashSet<string> _recentlyProcessedPaths = new();
        private readonly object _lock = new();

        private CancellationTokenSource? _watcherCts;
        private Task? _watcherTask;
        private bool _disposed;

        public IReadOnlyList<PhotoPopupModel> ActivePopups
        {
            get
            {
                lock (_lock)
                {
                    return _activePopups.ToList();
                }
            }
        }

        public event Action? OnPopupsChanged;
        public event Action<PhotoPopupModel>? OnPhotoCaptured;
        public event Action<string>? OnLogLine;

        public bool IsEnabled
        {
            get => Config.Get("EnableScreenshotPopup", "true") == "true";
            set => Config.Set("EnableScreenshotPopup", value ? "true" : "false");
        }

        private readonly QrCodeService _qrCodeService;

        public VRCLogWatcherService(QrCodeService? qrCodeService = null)
        {
            _qrCodeService = qrCodeService ?? new QrCodeService();
            StartWatcher();
        }

        public void StartWatcher()
        {
            if (_watcherTask != null && !_watcherTask.IsCompleted) return;

            _watcherCts = new CancellationTokenSource();
            _watcherTask = Task.Run(() => WatchLogsAsync(_watcherCts.Token));
        }

        public void StopWatcher()
        {
            try
            {
                _watcherCts?.Cancel();
                _watcherCts?.Dispose();
                _watcherCts = null;
            }
            catch { }
        }

        public static string GetVRChatLogDirectory()
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(userProfile, "AppData", "LocalLow", "VRChat", "VRChat");
        }

        private async Task WatchLogsAsync(CancellationToken token)
        {
            string logDir = GetVRChatLogDirectory();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (!Directory.Exists(logDir))
                    {
                        await Task.Delay(3000, token);
                        continue;
                    }

                    var logFiles = Directory.GetFiles(logDir, "output_log_*.txt");
                    if (logFiles.Length == 0)
                    {
                        await Task.Delay(2000, token);
                        continue;
                    }

                    // Pick the newest log file
                    string currentLog = logFiles.OrderByDescending(File.GetLastWriteTimeUtc).First();

                    using var fs = new FileStream(currentLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var reader = new StreamReader(fs);

                    // Start from the end of the file so we only capture new live events
                    fs.Seek(0, SeekOrigin.End);

                    var lastLogCheck = DateTime.UtcNow;

                    while (!token.IsCancellationRequested)
                    {
                        string? line = await reader.ReadLineAsync();

                        if (line != null)
                        {
                            _ = ProcessPhotoLogLineAsync(line, token);
                            try
                            {
                                OnLogLine?.Invoke(line);
                            }
                            catch { }
                        }
                        else
                        {
                            // Check periodically if a newer log file was created (e.g. VRChat restarted)
                            if ((DateTime.UtcNow - lastLogCheck).TotalSeconds > 2)
                            {
                                lastLogCheck = DateTime.UtcNow;
                                var latestFile = Directory.GetFiles(logDir, "output_log_*.txt")
                                    .OrderByDescending(File.GetLastWriteTimeUtc)
                                    .FirstOrDefault();

                                if (!string.IsNullOrEmpty(latestFile) &&
                                    !string.Equals(latestFile, currentLog, StringComparison.OrdinalIgnoreCase))
                                {
                                    // A newer log file appeared! Break inner loop to tail the new file
                                    break;
                                }
                            }

                            await Task.Delay(300, token);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    await Task.Delay(2000, token);
                }
            }
        }

        private async Task ProcessPhotoLogLineAsync(string line, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(line) || !line.Contains("[VRC Camera] Took screenshot to:", StringComparison.OrdinalIgnoreCase))
                return;

            var match = ScreenshotRegex.Match(line);
            if (!match.Success) return;

            string rawPath = match.Groups["path"].Value.Trim();
            if (string.IsNullOrEmpty(rawPath)) return;

            // Normalize path
            string normalizedPath = rawPath.Replace('/', '\\');

            lock (_lock)
            {
                if (_recentlyProcessedPaths.Contains(normalizedPath))
                    return;

                _recentlyProcessedPaths.Add(normalizedPath);

                // Trim recently processed set if it grows large
                if (_recentlyProcessedPaths.Count > 100)
                {
                    _recentlyProcessedPaths.Clear();
                    _recentlyProcessedPaths.Add(normalizedPath);
                }
            }

            if (!IsEnabled) return;

            // Wait until file is written to disk and accessible
            bool isReady = await WaitForFileReadyAsync(normalizedPath, token);
            if (!isReady) return;

            string fileName = Path.GetFileName(normalizedPath);
            string resolution = "";

            var resMatch = ResolutionRegex.Match(fileName);
            if (resMatch.Success)
            {
                resolution = resMatch.Groups["res"].Value;
            }

            string? worldName = null;
            string? worldId = null;

            try
            {
                var vrcxData = MetaDataImageReader.ExtractVrcxData(normalizedPath);
                if (vrcxData?.World != null)
                {
                    worldName = vrcxData.World.Name;
                    worldId = vrcxData.World.Id;
                }
            }
            catch { }

            string? thumbBase64 = null;
            try
            {
                thumbBase64 = await MakeThumbnailBase64Async(normalizedPath, 360, token);
            }
            catch { }

            List<QrCodeItem> detectedQrs = new();
            try
            {
                detectedQrs = await _qrCodeService.ScanFileAsync(normalizedPath, token);
            }
            catch { }

            var popup = new PhotoPopupModel
            {
                FilePath = normalizedPath,
                FileName = fileName,
                Resolution = resolution,
                WorldName = worldName,
                WorldId = worldId,
                Timestamp = DateTime.Now,
                ThumbnailBase64 = thumbBase64,
                DetectedQrCodes = detectedQrs
            };

            lock (_lock)
            {
                // Keep max 3 popups stacked on screen
                while (_activePopups.Count >= 3)
                {
                    var oldest = _activePopups[0];
                    oldest.DismissCts?.Cancel();
                    _activePopups.RemoveAt(0);
                }

                _activePopups.Add(popup);
            }

            OnPopupsChanged?.Invoke();
            OnPhotoCaptured?.Invoke(popup);

            // Start auto dismiss after 10 seconds
            ScheduleAutoDismiss(popup, 10000);
        }

        private void ScheduleAutoDismiss(PhotoPopupModel popup, int durationMs)
        {
            popup.DismissCts?.Cancel();
            var cts = new CancellationTokenSource();
            popup.DismissCts = cts;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(durationMs, cts.Token);
                    Collapse(popup);
                }
                catch (OperationCanceledException) { }
            });
        }

        public void Collapse(PhotoPopupModel popup)
        {
            lock (_lock)
            {
                if (_activePopups.Contains(popup))
                {
                    popup.IsCollapsed = true;
                    popup.DismissCts?.Cancel();
                    popup.DismissCts = null;
                }
            }
            OnPopupsChanged?.Invoke();
        }

        public void Expand(PhotoPopupModel popup)
        {
            lock (_lock)
            {
                if (_activePopups.Contains(popup))
                {
                    popup.IsCollapsed = false;
                }
            }
            OnPopupsChanged?.Invoke();
        }

        public void ExpandAll()
        {
            lock (_lock)
            {
                foreach (var p in _activePopups)
                {
                    p.IsCollapsed = false;
                }
            }
            OnPopupsChanged?.Invoke();
        }

        public void CollapseAll()
        {
            lock (_lock)
            {
                foreach (var p in _activePopups)
                {
                    p.IsCollapsed = true;
                    p.DismissCts?.Cancel();
                    p.DismissCts = null;
                }
            }
            OnPopupsChanged?.Invoke();
        }

        public async Task DismissAsync(PhotoPopupModel popup)
        {
            bool wasPresent = false;
            lock (_lock)
            {
                if (_activePopups.Contains(popup))
                {
                    popup.IsLeaving = true;
                    wasPresent = true;
                }
            }

            if (!wasPresent) return;

            OnPopupsChanged?.Invoke();

            // Wait for slide-out CSS animation (350ms)
            await Task.Delay(350);

            lock (_lock)
            {
                popup.DismissCts?.Cancel();
                _activePopups.Remove(popup);
            }

            OnPopupsChanged?.Invoke();
        }

        public async Task DismissAllAsync()
        {
            List<PhotoPopupModel> toDismiss;
            lock (_lock)
            {
                toDismiss = _activePopups.ToList();
                foreach (var p in toDismiss)
                {
                    p.IsLeaving = true;
                }
            }

            OnPopupsChanged?.Invoke();
            await Task.Delay(350);

            lock (_lock)
            {
                foreach (var p in toDismiss)
                {
                    p.DismissCts?.Cancel();
                    _activePopups.Remove(p);
                }
            }

            OnPopupsChanged?.Invoke();
        }

        public void PauseAutoDismiss(PhotoPopupModel popup)
        {
            popup.DismissCts?.Cancel();
            popup.DismissCts = null;
        }

        public void ResumeAutoDismiss(PhotoPopupModel popup, int remainingMs = 4000)
        {
            ScheduleAutoDismiss(popup, remainingMs);
        }

        private static async Task<bool> WaitForFileReadyAsync(string path, CancellationToken token, int maxRetries = 20, int delayMs = 150)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                if (token.IsCancellationRequested) return false;
                try
                {
                    if (File.Exists(path))
                    {
                        var info = new FileInfo(path);
                        if (info.Length > 0)
                        {
                            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            if (stream.Length > 0)
                                return true;
                        }
                    }
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }

                await Task.Delay(delayMs, token);
            }

            return File.Exists(path);
        }

        private static async Task<string?> MakeThumbnailBase64Async(string path, int maxSize, CancellationToken token)
        {
            if (token.IsCancellationRequested || !File.Exists(path)) return null;

            return await Task.Run(() =>
            {
                try
                {
                    using var image = Image.Load<Rgba32>(path);
                    if (image.Width <= maxSize && image.Height <= maxSize)
                    {
                        using var ms = new MemoryStream();
                        image.SaveAsJpeg(ms);
                        return $"data:image/jpeg;base64,{Convert.ToBase64String(ms.ToArray())}";
                    }

                    var ratio = (double)maxSize / Math.Max(image.Width, image.Height);
                    var width = (int)(image.Width * ratio);
                    var height = (int)(image.Height * ratio);

                    using var thumb = image.Clone(x => x.Resize(width, height, KnownResamplers.Bicubic));
                    using var ms2 = new MemoryStream();
                    thumb.SaveAsJpeg(ms2);
                    return $"data:image/jpeg;base64,{Convert.ToBase64String(ms2.ToArray())}";
                }
                catch
                {
                    return null;
                }
            }, token);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            StopWatcher();
            lock (_lock)
            {
                foreach (var popup in _activePopups)
                {
                    popup.DismissCts?.Cancel();
                }
                _activePopups.Clear();
            }
        }
    }
}
