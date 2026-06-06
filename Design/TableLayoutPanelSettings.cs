using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace VRCGalleryManager.Design
{
    public class TableLayoutPanelSettings : TableLayoutPanel
    {
        private int borderSize = 0;
        private int borderRadius = 15;
        private Color borderColor = Color.PaleVioletRed;

        [Category("VRCGalleryManager")]
        public int BorderSize
        {
            get => borderSize;
            set
            {
                if (borderSize != value)
                {
                    borderSize = value;
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public int BorderRadius
        {
            get => borderRadius;
            set
            {
                if (borderRadius != value)
                {
                    borderRadius = Math.Min(value, Height);
                    UpdateRegion();
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public Color BorderColor
        {
            get => borderColor;
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
            get => BackColor;
            set
            {
                if (BackColor != value)
                {
                    BackColor = value;
                    Invalidate();
                }
            }
        }

        private void UpdateRegion()
        {
            Rectangle rectSurface = ClientRectangle;
            if (rectSurface.Width <= 0 || rectSurface.Height <= 0)
                return;

            Region oldRegion = this.Region;
            if (borderRadius > 2)
            {
                using (GraphicsPath pathSurface = GetFigurePath(rectSurface, borderRadius))
                {
                    this.Region = new Region(pathSurface);
                }
            }
            else
            {
                this.Region = new Region(rectSurface);
            }
            oldRegion?.Dispose();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            Rectangle rectSurface = ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -borderSize, -borderSize);
            int smoothSize = borderSize > 0 ? borderSize : 2;
            Color parentColor = Parent?.BackColor ?? Color.Transparent;

            if (borderRadius > 2)
            {
                using (GraphicsPath pathSurface = GetFigurePath(rectSurface, borderRadius))
                using (GraphicsPath pathBorder = GetFigurePath(rectBorder, borderRadius - borderSize))
                using (Pen penSurface = new Pen(parentColor, smoothSize))
                using (Pen penBorder = new Pen(borderColor, borderSize))
                {
                    pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    pevent.Graphics.DrawPath(penSurface, pathSurface);

                    if (borderSize >= 1)
                        pevent.Graphics.DrawPath(penBorder, pathBorder);
                }
            }
            else
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.None;

                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    pevent.Graphics.FillRectangle(brush, rectSurface);
                }

                if (borderSize >= 1)
                {
                    using (Pen penBorder = new Pen(borderColor, borderSize))
                    {
                        penBorder.Alignment = PenAlignment.Inset;
                        pevent.Graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);
                    }
                }
            }
        }

        public TableLayoutPanelSettings()
        {
            BackColor = Color.MediumSlateBlue;
        }

        private GraphicsPath GetFigurePath(Rectangle rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
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

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateRegion();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            borderRadius = Math.Min(borderRadius, Height);
            UpdateRegion();
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
            }
            base.Dispose(disposing);
        }
    }
}
