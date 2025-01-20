using VRCGalleryManager.Core;
using VRCGalleryManager.Design;
using static VRCGalleryManager.Core.EmojiType;

namespace VRCGalleryManager.Forms.Panels
{
    public class TypePanel
    {
        static public void LoadEmojiType(Button buttonTypePanel, Panel typePanel)
        {
            EmojiType emojiType = new EmojiType();

            foreach (var typeWithImage in emojiType.TypesWithImages)
            {
                AddEmojiTypePanel(buttonTypePanel, typePanel, typeWithImage);
            }
        }

        static private void AddEmojiTypePanel(Button buttonTypePanel, Panel typePanel, TypeWithImage typeWithImage)
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
                buttonTypePanel.Text = typeWithImage.Type;
                typePanel.Visible = false;
            };

            panel.Click += eventClick;
            pictureBox.Click += eventClick;
            labelName.Click += eventClick;

            typePanel.Controls.Add(panel);
        }
    }
}
