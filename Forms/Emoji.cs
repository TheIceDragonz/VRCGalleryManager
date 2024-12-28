using System.Diagnostics;
using VRCGalleryManager.Core;

namespace VRCGalleryManager.Forms
{
    public partial class Emoji : Form
    {
        private ApiRequest apiRequest;

        private List<string> emojiId = new List<string>();
        private string emojiCount;

        private string tagEmoji = "emoji";
        private string tagSticker = "sticker";

        public Emoji(VRCAuth auth)
        {
            InitializeComponent();

            apiRequest = new ApiRequest(auth);

            //EmojiList();
        }

        private async void _refreshButton_Click(object sender, EventArgs e)
        {
            EmojiList();
        }

        private async void EmojiList()
        {
            emojiPanel.Controls.Clear();
            emojiId.Clear();

            ApiRequest.ApiData emoji = await apiRequest.GetApiData(tagEmoji);

            emojiId = emoji.IdImage;
            emojiCount = emoji.CountImage;

            foreach (string id in emojiId)
            {
                //* Add Panel
                AddEmojiPanel(id);
            }

            limitStickerLabel.Text = $"{emojiCount}/9 Emoji";

            if (emojiCount.Contains("9"))
            {
                limitPanelEmoji.Visible = true;
            }
            else
            {
                limitPanelEmoji.Visible = false;
            }
        }

        private void uploadEmoji_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.webp;*.svg;*.xml;*.tiff;*.bmp";

                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    MessageBox.Show($"File selezionato: {selectedFilePath}");



                }
            }
        }

    }
}
