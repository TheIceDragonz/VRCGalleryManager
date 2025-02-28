using System.ComponentModel;
using System.Drawing.Drawing2D;

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

        [Category("VRCGalleryManager")]
        [DefaultValue(false)]
        public bool UseMaxRoundness { get; set; }

        public RoundedPictureBox()
        {
            Size = new Size(150, 150);
            SizeMode = PictureBoxSizeMode.StretchImage;
            BackColor = Color.MediumSlateBlue;
            UseMaxRoundness = false;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
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
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Region = new Region(pathSurface);
                pevent.Graphics.DrawPath(penSurface, pathSurface);
                if (borderSize >= 1)
                    pevent.Graphics.DrawPath(penBorder, pathBorder);
            }
        }

        private GraphicsPath GetFigurePath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            if (UseMaxRoundness)
            {
                int maxRadius = Math.Min(rect.Width, rect.Height) / 2;
                if (maxRadius > 0)
                {
                    path.StartFigure();
                    path.AddArc(rect.X, rect.Y, maxRadius * 2, maxRadius * 2, 180, 90);
                    path.AddArc(rect.Right - maxRadius * 2, rect.Y, maxRadius * 2, maxRadius * 2, 270, 90);
                    path.AddArc(rect.Right - maxRadius * 2, rect.Bottom - maxRadius * 2, maxRadius * 2, maxRadius * 2, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - maxRadius * 2, maxRadius * 2, maxRadius * 2, 90, 90);
                    path.CloseFigure();
                }
                else
                {
                    path.AddRectangle(rect);
                }
                return path;
            }
            int tl = Math.Min(borderRadiusTopLeft, Math.Min(rect.Width, rect.Height) / 2);
            int tr = Math.Min(borderRadiusTopRight, Math.Min(rect.Width, rect.Height) / 2);
            int br = Math.Min(borderRadiusBottomRight, Math.Min(rect.Width, rect.Height) / 2);
            int bl = Math.Min(borderRadiusBottomLeft, Math.Min(rect.Width, rect.Height) / 2);
            if (tl == 0 && tr == 0 && br == 0 && bl == 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            path.StartFigure();
            if (tl > 0)
                path.AddArc(new Rectangle(rect.X, rect.Y, tl * 2, tl * 2), 180, 90);
            else
                path.AddLine(new Point(rect.X, rect.Y), new Point(rect.X, rect.Y));
            path.AddLine(new Point(rect.X + tl, rect.Y), new Point(rect.Right - tr, rect.Y));
            if (tr > 0)
                path.AddArc(new Rectangle(rect.Right - tr * 2, rect.Y, tr * 2, tr * 2), 270, 90);
            else
                path.AddLine(new Point(rect.Right, rect.Y), new Point(rect.Right, rect.Y));
            path.AddLine(new Point(rect.Right, rect.Y + tr), new Point(rect.Right, rect.Bottom - br));
            if (br > 0)
                path.AddArc(new Rectangle(rect.Right - br * 2, rect.Bottom - br * 2, br * 2, br * 2), 0, 90);
            else
                path.AddLine(new Point(rect.Right, rect.Bottom), new Point(rect.Right, rect.Bottom));
            path.AddLine(new Point(rect.Right - br, rect.Bottom), new Point(rect.X + bl, rect.Bottom));
            if (bl > 0)
                path.AddArc(new Rectangle(rect.X, rect.Bottom - bl * 2, bl * 2, bl * 2), 90, 90);
            else
                path.AddLine(new Point(rect.X, rect.Bottom), new Point(rect.X, rect.Bottom));
            path.AddLine(new Point(rect.X, rect.Bottom - bl), new Point(rect.X, rect.Y + tl));
            path.CloseFigure();
            return path;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Parent.BackColorChanged += Container_BackColorChanged;
        }

        private void Container_BackColorChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (Parent != null)
                Parent.BackColorChanged -= Container_BackColorChanged;
            base.OnHandleDestroyed(e);
        }
    }
}
