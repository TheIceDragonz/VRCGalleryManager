using System.Diagnostics;
using System.Windows.Forms;
using VRCGalleryManager.Core;
using VRCGalleryManager.Design;
using VRCGalleryManager.Properties;

namespace VRCGalleryManager.Forms
{
    public class TypeWithImage
    {
        public string Type { get; set; }
        public Image Image { get; set; }

        public TypeWithImage(string type, Image image)
        {
            Type = type;
            Image = image;
        }
    }

    public partial class Emoji
    {
        private async void LoadEmojiType()
        {
            EmojiType emojiType = new EmojiType();
            var typesWithImages = emojiType.TypesWithImages;

            foreach (var typeWithImage in typesWithImages)
            {
                AddEmojiTypePanel(typeWithImage);
            }
        }

        private async void AddEmojiTypePanel(TypeWithImage typeWithImage)
        {
            RoundedPanel panel = new RoundedPanel
            {
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(24, 27, 31),
                Padding = new Padding(5),
                Height = 100,
                BorderRadius = 5,
                Margin = new Padding(5)
            };

            RoundedPictureBox pictureBox = new RoundedPictureBox
            {
                Width = 90,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(24, 27, 31),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderRadiusBottomLeft = 10,
                BorderRadiusBottomRight = 10,
                BorderRadiusTopLeft = 10,
                BorderRadiusTopRight = 10,
            };
            pictureBox.Image = typeWithImage.Image;
            panel.Controls.Add(pictureBox);

            Label labelName = new Label
            {
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Text = typeWithImage.Type,
                Width = 100,
                Height = 150
            };
            panel.Controls.Add(labelName);

            EventHandler eventEnter = (sender, e) => panel.BackColor = Color.FromArgb(34, 37, 41);
            EventHandler eventLeave = (sender, e) => panel.BackColor = Color.FromArgb(24, 27, 31);

            panel.MouseEnter += eventEnter;
            pictureBox.MouseEnter += eventEnter;
            labelName.MouseEnter += eventEnter;

            panel.MouseLeave += eventLeave;
            pictureBox.MouseLeave += eventLeave;
            labelName.MouseLeave += eventLeave;

            EventHandler eventClick = (sender, e) =>
            {
                emojiOpenTypePanel.Text = typeWithImage.Type;
                emojiTypePanel.Visible = false;
            };

            panel.Click += eventClick;
            pictureBox.Click += eventClick;
            labelName.Click += eventClick;

            emojiTypePanel.Controls.Add(panel);
        }
    }
}
