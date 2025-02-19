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

        [Category("RoundedLabel")]
        public int BorderSize
        {
            get { return borderSize; }
            set { borderSize = value; Invalidate(); }
        }

        [Category("RoundedLabel")]
        public int BorderRadiusTopLeft
        {
            get { return borderRadiusTopLeft; }
            set { borderRadiusTopLeft = Math.Max(0, Math.Min(value, Math.Min(Width, Height) / 2)); Invalidate(); }
        }

        [Category("RoundedLabel")]
        public int BorderRadiusTopRight
        {
            get { return borderRadiusTopRight; }
            set { borderRadiusTopRight = Math.Max(0, Math.Min(value, Math.Min(Width, Height) / 2)); Invalidate(); }
        }

        [Category("RoundedLabel")]
        public int BorderRadiusBottomLeft
        {
            get { return borderRadiusBottomLeft; }
            set { borderRadiusBottomLeft = Math.Max(0, Math.Min(value, Math.Min(Width, Height) / 2)); Invalidate(); }
        }

        [Category("RoundedLabel")]
        public int BorderRadiusBottomRight
        {
            get { return borderRadiusBottomRight; }
            set { borderRadiusBottomRight = Math.Max(0, Math.Min(value, Math.Min(Width, Height) / 2)); Invalidate(); }
        }

        [Category("RoundedLabel")]
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; Invalidate(); }
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

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rectClient = ClientRectangle;
            // Utilizziamo l'intero rettangolo del controllo, senza padding.
            Rectangle rectBackground = rectClient;
            Rectangle rectBorder = Rectangle.Inflate(rectBackground, -borderSize, -borderSize);

            using (GraphicsPath pathBackground = GetFigurePath(rectBackground))
            using (GraphicsPath pathBorder = GetFigurePath(rectBorder))
            using (Pen penBorder = new Pen(borderColor, borderSize))
            {
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    e.Graphics.FillPath(brush, pathBackground);
                }
                if (borderSize > 0)
                    e.Graphics.DrawPath(penBorder, pathBorder);
            }
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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
            path.AddArc(rect.X, rect.Y, tl * 2, tl * 2, 180, 90);
            path.AddArc(rect.Right - tr * 2, rect.Y, tr * 2, tr * 2, 270, 90);
            path.AddArc(rect.Right - br * 2, rect.Bottom - br * 2, br * 2, br * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - bl * 2, bl * 2, bl * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Region = new Region(GetFigurePath(ClientRectangle));
            Invalidate();
        }
    }
}
