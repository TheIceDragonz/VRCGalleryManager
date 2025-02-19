using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Forms.Panels;

namespace VRCGalleryManager.Forms
{
    public partial class Picflow : Form
    {
        private ApiRequest apiRequest;

        HashSet<string> allStickers = new();

        public Picflow(VRCAuth auth)
        {
            InitializeComponent();

            apiRequest = new ApiRequest(auth);

            this.Shown += (s, e) => { if (picflowPanel.Controls.Count == 0) PicflowList(); };
        }

        private void _refreshButton_Click(object sender, EventArgs e)
        {
            PicflowList();
        }

        private async void PicflowList()
        {
            _refreshButton.Enabled = false;

            await ExtractAllStickersAsync();

            _refreshButton.Enabled = true;
        }

        private async Task ExtractAllStickersAsync()
        {
            picflowPanel.Controls.Clear();
            allStickers = new HashSet<string>();
            object lockObj = new object();

            string vrchatLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), "VRChat", "VRChat");

            string[] logFiles = Directory.GetFiles(vrchatLogPath, "output_log_*.txt");

            if (logFiles.Length == 0)
            {
                await AnimateTextAsync("No data found");
                return;
            }
            else
            {
                await AnimateTextAsync("Found data from VRChat | Wait for the process...");
            }

            Regex stickerRegex = new Regex(@"\[StickersManager\] User (?<userId>.*?) \((?<username>.*?)\) spawned sticker (?<sticker>file_[a-f0-9\-]+)", RegexOptions.Compiled);

            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

            await Parallel.ForEachAsync(logFiles, parallelOptions, async (logFile, cancellationToken) =>
            {
                try
                {
                    using FileStream fileStream = new(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using StreamReader reader = new(fileStream);

                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        foreach (Match match in stickerRegex.Matches(line))
                        {
                            if (match.Success)
                            {
                                string userId = match.Groups["userId"].Value;
                                string username = match.Groups["username"].Value;
                                string sticker = match.Groups["sticker"].Value;

                                bool isNewSticker = false;
                                lock (lockObj)
                                {
                                    isNewSticker = allStickers.Add(sticker);
                                }

                                if (isNewSticker)
                                {
                                    picflowPanel.Invoke(() =>
                                    {
                                        ImagePanel.AddImagePanel(picflowPanel, apiRequest, userId, username, sticker);
                                        limitCounterLabel.Text = $"{allStickers.Count} Stickers";
                                    });
                                }
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Errore nell'aprire il file {logFile}: {ex.Message}");
                }
            });

            if (allStickers.Count == 0)
            {
                await AnimateTextAsync("No stickers found");
                return;
            }

            await AnimateClearTextAsync();
        }

        private async Task AnimateTextAsync(string text)
        {
            logLabel.Text = "";
            foreach (char c in text)
            {
                logLabel.Text += c;
                await Task.Delay(1);
            }
        }

        private async Task AnimateClearTextAsync()
        {
            while (!string.IsNullOrEmpty(logLabel.Text))
            {
                logLabel.Text = logLabel.Text.Substring(0, logLabel.Text.Length - 1);
                await Task.Delay(1);
            }
        }
    }
}
