using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Forms;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Forms.Panels;

namespace VRCGalleryManager.Forms
{
    public partial class Sticker : Form
    {
        private ApiRequest apiRequest;

        private List<string> stickerJson = new List<string>();
        private int stickerCount;

        private static string STICKER_TAG = "sticker";
        private static string STICKER_MASK_TYPE = "square";

        public Sticker(VRCAuth auth)
        {
            InitializeComponent();

            apiRequest = new ApiRequest(auth);

            this.Shown += (s, e) => { if (stickerPanel.Controls.Count == 0) StickerList(); };
        }

        private async void _refreshButton_Click(object sender, EventArgs e)
        {
            StickerList();
        }

        private async void StickerList()
        {
            stickerPanel.Controls.Clear();
            stickerJson.Clear();

            ApiRequest.ApiData sticker = await apiRequest.GetApiData(STICKER_TAG);

            stickerJson = sticker.JsonImage;
            stickerCount = sticker.JsonImage.Count;

            foreach (string json in stickerJson)
            {
                JObject jsonObject = JObject.Parse(json);

                string id = jsonObject["id"]?.ToString();

                ImagePanel.AddImagePanel(stickerPanel, apiRequest, id);
            }

            limitStickerLabel.Text = $"{stickerCount}/9 Sticker";
            if (stickerCount >= 9) limitPanelSticker.Visible = true;
            else limitPanelSticker.Visible = false;
        }

        private async void uploadSticker_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.webp";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    UploadImage(openFileDialog.FileName);
                }
            }
        }
        private async void UploadImage(string path)
        {
            string resizedImage = ImageResizer.ResizeImage1024x1024(path);

            try
            {
                ApiRequest.ApiData sticker = await apiRequest.UploadImage(resizedImage, STICKER_MASK_TYPE, TagType.Sticker);

                ImagePanel.AddImagePanel(stickerPanel, apiRequest, sticker.IdImageUploaded);

                stickerCount = stickerCount + 1;
                limitStickerLabel.Text = $"{stickerCount}/9 Sticker";
                if (stickerCount == 9) limitPanelSticker.Visible = true;
                else limitPanelSticker.Visible = false;

                NotificationManager.ShowNotification("Sticker uploaded successfully", "Sticker uploaded", NotificationType.Success);
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification(ex.Message, "Error during file upload", NotificationType.Error);
            }
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {
            ClipboardHandler.ClipboardDataImage(pasteButton, UploadImage);
        }
    }
}
