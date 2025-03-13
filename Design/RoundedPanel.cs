using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VRCGalleryManager.Design
{
    public class RoundedPanel : Panel
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
                    borderRadius = value;
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
            set => BackColor = value;
        }

        public RoundedPanel()
        {
            Size = new Size(150, 150);
            BackColor = Color.MediumSlateBlue;
            DoubleBuffered = true;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Region = null;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int extraWidth = (AutoScroll && (DisplayRectangle.Height > ClientSize.Height)) ? 17 : 0;
            Rectangle rectSurface = new Rectangle(0, 0, ClientRectangle.Width + extraWidth, ClientRectangle.Height);
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -borderSize, -borderSize);
            int smoothSize = borderSize > 0 ? borderSize : 2;
            int effectiveRadius = Math.Min(borderRadius, Math.Min(rectSurface.Width, rectSurface.Height) / 2);

            if (effectiveRadius > 2)
            {
                using (GraphicsPath pathSurface = GetFigurePath(rectSurface, effectiveRadius))
                using (GraphicsPath pathBorder = GetFigurePath(rectBorder, effectiveRadius - borderSize))
                using (Pen penSurface = new Pen(Parent?.BackColor ?? Color.Transparent, smoothSize))
                using (Pen penBorder = new Pen(borderColor, borderSize))
                {
                    Region = new Region(pathSurface);
                    pevent.Graphics.DrawPath(penSurface, pathSurface);
                    if (borderSize >= 1)
                        pevent.Graphics.DrawPath(penBorder, pathBorder);
                }
            }
            else
            {
                Region = new Region(rectSurface);
                if (borderSize >= 1)
                {
                    using (Pen penBorder = new Pen(borderColor, borderSize))
                    {
                        penBorder.Alignment = PenAlignment.Inset;
                        pevent.Graphics.DrawRectangle(penBorder, 0, 0, rectSurface.Width - 1, rectSurface.Height - 1);
                    }
                }
            }
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
    }
}
