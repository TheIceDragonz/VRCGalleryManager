using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using VRCGalleryManager.Core.Helpers;
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
        public string? InstanceId { get; set; }
        public string? AuthorDisplayName { get; set; }
        public string? AuthorId { get; set; }
        public List<MetaDataImageReader.PlayerInfo> Players { get; set; } = new();
        public int PlayerCount => Players?.Count ?? 0;
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

        // State tracking
        public string CurrentWorldName { get; private set; } = "";
        public string CurrentWorldId { get; private set; } = "";
        public string CurrentInstanceId { get; private set; } = "";
        private readonly ConcurrentDictionary<string, string> _currentPlayers = new(StringComparer.OrdinalIgnoreCase);
        public IReadOnlyDictionary<string, string> CurrentPlayers => _currentPlayers;
        public string LocalUserDisplayName { get; private set; } = "";
        public string LocalUserId { get; private set; } = "";

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

        public bool EnableMetadataInjection
        {
            get => Config.Get("EnableMetadataInjection", "true") == "true";
            set => Config.Set("EnableMetadataInjection", value ? "true" : "false");
        }

        private readonly QrCodeService _qrCodeService;

        public VRCLogWatcherService(QrCodeService? qrCodeService = null)
        {
            _qrCodeService = qrCodeService ?? new QrCodeService();
            StartWatcher();
        }

        public void StartWatcher()
        {
            if (OperatingSystem.IsAndroid()) return;
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

                    // Initial catch-up: scan recent log content so current world and players are known immediately
                    if (fs.Length > 0)
                    {
                        long catchupStart = Math.Max(0, fs.Length - 512 * 1024);
                        fs.Seek(catchupStart, SeekOrigin.Begin);
                        if (catchupStart > 0) reader.ReadLine(); // discard potential partial line
                        string? catchupLine;
                        while ((catchupLine = await reader.ReadLineAsync()) != null)
                        {
                            ProcessGeneralLogLine(catchupLine);
                        }
                    }

                    // Start from the end of the file so we only capture new live events
                    fs.Seek(0, SeekOrigin.End);

                    var lastLogCheck = DateTime.UtcNow;

                    while (!token.IsCancellationRequested)
                    {
                        string? line = await reader.ReadLineAsync();

                        if (line != null)
                        {
                            ProcessGeneralLogLine(line);
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
            string? instanceId = null;
            string? authorDisplayName = null;
            string? authorId = null;
            List<MetaDataImageReader.PlayerInfo> players = new();

            try
            {
                var photoMetadata = MetaDataImageReader.ExtractPhotoMetadata(normalizedPath);

                // If not present and metadata injection is enabled, inject it automatically!
                if (photoMetadata == null && EnableMetadataInjection && normalizedPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    // If VRCX is running, give it time to write metadata first to prevent duplicate chunks
                    bool isVrcxActive = IsVrcxRunning();
                    int maxAttempts = isVrcxActive ? 15 : 2;

                    for (int attempt = 0; attempt < maxAttempts; attempt++)
                    {
                        await Task.Delay(100, token);
                        photoMetadata = MetaDataImageReader.ExtractPhotoMetadata(normalizedPath);
                        if (photoMetadata != null) break;
                    }

                    if (photoMetadata == null)
                    {
                        string authId = LocalUserId;
                        string authName = LocalUserDisplayName;

                        var authUser = VRCAuth.Instance()?.CurrentUser;
                        if (authUser != null)
                        {
                            if (string.IsNullOrEmpty(authId)) authId = authUser.Id ?? "";
                            if (string.IsNullOrEmpty(authName)) authName = authUser.DisplayName ?? "";
                        }

                        if (string.IsNullOrEmpty(authName)) authName = "VRChat User";

                        var currentPlayersSnapshot = _currentPlayers.Select(p => new MetaDataImageReader.PlayerInfo
                        {
                            Id = p.Key.StartsWith("usr_", StringComparison.OrdinalIgnoreCase) ? p.Key : "",
                            DisplayName = p.Value
                        }).ToList();

                        photoMetadata = new MetaDataImageReader.PhotoMetadata
                        {
                            Application = "VRCGalleryManager",
                            Version = 1,
                            Author = new MetaDataImageReader.AuthorInfo
                            {
                                Id = authId,
                                DisplayName = authName
                            },
                            World = new MetaDataImageReader.WorldInfo
                            {
                                Name = !string.IsNullOrEmpty(CurrentWorldName) ? CurrentWorldName : "Unknown World",
                                Id = CurrentWorldId,
                                InstanceId = CurrentInstanceId
                            },
                            Players = currentPlayersSnapshot
                        };

                        try
                        {
                            bool injected = PngMetadataWriter.InjectPhotoMetadata(normalizedPath, photoMetadata);
                            if (injected)
                            {
                                System.Diagnostics.Debug.WriteLine($"[VRCLogWatcherService] Injected metadata into {fileName} ({currentPlayersSnapshot.Count} players, world: {photoMetadata.World.Name})");
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"[VRCLogWatcherService] Failed to inject metadata: {ex.Message}");
                        }
                    }
                }

                if (photoMetadata != null)
                {
                    worldName = photoMetadata.World?.Name;
                    worldId = photoMetadata.World?.Id;
                    instanceId = photoMetadata.World?.InstanceId;
                    authorDisplayName = photoMetadata.Author?.DisplayName;
                    authorId = photoMetadata.Author?.Id;
                    players = photoMetadata.Players ?? new();
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
                InstanceId = instanceId,
                AuthorDisplayName = authorDisplayName,
                AuthorId = authorId,
                Players = players,
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

        private void ProcessGeneralLogLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return;

            // Room / World name: [Behaviour] Entering Room: <WorldName>
            if (line.Contains("[Behaviour] Entering Room: ", StringComparison.Ordinal))
            {
                int idx = line.LastIndexOf("] Entering Room: ", StringComparison.Ordinal);
                if (idx >= 0)
                {
                    CurrentWorldName = line.Substring(idx + 17).Trim();
                    _currentPlayers.Clear();
                }
                return;
            }

            // Instance / Location: [Behaviour] Joining wrld_...
            if (line.Contains("[Behaviour] Joining ", StringComparison.Ordinal) &&
                !line.Contains("] Joining or Creating Room:", StringComparison.Ordinal) &&
                !line.Contains("] Joining friend:", StringComparison.Ordinal))
            {
                int idx = line.LastIndexOf("] Joining ", StringComparison.Ordinal);
                if (idx >= 0)
                {
                    string loc = line.Substring(idx + 10).Trim();
                    CurrentInstanceId = loc;
                    int colon = loc.IndexOf(':');
                    CurrentWorldId = colon > 0 ? loc.Substring(0, colon) : loc;
                }
                return;
            }

            // Local player initialized: [Behaviour] Initialized PlayerAPI "<DisplayName>" is local
            if (line.Contains("Initialized PlayerAPI \"", StringComparison.Ordinal) &&
                line.Contains("\" is local", StringComparison.Ordinal))
            {
                int start = line.IndexOf("Initialized PlayerAPI \"", StringComparison.Ordinal) + 23;
                int end = line.IndexOf("\" is local", start, StringComparison.Ordinal);
                if (start >= 23 && end > start)
                {
                    LocalUserDisplayName = line.Substring(start, end - start).Trim();
                }
                return;
            }

            // Player joined: [Behaviour] OnPlayerJoined <DisplayName> (<UserId>)
            if (line.Contains("[Behaviour] OnPlayerJoined ", StringComparison.Ordinal) &&
                !line.Contains("] OnPlayerJoined:", StringComparison.Ordinal))
            {
                int idx = line.LastIndexOf("] OnPlayerJoined ", StringComparison.Ordinal);
                if (idx >= 0)
                {
                    string rawInfo = line.Substring(idx + 17).Trim();
                    var (pName, pId) = ParsePlayerInfo(rawInfo);
                    if (!string.IsNullOrEmpty(pName))
                    {
                        string key = !string.IsNullOrEmpty(pId) ? pId : pName;
                        _currentPlayers[key] = pName;

                        if (!string.IsNullOrEmpty(LocalUserDisplayName) &&
                            string.Equals(pName, LocalUserDisplayName, StringComparison.OrdinalIgnoreCase) &&
                            !string.IsNullOrEmpty(pId))
                        {
                            LocalUserId = pId;
                        }
                    }
                }
                return;
            }

            // Player left: [Behaviour] OnPlayerLeft <DisplayName> (<UserId>)
            if (line.Contains("[Behaviour] OnPlayerLeft ", StringComparison.Ordinal) &&
                !line.Contains("] OnPlayerLeftRoom", StringComparison.Ordinal) &&
                !line.Contains("] OnPlayerLeft:", StringComparison.Ordinal))
            {
                int idx = line.LastIndexOf("] OnPlayerLeft ", StringComparison.Ordinal);
                if (idx >= 0)
                {
                    string rawInfo = line.Substring(idx + 15).Trim();
                    var (pName, pId) = ParsePlayerInfo(rawInfo);
                    if (!string.IsNullOrEmpty(pId))
                    {
                        _currentPlayers.TryRemove(pId, out _);
                    }
                    else if (!string.IsNullOrEmpty(pName))
                    {
                        foreach (var kvp in _currentPlayers)
                        {
                            if (string.Equals(kvp.Value, pName, StringComparison.OrdinalIgnoreCase))
                            {
                                _currentPlayers.TryRemove(kvp.Key, out _);
                                break;
                            }
                        }
                    }
                }
                return;
            }

            // Left room
            if (line.Contains("[Behaviour] OnLeftRoom", StringComparison.Ordinal))
            {
                _currentPlayers.Clear();
                return;
            }
        }

        private static (string Name, string Id) ParsePlayerInfo(string raw)
        {
            int paren = raw.LastIndexOf(" (usr_", StringComparison.Ordinal);
            if (paren > 0 && raw.EndsWith(")"))
            {
                string name = raw.Substring(0, paren).Trim();
                string id = raw.Substring(paren + 2, raw.Length - paren - 3).Trim();
                return (name, id);
            }
            return (raw.Trim(), string.Empty);
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

        private static bool IsVrcxRunning()
        {
            try
            {
                return System.Diagnostics.Process.GetProcessesByName("VRCX").Length > 0;
            }
            catch
            {
                return false;
            }
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
