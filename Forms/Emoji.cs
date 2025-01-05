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
        private string emojiCount;

        private static string EMOKI_MASK_TAG = "square";


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
            emojiCount = emoji.CountImages;
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
                AddEmojiPanel(id, animationStyle, tags, frames, framesOverTime);
            }

           limitStickerLabel.Text = $"{emojiCount}/9 Emoji";

            if (emojiCount.Contains("9"))
            {
                limitPanelEmoji.Visible = true;
            }
            else
            {
                limitPanelEmoji.Visible = false;
            }
        }

        private async void uploadEmoji_ClickAsync(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.webp;*.svg;*.xml;*.tiff;*.bmp";

                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    try
                    {
                        //TODO aggiungere selezione parametri
                        ApiRequest.ApiData sticker = await apiRequest.UploadImage(selectedFilePath, EMOKI_MASK_TAG, TagType.Emoji, "aura", 10, 10);
                        EmojiList();
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
