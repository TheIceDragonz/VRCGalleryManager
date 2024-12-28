using System.Diagnostics;
using VRCGalleryManager.Core;

namespace VRCGalleryManager.Forms
{
    public partial class Sticker : Form
    {
        private ApiRequest apiRequest;

        private List<string> stickerId = new List<string>();
        private string stickerCount;

        private string tagSticker = "sticker";

        public Sticker(VRCAuth auth)
        {
            InitializeComponent();

            apiRequest = new ApiRequest(auth);

            //StickerList();
        }

        private async void _refreshButton_Click(object sender, EventArgs e)
        {
            StickerList();
        }

        private async void StickerList()
        {
            stickerPanel.Controls.Clear();
            stickerId.Clear();

            ApiRequest.ApiData sticker = await apiRequest.GetApiData(tagSticker);

            stickerId = sticker.IdImage;
            stickerCount = sticker.CountImages;

            foreach (string id in stickerId)
            {
                //* Add Panel
                AddStickerPanel(id);
            }

            limitStickerLabel.Text = $"{stickerCount}/9 emoji";

            if (stickerCount.Contains("9"))
            {
                limitPanelSticker.Visible = true;
            }
            else
            {
                limitPanelSticker.Visible = false;
            }
        }

        private void uploadSticker_Click(object sender, EventArgs e)
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
