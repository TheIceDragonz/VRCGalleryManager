using Newtonsoft.Json.Linq;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Core.Helpers;
using VRCGalleryManager.Forms.Panels;

namespace VRCGalleryManager.Forms
{
    public partial class Prints : Form
    {
        private ApiRequest apiRequest;

        private List<string> printsJson = new List<string>();
        private int imageCount;

        private static string PRINTS_MASK_TYPE = "square";

        public Prints(VRCAuth auth)
        {
            InitializeComponent();

            apiRequest = new ApiRequest(auth);

            this.Shown += (s, e) => { if (printsPanel.Controls.Count == 0) PrintsList(); };
        }

        private void _refreshButton_Click(object sender, EventArgs e)
        {
            PrintsList();
        }

        private async void PrintsList()
        {
            _refreshButton.Enabled = false;

            printsPanel.Controls.Clear();
            printsJson.Clear();

            ApiRequest.ApiData prints = await apiRequest.GetApiData(TagType.Print.ToString().ToLower());

            printsJson = prints.JsonImage;
            imageCount = prints.JsonImage.Count;

            foreach (string json in printsJson)
            {
                JObject jsonObject = JObject.Parse(json);

                string authorId = jsonObject["authorId"]?.ToString();
                string authorName = jsonObject["authorName"]?.ToString();
                string printid = jsonObject["id"]?.ToString();
                string fileid = jsonObject["files"]?["fileId"]?.ToString();

                ImagePanel.AddPrintsPanel(printsPanel, apiRequest, printid, authorId, authorName , fileid, UpdateCounter);
            }

            UpdateCounter("");

            _refreshButton.Enabled = true;
        }

        private async void uploadPrints_Click(object sender, EventArgs e)
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
                ApiRequest.ApiData prints = await apiRequest.UploadPrint(resizedImage, textBoxNotePrint.Text);
                //ImagePanel.AddImagePanel(printsPanel, apiRequest, prints.IdImageUploaded, UpdateCounter);
                //UpdateCounter("Add");

                PrintsList();

                NotificationManager.ShowNotification("File uploaded successfully", "File upload", NotificationType.Success);
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
            limitCounterLabel.Text = $"{imageCount}/64 Prints";
            if (imageCount >= 64) limitPanel.Visible = true;
            else limitPanel.Visible = false;
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
