using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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

        // ── Cache fields for optimization ────────────────────────────────────────
        private GraphicsPath _cachedTrackPath = null;
        private Size _lastTrackSize;
        private int _lastTrackRadius;
        private int _lastTrackPadding;

        private GraphicsPath _cachedThumbPath = null;
        private int _lastThumbSize;

        [Category("VRCGalleryManager")]
        public int BorderRadius
        {
            get => borderRadius;
            set
            {
                int val = Math.Max(0, value);
                if (borderRadius != val)
                {
                    borderRadius = val;
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public int ThumbSize
        {
            get => thumbSize;
            set
            {
                int val = Math.Max(10, value);
                if (thumbSize != val)
                {
                    thumbSize = val;
                    UpdateValueLabelPosition();
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public Point LabelOffset
        {
            get => labelOffset;
            set
            {
                if (labelOffset != value)
                {
                    labelOffset = value;
                    UpdateValueLabelPosition();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public new int Padding
        {
            get => padding;
            set
            {
                int val = Math.Max(0, value);
                if (padding != val)
                {
                    padding = val;
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public Color TrackColor
        {
            get => trackColor;
            set
            {
                if (trackColor != value)
                {
                    trackColor = value;
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public Color ThumbColor
        {
            get => thumbColor;
            set
            {
                if (thumbColor != value)
                {
                    thumbColor = value;
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

            valueLabel.MouseDown += (s, e) => {
                Point clientPt = this.PointToClient(valueLabel.PointToScreen(e.Location));
                MouseEventArgs translated = new MouseEventArgs(e.Button, e.Clicks, clientPt.X, clientPt.Y, e.Delta);
                this.OnMouseDown(translated);
            };
            Controls.Add(valueLabel);
            UpdateValueLabelPosition();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw track
            Rectangle trackRect = new Rectangle(padding, Height / 2 - 4, Width - 2 * padding, 8);
            if (_cachedTrackPath == null || _lastTrackSize != ClientRectangle.Size || _lastTrackRadius != borderRadius || _lastTrackPadding != padding)
            {
                _cachedTrackPath?.Dispose();
                _cachedTrackPath = GetRoundedRectangle(trackRect, borderRadius);
                _lastTrackSize = ClientRectangle.Size;
                _lastTrackRadius = borderRadius;
                _lastTrackPadding = padding;
            }

            using (Brush trackBrush = new SolidBrush(trackColor))
            {
                e.Graphics.FillPath(trackBrush, _cachedTrackPath);
            }

            // Draw thumb
            if (_cachedThumbPath == null || _lastThumbSize != thumbSize)
            {
                _cachedThumbPath?.Dispose();
                Rectangle localThumbRect = new Rectangle(0, 0, thumbSize, thumbSize);
                _cachedThumbPath = GetRoundedRectangle(localThumbRect, thumbSize / 2);
                _lastThumbSize = thumbSize;
            }

            Color currentThumbColor = Enabled ? thumbColor : ControlPaint.Dark(thumbColor);
            Color currentBorderColor = Enabled ? borderColor : ControlPaint.Dark(borderColor);

            using (Brush thumbBrush = new SolidBrush(currentThumbColor))
            {
                int x = ValueToPixel(Value) - thumbSize / 2;
                int y = Height / 2 - thumbSize / 2;

                e.Graphics.TranslateTransform(x, y);
                e.Graphics.FillPath(thumbBrush, _cachedThumbPath);
                using (Pen borderPen = new Pen(currentBorderColor, 2))
                {
                    e.Graphics.DrawPath(borderPen, _cachedThumbPath);
                }
                e.Graphics.TranslateTransform(-x, -y);
            }
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
            if (e.Button == MouseButtons.Left)
            {
                Capture = true;
                int newValue = PixelToValue(e.X);
                if (newValue != Value)
                {
                    Value = newValue;
                    OnScroll(EventArgs.Empty);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Capture && e.Button == MouseButtons.Left)
            {
                int newValue = PixelToValue(e.X);
                if (newValue != Value)
                {
                    Value = newValue;
                    OnScroll(EventArgs.Empty);
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

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (valueLabel != null)
            {
                valueLabel.Enabled = Enabled;
            }
            Invalidate();
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cachedTrackPath?.Dispose();
                _cachedTrackPath = null;
                _cachedThumbPath?.Dispose();
                _cachedThumbPath = null;
            }
            base.Dispose(disposing);
        }
    }
}
