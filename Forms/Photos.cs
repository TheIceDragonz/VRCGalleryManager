using Newtonsoft.Json.Linq;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Core.Helpers;
using VRCGalleryManager.Forms.Panels;

namespace VRCGalleryManager.Forms
{
    public partial class Photos : ApiConnectedForm
    {
        private List<string> photosJson = new List<string>();
        private int imageCount;

        private static string PHOTOS_TAG = "gallery";
        private static string PHOTOS_MASK_TYPE = "square";

        public Photos(VRCAuth auth)
        {
            InitializeComponent();

            InitApiRequest(auth);

            this.Shown += (s, e) => { if (photosPanel.Controls.Count == 0) PhotosList(); };
        }

        private void _refreshButton_Click(object sender, EventArgs e)
        {
            PhotosList();
        }

        private async void PhotosList()
        {
            _refreshButton.Enabled = false;

            photosPanel.Controls.Clear();
            photosJson.Clear();

            ApiRequest.ApiData photos = await apiRequest.GetApiData(PHOTOS_TAG);

            photosJson = photos.JsonImage;
            imageCount = photos.JsonImage.Count;

            foreach (string json in photosJson)
            {
                JObject jsonObject = JObject.Parse(json);

                string id = jsonObject["id"]?.ToString();

                ImagePanel.AddImagePanel(photosPanel, apiRequest, id, UpdateCounter);
            }

            UpdateCounter("");

            _refreshButton.Enabled = true;
        }

        private async void uploadPhotos_Click(object sender, EventArgs e)
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
            string resizedImage = ImageResizer.ResizeImage16x9(path);

            try
            {
                ApiRequest.ApiData photos = await apiRequest.UploadImage(resizedImage, PHOTOS_MASK_TYPE, TagType.Gallery);

                ImagePanel.AddImagePanel(photosPanel, apiRequest, photos.IdImageUploaded, UpdateCounter);
                UpdateCounter("Add");

                NotificationManager.ShowNotification("Photos uploaded successfully", "Photos uploaded", NotificationType.Success);
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
            limitCounterLabel.Text = $"{imageCount}/64 Photos";
            if (imageCount >= 64)
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
