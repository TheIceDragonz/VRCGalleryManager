using VRCGalleryManager.Core;
using VRCGalleryManager.Design;
using static VRCGalleryManager.Core.EmojiType;

namespace VRCGalleryManager.Forms.Panels
{
    public class TypePanel
    {
        private class EmojiPanelTag
        {
            public Button ButtonTypePanel { get; set; }
            public Panel TypePanel { get; set; }
            public TypeWithImage TypeWithImage { get; set; }
        }

        private static void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control c)
            {
                Panel panel = c as Panel ?? c.Parent as Panel;
                if (panel != null)
                    panel.BackColor = System.Drawing.Color.FromArgb(34, 37, 41);
            }
        }

        private static void Control_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Control c)
            {
                Panel panel = c as Panel ?? c.Parent as Panel;
                if (panel != null)
                    panel.BackColor = System.Drawing.Color.FromArgb(24, 27, 31);
            }
        }

        private static void Control_Click(object sender, EventArgs e)
        {
            if (sender is Control c)
            {
                Panel panel = c as Panel ?? c.Parent as Panel;
                if (panel != null && panel.Tag is EmojiPanelTag tag)
                {
                    tag.ButtonTypePanel.Text = tag.TypeWithImage.Type;
                    ClearEmojiType(tag.TypePanel);
                    tag.TypePanel.Visible = false;
                }
            }
        }

        public static void LoadEmojiType(Button buttonTypePanel, Panel typePanel)
        {
            typePanel.SuspendLayout();
            EmojiType emojiType = new EmojiType();
            foreach (var typeWithImage in emojiType.TypesWithImages)
                AddEmojiTypePanel(buttonTypePanel, typePanel, typeWithImage);
            typePanel.ResumeLayout();
        }

        public static void ClearEmojiType(Panel typePanel)
        {
            foreach (Control ctrl in typePanel.Controls)
                DisposeControl(ctrl);
            typePanel.Controls.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private static void DisposeControl(Control ctrl)
        {
            ctrl.MouseEnter -= Control_MouseEnter;
            ctrl.MouseLeave -= Control_MouseLeave;
            ctrl.Click -= Control_Click;
            foreach (Control child in ctrl.Controls)
                DisposeControl(child);
            if (ctrl is PictureBox pb && pb.Image != null)
            {
                pb.Image.Dispose();
                pb.Image = null;
            }
            ctrl.Tag = null;
            ctrl.Dispose();
        }

        private static void AddEmojiTypePanel(Button buttonTypePanel, Panel typePanel, TypeWithImage typeWithImage)
        {
            RoundedPanel panel = new RoundedPanel
            {
                Dock = System.Windows.Forms.DockStyle.Top,
                BackColor = System.Drawing.Color.FromArgb(24, 27, 31),
                Padding = new System.Windows.Forms.Padding(5),
                Height = 100,
                BorderRadius = 15,
                Margin = new System.Windows.Forms.Padding(5)
            };

            RoundedPictureBox pictureBox = new RoundedPictureBox
            {
                Width = 90,
                Dock = System.Windows.Forms.DockStyle.Left,
                BackColor = System.Drawing.Color.FromArgb(24, 27, 31),
                SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage,
                BorderRadiusBottomLeft = 10,
                BorderRadiusBottomRight = 10,
                BorderRadiusTopLeft = 10,
                BorderRadiusTopRight = 10,
                Image = (Image)typeWithImage.Image.Clone()
            };
            panel.Controls.Add(pictureBox);

            System.Windows.Forms.Label labelName = new System.Windows.Forms.Label
            {
                Dock = System.Windows.Forms.DockStyle.Right,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                Text = typeWithImage.Type
            };
            panel.Controls.Add(labelName);

            panel.Tag = new EmojiPanelTag
            {
                ButtonTypePanel = buttonTypePanel,
                TypePanel = typePanel,
                TypeWithImage = typeWithImage
            };

            panel.MouseEnter += Control_MouseEnter;
            pictureBox.MouseEnter += Control_MouseEnter;
            labelName.MouseEnter += Control_MouseEnter;

            panel.MouseLeave += Control_MouseLeave;
            pictureBox.MouseLeave += Control_MouseLeave;
            labelName.MouseLeave += Control_MouseLeave;

            panel.Click += Control_Click;
            pictureBox.Click += Control_Click;
            labelName.Click += Control_Click;

            typePanel.Controls.Add(panel);
        }
    }
}
