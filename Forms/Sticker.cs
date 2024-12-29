using System.Diagnostics;
using VRCGalleryManager.Core;
using VRChat.API.Model;

namespace VRCGalleryManager.Forms
{
    public partial class Sticker : Form
    {
        private ApiRequest apiRequest;

        private List<string> stickerId = new List<string>();
        private string stickerCount;

        private string tagSticker = "sticker";

        private MIMEType mimeType;

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

            limitStickerLabel.Text = $"{stickerCount}/9 Sticker";
            if (stickerCount.Contains("9")) limitPanelSticker.Visible = true;
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
                    string selectedFilePath = openFileDialog.FileName;

                    string name = Path.GetFileNameWithoutExtension(selectedFilePath);
                    string extension = Path.GetExtension(selectedFilePath);
                    List<string> tags = new List<string> { "sticker" };

                    if(extension == ".png") { mimeType = MIMEType.ImagePng; }
                    else if(extension == ".jpg") { mimeType = MIMEType.ImageJpg; }
                    else if (extension == ".jpeg") { mimeType = MIMEType.ImageJpeg; }
                    else if (extension == ".webp") { mimeType = MIMEType.ImageWebp; }

                    ApiRequest.ApiData sticker = await apiRequest.UploadApiData(name, mimeType, extension, tags);

                    AddStickerPanel(sticker.IdImageUploaded);

                    stickerCount = sticker.CountImages;

                    limitStickerLabel.Text = $"{stickerCount}/9 Sticker";
                    if (stickerCount.Contains("9")) limitPanelSticker.Visible = true;
                    else limitPanelSticker.Visible = false;
                }
            }
        }

    }
}
