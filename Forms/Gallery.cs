using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Forms;
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

        private void pasteButton_Click(object sender, EventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data != null)
            {
                if (data.GetDataPresent(DataFormats.Bitmap))
                {
                    var image = (Image)data.GetData(DataFormats.Bitmap);

                    string directoryPath = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
                    Directory.CreateDirectory(directoryPath);
                    string tempPath = Path.Combine(directoryPath, $"Pasted-Image_{Guid.NewGuid()}.png");
                    image.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);
                    UploadImage(tempPath);
                    NotificationManager.ShowNotification("Image pasted and saved successfully!", "Paste Image", NotificationType.Success);
                }
                else if (data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])data.GetData(DataFormats.FileDrop);
                    if (files.Length > 0)
                    {
                        UploadImage(files[0]);
                    }
                }
                else
                {
                    NotificationManager.ShowNotification("No image or file found in the clipboard!", "Error", NotificationType.Error);
                }
            }
            else
            {
                NotificationManager.ShowNotification("Clipboard is empty!", "Error", NotificationType.Error);
            }
        }
    }
}
