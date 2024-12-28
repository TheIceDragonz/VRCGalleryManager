using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VRCGalleryManager.Design
{
    public class CircularButton : Button
    {
        private int borderSize = 0;
        private Color borderColor = Color.PaleVioletRed;

        [Category("Custom Controls")]
        public int BorderSize
        {
            get => borderSize;
            set
            {
                borderSize = value;
                Invalidate();
            }
        }

        [Category("Custom Controls")]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        [Category("Custom Controls")]
        public Color BackgroundColor
        {
            get => BackColor;
            set => BackColor = value;
        }

        [Category("Custom Controls")]
        public Color TextColor
        {
            get => ForeColor;
            set => ForeColor = value;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            var rectSurface = ClientRectangle;
            var rectBorder = Rectangle.Inflate(rectSurface, -borderSize, -borderSize);

            using (var pathSurface = new GraphicsPath())
            using (var pathBorder = new GraphicsPath())
            using (var penSurface = new Pen(Parent.BackColor, borderSize))
            using (var penBorder = new Pen(borderColor, borderSize))
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                pathSurface.AddEllipse(rectSurface);
                Region = new Region(pathSurface);

                pevent.Graphics.DrawEllipse(penSurface, rectSurface);

                if (borderSize > 0)
                {
                    pathBorder.AddEllipse(rectBorder);
                    pevent.Graphics.DrawPath(penBorder, pathBorder);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Width = Height;
        }

        public CircularButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Size = new Size(100, 100);
            BackColor = Color.MediumSlateBlue;
            ForeColor = Color.White;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Parent.BackColorChanged += Container_BackColorChanged;
            Cursor = Cursors.Hand;
        }

        private void Container_BackColorChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
