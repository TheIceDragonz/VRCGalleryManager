using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;

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

            LoadEmojiType();
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

                AddEmojiPanel(id, tags, frames, framesOverTime);
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
                        string selectedFilePath = openFileDialog.FileName;

                        try
                        {
                            EMOJI_ANIMATION_STYLE = emojiOpenTypePanel.Text.ToLower();

                            ApiRequest.ApiData emoji = await apiRequest.UploadImage(selectedFilePath, EMOJI_MASK_TAG, TagType.Emoji, EMOJI_ANIMATION_STYLE);

                            AddEmojiPanel(emoji.IdImageUploaded, emoji.Tags, emoji.Frames, emoji.FramesOverTime);

                            emojiCount = emojiCount + 1;
                            limitStickerLabel.Text = $"{emojiCount}/9 Emoji";
                            if (emojiCount == 9) limitPanelEmoji.Visible = true;
                            else limitPanelEmoji.Visible = false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error during file upload", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                DialogMessage.ShowMissingTypeDialog();
            }
        }

        private void emojiOpenTypePanel_Click(object sender, EventArgs e)
        {
            emojiTypePanel.Visible = !emojiTypePanel.Visible;
        }
    }
}
