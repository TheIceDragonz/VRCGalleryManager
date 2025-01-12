using System;
using System.Diagnostics;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;

namespace VRCGalleryManager.Forms
{
    public partial class Emoji : Form
    {
        private ApiRequest apiRequest;

        private List<string> emojiId = new List<string>();
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
            emojiId.Clear();

            ApiRequest.ApiData emoji = await apiRequest.GetApiData(TagType.Emoji.ToString().ToLower());

            emojiId = emoji.IdImage;
            emojiCount = int.Parse(emoji.CountImages);
            tags = emoji.Tags;
            if (tags.Contains("animated"))
            {
                animationStyle = emoji.AnimationStyle;
                frames = emoji.Frames;
                framesOverTime = emoji.FramesOverTime;
            }
            maskTag = emoji.MaskTag;

            foreach (string id in emojiId)
            {
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
