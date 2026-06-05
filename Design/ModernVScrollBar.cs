using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VRCGalleryManager.Design
{
    public class ModernVScrollBar : Control
    {
        private int _minimum = 0;
        private int _maximum = 100;
        private int _value = 0;
        private int _largeChange = 10;

        private bool _isDragging = false;
        private int _dragClickOffset = 0;

        private Color _trackColor = Color.FromArgb(7, 36, 43);
        private Color _thumbColor = Color.FromArgb(106, 227, 249);
        private Color _thumbHoverColor = Color.FromArgb(150, 240, 255);

        private bool _isHovered = false;

        public event EventHandler Scroll;

        public int Minimum
        {
            get => _minimum;
            set { _minimum = Math.Max(0, value); Invalidate(); }
        }

        public int Maximum
        {
            get => _maximum;
            set { _maximum = Math.Max(1, value); Invalidate(); }
        }

        public int Value
        {
            get => _value;
            set
            {
                int maxVal = Math.Max(0, _maximum - _largeChange);
                int newValue = Math.Clamp(value, _minimum, maxVal);
                if (_value != newValue)
                {
                    _value = newValue;
                    Invalidate();
                    Scroll?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public int LargeChange
        {
            get => _largeChange;
            set { _largeChange = Math.Max(1, value); Invalidate(); }
        }

        public ModernVScrollBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            Width = 8;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            Invalidate();
        }

        private int GetThumbHeight()
        {
            if (_maximum <= 0) return Height;
            float ratio = (float)_largeChange / _maximum;
            int height = (int)(Height * ratio);
            return Math.Clamp(height, 20, Height);
        }

        private int GetThumbY()
        {
            int maxScroll = _maximum - _largeChange;
            if (maxScroll <= 0) return 0;
            int thumbHeight = GetThumbHeight();
            int trackHeight = Height - thumbHeight;
            float ratio = (float)_value / maxScroll;
            return (int)(ratio * trackHeight);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw track
            using (Brush trackBrush = new SolidBrush(_trackColor))
            {
                using (GraphicsPath trackPath = GetRoundedRectPath(ClientRectangle, Width / 2))
                {
                    g.FillPath(trackBrush, trackPath);
                }
            }

            // Draw thumb
            int thumbHeight = GetThumbHeight();
            int thumbY = GetThumbY();
            Rectangle thumbRect = new Rectangle(0, thumbY, Width, thumbHeight);

            Color currentThumbColor = _isDragging || _isHovered ? _thumbHoverColor : _thumbColor;
            using (Brush thumbBrush = new SolidBrush(currentThumbColor))
            {
                using (GraphicsPath thumbPath = GetRoundedRectPath(thumbRect, Width / 2))
                {
                    g.FillPath(thumbBrush, thumbPath);
                }
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            if (diameter > rect.Width) diameter = rect.Width;
            if (diameter > rect.Height) diameter = rect.Height;

            if (diameter > 0)
            {
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
            }
            else
            {
                path.AddRectangle(rect);
            }
            return path;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                int thumbY = GetThumbY();
                int thumbHeight = GetThumbHeight();
                if (e.Y >= thumbY && e.Y <= thumbY + thumbHeight)
                {
                    _isDragging = true;
                    _dragClickOffset = e.Y - thumbY;
                }
                else
                {
                    if (e.Y < thumbY)
                    {
                        Value -= _largeChange;
                    }
                    else
                    {
                        Value += _largeChange;
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDragging && e.Button == MouseButtons.Left)
            {
                int thumbHeight = GetThumbHeight();
                int trackHeight = Height - thumbHeight;
                if (trackHeight > 0)
                {
                    int proposedThumbY = e.Y - _dragClickOffset;
                    proposedThumbY = Math.Clamp(proposedThumbY, 0, trackHeight);
                    float ratio = (float)proposedThumbY / trackHeight;
                    int maxScroll = _maximum - _largeChange;
                    Value = (int)Math.Round(ratio * maxScroll);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isDragging = false;
            Invalidate();
        }
    }
}
