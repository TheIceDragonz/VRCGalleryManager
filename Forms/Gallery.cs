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
        private int galleryCount;

        private static string GALLERY_TAG = "gallery";
        private static string GALLERY_MASK_TYPE = "square";

        public Gallery(VRCAuth auth)
        {
            InitializeComponent();

            apiRequest = new ApiRequest(auth);

            //GalleryList();
        }

        private async void _refreshButton_Click(object sender, EventArgs e)
        {
            GalleryList();
        }

        private async void GalleryList()
        {
            galleryPanel.Controls.Clear();
            galleryJson.Clear();

            ApiRequest.ApiData gallery = await apiRequest.GetApiData(GALLERY_TAG);

            galleryJson = gallery.JsonImage;
            galleryCount = gallery.JsonImage.Count;

            foreach (string json in galleryJson)
            {
                JObject jsonObject = JObject.Parse(json);

                string id = jsonObject["id"]?.ToString();

                ImagePanel.AddImagePanel(galleryPanel, apiRequest, id);
            }

            limitGalleryLabel.Text = $"{galleryCount}/64 Photos";
            if (galleryCount == 64) limitPanelGallery.Visible = true;
            else limitPanelGallery.Visible = false;
        }

        private async void uploadGallery_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.webp";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string resizedImage = ImageResizer.ResizeImage16x9(openFileDialog.FileName);

                    try
                    {
                        ApiRequest.ApiData gallery = await apiRequest.UploadImage(resizedImage, GALLERY_MASK_TYPE, TagType.Gallery);

                        ImagePanel.AddImagePanel(galleryPanel, apiRequest, gallery.IdImageUploaded);

                        galleryCount = galleryCount + 1;
                        limitGalleryLabel.Text = $"{galleryCount}/64 Photos";
                        if (galleryCount == 64) limitPanelGallery.Visible = true;
                        else limitPanelGallery.Visible = false;

                        NotificationManager.ShowNotification("Photos uploaded successfully", "Photos uploaded", NotificationType.Success);
                    }
                    catch (Exception ex)
                    {
                        NotificationManager.ShowNotification(ex.Message, "Error during file upload", NotificationType.Error);
                    }
                }
            }
        }

    }
}
