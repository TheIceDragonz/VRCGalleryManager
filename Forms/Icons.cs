using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Forms;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Forms.Panels;

namespace VRCGalleryManager.Forms
{
    public partial class Icons : Form
    {
        private ApiRequest apiRequest;

        private List<string> iconsJson = new List<string>();
        private int iconsCount;

        private static string ICONS_TAG = "icon";
        private static string ICONS_MASK_TYPE = "square";

        public Icons(VRCAuth auth)
        {
            InitializeComponent();

            apiRequest = new ApiRequest(auth);

            //IconsList();
        }

        private async void _refreshButton_Click(object sender, EventArgs e)
        {
            IconsList();
        }

        private async void IconsList()
        {
            iconsPanel.Controls.Clear();
            iconsJson.Clear();

            ApiRequest.ApiData icons = await apiRequest.GetApiData(ICONS_TAG);

            iconsJson = icons.JsonImage;
            iconsCount = icons.JsonImage.Count;

            foreach (string json in iconsJson)
            {
                JObject jsonObject = JObject.Parse(json);

                string id = jsonObject["id"]?.ToString();

                ImagePanel.AddImagePanel(iconsPanel, apiRequest, id);
            }

            limitIconsLabel.Text = $"{iconsCount}/64 Icons";
            if (iconsCount == 64) limitPanelIcons.Visible = true;
            else limitPanelIcons.Visible = false;
        }

        private async void uploadIcons_Click(object sender, EventArgs e)
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
                ApiRequest.ApiData icons = await apiRequest.UploadImage(resizedImage, ICONS_MASK_TYPE, TagType.Icon);

                ImagePanel.AddImagePanel(iconsPanel, apiRequest, icons.IdImageUploaded);

                iconsCount = iconsCount + 1;
                limitIconsLabel.Text = $"{iconsCount}/64 Icons";
                if (iconsCount == 64) limitPanelIcons.Visible = true;
                else limitPanelIcons.Visible = false;

                NotificationManager.ShowNotification("Icons uploaded successfully", "Icons uploaded", NotificationType.Success);
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
