using Newtonsoft.Json.Linq;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Core.Helpers;
using VRCGalleryManager.Forms.Panels;

namespace VRCGalleryManager.Forms
{
    public partial class Sticker : ApiConnectedForm
    {
        private List<string> stickerJson = new List<string>();
        private int imageCount;

        private static string STICKER_TAG = "sticker";
        private static string STICKER_MASK_TYPE = "square";

        public Sticker(VRCAuth auth)
        {
            InitializeComponent();
            InitApiRequest(auth);
            this.Shown += (s, e) => { if (stickerPanel.Controls.Count == 0) StickerList(); };
        }

        private void _refreshButton_Click(object sender, EventArgs e)
        {
            StickerList();
        }

        private async void StickerList()
        {
            _refreshButton.Enabled = false;

            stickerPanel.Controls.Clear();
            stickerJson.Clear();

            ApiRequest.ApiData sticker = await apiRequest.GetApiData(STICKER_TAG);

            stickerJson = sticker.JsonImage;
            imageCount = sticker.JsonImage.Count;

            foreach (string json in stickerJson)
            {
                JObject jsonObject = JObject.Parse(json);

                string id = jsonObject["id"]?.ToString();

                ImagePanel.AddImagePanel(stickerPanel, apiRequest, id, UpdateCounter);
            }

            UpdateCounter("");

            _refreshButton.Enabled = true;
        }

        private async void uploadSticker_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                ImageHelper.SetOpenFileDialogFilter(openFileDialog);
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    UploadImage(openFileDialog.FileName);
                }
            }
        }
        private async void UploadImage(string path)
        {
            string resizedImage = ImageResizer.ResizeImage1x1(path);

            try
            {
                ApiRequest.ApiData sticker = await apiRequest.UploadImage(resizedImage, STICKER_MASK_TYPE, TagType.Sticker);

                ImagePanel.AddImagePanel(stickerPanel, apiRequest, sticker.IdImageUploaded, UpdateCounter);
                UpdateCounter("Add");

                NotificationManager.ShowNotification("Sticker uploaded successfully", "Sticker uploaded", NotificationType.Success);
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification(ex.Message, "Error during file upload", NotificationType.Error);
            }
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {
            ClipboardHandler.ClipboardDataImageOrLink(pasteButton, UploadImage);
        }
        private void UpdateCounter(string action)
        {
            if (action == "Add") imageCount += 1;
            else if (action == "Remove") imageCount -= 1;
            limitCounterLabel.Text = $"{imageCount}/18 Sticker";
            if (imageCount >= 18)
            {
                pasteButton.Enabled = false;
                uploadButton.Enabled = false;
                limitPanel.Visible = true;
            }
            else
            {
                pasteButton.Enabled = true;
                uploadButton.Enabled = true;
                limitPanel.Visible = false;
            }
        }

        private void File_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = ImageHelper.ProcessDragEnter(e);
        }

        private void File_DragDrop(object sender, DragEventArgs e)
        {
            ImageHelper.ProcessDragDrop(e, UploadImage);
        }
    }
}
