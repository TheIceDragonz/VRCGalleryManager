using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CustomControls
{
    public class RoundedTrackBar : TrackBar
    {
        private int borderRadius = 10;
        private Color trackColor = Color.MediumSlateBlue;
        private Color thumbColor = Color.PaleVioletRed;
        private Color borderColor = Color.Gray;

        [Category("Appearance")]
        public int BorderRadius
        {
            get => borderRadius;
            set
            {
                borderRadius = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color TrackColor
        {
            get => trackColor;
            set
            {
                trackColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color ThumbColor
        {
            get => thumbColor;
            set
            {
                thumbColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        public RoundedTrackBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw track
            Rectangle trackRect = new Rectangle(0, Height / 2 - 4, Width, 8);
            using (GraphicsPath trackPath = GetRoundedRectangle(trackRect, borderRadius))
            using (Brush trackBrush = new SolidBrush(trackColor))
            {
                e.Graphics.FillPath(trackBrush, trackPath);
            }

            // Draw thumb
            int thumbSize = 16;
            Rectangle thumbRect = new Rectangle(ValueToPixel(Value) - thumbSize / 2, Height / 2 - thumbSize / 2, thumbSize, thumbSize);
            using (GraphicsPath thumbPath = GetRoundedRectangle(thumbRect, thumbSize / 2))
            using (Brush thumbBrush = new SolidBrush(thumbColor))
            {
                e.Graphics.FillPath(thumbBrush, thumbPath);
                using (Pen borderPen = new Pen(borderColor, 2))
                {
                    e.Graphics.DrawPath(borderPen, thumbPath);
                }
            }
        }

        private int ValueToPixel(int value)
        {
            float range = Maximum - Minimum;
            float percentage = (value - Minimum) / range;
            return (int)(percentage * (Width - 16) + 8); // Adjust for thumb size
        }

        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            Invalidate(); // Redraw the control when the value changes
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Capture = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Capture && e.Button == MouseButtons.Left)
            {
                int newValue = PixelToValue(e.X);
                if (newValue != Value)
                {
                    Value = newValue;
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Capture = false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            Invalidate(); // Ensure control is redrawn when using keyboard arrows
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            Invalidate(); // Ensure control is redrawn when using keyboard arrows
        }

        private int PixelToValue(int x)
        {
            float range = Maximum - Minimum;
            float percentage = Math.Max(0, Math.Min(1, (float)(x - 8) / (Width - 16)));
            return (int)(percentage * range) + Minimum;
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2;
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
