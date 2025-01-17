using System.Diagnostics;
using System.Windows.Forms;
using VRCGalleryManager.Core;
using VRCGalleryManager.Design;

namespace VRCGalleryManager.Forms
{
    public partial class Emoji
    {
        private async void AddEmojiPanel(string emojiId, string tags, string frames, string framesOverTime)
        {
            //* IMAGE PANEL
            RoundedPictureBox pictureBox = new RoundedPictureBox
            {
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(24, 27, 31),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderRadiusBottomLeft = 10,
                BorderRadiusBottomRight = 10,
                BorderRadiusTopLeft = 10,
                BorderRadiusTopRight = 10,
                BorderColor = Color.FromArgb(24, 27, 31),
                BorderSize = 5,
                Padding = new Padding(7)
            };
            string image = $"https://api.vrchat.cloud/api/1/file/{emojiId}/1/file";
            string finalaviImage = await HttpImage.GetFinalUrlAsync(image);

            if (!finalaviImage.Contains("imageNotFound"))
            {
                CircularButton btn_open = new CircularButton
                {
                    Text = "🖼️",
                    BackColor = Color.FromArgb(96, 159, 255),
                    ForeColor = Color.White,
                    Dock = DockStyle.Right,
                    Width = 25,
                    Height = 25,
                    Location = new Point(pictureBox.Size.Width - 35, pictureBox.Size.Height - 35),
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                };
                if (tags.Contains("animated"))
                {
                    SpriteSheetViewer viewer = new SpriteSheetViewer(pictureBox);
                    await viewer.LoadSpriteSheetAsync(finalaviImage, int.Parse(frames), int.Parse(framesOverTime));
                    viewer.StartAnimation();
                }
                else
                {
                    pictureBox.LoadAsync(finalaviImage);
                }

                btn_open.Click += (sender, e) => Process.Start("explorer.exe", image);
                btn_open.Cursor = Cursors.Hand;
                pictureBox.Controls.Add(btn_open);
            }
            else
            {
                pictureBox.Cursor = Cursors.Default;
            }

            CircularButton btn_delete = new CircularButton
            {
                Text = "✖",
                BackColor = Color.FromArgb(255, 114, 114),
                ForeColor = Color.White,
                Dock = DockStyle.Right,
                Width = 25,
                Height = 25,
                Location = new Point(pictureBox.Size.Width - 60, pictureBox.Size.Height - 35),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btn_delete.Click += async (sender, e) =>
            {
                DialogResult result = DialogMessage.ShowDeleteFileDialog(emojiId);

                if (result == DialogResult.Yes)
                {
                    Debug.WriteLine("Delete: " + emojiId);

                    await apiRequest.DeleteApiData(emojiId);

                    emojiPanel.Controls.Remove(pictureBox);

                    emojiCount = emojiCount - 1;
                    limitStickerLabel.Text = $"{emojiCount}/9 Emoji";
                    if (emojiCount == 9) limitPanelEmoji.Visible = true;
                    else limitPanelEmoji.Visible = false;
                }
            };
            pictureBox.Controls.Add(btn_delete);

            emojiPanel.Controls.Add(pictureBox);
        }
    }
}
