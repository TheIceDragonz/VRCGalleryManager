using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VRCGalleryManager.Design
{
    public class RoundedCheckBox : CheckBox
    {
        private int borderSize = 1;
        private int borderRadius = 5;
        private Color borderColor = Color.Gray;
        private Color checkColor = Color.Green;
        private Color boxFillColor = Color.White;

        [Category("VRCGalleryManager")]
        public int BorderSize
        {
            get => borderSize;
            set { borderSize = value; Invalidate(); }
        }

        [Category("VRCGalleryManager")]
        public int BorderRadius
        {
            get => borderRadius;
            set { borderRadius = value; Invalidate(); }
        }

        [Category("VRCGalleryManager")]
        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; Invalidate(); }
        }

        [Category("VRCGalleryManager")]
        public Color CheckColor
        {
            get => checkColor;
            set { checkColor = value; Invalidate(); }
        }

        [Category("VRCGalleryManager")]
        public Color BoxFillColor
        {
            get => boxFillColor;
            set { boxFillColor = value; Invalidate(); }
        }

        public RoundedCheckBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Size = new Size(150, 25);
            AutoCheck = false;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Imposta il colore di sfondo del controllo (se presente il Parent)
            if (Parent != null)
                g.Clear(Parent.BackColor);
            else
                g.Clear(BackColor);

            // Se il controllo è disabilitato, utilizziamo dei colori più spenti
            Color currentBorderColor = Enabled ? borderColor : ControlPaint.Dark(borderColor);
            Color currentCheckColor = Enabled ? checkColor : ControlPaint.Dark(checkColor);
            Color currentBoxFillColor = Enabled ? boxFillColor : ControlPaint.Light(boxFillColor);
            Color currentForeColor = Enabled ? ForeColor : SystemColors.GrayText;

            // Calcola la dimensione e la posizione della "checkbox"
            int boxSize = Height - 4;
            Rectangle boxRect = new Rectangle(2, 2, boxSize, boxSize);

            // Disegna il rettangolo arrotondato
            using (GraphicsPath path = GetRoundedRectanglePath(boxRect, borderRadius))
            {
                using (SolidBrush brush = new SolidBrush(currentBoxFillColor))
                    g.FillPath(brush, path);
                using (Pen penBorder = new Pen(currentBorderColor, borderSize))
                    g.DrawPath(penBorder, path);
            }

            // Disegna il segno di spunta se Checked
            if (Checked)
            {
                using (Pen penCheck = new Pen(currentCheckColor, 2))
                {
                    g.DrawLine(penCheck,
                        boxRect.Left + 3, boxRect.Top + boxRect.Height / 2,
                        boxRect.Left + boxRect.Width / 2, boxRect.Bottom - 3);
                    g.DrawLine(penCheck,
                        boxRect.Left + boxRect.Width / 2, boxRect.Bottom - 3,
                        boxRect.Right - 3, boxRect.Top + 3);
                }
            }

            // Disegna il testo a destra della "checkbox"
            int textX = boxRect.Right + 5;
            Rectangle textRect = new Rectangle(textX, 0, Width - textX, Height);
            TextRenderer.DrawText(g, Text, Font, textRect, currentForeColor,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            if (radius > 0)
            {
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
            }
            else
            {
                path.AddRectangle(rect);
            }
            return path;
        }

        protected override void OnClick(EventArgs e)
        {
            Checked = !Checked;
            Invalidate();
            base.OnClick(e);
        }
    }
}
