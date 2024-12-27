using VRCEmojiManager.Core;
using VRCEmojiManager.Design;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace VRCEmojiManager.Forms
{
    public partial class Emoji
    {
        private string linkEmoji = "https://api.vrchat.cloud/api/1/file/";
        private string endlinkEmoji = "/1/file";

        private async void AddEmojiPanel(string emojiId)
        {
            //* MAIN PANEL
            RoundedPanel panel = new RoundedPanel
            {
                BackColor = Color.FromArgb(50, 50, 50),
                Padding = new Padding(5),
                BorderRadius = 10,
            };

            //* IMAGE PANEL
            RoundedPictureBox pictureBox = new RoundedPictureBox
            {
                Dock = DockStyle.Top,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderRadiusBottomLeft = 5,
                BorderRadiusBottomRight = 5,
                BorderRadiusTopLeft = 5,
                BorderRadiusTopRight = 5,
            };
            string image = linkEmoji + emojiId + endlinkEmoji;
            string finalaviImage = await httpImage.GetFinalUrlAsync(image);
            if (!finalaviImage.Contains("imageNotFound"))
            {
                await Task.Run(() => pictureBox.Load(finalaviImage));
                pictureBox.Click += (sender, e) => Process.Start("explorer.exe", image);
                pictureBox.Cursor = Cursors.Hand;
            }
            else
            {
                //pictureBox.Image = Resources.NotFound;
                pictureBox.Cursor = Cursors.Default;
            }
            panel.Controls.Add(pictureBox);

            CircularButton btn_delete = new CircularButton
            {
                Text = "✖",
                BackColor = Color.FromArgb(255, 114, 114),
                ForeColor = Color.White,
                Width = 25,
                Height = 25,
            };
            btn_delete.Click += (sender, e) =>
            {
                DialogResult result = DialogMessage.ShowDeleteFileDialog(emojiId);

                if (result == DialogResult.Yes)
                {
                    Debug.WriteLine("Delete: " + emojiId);
                }
            };
            pictureBox.Controls.Add(btn_delete);

            emojiPanel.Controls.Add(panel);
        }
    }
}
