using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace VRCGalleryManager.Design
{
    public class RoundedLabel : Label
    {
        private int borderSize = 1;
        private int borderRadiusTopLeft = 20;
        private int borderRadiusTopRight = 20;
        private int borderRadiusBottomLeft = 20;
        private int borderRadiusBottomRight = 20;
        private Color borderColor = Color.PaleVioletRed;

        // ── Cache fields for optimization ────────────────────────────────────────
        private GraphicsPath _cachedPathBackground = null;
        private GraphicsPath _cachedPathBorder = null;

        private Size _lastPathSize;
        private int _lastPathBorderSize;
        private int _lastPathTl, _lastPathTr, _lastPathBl, _lastPathBr;

        private Size _lastRegionSize;
        private int _lastRegionTl, _lastRegionTr, _lastRegionBl, _lastRegionBr;

        [Category("VRCGalleryManager")]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                if (borderSize != value)
                {
                    borderSize = value;
                    UpdateRegion();
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public int BorderRadiusTopLeft
        {
            get { return borderRadiusTopLeft; }
            set
            {
                int val = Math.Max(0, value);
                if (borderRadiusTopLeft != val)
                {
                    borderRadiusTopLeft = val;
                    UpdateRegion();
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public int BorderRadiusTopRight
        {
            get { return borderRadiusTopRight; }
            set
            {
                int val = Math.Max(0, value);
                if (borderRadiusTopRight != val)
                {
                    borderRadiusTopRight = val;
                    UpdateRegion();
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public int BorderRadiusBottomLeft
        {
            get { return borderRadiusBottomLeft; }
            set
            {
                int val = Math.Max(0, value);
                if (borderRadiusBottomLeft != val)
                {
                    borderRadiusBottomLeft = val;
                    UpdateRegion();
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public int BorderRadiusBottomRight
        {
            get { return borderRadiusBottomRight; }
            set
            {
                int val = Math.Max(0, value);
                if (borderRadiusBottomRight != val)
                {
                    borderRadiusBottomRight = val;
                    UpdateRegion();
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                if (borderColor != value)
                {
                    borderColor = value;
                    Invalidate();
                }
            }
        }

        public RoundedLabel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            BackColor = Color.MediumSlateBlue;
            ForeColor = Color.White;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (Parent != null)
                e.Graphics.Clear(Parent.BackColor);
            else
                base.OnPaintBackground(e);
        }

        private void UpdateRegion()
        {
            Rectangle rectSurface = ClientRectangle;
            if (rectSurface.Width <= 0 || rectSurface.Height <= 0)
                return;

            if (_lastRegionSize == rectSurface.Size &&
                _lastRegionTl == borderRadiusTopLeft &&
                _lastRegionTr == borderRadiusTopRight &&
                _lastRegionBl == borderRadiusBottomLeft &&
                _lastRegionBr == borderRadiusBottomRight &&
                this.Region != null)
            {
                return;
            }

            _lastRegionSize = rectSurface.Size;
            _lastRegionTl = borderRadiusTopLeft;
            _lastRegionTr = borderRadiusTopRight;
            _lastRegionBl = borderRadiusBottomLeft;
            _lastRegionBr = borderRadiusBottomRight;

            using (GraphicsPath pathSurface = GetFigurePath(rectSurface))
            {
                Region oldRegion = this.Region;
                this.Region = new Region(pathSurface);
                oldRegion?.Dispose();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rectClient = ClientRectangle;
            Rectangle rectBackground = rectClient;
            Rectangle rectBorder = Rectangle.Inflate(rectBackground, -borderSize, -borderSize);

            if (_cachedPathBackground == null || _cachedPathBorder == null ||
                _lastPathSize != rectClient.Size ||
                _lastPathBorderSize != borderSize ||
                _lastPathTl != borderRadiusTopLeft ||
                _lastPathTr != borderRadiusTopRight ||
                _lastPathBl != borderRadiusBottomLeft ||
                _lastPathBr != borderRadiusBottomRight)
            {
                _cachedPathBackground?.Dispose();
                _cachedPathBorder?.Dispose();

                _cachedPathBackground = GetFigurePath(rectBackground);
                _cachedPathBorder = GetFigurePath(rectBorder);

                _lastPathSize = rectClient.Size;
                _lastPathBorderSize = borderSize;
                _lastPathTl = borderRadiusTopLeft;
                _lastPathTr = borderRadiusTopRight;
                _lastPathBl = borderRadiusBottomLeft;
                _lastPathBr = borderRadiusBottomRight;
            }

            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillPath(brush, _cachedPathBackground);
            }
            if (borderSize > 0)
            {
                using (Pen penBorder = new Pen(borderColor, borderSize))
                {
                    e.Graphics.DrawPath(penBorder, _cachedPathBorder);
                }
            }
            // Map TextAlign to TextFormatFlags
            TextFormatFlags flags = TextFormatFlags.WordBreak;
            switch (TextAlign)
            {
                case ContentAlignment.TopLeft:
                    flags |= TextFormatFlags.Top | TextFormatFlags.Left;
                    break;
                case ContentAlignment.TopCenter:
                    flags |= TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.TopRight:
                    flags |= TextFormatFlags.Top | TextFormatFlags.Right;
                    break;
                case ContentAlignment.MiddleLeft:
                    flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                    break;
                case ContentAlignment.MiddleCenter:
                    flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.MiddleRight:
                    flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
                    break;
                case ContentAlignment.BottomLeft:
                    flags |= TextFormatFlags.Bottom | TextFormatFlags.Left;
                    break;
                case ContentAlignment.BottomCenter:
                    flags |= TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.BottomRight:
                    flags |= TextFormatFlags.Bottom | TextFormatFlags.Right;
                    break;
            }

            Rectangle textRect = new Rectangle(
                rectClient.X + Padding.Left,
                rectClient.Y + Padding.Top,
                rectClient.Width - (Padding.Left + Padding.Right),
                rectClient.Height - (Padding.Top + Padding.Bottom)
            );

            TextRenderer.DrawText(e.Graphics, Text, Font, textRect, ForeColor, flags);
        }

        private GraphicsPath GetFigurePath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            int maxRadius = Math.Min(rect.Width, rect.Height) / 2;

            int tl = Math.Min(borderRadiusTopLeft, maxRadius);
            int tr = Math.Min(borderRadiusTopRight, maxRadius);
            int br = Math.Min(borderRadiusBottomRight, maxRadius);
            int bl = Math.Min(borderRadiusBottomLeft, maxRadius);

            path.StartFigure();

            if (tl > 0) path.AddArc(rect.X, rect.Y, tl * 2, tl * 2, 180, 90);
            else path.AddLine(rect.X, rect.Y, rect.X, rect.Y);
            if (tr > 0) path.AddArc(rect.Right - tr * 2, rect.Y, tr * 2, tr * 2, 270, 90);
            else path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);
            if (br > 0) path.AddArc(rect.Right - br * 2, rect.Bottom - br * 2, br * 2, br * 2, 0, 90);
            else path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);
            if (bl > 0) path.AddArc(rect.X, rect.Bottom - bl * 2, bl * 2, bl * 2, 90, 90);
            else path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom);

            path.CloseFigure();
            return path;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRegion();
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Region oldRegion = this.Region;
                this.Region = null;
                oldRegion?.Dispose();

                _cachedPathBackground?.Dispose();
                _cachedPathBackground = null;
                _cachedPathBorder?.Dispose();
                _cachedPathBorder = null;
            }
            base.Dispose(disposing);
        }
    }
}
