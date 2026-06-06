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
        private bool useMaxRoundness = false;

        // ── Cache fields for optimization ────────────────────────────────────────
        private GraphicsPath _cachedPathSurface = null;
        private GraphicsPath _cachedPathBorder = null;
        private Region _cachedClipRegion = null;

        private Size _lastPathSize;
        private int _lastPathBorderSize;
        private int _lastPathTl, _lastPathTr, _lastPathBl, _lastPathBr;
        private bool _lastPathMaxRoundness;

        private Size _lastRegionSize;
        private int _lastRegionTl, _lastRegionTr, _lastRegionBl, _lastRegionBr;
        private bool _lastRegionMaxRoundness;

        private InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic;

        private static readonly object EventPaint = typeof(Control)
            .GetField("EventPaint", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            ?.GetValue(null);

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

        [Category("VRCGalleryManager")]
        public Color BackgroundColor
        {
            get { return BackColor; }
            set
            {
                if (BackColor != value)
                {
                    BackColor = value;
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        [DefaultValue(false)]
        public bool UseMaxRoundness
        {
            get { return useMaxRoundness; }
            set
            {
                if (useMaxRoundness != value)
                {
                    useMaxRoundness = value;
                    UpdateRegion();
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        [DefaultValue(InterpolationMode.HighQualityBicubic)]
        public InterpolationMode InterpolationMode
        {
            get { return interpolationMode; }
            set
            {
                if (interpolationMode != value)
                {
                    interpolationMode = value;
                    Invalidate();
                }
            }
        }

        public RoundedPictureBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            Size = new Size(150, 150);
            SizeMode = PictureBoxSizeMode.StretchImage;
            BackColor = Color.MediumSlateBlue;
            useMaxRoundness = false;
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
                _lastRegionMaxRoundness == useMaxRoundness &&
                this.Region != null)
            {
                return; // Region already matches, skip allocation
            }

            _lastRegionSize = rectSurface.Size;
            _lastRegionTl = borderRadiusTopLeft;
            _lastRegionTr = borderRadiusTopRight;
            _lastRegionBl = borderRadiusBottomLeft;
            _lastRegionBr = borderRadiusBottomRight;
            _lastRegionMaxRoundness = useMaxRoundness;

            using (GraphicsPath pathSurface = GetFigurePath(rectSurface))
            {
                Region oldRegion = this.Region;
                this.Region = new Region(pathSurface);
                oldRegion?.Dispose();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateRegion();
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

            // Update cached paths and clip region if any parameters change
            if (_cachedPathSurface == null || _cachedPathBorder == null || _cachedClipRegion == null ||
                _lastPathSize != rectSurface.Size ||
                _lastPathBorderSize != borderSize ||
                _lastPathTl != borderRadiusTopLeft ||
                _lastPathTr != borderRadiusTopRight ||
                _lastPathBl != borderRadiusBottomLeft ||
                _lastPathBr != borderRadiusBottomRight ||
                _lastPathMaxRoundness != useMaxRoundness)
            {
                _cachedPathSurface?.Dispose();
                _cachedPathBorder?.Dispose();
                _cachedClipRegion?.Dispose();

                _cachedPathSurface = GetFigurePath(rectSurface);
                _cachedPathBorder = GetFigurePath(rectBorder);
                _cachedClipRegion = new Region(_cachedPathSurface);

                _lastPathSize = rectSurface.Size;
                _lastPathBorderSize = borderSize;
                _lastPathTl = borderRadiusTopLeft;
                _lastPathTr = borderRadiusTopRight;
                _lastPathBl = borderRadiusBottomLeft;
                _lastPathBr = borderRadiusBottomRight;
                _lastPathMaxRoundness = useMaxRoundness;
            }

            bool isAnimated = Image != null && ImageAnimator.CanAnimate(Image);

            if (isAnimated)
            {
                base.OnPaint(pevent);
            }
            else
            {
                if (Image != null)
                {
                    pevent.Graphics.SetClip(_cachedClipRegion, CombineMode.Replace);
                    pevent.Graphics.InterpolationMode = interpolationMode;
                    pevent.Graphics.DrawImage(Image, rectContent);
                    pevent.Graphics.ResetClip();
                }
            }

            Color parentColor = Parent != null ? Parent.BackColor : Color.Transparent;
            using (Pen penSurface = new Pen(parentColor, smoothSize))
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                pevent.Graphics.DrawPath(penSurface, _cachedPathSurface);
            }
            if (borderSize >= 1)
            {
                using (Pen penBorder = new Pen(borderColor, borderSize))
                {
                    pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    pevent.Graphics.DrawPath(penBorder, _cachedPathBorder);
                }
            }

            // Manually raise the Paint event only if we bypassed base.OnPaint
            if (!isAnimated && EventPaint != null)
            {
                var paintHandler = Events[EventPaint] as PaintEventHandler;
                paintHandler?.Invoke(this, pevent);
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

        private Control _observedParent;

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            SetupParentEvent();
        }

        private void SetupParentEvent()
        {
            if (_observedParent != null)
            {
                _observedParent.BackColorChanged -= Container_BackColorChanged;
            }
            _observedParent = Parent;
            if (_observedParent != null)
            {
                _observedParent.BackColorChanged += Container_BackColorChanged;
            }
        }

        private void Container_BackColorChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_observedParent != null)
                {
                    _observedParent.BackColorChanged -= Container_BackColorChanged;
                    _observedParent = null;
                }

                Region oldRegion = this.Region;
                this.Region = null;
                oldRegion?.Dispose();

                _cachedPathSurface?.Dispose();
                _cachedPathSurface = null;
                _cachedPathBorder?.Dispose();
                _cachedPathBorder = null;
                _cachedClipRegion?.Dispose();
                _cachedClipRegion = null;
            }
            base.Dispose(disposing);
        }
    }
}
