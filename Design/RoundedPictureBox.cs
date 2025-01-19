using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VRCGalleryManager.Design
{
    public class RoundedPictureBox : PictureBox
    {
        private int borderSize = 0;
        private int borderRadiusTopLeft = 20;
        private int borderRadiusTopRight = 20;
        private int borderRadiusBottomLeft = 20;
        private int borderRadiusBottomRight = 20;
        private Color borderColor = Color.PaleVioletRed;

        [Category("VRCGalleryManager")]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                borderSize = value;
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public int BorderRadiusTopLeft
        {
            get { return borderRadiusTopLeft; }
            set
            {
                borderRadiusTopLeft = Math.Max(0, Math.Min(value, Math.Min(Width, Height) / 2));
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public int BorderRadiusTopRight
        {
            get { return borderRadiusTopRight; }
            set
            {
                borderRadiusTopRight = Math.Max(0, Math.Min(value, Math.Min(Width, Height) / 2));
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public int BorderRadiusBottomLeft
        {
            get { return borderRadiusBottomLeft; }
            set
            {
                borderRadiusBottomLeft = Math.Max(0, Math.Min(value, Math.Min(Width, Height) / 2));
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public int BorderRadiusBottomRight
        {
            get { return borderRadiusBottomRight; }
            set
            {
                borderRadiusBottomRight = Math.Max(0, Math.Min(value, Math.Min(Width, Height) / 2));
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public Color BackgroundColor
        {
            get { return BackColor; }
            set { BackColor = value; }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Rectangle rectSurface = ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -borderSize + 2, -borderSize + 2);

            if (rectBorder.Width <= 0 || rectBorder.Height <= 0)
                return;

            int smoothSize = 2;
            if (borderSize > 0)
                smoothSize = Math.Max(2, borderSize);

            Rectangle rectContent = new Rectangle(
                rectSurface.X + Padding.Left,
                rectSurface.Y + Padding.Top,
                rectSurface.Width - Padding.Horizontal,
                rectSurface.Height - Padding.Vertical
            );

            if (Image != null)
            {
                using (GraphicsPath imagePath = GetFigurePath(rectSurface))
                using (Region clipRegion = new Region(imagePath))
                {
                    pevent.Graphics.SetClip(clipRegion, CombineMode.Replace);
                    pevent.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    pevent.Graphics.DrawImage(Image, rectContent);
                    pevent.Graphics.ResetClip();
                }
            }

            using (GraphicsPath pathSurface = GetFigurePath(rectSurface))
            using (GraphicsPath pathBorder = GetFigurePath(rectBorder))
            using (Pen penSurface = new Pen(Parent.BackColor, smoothSize))
            using (Pen penBorder = new Pen(borderColor, borderSize))
            {
                Region = new Region(pathSurface);
                pevent.Graphics.DrawPath(penSurface, pathSurface);

                if (borderSize >= 1)
                    pevent.Graphics.DrawPath(penBorder, pathBorder);
            }
        }



        public RoundedPictureBox()
        {
            Size = new Size(150, 150);
            SizeMode = PictureBoxSizeMode.StretchImage;
            BackColor = Color.MediumSlateBlue;
        }

        private GraphicsPath GetFigurePath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            int topLeft = Math.Min(borderRadiusTopLeft, Math.Min(rect.Width, rect.Height) / 2);
            int topRight = Math.Min(borderRadiusTopRight, Math.Min(rect.Width, rect.Height) / 2);
            int bottomLeft = Math.Min(borderRadiusBottomLeft, Math.Min(rect.Width, rect.Height) / 2);
            int bottomRight = Math.Min(borderRadiusBottomRight, Math.Min(rect.Width, rect.Height) / 2);

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, topLeft * 2, topLeft * 2, 180, 90);
            path.AddArc(rect.Right - (topRight * 2), rect.Y, topRight * 2, topRight * 2, 270, 90);
            path.AddArc(rect.Right - (bottomRight * 2), rect.Bottom - (bottomRight * 2), bottomRight * 2, bottomRight * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - (bottomLeft * 2), bottomLeft * 2, bottomLeft * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Parent.BackColorChanged += new EventHandler(Container_BackColorChanged);
        }

        private void Container_BackColorChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
