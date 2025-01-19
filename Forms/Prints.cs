using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Forms.Panels;

namespace VRCGalleryManager.Forms
{
    public partial class Prints : Form
    {
        private ApiRequest apiRequest;

        private List<string> printsJson = new List<string>();
        private int printsCount;

        private static string PRINTS_MASK_TYPE = "square";

        public Prints(VRCAuth auth)
        {
            InitializeComponent();

            apiRequest = new ApiRequest(auth);

            //PrintsList();
        }

        private async void _refreshButton_Click(object sender, EventArgs e)
        {
            PrintsList();
        }

        private async void PrintsList()
        {
            printsPanel.Controls.Clear();
            printsJson.Clear();

            ApiRequest.ApiData prints = await apiRequest.GetApiData(TagType.Print.ToString().ToLower());

            printsJson = prints.JsonImage;
            printsCount = prints.JsonImage.Count;

            foreach (string json in printsJson)
            {
                JObject jsonObject = JObject.Parse(json);

                string id = jsonObject["id"]?.ToString();

                ImagePanel.AddImagePanel(printsPanel, apiRequest, id);
            }

            limitPrintsLabel.Text = $"{printsCount}/64 Prints";
            if (printsCount == 9) limitPanelPrints.Visible = true;
            else limitPanelPrints.Visible = false;
        }

        private async void uploadPrints_Click(object sender, EventArgs e)
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
            string selectedFilePath = path;

            try
            {
                ApiRequest.ApiData prints = await apiRequest.UploadImage(selectedFilePath, PRINTS_MASK_TYPE, TagType.Print);

                ImagePanel.AddImagePanel(printsPanel, apiRequest, prints.IdImageUploaded);

                printsCount = printsCount + 1;
                limitPrintsLabel.Text = $"{printsCount}/64 Emoji";
                if (printsCount == 9) limitPanelPrints.Visible = true;
                else limitPanelPrints.Visible = false;

                NotificationManager.ShowNotification("File uploaded successfully", "File upload", NotificationType.Success);
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
                        pasteButton.Enabled = false;

                        UploadImage(files[0]);

                        pasteButton.Enabled = true;
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
