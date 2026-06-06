using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VRCGalleryManager.Design
{
    public enum ScrollOrientation { Vertical, Horizontal }

    /// <summary>
    /// Scrollbar personalizzata con stile dark/cyan che supporta
    /// sia orientamento verticale che orizzontale.
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

        public event EventHandler Scroll;

        // ── Properties ───────────────────────────────────────────────────────────
        public ScrollOrientation Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                // Swap default size axis
                if (_orientation == ScrollOrientation.Horizontal)
                    Height = 8;
                else
                    Width = 8;
                Invalidate();
            }
        }

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
            set { _largeChange = Math.Max(1, value); Invalidate(); }
        }

        public Color TrackColor
        {
            get => _trackColor;
            set { _trackColor = value; Invalidate(); }
        }

        public Color ThumbColor
        {
            get => _thumbColor;
            set { _thumbColor = value; Invalidate(); }
        }

        public Color ThumbHoverColor
        {
            get => _thumbHoverColor;
            set { _thumbHoverColor = value; Invalidate(); }
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
            using (Brush b = new SolidBrush(_trackColor))
            using (GraphicsPath p = RoundedPath(ClientRectangle, radius))
                g.FillPath(b, p);

            // Thumb
            int thumbSize = GetThumbSize();
            int thumbPos  = GetThumbPos();
            Rectangle thumbRect = IsVertical
                ? new Rectangle(0, thumbPos, Width, thumbSize)
                : new Rectangle(thumbPos, 0, thumbSize, Height);

            Color thumbCol = (_isDragging || _isHovered) ? _thumbHoverColor : _thumbColor;
            using (Brush b = new SolidBrush(thumbCol))
            using (GraphicsPath p = RoundedPath(thumbRect, radius))
                g.FillPath(b, p);
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
    }
}
