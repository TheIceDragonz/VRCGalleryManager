using System.Diagnostics;
using VRCGalleryManager.Core;
using VRCGalleryManager.Design;

namespace VRCGalleryManager.Forms
{
    public partial class Sticker
    {
        private async void AddStickerPanel(string stickerId)
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
            string image = $"https://api.vrchat.cloud/api/1/file/{stickerId}/1/file";
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
                btn_open.Click += (sender, e) => Process.Start("explorer.exe", image);
                btn_open.Cursor = Cursors.Hand;
                pictureBox.Controls.Add(btn_open);
            }
            else
            {
                //pictureBox.Image = Resources.NotFound;
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
                DialogResult result = DialogMessage.ShowDeleteFileDialog(stickerId);

                if (result == DialogResult.Yes)
                {
                    Debug.WriteLine("Delete: " + stickerId);

                    await apiRequest.DeleteApiData(stickerId);

                    stickerPanel.Controls.Remove(pictureBox);

                    stickerCount = stickerCount - 1;
                    limitStickerLabel.Text = $"{stickerCount}/9 Sticker";
                    if (stickerCount == 9) limitPanelSticker.Visible = true;
                    else limitPanelSticker.Visible = false;
                }
            };
            pictureBox.Controls.Add(btn_delete);

            stickerPanel.Controls.Add(pictureBox);
        }
    }
}
