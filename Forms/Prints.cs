using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
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
                    string selectedFilePath = openFileDialog.FileName;

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
            }
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {

        }
    }
}
