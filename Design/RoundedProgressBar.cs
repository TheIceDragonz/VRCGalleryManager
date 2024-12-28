using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VRCGalleryManager.Design
{
    public class RoundedProgressBar : Control
    {
        private int borderRadius = 15;
        private int progressValue = 0;
        private Color progressColor = Color.MediumSlateBlue;
        private Color backgroundColor = Color.White;

        [Category("VRCGalleryManager")]
        public int BorderRadius
        {
            get => borderRadius;
            set
            {
                borderRadius = value;
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public int ProgressValue
        {
            get => progressValue;
            set
            {
                if (value >= 0 && value <= 100)
                {
                    progressValue = value;
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public Color ProgressColor
        {
            get => progressColor;
            set
            {
                progressColor = value;
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
                Invalidate();
            }
        }

        public RoundedProgressBar()
        {
            Size = new Size(200, 30);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Crea un'immagine bitmap temporanea con le dimensioni del controllo
            using (Bitmap buffer = new Bitmap(ClientSize.Width, ClientSize.Height))
            {
                // Disegna sul buffer anziché direttamente sul controllo
                using (Graphics bufferGraphics = Graphics.FromImage(buffer))
                {
                    bufferGraphics.SmoothingMode = SmoothingMode.AntiAlias;

                    Rectangle rectBase = ClientRectangle;
                    Rectangle rectProgress = new Rectangle(0, 0, (int)(rectBase.Width * ((double)progressValue / 100)), rectBase.Height);

                    using (GraphicsPath pathBase = GetRoundedRectanglePath(rectBase, borderRadius))
                    using (GraphicsPath pathProgress = GetRoundedRectanglePath(rectProgress, borderRadius))
                    using (Pen brushBackground = new Pen(Parent.BackColor))
                    using (Pen brushProgress = new Pen(progressColor))
                    {
                        bufferGraphics.DrawPath(brushBackground, pathBase);
                        bufferGraphics.DrawPath(brushProgress, pathProgress);
                    }
                }

                // Disegna l'immagine bitmap sul controllo
                e.Graphics.DrawImage(buffer, 0, 0);
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float diameter = radius * 2f;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }
}
