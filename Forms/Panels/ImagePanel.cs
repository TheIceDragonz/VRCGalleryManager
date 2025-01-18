using System.Diagnostics;
using VRCGalleryManager.Core;
using VRCGalleryManager.Design;

namespace VRCGalleryManager.Forms
{
    public class ImagePanel
    {
        public async void AddImagePanel(FlowLayoutPanel mainPanel, ApiRequest apiRequest, string imageId, string tags, string frames, string framesOverTime)
        {
            //* IMAGE Animated PANEL
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
                Padding = new Padding(5)
            };
            string image = tags.Contains("animated")
                ? $"https://api.vrchat.cloud/api/1/file/{imageId}/1/file"
                : $"https://api.vrchat.cloud/api/1/image/{imageId}/1/256";

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

                btn_open.Click += (sender, e) => Process.Start("explorer.exe", $"https://api.vrchat.cloud/api/1/file/{imageId}/1/file");
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
                DialogResult result = DialogMessage.ShowDeleteFileDialog(imageId);

                if (result == DialogResult.Yes)
                {
                    Debug.WriteLine("Delete: " + imageId);

                    await apiRequest.DeleteApiData(imageId);

                    mainPanel.Controls.Remove(pictureBox);

                    /*
                    emojiCount = emojiCount - 1;
                    limitStickerLabel.Text = $"{emojiCount}/9 Emoji";
                    if (emojiCount == 9) limitPanelEmoji.Visible = true;
                    else limitPanelEmoji.Visible = false;
                    */
                }
            };
            pictureBox.Controls.Add(btn_delete);

            mainPanel.Controls.Add(pictureBox);
        }
        public async void AddImagePanel(FlowLayoutPanel mainPanel, ApiRequest apiRequest, string imageId)
        {
            Size size = new Size(150, 150);
            if (mainPanel.Name.Contains("prints")) size = new Size(190, 150);
            if (mainPanel.Name.Contains("gallery")) size = new Size(250, 150);

            int value = 10;
            if (mainPanel.Name.Contains("icons")) value = 100; //Max Rounded

            //* IMAGE Static PANEL
            RoundedPictureBox pictureBox = new RoundedPictureBox
            {
                Size = size,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(24, 27, 31),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderRadiusBottomLeft = value,
                BorderRadiusBottomRight = 10,
                BorderRadiusTopLeft = value,
                BorderRadiusTopRight = value,
                BorderColor = Color.FromArgb(24, 27, 31),
                BorderSize = 5,
                Padding = new Padding(5)
            };
            string image = $"https://api.vrchat.cloud/api/1/image/{imageId}/1/256";
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
                pictureBox.LoadAsync(finalaviImage);

                btn_open.Click += (sender, e) => Process.Start("explorer.exe", $"https://api.vrchat.cloud/api/1/file/{imageId}/1/file");
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
                DialogResult result = DialogMessage.ShowDeleteFileDialog(imageId);

                if (result == DialogResult.Yes)
                {
                    Debug.WriteLine("Delete: " + imageId);

                    await apiRequest.DeleteApiData(imageId);

                    mainPanel.Controls.Remove(pictureBox);

                    /*
                    emojiCount = emojiCount - 1;
                    limitStickerLabel.Text = $"{emojiCount}/9 Emoji";
                    if (emojiCount == 9) limitPanelEmoji.Visible = true;
                    else limitPanelEmoji.Visible = false;
                    */
                }
            };
            pictureBox.Controls.Add(btn_delete);

            mainPanel.Controls.Add(pictureBox);
        }
    }
}
