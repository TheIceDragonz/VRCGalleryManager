using System.Diagnostics;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRChat.API.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VRCGalleryManager.Forms
{
    public partial class Sticker : Form
    {
        private ApiRequest apiRequest;

        private List<string> stickerId = new List<string>();
        private string stickerCount;

        private static string STICKER_TAG = "sticker";
        private static string STICKER_MASK_TYPE = "square";

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

            ApiRequest.ApiData sticker = await apiRequest.GetApiData(STICKER_TAG);

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
                   
                    try
                    {
                        ApiRequest.ApiData sticker = await apiRequest.UploadImage(selectedFilePath, STICKER_MASK_TYPE, TagType.Sticker);

                        AddStickerPanel(sticker.IdImageUploaded);

                        stickerCount = sticker.CountImages;

                        limitStickerLabel.Text = $"{stickerCount}/9 Sticker";
                        if (stickerCount.Contains("9"))
                        {
                            limitPanelSticker.Visible = true;
                        }
                        else
                        {
                            limitPanelSticker.Visible = false;
                        }
                    } catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error during file upload", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

    }
}
