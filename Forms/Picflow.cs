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

        private int imageCount;

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

            picflowPanel.Controls.Clear();
            limitCounterLabel.Text = $"0 Stickers";

            var mediaItems = await ExtractAllStickersAsync();

            imageCount = mediaItems.Count();

            foreach (var id in mediaItems)
            {
                ImagePanel.AddImagePanel(picflowPanel, apiRequest, id);
            }

            limitCounterLabel.Text = $"{imageCount} Stickers";

            await AnimateClearTextAsync();

            _refreshButton.Enabled = true;
        }

        private async Task<HashSet<string>> ExtractAllStickersAsync()
        {
            string vrchatLogPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"),
                "VRChat", "VRChat");

            string[] logFiles = Directory.GetFiles(vrchatLogPath, "output_log_*.txt");

            if (logFiles.Length == 0)
            {
                await AnimateTextAsync("No data found");
                return new HashSet<string>();
            }
            else
            {
                await AnimateTextAsync($"Found data from VRChat | Wait for the process...");
            }

            ConcurrentBag<string> allStickers = new();

            Regex stickerRegex = new(@"\[Always\] \[StickersManager\] User .*? spawned sticker (file_[a-f0-9\-]+)", RegexOptions.Compiled);

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
                                string sticker = match.Groups[1].Value;
                                allStickers.Add(sticker);
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error opening the file {logFile}: {ex.Message}");
                }
            });

            return allStickers.ToHashSet();
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
