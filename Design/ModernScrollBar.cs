using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VRCGalleryManager.Design
{
    public enum ScrollOrientation { Vertical, Horizontal }

    /// <summary>
    /// Custom scrollbar with dark/cyan style supporting
    /// both vertical and horizontal orientations.
    /// </summary>
    public class ModernScrollBar : Control
    {
        // ── State ────────────────────────────────────────────────────────────────
        private int _minimum = 0;
        private int _maximum = 100;
        private int _value = 0;
        private int _largeChange = 10;
        private bool _isDragging = false;
        private int _dragClickOffset = 0;
        private bool _isHovered = false;
        private ScrollOrientation _orientation = ScrollOrientation.Vertical;

        // ── Colors ───────────────────────────────────────────────────────────────
        private Color _trackColor      = Color.FromArgb(7, 36, 43);
        private Color _thumbColor      = Color.FromArgb(106, 227, 249);
        private Color _thumbHoverColor = Color.FromArgb(160, 240, 255);

        // ── Cache fields for optimization ────────────────────────────────────────
        private GraphicsPath _cachedTrackPath = null;
        private Size _lastTrackSize;
        private int _lastTrackRadius;

        private GraphicsPath _cachedThumbPath = null;
        private Size _lastThumbSize;
        private int _lastThumbRadius;

        public new event EventHandler Scroll;

        // ── Properties ───────────────────────────────────────────────────────────
        public ScrollOrientation Orientation
        {
            get => _orientation;
            set
            {
                if (_orientation != value)
                {
                    _orientation = value;
                    if (_orientation == ScrollOrientation.Horizontal)
                        Height = 8;
                    else
                        Width = 8;
                    Invalidate();
                }
            }
        }

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
                int clamped = Math.Clamp(value, _minimum, maxVal);
                if (_value == clamped) return;
                _value = clamped;
                Invalidate();
                Scroll?.Invoke(this, EventArgs.Empty);
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

        public Color TrackColor
        {
            get => _trackColor;
            set
            {
                if (_trackColor != value)
                {
                    _trackColor = value;
                    Invalidate();
                }
            }
        }

        public Color ThumbColor
        {
            get => _thumbColor;
            set
            {
                if (_thumbColor != value)
                {
                    _thumbColor = value;
                    Invalidate();
                }
            }
        }

        public Color ThumbHoverColor
        {
            get => _thumbHoverColor;
            set
            {
                if (_thumbHoverColor != value)
                {
                    _thumbHoverColor = value;
                    Invalidate();
                }
            }
        }

        // ── Constructor ──────────────────────────────────────────────────────────
        public ModernScrollBar()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint, true);
            Width = 8;
        }

        // ── Mouse ────────────────────────────────────────────────────────────────
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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left) return;

            int thumbPos  = GetThumbPos();
            int thumbSize = GetThumbSize();
            int cursor    = IsVertical ? e.Y : e.X;

            if (cursor >= thumbPos && cursor <= thumbPos + thumbSize)
            {
                _isDragging = true;
                _dragClickOffset = cursor - thumbPos;
            }
            else
            {
                Value += cursor < thumbPos ? -_largeChange : _largeChange;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_isDragging || e.Button != MouseButtons.Left) return;

            int thumbSize  = GetThumbSize();
            int trackSize  = (IsVertical ? Height : Width) - thumbSize;
            if (trackSize <= 0) return;

            int cursor       = (IsVertical ? e.Y : e.X) - _dragClickOffset;
            int clampedThumb = Math.Clamp(cursor, 0, trackSize);
            float ratio      = (float)clampedThumb / trackSize;
            int maxScroll    = _maximum - _largeChange;
            Value = (int)Math.Round(ratio * maxScroll);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isDragging = false;
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            int delta = -(e.Delta / 120) * (_largeChange / 3);
            Value += delta;
        }

        // ── Layout helpers ───────────────────────────────────────────────────────
        private bool IsVertical => _orientation == ScrollOrientation.Vertical;

        private int GetThumbSize()
        {
            int total = IsVertical ? Height : Width;
            if (_maximum <= 0) return total;
            float ratio = (float)_largeChange / _maximum;
            int size = (int)(total * ratio);
            return Math.Clamp(size, 20, total);
        }

        private int GetThumbPos()
        {
            int maxScroll  = _maximum - _largeChange;
            if (maxScroll <= 0) return 0;
            int thumbSize  = GetThumbSize();
            int trackSize  = (IsVertical ? Height : Width) - thumbSize;
            float ratio    = (float)_value / maxScroll;
            return (int)(ratio * trackSize);
        }

        // ── Paint ────────────────────────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Track
            int radius = IsVertical ? Width / 2 : Height / 2;
            if (_cachedTrackPath == null || _lastTrackSize != ClientRectangle.Size || _lastTrackRadius != radius)
            {
                _cachedTrackPath?.Dispose();
                _cachedTrackPath = RoundedPath(ClientRectangle, radius);
                _lastTrackSize = ClientRectangle.Size;
                _lastTrackRadius = radius;
            }

            using (Brush b = new SolidBrush(_trackColor))
                g.FillPath(b, _cachedTrackPath);

            // Thumb
            int thumbSize = GetThumbSize();
            int thumbPos  = GetThumbPos();

            Size currentThumbSize = IsVertical ? new Size(Width, thumbSize) : new Size(thumbSize, Height);
            if (_cachedThumbPath == null || _lastThumbSize != currentThumbSize || _lastThumbRadius != radius)
            {
                _cachedThumbPath?.Dispose();
                Rectangle localThumbRect = new Rectangle(0, 0, currentThumbSize.Width, currentThumbSize.Height);
                _cachedThumbPath = RoundedPath(localThumbRect, radius);
                _lastThumbSize = currentThumbSize;
                _lastThumbRadius = radius;
            }

            Color thumbCol = (_isDragging || _isHovered) ? _thumbHoverColor : _thumbColor;
            using (Brush b = new SolidBrush(thumbCol))
            {
                int x = IsVertical ? 0 : thumbPos;
                int y = IsVertical ? thumbPos : 0;
                
                g.TranslateTransform(x, y);
                g.FillPath(b, _cachedThumbPath);
                g.TranslateTransform(-x, -y);
            }
        }

        private static GraphicsPath RoundedPath(Rectangle r, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
            if (d > 0)
            {
                path.AddArc(r.X,           r.Y,            d, d, 180, 90);
                path.AddArc(r.Right - d,   r.Y,            d, d, 270, 90);
                path.AddArc(r.Right - d,   r.Bottom - d,   d, d,   0, 90);
                path.AddArc(r.X,           r.Bottom - d,   d, d,  90, 90);
                path.CloseFigure();
            }
            else
            {
                path.AddRectangle(r);
            }
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
