using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Forms;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Forms.Panels;

namespace VRCGalleryManager.Forms
{
    public partial class Emoji : Form
    {
        private ApiRequest apiRequest;

        private List<string> emojiJson = new List<string>();
        private int emojiCount;

        private static string EMOJI_MASK_TAG = "square";
        private static string EMOJI_ANIMATION_STYLE = "";

        private string tags = new string("");
        private string animationStyle = new string("");
        private string frames = new string("");
        private string framesOverTime = new string("");
        private string maskTag = new string("");

        public Emoji(VRCAuth auth)
        {
            InitializeComponent();

            apiRequest = new ApiRequest(auth);

            //EmojiList();

            TypePanel.LoadEmojiType(emojiOpenTypePanel, emojiTypePanel);
        }

        private async void _refreshButton_Click(object sender, EventArgs e)
        {
            EmojiList();
        }

        private async void EmojiList()
        {
            emojiPanel.Controls.Clear();
            emojiJson.Clear();

            ApiRequest.ApiData emoji = await apiRequest.GetApiData(TagType.Emoji.ToString().ToLower());

            emojiJson = emoji.JsonImage;
            emojiCount = emoji.JsonImage.Count;

            foreach (string json in emojiJson)
            {
                JObject jsonObject = JObject.Parse(json);

                string id = jsonObject["id"]?.ToString();
                string name = jsonObject["name"]?.ToString();
                string frames = jsonObject["frames"]?.ToString();
                string framesOverTime = jsonObject["framesOverTime"]?.ToString();
                string tags = jsonObject["tags"]?.ToString();

                ImagePanel.AddImagePanel(emojiPanel, apiRequest, id, tags, frames, framesOverTime);
            }

            limitStickerLabel.Text = $"{emojiCount}/9 Emoji";
            if (emojiCount == 9) limitPanelEmoji.Visible = true;
            else limitPanelEmoji.Visible = false;
        }

        private async void uploadEmoji_Click(object sender, EventArgs e)
        {
            if (!emojiOpenTypePanel.Text.Contains("Type"))
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
            else
            {
                DialogMessage.ShowMissingTypeDialog(this);
            }
        }
        private async void UploadImage(string path)
        {
            string resizedImage = ImageResizer.ResizeImage1024x1024(path);

            try
            {
                EMOJI_ANIMATION_STYLE = emojiOpenTypePanel.Text.ToLower();

                ApiRequest.ApiData emoji = await apiRequest.UploadImage(resizedImage, EMOJI_MASK_TAG, TagType.Emoji, EMOJI_ANIMATION_STYLE);

                ImagePanel.AddImagePanel(emojiPanel, apiRequest, emoji.IdImageUploaded, emoji.Tags, emoji.Frames, emoji.FramesOverTime);

                emojiCount = emojiCount + 1;
                limitStickerLabel.Text = $"{emojiCount}/9 Emoji";
                if (emojiCount >= 9) limitPanelEmoji.Visible = true;
                else limitPanelEmoji.Visible = false;

                NotificationManager.ShowNotification("Emoji uploaded successfully", "Emoji uploaded", NotificationType.Success);
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification(ex.Message, "Error during file upload", NotificationType.Error);
            }
        }

        private void emojiOpenTypePanel_Click(object sender, EventArgs e)
        {
            emojiTypePanel.Visible = !emojiTypePanel.Visible;
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
                        if (!emojiOpenTypePanel.Text.Contains("Type"))
                        {
                            pasteButton.Enabled = false;

                            UploadImage(files[0]);

                            pasteButton.Enabled = true;
                        }
                        else
                        {
                            DialogMessage.ShowMissingTypeDialog(this);
                        }
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
