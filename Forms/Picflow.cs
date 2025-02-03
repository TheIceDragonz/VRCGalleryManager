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
            picflowPanel.Controls.Clear();

            var mediaItems = await ExtractAllStickersAsync();

            imageCount = mediaItems.Count();

            foreach (var id in mediaItems)
            {
                ImagePanel.AddImagePanel(picflowPanel, apiRequest, id);
            }

            limitCounterLabel.Text = $"{imageCount} Stickers";
        }

        private async Task<HashSet<string>> ExtractAllStickersAsync()
        {
            string vrchatLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                .Replace("Local", "LocalLow"), "VRChat", "VRChat");

            string[] logFiles = Directory.GetFiles(vrchatLogPath, "output_log_*.txt");

            if (logFiles.Length == 0) return new();

            ConcurrentBag<string> allStickers = new();
            Regex stickerRegex = new(@"\[Always\] \[StickersManager\] User .*? spawned sticker (file_[a-f0-9\-]+)");

            await Parallel.ForEachAsync(logFiles, async (logFile, _) =>
            {
                try
                {
                    using FileStream fileStream = new(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using StreamReader reader = new(fileStream);
                    string logContent = await reader.ReadToEndAsync();

                    var stickers = stickerRegex.Matches(logContent)
                        .Select(m => m.Groups[1].Value)
                        .Distinct();

                    foreach (var sticker in stickers)
                    {
                        allStickers.Add(sticker);
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error opening the file {logFile}: {ex.Message}");
                }
            });

            return allStickers.ToHashSet();
        }
    }
}
