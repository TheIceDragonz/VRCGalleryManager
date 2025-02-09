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
            await AnimateClearTextAsync();

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
            }
            else
            {
                await AnimateTextAsync("Found data from VRChat | Wait for the process...");
            }

            Regex stickerRegex = new Regex(@"\[Always\] \[StickersManager\] User .*? spawned sticker (file_[a-f0-9\-]+)", RegexOptions.Compiled);

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
                                bool isNewSticker = false;
                                lock (lockObj) isNewSticker = allStickers.Add(sticker);

                                if (isNewSticker)
                                {
                                    picflowPanel.Invoke((() =>
                                    {
                                        ImagePanel.AddImagePanel(picflowPanel, apiRequest, sticker);
                                        limitCounterLabel.Text = $"{allStickers.Count} Stickers";
                                    }));
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
