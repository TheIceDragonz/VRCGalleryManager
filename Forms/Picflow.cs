using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Forms.Panels;

namespace VRCGalleryManager.Forms
{
    public partial class Picflow : ApiConnectedForm
    {
        private CancellationTokenSource? streamingCancellationTokenSource;
        private Task? streamingTask;
        private readonly HashSet<string> allStickers = new();
        private static readonly Regex StickerRegex = new Regex(@"\[StickersManager\] User (?<userId>.*?) \((?<username>.*?)\) spawned sticker (?<sticker>file_[a-f0-9\-]+)", RegexOptions.Compiled);
        private readonly SynchronizationContext uiContext;

        public Picflow(VRCAuth auth)
        {
            InitializeComponent();
            InitApiRequest(auth);
            uiContext = SynchronizationContext.Current ?? new SynchronizationContext();
            this.Shown += (s, e) => { if (picflowPanel.Controls.Count == 0) PicflowList(); };
        }

        private async void PicflowList()
        {
            ClearStickers();
            if (streamPicFlow.Checked)
            {
                if (streamingTask == null || streamingTask.IsCompleted)
                {
                    streamingCancellationTokenSource = new CancellationTokenSource();
                    streamingTask = Task.Run(() => StreamStickersAsync(streamingCancellationTokenSource.Token));
                }
            }
            else
            {
                streamingCancellationTokenSource?.Cancel();
                await ExtractNonStreamingStickersAsync();
            }
        }

        private async Task ExtractNonStreamingStickersAsync()
        {
            uiContext.Post(_ =>
            {
                picflowPanel.Controls.Clear();
                limitCounterLabel.Text = "0 Stickers";
            }, null);

            string vrchatLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), "VRChat", "VRChat");
            string[] logFiles = Directory.GetFiles(vrchatLogPath, "output_log_*.txt");

            if (logFiles.Length == 0)
            {
                await AnimateTextAsync("No data found");
                return;
            }
            await AnimateTextAsync("Found data from VRChat | Processing...");

            foreach (var logFile in logFiles.OrderByDescending(f => File.GetLastWriteTime(f)))
            {
                try
                {
                    using FileStream fileStream = new(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using StreamReader reader = new(fileStream);
                    string? line;
                    var newStickers = new List<(string userId, string username, string sticker)>();
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var stickerData = ProcessLine(line);
                        if (stickerData != null && allStickers.Add(stickerData.Value.sticker))
                        {
                            newStickers.Add(stickerData.Value);
                        }
                    }
                    if (newStickers.Count > 0)
                    {
                        uiContext.Post(_ =>
                        {
                            foreach (var (userId, username, sticker) in newStickers)
                            {
                                ImagePanel.AddImagePanel(picflowPanel, apiRequest, userId, username, sticker);
                            }
                            limitCounterLabel.Text = $"{allStickers.Count} Stickers";
                        }, null);
                    }
                }
                catch (IOException ex)
                {
                    Debug.WriteLine($"Errore nell'aprire il file {logFile}: {ex.Message}");
                }
            }
            await AnimateClearTextAsync();
        }

        private async Task StreamStickersAsync(CancellationToken token)
        {
            string vrchatLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), "VRChat", "VRChat");
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
                        if (stickerData != null && allStickers.Add(stickerData.Value.sticker))
                        {
                            picflowPanel.Invoke(() =>
                            {
                                ImagePanel.AddImagePanel(picflowPanel, apiRequest, stickerData.Value.userId, stickerData.Value.username, stickerData.Value.sticker);
                                limitCounterLabel.Text = $"{allStickers.Count} Stickers";
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
            ClearStickers();
        }

        private void ClearStickers()
        {
            picflowPanel.Controls.Clear();
            allStickers.Clear();
            limitCounterLabel.Text = "0 Stickers";
        }

        private void streamPicFlow_CheckedChanged(object sender, EventArgs e)
        {
            if (streamPicFlow.Checked)
            {
                _refreshButton.Enabled = false;
                streamingCancellationTokenSource = new CancellationTokenSource();
                streamingTask = Task.Run(() => StreamStickersAsync(streamingCancellationTokenSource.Token));
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
            while (!string.IsNullOrEmpty(logLabel.Text))
            {
                uiContext.Post(_ => logLabel.Text = logLabel.Text.Substring(0, logLabel.Text.Length - 1), null);
                await Task.Delay(1);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            streamingCancellationTokenSource?.Cancel();
            base.OnFormClosing(e);
        }
    }
}
