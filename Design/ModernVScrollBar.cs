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

        // ── Cache fields for optimization ────────────────────────────────────────
        private GraphicsPath _cachedTrackPath = null;
        private Size _lastTrackSize;
        private int _lastTrackRadius;

        private GraphicsPath _cachedThumbPath = null;
        private Size _lastThumbSize;
        private int _lastThumbRadius;

        public new event EventHandler Scroll;

        public int Minimum
        {
            get => _minimum;
            set
            {
                int val = Math.Max(0, value);
                if (_minimum != val)
                {
                    _minimum = val;
                    Invalidate();
                }
            }
        }

        public int Maximum
        {
            get => _maximum;
            set
            {
                int val = Math.Max(1, value);
                if (_maximum != val)
                {
                    _maximum = val;
                    Invalidate();
                }
            }
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
            set
            {
                int val = Math.Max(1, value);
                if (_largeChange != val)
                {
                    _largeChange = val;
                    Invalidate();
                }
            }
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

            int radius = Width / 2;

            // Draw track
            if (_cachedTrackPath == null || _lastTrackSize != ClientRectangle.Size || _lastTrackRadius != radius)
            {
                _cachedTrackPath?.Dispose();
                _cachedTrackPath = GetRoundedRectPath(ClientRectangle, radius);
                _lastTrackSize = ClientRectangle.Size;
                _lastTrackRadius = radius;
            }

            using (Brush trackBrush = new SolidBrush(_trackColor))
            {
                g.FillPath(trackBrush, _cachedTrackPath);
            }

            // Draw thumb
            int thumbHeight = GetThumbHeight();
            int thumbY = GetThumbY();

            Size currentThumbSize = new Size(Width, thumbHeight);
            if (_cachedThumbPath == null || _lastThumbSize != currentThumbSize || _lastThumbRadius != radius)
            {
                _cachedThumbPath?.Dispose();
                Rectangle localThumbRect = new Rectangle(0, 0, currentThumbSize.Width, currentThumbSize.Height);
                _cachedThumbPath = GetRoundedRectPath(localThumbRect, radius);
                _lastThumbSize = currentThumbSize;
                _lastThumbRadius = radius;
            }

            Color currentThumbColor = _isDragging || _isHovered ? _thumbHoverColor : _thumbColor;
            using (Brush thumbBrush = new SolidBrush(currentThumbColor))
            {
                g.TranslateTransform(0, thumbY);
                g.FillPath(thumbBrush, _cachedThumbPath);
                g.TranslateTransform(0, -thumbY);
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
