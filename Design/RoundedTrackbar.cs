using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace CustomControls
{
    public class RoundedTrackBar : TrackBar
    {
        private int borderRadius = 5;
        private int thumbSize = 16;
        private int padding = 4;
        private Point labelOffset = Point.Empty;
        private Color trackColor = Color.MediumSlateBlue;
        private Color thumbColor = Color.PaleVioletRed;
        private Color borderColor = Color.Gray;
        private Label valueLabel;

        [Category("VRCGalleryManager")]
        public int BorderRadius
        {
            get => borderRadius;
            set
            {
                borderRadius = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public int ThumbSize
        {
            get => thumbSize;
            set
            {
                thumbSize = Math.Max(10, value);
                Invalidate();
                UpdateValueLabelPosition();
            }
        }

        [Category("VRCGalleryManager")]
        public Point LabelOffset
        {
            get => labelOffset;
            set
            {
                labelOffset = value;
                UpdateValueLabelPosition();
            }
        }

        [Category("VRCGalleryManager")]
        public int Padding
        {
            get => padding;
            set
            {
                padding = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public Color TrackColor
        {
            get => trackColor;
            set
            {
                trackColor = value;
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public Color ThumbColor
        {
            get => thumbColor;
            set
            {
                thumbColor = value;
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public string LabelText
        {
            get => valueLabel.Text;
            set
            {
                valueLabel.Text = value;
                UpdateValueLabelPosition();
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public Font LabelFont
        {
            get => valueLabel.Font;
            set
            {
                valueLabel.Font = value;
                UpdateValueLabelPosition();
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public Color LabelTextColor
        {
            get => valueLabel.ForeColor;
            set
            {
                valueLabel.ForeColor = value;
                Invalidate();
            }
        }

        public RoundedTrackBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            valueLabel = new Label
            {
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter
            };

            valueLabel.MouseDown += (s, e) => OnMouseDown(e);
            Controls.Add(valueLabel);
            UpdateValueLabelPosition();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw track
            Rectangle trackRect = new Rectangle(padding, Height / 2 - 4, Width - 2 * padding, 8);
            using (GraphicsPath trackPath = GetRoundedRectangle(trackRect, borderRadius))
            using (Brush trackBrush = new SolidBrush(trackColor))
            {
                e.Graphics.FillPath(trackBrush, trackPath);
            }

            // Draw thumb
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

            UpdateValueLabelPosition();
        }

        private int ValueToPixel(int value)
        {
            float range = Maximum - Minimum;
            float percentage = (value - Minimum) / range;
            return (int)(percentage * (Width - 2 * padding - thumbSize) + padding + thumbSize / 2);
        }

        private void UpdateValueLabelPosition()
        {
            int thumbX = ValueToPixel(Value) - thumbSize / 2;
            int thumbY = Height / 2 - thumbSize / 2;

            valueLabel.Size = valueLabel.PreferredSize;
            valueLabel.Location = new Point(
                thumbX + thumbSize / 2 - valueLabel.Width / 2 + labelOffset.X,
                thumbY + thumbSize / 2 - valueLabel.Height / 2 + labelOffset.Y
            );
            valueLabel.BringToFront();
        }

        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            Invalidate();
            UpdateValueLabelPosition();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            UpdateValueLabelPosition();
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
            if (e.KeyCode == Keys.Left && Value > Minimum)
            {
                Value--;
            }
            else if (e.KeyCode == Keys.Right && Value < Maximum)
            {
                Value++;
            }
            Invalidate();
            UpdateValueLabelPosition();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            Invalidate();
            UpdateValueLabelPosition();
        }

        private int PixelToValue(int x)
        {
            float range = Maximum - Minimum;
            float percentage = Math.Max(0, Math.Min(1, (float)(x - padding - thumbSize / 2) / (Width - 2 * padding - thumbSize)));
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
