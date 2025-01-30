using Newtonsoft.Json.Linq;
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

            var mediaItems = await ExtractLatestStickersAsync();

            imageCount = mediaItems.Count();

            foreach (var id in mediaItems)
            {
                ImagePanel.AddImagePanel(picflowPanel, apiRequest, id);
            }

            limitCounterLabel.Text = $"{imageCount} Stickers";
        }

        private async Task<HashSet<string>> ExtractLatestStickersAsync()
        {
            string vrchatLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), "VRChat", "VRChat");
            string logFile = Directory.GetFiles(vrchatLogPath, "output_log_*.txt").MaxBy(File.GetCreationTime);

            if (logFile is null) return new();

            try
            {
                using FileStream fileStream = new(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using StreamReader reader = new(fileStream);
                string logContent = await reader.ReadToEndAsync();

                Regex stickerRegex = new(@"\[Always\] \[StickersManager\] User .*? spawned sticker (file_[a-f0-9\-]+)");

                var stickers = stickerRegex.Matches(logContent)
                    .Select(m => m.Groups[1].Value)
                    .Distinct()
                    .ToHashSet();

                return stickers;
            }
            catch (IOException ex)
            {
                throw new Exception($"Error opening the file: {ex.Message}");
            }
        }
    }
}
