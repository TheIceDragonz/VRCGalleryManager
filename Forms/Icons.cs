﻿using Newtonsoft.Json.Linq;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Core.Helpers;
using VRCGalleryManager.Forms.Panels;

namespace VRCGalleryManager.Forms
{
    public partial class Icons : ApiConnectedForm
    {
        private List<string> iconsJson = new List<string>();
        private int imageCount;

        private static string ICONS_TAG = "icon";
        private static string ICONS_MASK_TYPE = "square";

        public Icons(VRCAuth auth)
        {
            InitializeComponent();
            this.Shown += (s, e) => { if (iconsPanel.Controls.Count == 0) IconsList(); };
            InitApiRequest(auth);
        }

        private void _refreshButton_Click(object sender, EventArgs e)
        {
            IconsList();
        }

        private async void IconsList()
        {
            _refreshButton.Enabled = false;

            iconsPanel.Controls.Clear();
            iconsJson.Clear();

            ApiRequest.ApiData icons = await apiRequest.GetApiData(ICONS_TAG);

            iconsJson = icons.JsonImage;
            imageCount = icons.JsonImage.Count;

            foreach (string json in iconsJson)
            {
                JObject jsonObject = JObject.Parse(json);

                string id = jsonObject["id"]?.ToString();

                ImagePanel.AddImagePanel(iconsPanel, apiRequest, id, UpdateCounter);
            }

            UpdateCounter("");

            _refreshButton.Enabled = true;
        }

        private async void uploadIcons_Click(object sender, EventArgs e)
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
            string resizedImage = ImageResizer.ResizeImage1x1(path);

            try
            {
                ApiRequest.ApiData icons = await apiRequest.UploadImage(resizedImage, ICONS_MASK_TYPE, TagType.Icon);

                ImagePanel.AddImagePanel(iconsPanel, apiRequest, icons.IdImageUploaded, UpdateCounter);
                UpdateCounter("Add");

                NotificationManager.ShowNotification("Icons uploaded successfully", "Icons uploaded", NotificationType.Success);
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
            limitCounterLabel.Text = $"{imageCount}/64 Icons";
            if (imageCount >= 64)
            {
                pasteButton.Enabled = false;
                uploadButton.Enabled = false;
                limitPanel.Visible = true;
            }
            else
            {
                pasteButton.Enabled = true;
                uploadButton.Enabled = true;
                limitPanel.Visible = false;
            }
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
