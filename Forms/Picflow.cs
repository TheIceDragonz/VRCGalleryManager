using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Forms.Panels;
using static VRCGalleryManager.Core.ApiRequest;

namespace VRCGalleryManager.Forms
{
    public partial class Picflow : ApiConnectedForm
    {
        private CancellationTokenSource? streamingCancellationTokenSource;
        private Task? streamingTask;
        private readonly HashSet<string> allItems = new();

        // ✅ Regex aggiornata: cattura userId, username e stickerId
        private static readonly Regex StickerRegex = new Regex(
            @"User\s+(?<userId>usr_[a-f0-9\-]+)\s+\((?<username>[^)]+)\).*?sticker\s+(?<sticker>inv_[a-f0-9\-]+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        private readonly SynchronizationContext uiContext;

        public Picflow(VRCAuth auth)
        {
            InitializeComponent();
            InitApiRequest(auth);
            uiContext = SynchronizationContext.Current ?? new SynchronizationContext();
            this.Shown += (s, e) =>
            {
                if (picflowPanel.Controls.Count == 0)
                    PicflowList();
            };
        }

        private async void PicflowList()
        {
            ClearItems();
            if (streamPicFlow.Checked)
            {
                if (streamingTask == null || streamingTask.IsCompleted)
                {
                    streamingCancellationTokenSource = new CancellationTokenSource();
                    streamingTask = Task.Run(() => StreamItemsAsync(streamingCancellationTokenSource.Token));
                }
            }
            else
            {
                streamingCancellationTokenSource?.Cancel();
                await ExtractNonStreamingItemsAsync();
            }
        }

        private async Task ExtractNonStreamingItemsAsync(CancellationToken ct = default)
        {
            uiContext.Post(_ =>
            {
                picflowPanel.Controls.Clear();
                limitCounterLabel.Text = "0 Items";
            }, null);

            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string vrchatLogPath = Path.Combine(userProfile, "AppData", "LocalLow", "VRChat", "VRChat");

            if (!Directory.Exists(vrchatLogPath))
            {
                await AnimateTextAsync("No data found").ConfigureAwait(false);
                return;
            }

            var logFiles = Directory.EnumerateFiles(vrchatLogPath, "output_log_*.txt")
                                    .OrderByDescending(f => File.GetLastWriteTimeUtc(f));

            if (!logFiles.Any())
            {
                await AnimateTextAsync("No data found").ConfigureAwait(false);
                return;
            }

            await AnimateTextAsync("Found data from VRChat | Processing...").ConfigureAwait(false);

            foreach (var logFile in logFiles)
            {
                ct.ThrowIfCancellationRequested();

                try
                {
                    using var fileStream = new FileStream(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var reader = new StreamReader(fileStream);

                    string? line;
                    while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                    {
                        ct.ThrowIfCancellationRequested();

                        var stickerData = ProcessLine(line);
                        if (stickerData != null && allItems.Add(stickerData.Value.sticker))
                        {
                            var (userId, username, sticker) = stickerData.Value;

                            uiContext.Post(_ =>
                            {
                                ImagePanel.AddImagePanel(picflowPanel, apiRequest, username, userId, sticker);
                                limitCounterLabel.Text = $"{allItems.Count} Items";
                            }, null);
                        }
                    }
                }
                catch (IOException ex)
                {
                    Debug.WriteLine($"Errore nell'aprire/leggere {logFile}: {ex.Message}");
                }
            }

            await AnimateClearTextAsync();
        }

        private async Task StreamItemsAsync(CancellationToken token)
        {
            string vrchatLogPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"),
                "VRChat", "VRChat");

            string[] logFiles = Directory.GetFiles(vrchatLogPath, "output_log_*.txt");

            if (logFiles.Length == 0)
                return;

            string lastLogFile = logFiles.OrderByDescending(f => File.GetLastWriteTime(f)).First();

            try
            {
                using FileStream fileStream = new(lastLogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using StreamReader reader = new(fileStream);

                fileStream.Seek(0, SeekOrigin.End);

                while (!token.IsCancellationRequested)
                {
                    string? line = await reader.ReadLineAsync();

                    if (line != null)
                    {
                        var stickerData = ProcessLine(line);
                        if (stickerData != null && allItems.Add(stickerData.Value.sticker))
                        {
                            var (userId, username, sticker) = stickerData.Value;

                            picflowPanel.Invoke(() =>
                            {
                                ImagePanel.AddImagePanel(picflowPanel, apiRequest, username, userId, sticker);
                                limitCounterLabel.Text = $"{allItems.Count} Items";
                            });
                        }
                    }
                    else
                    {
                        await Task.Delay(500, token);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Errore nell'aprire il file {lastLogFile}: {ex.Message}");
            }
        }

        // ✅ Ora include anche "username"
        private (string userId, string username, string sticker)? ProcessLine(string line)
        {
            var match = StickerRegex.Match(line);
            if (match.Success)
            {
                string userId = match.Groups["userId"].Value;
                string username = match.Groups["username"].Value;
                string sticker = match.Groups["sticker"].Value;
                return (userId, username, sticker);
            }
            return null;
        }

        private void _refreshButton_Click(object sender, EventArgs e)
        {
            _refreshButton.Enabled = false;
            PicflowList();
            _refreshButton.Enabled = true;
        }

        private void _clearButton_Click(object sender, EventArgs e)
        {
            ClearItems();
        }

        private void ClearItems()
        {
            picflowPanel.Controls.Clear();
            allItems.Clear();
            limitCounterLabel.Text = "0 Items";
        }

        private void streamPicFlow_CheckedChanged(object sender, EventArgs e)
        {
            if (streamPicFlow.Checked)
            {
                _refreshButton.Enabled = false;
                streamingCancellationTokenSource = new CancellationTokenSource();
                streamingTask = Task.Run(() => StreamItemsAsync(streamingCancellationTokenSource.Token));
            }
            else
            {
                _refreshButton.Enabled = true;
                streamingCancellationTokenSource?.Cancel();
            }
        }

        private async Task AnimateTextAsync(string text)
        {
            uiContext.Post(_ => logLabel.Text = string.Empty, null);
            foreach (char c in text)
            {
                uiContext.Post(_ => logLabel.Text += c, null);
                await Task.Delay(1);
            }
        }

        private async Task AnimateClearTextAsync()
        {
            while (true)
            {
                string currentText = logLabel.Text;
                if (string.IsNullOrEmpty(currentText))
                    break;

                int newLength = currentText.Length - 1;
                if (newLength < 0) newLength = 0;

                uiContext.Post(_ => logLabel.Text = currentText.Substring(0, newLength), null);
                await Task.Delay(1);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            streamingCancellationTokenSource?.Cancel();
            base.OnFormClosing(e);
        }

        private void infoPicflowButton_Click(object sender, EventArgs e)
        {
            infoPicFlowLabel.Visible = !infoPicFlowLabel.Visible;
        }

        private void infoPicFlowLabel_Click(object sender, EventArgs e)
        {
            string textToCopy = "--enable-sdk-log-levels";
            Clipboard.SetText(textToCopy);
            NotificationManager.ShowNotification("Text copied to clipboard!", "--enable-sdk-log-levels", NotificationType.Success);
            infoPicFlowLabel.Visible = false;
        }
    }
}
