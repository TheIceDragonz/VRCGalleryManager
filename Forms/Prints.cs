using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;

namespace VRCGalleryManager.Forms
{
    public partial class Prints : Form
    {
        private ApiRequest apiRequest;

        private List<string> emojiJson = new List<string>();
        private int printsCount;

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
            emojiJson.Clear();

            ApiRequest.ApiData prints = await apiRequest.GetApiData(TagType.Print.ToString().ToLower());

            emojiJson = prints.JsonImage;
            printsCount = prints.JsonImage.Count;

            foreach (string json in emojiJson)
            {
                JObject jsonObject = JObject.Parse(json);

                string id = jsonObject["id"]?.ToString();

                AddPrintsPanel(id);
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
                        //ApiRequest.ApiData prints = await apiRequest.UploadImage();

                        //AddPrintsPanel();

                        printsCount = printsCount + 1;
                        limitPrintsLabel.Text = $"{printsCount}/64 Emoji";
                        if (printsCount == 9) limitPanelPrints.Visible = true;
                        else limitPanelPrints.Visible = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error during file upload", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
