using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VRCEmojiManager.Design
{
    public class RoundedPictureBox : PictureBox
    {
        private int borderSize = 0;
        private int borderRadiusTopLeft = 20;
        private int borderRadiusTopRight = 20;
        private int borderRadiusBottomLeft = 20;
        private int borderRadiusBottomRight = 20;
        private Color borderColor = Color.PaleVioletRed;

        [Category("VRCEmojiManager")]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                borderSize = value;
                Invalidate();
            }
        }

        [Category("VRCEmojiManager")]
        public int BorderRadiusTopLeft
        {
            get { return borderRadiusTopLeft; }
            set
            {
                borderRadiusTopLeft = value;
                Invalidate();
            }
        }

        [Category("VRCEmojiManager")]
        public int BorderRadiusTopRight
        {
            get { return borderRadiusTopRight; }
            set
            {
                borderRadiusTopRight = value;
                Invalidate();
            }
        }

        [Category("VRCEmojiManager")]
        public int BorderRadiusBottomLeft
        {
            get { return borderRadiusBottomLeft; }
            set
            {
                borderRadiusBottomLeft = value;
                Invalidate();
            }
        }

        [Category("VRCEmojiManager")]
        public int BorderRadiusBottomRight
        {
            get { return borderRadiusBottomRight; }
            set
            {
                borderRadiusBottomRight = value;
                Invalidate();
            }
        }

        [Category("VRCEmojiManager")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        [Category("VRCEmojiManager")]
        public Color BackgroundColor
        {
            get { return BackColor; }
            set { BackColor = value; }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            Rectangle rectSurface = ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -borderSize, -borderSize);
            int smoothSize = 2;
            if (borderSize > 0)
                smoothSize = borderSize;

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

        public RoundedPictureBox()
        {
            Size = new Size(150, 150);
            SizeMode = PictureBoxSizeMode.StretchImage;
            BackColor = Color.MediumSlateBlue;
        }

        private GraphicsPath GetFigurePath(Rectangle rect)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, borderRadiusTopLeft * 2, borderRadiusTopLeft * 2, 180, 90);
            path.AddArc(rect.Right - (borderRadiusTopRight * 2), rect.Y, borderRadiusTopRight * 2, borderRadiusTopRight * 2, 270, 90);
            path.AddArc(rect.Right - (borderRadiusBottomRight * 2), rect.Bottom - (borderRadiusBottomRight * 2), borderRadiusBottomRight * 2, borderRadiusBottomRight * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - (borderRadiusBottomLeft * 2), borderRadiusBottomLeft * 2, borderRadiusBottomLeft * 2, 90, 90);
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (Width != Height)
            {
                int size = Math.Min(Width, Height);
                this.Size = new Size(size, size);
            }

            Invalidate();
        }

    }
}
