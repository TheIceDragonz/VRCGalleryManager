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

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            Rectangle rectSurface = ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -borderSize, -borderSize);
            int smoothSize = borderSize > 0 ? borderSize : 2;

            if (borderRadius > 2)
            {
                using (GraphicsPath pathSurface = GetFigurePath(rectSurface, borderRadius))
                using (GraphicsPath pathBorder = GetFigurePath(rectBorder, borderRadius - borderSize))
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
            else
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.None;

                Region = new Region(rectSurface);

                pevent.Graphics.FillRectangle(new SolidBrush(BackColor), rectSurface);

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
            Resize += Panel_Resize;
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

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Parent.BackColorChanged += Container_BackColorChanged;
        }

        private void Container_BackColorChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void Panel_Resize(object sender, EventArgs e)
        {
            borderRadius = Math.Min(borderRadius, Height);
        }
    }
}
