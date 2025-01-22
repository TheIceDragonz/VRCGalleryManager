using Newtonsoft.Json.Linq;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Forms.Panels;

namespace VRCGalleryManager.Forms
{
    public partial class Gallery : Form
    {
        private ApiRequest apiRequest;

        private List<string> galleryJson = new List<string>();
        private int imageCount;

        private static string GALLERY_TAG = "gallery";
        private static string GALLERY_MASK_TYPE = "square";

        public Gallery(VRCAuth auth)
        {
            InitializeComponent();

            apiRequest = new ApiRequest(auth);

            this.Shown += (s, e) => { if (galleryPanel.Controls.Count == 0) GalleryList(); };
        }

        private void _refreshButton_Click(object sender, EventArgs e)
        {
            GalleryList();
        }

        private async void GalleryList()
        {
            galleryPanel.Controls.Clear();
            galleryJson.Clear();

            ApiRequest.ApiData gallery = await apiRequest.GetApiData(GALLERY_TAG);

            galleryJson = gallery.JsonImage;
            imageCount = gallery.JsonImage.Count;

            foreach (string json in galleryJson)
            {
                JObject jsonObject = JObject.Parse(json);

                string id = jsonObject["id"]?.ToString();

                ImagePanel.AddImagePanel(galleryPanel, apiRequest, id, UpdateCounter);
            }

            UpdateCounter("Add");
        }

        private async void uploadGallery_Click(object sender, EventArgs e)
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
            string resizedImage = ImageResizer.ResizeImage16x9(path);

            try
            {
                ApiRequest.ApiData gallery = await apiRequest.UploadImage(resizedImage, GALLERY_MASK_TYPE, TagType.Gallery);

                ImagePanel.AddImagePanel(galleryPanel, apiRequest, gallery.IdImageUploaded, UpdateCounter);
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
            if (imageCount >= 64) limitPanel.Visible = true;
            else limitPanel.Visible = false;
        }
    }
}
