using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Core.Helpers;
using VRCGalleryManager.Forms.Panels;

namespace VRCGalleryManager.Forms
{
    public partial class Emoji : ApiConnectedForm
    {

        private List<string> emojiJson = new List<string>();
        private int imageCount;

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
            InitApiRequest(auth);
            this.Shown += (s, e) => { if (emojiPanel.Controls.Count == 0) EmojiList(); };
        }

        private void _refreshButton_Click(object sender, EventArgs e)
        {
            EmojiList();
        }

        private async void EmojiList()
        {
            _refreshButton.Enabled = false;

            emojiPanel.Controls.Clear();
            emojiJson.Clear();

            ApiRequest.ApiData emoji = await apiRequest.GetApiData(TagType.Emoji.ToString().ToLower());

            emojiJson = emoji.JsonImage;
            imageCount = emoji.JsonImage.Count;

            foreach (string json in emojiJson)
            {
                JObject jsonObject = JObject.Parse(json);

                string id = jsonObject["id"]?.ToString();
                string name = jsonObject["name"]?.ToString();
                string frames = jsonObject["frames"]?.ToString();
                string framesOverTime = jsonObject["framesOverTime"]?.ToString();
                string tags = jsonObject["tags"]?.ToString();

                ImagePanel.AddImagePanel(emojiPanel, apiRequest, id, tags, frames, framesOverTime, UpdateCounter);
            }

            UpdateCounter("");

            _refreshButton.Enabled = true;
        }

        private async void uploadEmoji_Click(object sender, EventArgs e)
        {
            if (!emojiOpenTypePanel.Text.Contains("Type"))
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    ImageHelper.SetOpenFileDialogFilter(openFileDialog);
                    openFileDialog.Multiselect = false;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (!emojiOpenTypePanel.Text.Contains("Type"))
                        {
                            UploadImage(openFileDialog.FileName);
                        }
                        else
                        {
                            DialogMessage.ShowMissingTypeDialog(this);
                        }
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

                ImagePanel.AddImagePanel(emojiPanel, apiRequest, emoji.IdImageUploaded, emoji.Tags, emoji.Frames, emoji.FramesOverTime, UpdateCounter);
                UpdateCounter("Add");

                NotificationManager.ShowNotification("Emoji uploaded successfully", "Emoji uploaded", NotificationType.Success);
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification(ex.Message, "Error during file upload", NotificationType.Error);
            }
        }

        private void emojiOpenTypePanel_Click(object sender, EventArgs e)
        {
            if (emojiTypePanel.Visible)
            {
                TypePanel.ClearEmojiType(emojiTypePanel);
            }
            else
            {
                TypePanel.LoadEmojiType(emojiOpenTypePanel, emojiTypePanel);
            }

            emojiTypePanel.Visible = !emojiTypePanel.Visible;
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {
            if (!emojiOpenTypePanel.Text.Contains("Type"))
            {
                ClipboardHandler.ClipboardDataImageOrLink(pasteButton, UploadImage);
            }
            else
            {
                DialogMessage.ShowMissingTypeDialog(this);
            }
        }

        private void UpdateCounter(string action)
        {
            if (action == "Add") imageCount += 1;
            else if (action == "Remove") imageCount -= 1;
            limitCounterLabel.Text = $"{imageCount}/9 Emoji";
            if (imageCount >= 9)
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
            if (!emojiOpenTypePanel.Text.Contains("Type"))
            {
                ImageHelper.ProcessDragDrop(e, UploadImage);
            }
            else
            {
                DialogMessage.ShowMissingTypeDialog(this);
            }
        }
    }
}
