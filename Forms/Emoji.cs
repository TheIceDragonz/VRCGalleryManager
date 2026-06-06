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

        public Emoji(VRCAuth auth)
        {
            InitializeComponent();
            InitApiRequest(auth);

            // Position pasteButton to the right and expand uploadButton
            pasteButton.Left = 825;
            uploadButton.Width = 802;

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

        private void uploadEmoji_Click(object sender, EventArgs e)
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
        private void UploadImage(string path)
        {
            uploadButton.Enabled = false;
            pasteButton.Enabled = false;

            var mainPanel = this.TopLevelControl as MainPanel;
            if (mainPanel == null) return;

            mainPanel.ShowEmojiEditor(
                path,
                onSave: async (editedPath, isAnimated, style, frames, fps) =>
                {
                    try
                    {
                        ApiRequest.ApiData emoji;
                        if (isAnimated)
                        {
                            emoji = await apiRequest.UploadImage(editedPath, EMOJI_MASK_TAG, TagType.EmojiAnimated, style.ToLower(), frames, fps);
                        }
                        else
                        {
                            emoji = await apiRequest.UploadImage(editedPath, EMOJI_MASK_TAG, TagType.Emoji, style.ToLower());
                        }

                        ImagePanel.AddImagePanel(emojiPanel, apiRequest, emoji.IdImageUploaded, emoji.Tags, emoji.Frames, emoji.FramesOverTime, UpdateCounter);
                        UpdateCounter("Add");

                        NotificationManager.ShowNotification("Emoji uploaded successfully", "Emoji uploaded", NotificationType.Success);
                    }
                    catch (Exception ex)
                    {
                        NotificationManager.ShowNotification(ex.Message, "Error during file upload", NotificationType.Error);
                    }
                    finally
                    {
                        try { File.Delete(editedPath); } catch { }
                        UpdateCounter("");
                    }
                },
                onCancel: () =>
                {
                    UpdateCounter("");
                }
            );
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {
            ClipboardHandler.ClipboardDataImageOrLink(pasteButton, UploadImage);
        }

        private void UpdateCounter(string action)
        {
            if (action == "Add") imageCount += 1;
            else if (action == "Remove") imageCount -= 1;
            limitCounterLabel.Text = $"{imageCount}/18 Emoji";
            if (imageCount >= 18)
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
