using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VRCGalleryManager.Design
{
    /// <summary>
    /// Panel con angoli arrotondati e scrollbar personalizzata disegnata
    /// direttamente in OnPaint (nessun controllo figlio per le barre →
    /// zero interferenza con DisplayRectangle e AutoScroll).
    /// </summary>
    public class RoundedPanel : Panel
    {
        // ── WinAPI ───────────────────────────────────────────────────────────────
        [DllImport("user32.dll")]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
        private const int SB_BOTH       = 3;
        private const int WM_NCPAINT    = 0x0085;
        private const int WM_NCCALCSIZE = 0x0083;
        private const int WM_VSCROLL    = 0x0115;
        private const int WM_HSCROLL    = 0x0114;

        // ── Border fields ────────────────────────────────────────────────────────
        private int   borderSize   = 0;
        private int   borderRadius = 15;
        private Color borderColor  = Color.PaleVioletRed;

        // ── Scrollbar visual state (NO child controls!) ──────────────────────────
        private bool _useCustomScrollBar = false;
        private bool _showVBar           = false;
        private bool _showHBar           = false;
        private bool _needV              = false;
        private bool _needH              = false;
        private bool _vBarDragging       = false;
        private bool _hBarDragging       = false;
        private int  _vBarDragOffset     = 0;
        private int  _hBarDragOffset     = 0;
        private bool _vBarHovered        = false;
        private bool _hBarHovered        = false;

        private const int BAR_W      = 6;   // spessore barra px
        private const int BAR_MARGIN = 3;   // margine dal bordo px

        // ── Cache fields for optimization ────────────────────────────────────────
        private Size _lastRegionSize;
        private int _lastRegionRadius;
        private bool _lastVScroll;

        private GraphicsPath _cachedPathSurface = null;
        private GraphicsPath _cachedPathBorder = null;
        private Size _lastPathSize;
        private int _lastPathRadius;
        private int _lastPathBorderSize;

        private static readonly Color TrackColor      = Color.FromArgb(7,  36,  43);
        private static readonly Color ThumbColor      = Color.FromArgb(106, 227, 249);
        private static readonly Color ThumbHoverColor = Color.FromArgb(160, 240, 255);

        // ── Border Properties ────────────────────────────────────────────────────

        [Category("VRCGalleryManager")]
        public int BorderSize
        {
            get => borderSize;
            set { if (borderSize != value) { borderSize = value; UpdateRegion(); Invalidate(); } }
        }

        [Category("VRCGalleryManager")]
        public int BorderRadius
        {
            get => borderRadius;
            set { if (borderRadius != value) { borderRadius = value; UpdateRegion(); Invalidate(); } }
        }

        [Category("VRCGalleryManager")]
        public Color BorderColor
        {
            get => borderColor;
            set { if (borderColor != value) { borderColor = value; Invalidate(); } }
        }

        [Category("VRCGalleryManager")]
        public Color BackgroundColor
        {
            get => BackColor;
            set => BackColor = value;
        }

        /// <summary>
        /// Quando true, nasconde le scrollbar native e le disegna in stile
        /// dark/cyan direttamente nel panel (funziona solo con AutoScroll=true).
        /// </summary>
        [Category("VRCGalleryManager")]
        [DefaultValue(false)]
        public bool UseCustomScrollBar
        {
            get => _useCustomScrollBar;
            set { _useCustomScrollBar = value; UpdateScrollPadding(); UpdateRegion(); Invalidate(); }
        }

        // ── Constructor ──────────────────────────────────────────────────────────

        public RoundedPanel()
        {
            Size = new Size(150, 150);
            BackColor = Color.MediumSlateBlue;
            DoubleBuffered = true;
        }

        // ── Padding per riservare spazio alla barra ──────────────────────────────

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateScrollPadding();
            UpdateRegion();
        }

        private int GetVirtualContentHeight() => DisplayRectangle.Height;

        private int GetVirtualContentWidth() => DisplayRectangle.Width;

        private void UpdateScrollPadding()
        {
            if (!_useCustomScrollBar || DesignMode || !IsHandleCreated)
            {
                _needV = false;
                _needH = false;
                return;
            }

            int reserve = BAR_W + BAR_MARGIN * 2;
            
            _needV = VScroll;
            _needH = HScroll;

            int right = _needV ? reserve : 0;
            int bottom = _needH ? reserve : 0;

            if (Padding.Right != right || Padding.Bottom != bottom)
            {
                Padding = new Padding(Padding.Left, Padding.Top, right, bottom);
            }
        }

        private void UpdateRegion()
        {
            int extraWidth = (AutoScroll && VScroll) ? SystemInformation.VerticalScrollBarWidth : 0;
            Rectangle rectSurface = new Rectangle(0, 0, ClientRectangle.Width + extraWidth, ClientRectangle.Height);
            if (rectSurface.Width <= 0 || rectSurface.Height <= 0)
                return;

            int effectiveRadius = Math.Min(borderRadius, Math.Min(rectSurface.Width, rectSurface.Height) / 2);
            bool currentVScroll = VScroll;

            if (_lastRegionSize == rectSurface.Size && 
                _lastRegionRadius == effectiveRadius && 
                _lastVScroll == currentVScroll && 
                this.Region != null)
            {
                return; // Region already matches, skip allocation
            }

            _lastRegionSize = rectSurface.Size;
            _lastRegionRadius = effectiveRadius;
            _lastVScroll = currentVScroll;

            Region oldRegion = this.Region;
            if (effectiveRadius > 2)
            {
                using (GraphicsPath pathSurface = GetFigurePath(rectSurface, effectiveRadius))
                {
                    this.Region = new Region(pathSurface);
                }
            }
            else
            {
                this.Region = new Region(rectSurface);
            }
            oldRegion?.Dispose();
        }

        // ── WndProc: nasconde le scrollbar native ────────────────────────────────

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (_useCustomScrollBar && AutoScroll && !DesignMode && IsHandleCreated)
            {
                if (m.Msg == WM_NCPAINT || m.Msg == WM_NCCALCSIZE ||
                    m.Msg == WM_VSCROLL  || m.Msg == WM_HSCROLL)
                {
                    try { ShowScrollBar(Handle, SB_BOTH, false); } catch { }
                }
            }
            if (m.Msg == WM_VSCROLL || m.Msg == WM_HSCROLL || m.Msg == 0x020A /* WM_MOUSEWHEEL */)
            {
                InvalidateScrollBars();
            }
        }

        // ── Geometry helpers ─────────────────────────────────────────────────────

        private (Rectangle track, Rectangle thumb) GetVBarRects()
        {
            int clientH  = ClientSize.Height;
            int displayH = GetVirtualContentHeight();
            int trackX   = ClientSize.Width - BAR_W - BAR_MARGIN;
            int trackH   = clientH - BAR_MARGIN * 2 - (_showHBar ? BAR_W + BAR_MARGIN : 0);
            var track    = new Rectangle(trackX, BAR_MARGIN, BAR_W, Math.Max(0, trackH));

            float ratio    = displayH > 0 ? Math.Min(1f, (float)clientH / displayH) : 1f;
            int   thumbH   = Math.Max(20, (int)(track.Height * ratio));
            int   maxScr   = displayH - clientH;
            int   range    = track.Height - thumbH;
            int   thumbY   = (maxScr > 0 && range > 0)
                               ? (int)((float)(-AutoScrollPosition.Y) / maxScr * range)
                               : 0;
            var thumb = new Rectangle(track.X, track.Y + thumbY, track.Width, thumbH);
            return (track, thumb);
        }

        private (Rectangle track, Rectangle thumb) GetHBarRects()
        {
            int clientW  = ClientSize.Width;
            int displayW = GetVirtualContentWidth();
            int trackY   = ClientSize.Height - BAR_W - BAR_MARGIN;
            int trackW   = clientW - BAR_MARGIN * 2 - (_showVBar ? BAR_W + BAR_MARGIN : 0);
            var track    = new Rectangle(BAR_MARGIN, trackY, Math.Max(0, trackW), BAR_W);

            float ratio  = displayW > 0 ? Math.Min(1f, (float)clientW / displayW) : 1f;
            int thumbW   = Math.Max(20, (int)(track.Width * ratio));
            int maxScr   = displayW - clientW;
            int range    = track.Width - thumbW;
            int thumbX   = (maxScr > 0 && range > 0)
                               ? (int)((float)(-AutoScrollPosition.X) / maxScr * range)
                               : 0;
            var thumb = new Rectangle(track.X + thumbX, track.Y, thumbW, track.Height);
            return (track, thumb);
        }

        private static GraphicsPath BuildRoundedPath(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
            if (d > 0)
            {
                path.AddArc(r.X,         r.Y,          d, d, 180, 90);
                path.AddArc(r.Right - d, r.Y,          d, d, 270, 90);
                path.AddArc(r.Right - d, r.Bottom - d, d, d,   0, 90);
                path.AddArc(r.X,         r.Bottom - d, d, d,  90, 90);
                path.CloseFigure();
            }
            else path.AddRectangle(r);
            return path;
        }

        // ── Paint ────────────────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // ── Border / rounded region ─────────────────────────────────────────
            int extraWidth = (AutoScroll && VScroll) ? SystemInformation.VerticalScrollBarWidth : 0;
            Rectangle rectSurface = new Rectangle(0, 0, ClientRectangle.Width + extraWidth, ClientRectangle.Height);
            Rectangle rectBorder  = Rectangle.Inflate(rectSurface, -borderSize, -borderSize);
            int smoothSize        = borderSize > 0 ? borderSize : 2;
            int effectiveRadius   = Math.Min(borderRadius, Math.Min(rectSurface.Width, rectSurface.Height) / 2);

            if (effectiveRadius > 2)
            {
                if (_cachedPathSurface == null || _cachedPathBorder == null || 
                    _lastPathSize != rectSurface.Size || 
                    _lastPathRadius != effectiveRadius || 
                    _lastPathBorderSize != borderSize)
                {
                    _cachedPathSurface?.Dispose();
                    _cachedPathBorder?.Dispose();

                    _cachedPathSurface = GetFigurePath(rectSurface, effectiveRadius);
                    _cachedPathBorder = GetFigurePath(rectBorder, Math.Max(1, effectiveRadius - borderSize));
                    _lastPathSize = rectSurface.Size;
                    _lastPathRadius = effectiveRadius;
                    _lastPathBorderSize = borderSize;
                }

                using var penSurface = new Pen(Parent?.BackColor ?? Color.Transparent, smoothSize);
                using var penBorder  = new Pen(borderColor, borderSize);
                g.DrawPath(penSurface, _cachedPathSurface);
                if (borderSize >= 1) g.DrawPath(penBorder, _cachedPathBorder);
            }
            else
            {
                if (borderSize >= 1)
                {
                    using var penBorder = new Pen(borderColor, borderSize);
                    penBorder.Alignment = PenAlignment.Inset;
                    g.DrawRectangle(penBorder, 0, 0, rectSurface.Width - 1, rectSurface.Height - 1);
                }
            }

            // ── Custom scrollbars ───────────────────────────────────────────────
            if (!_useCustomScrollBar || !AutoScroll || DesignMode) return;

            _showVBar = _needV;
            _showHBar = _needH;

            if (_showVBar)
            {
                var (track, thumb) = GetVBarRects();
                DrawBar(g, track, thumb, _vBarHovered || _vBarDragging);
            }

            if (_showHBar)
            {
                var (track, thumb) = GetHBarRects();
                DrawBar(g, track, thumb, _hBarHovered || _hBarDragging);
            }
        }

        private static void DrawBar(Graphics g, Rectangle track, Rectangle thumb, bool hovered)
        {
            int r = Math.Min(track.Width, track.Height) / 2;
            using var trackPath = BuildRoundedPath(track, r);
            using var thumbPath = BuildRoundedPath(thumb, r);
            using (var b = new SolidBrush(TrackColor))      g.FillPath(b, trackPath);
            using (var b = new SolidBrush(hovered ? ThumbHoverColor : ThumbColor)) g.FillPath(b, thumbPath);
        }

        // ── Mouse events ─────────────────────────────────────────────────────────

        private void InvalidateScrollBars()
        {
            if (!IsHandleCreated) return;

            int reserve = BAR_W + BAR_MARGIN * 2 + 4; // safety margin for rendering

            if (_showVBar)
            {
                var rect = new Rectangle(ClientSize.Width - reserve, 0, reserve, ClientSize.Height);
                Invalidate(rect);
            }
            if (_showHBar)
            {
                var rect = new Rectangle(0, ClientSize.Height - reserve, ClientSize.Width, reserve);
                Invalidate(rect);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!_useCustomScrollBar || e.Button != MouseButtons.Left) return;

            if (_showVBar)
            {
                var (track, thumb) = GetVBarRects();
                if (track.Contains(e.Location))
                {
                    if (thumb.Contains(e.Location))
                    {
                        _vBarDragging = true;
                        _vBarDragOffset = e.Y - thumb.Y;
                    }
                    else
                    {
                        ScrollBy(vertical: true, direction: e.Y < thumb.Y ? -1 : 1);
                    }
                    InvalidateScrollBars();
                    return;
                }
            }

            if (_showHBar)
            {
                var (track, thumb) = GetHBarRects();
                if (track.Contains(e.Location))
                {
                    if (thumb.Contains(e.Location))
                    {
                        _hBarDragging = true;
                        _hBarDragOffset = e.X - thumb.X;
                    }
                    else
                    {
                        ScrollBy(vertical: false, direction: e.X < thumb.X ? -1 : 1);
                    }
                    InvalidateScrollBars();
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_useCustomScrollBar) return;

            bool redraw = false;

            if (_vBarDragging && e.Button == MouseButtons.Left)
            {
                var (track, thumb) = GetVBarRects();
                int thumbH  = thumb.Height;
                int range   = track.Height - thumbH;
                if (range > 0)
                {
                    int pos       = Math.Clamp(e.Y - track.Y - _vBarDragOffset, 0, range);
                    int displayH  = DisplayRectangle.Height;
                    int maxScroll = Math.Max(0, displayH - ClientSize.Height);
                    AutoScrollPosition = new Point(-AutoScrollPosition.X, (int)((float)pos / range * maxScroll));
                }
                redraw = true;
            }
            else if (_hBarDragging && e.Button == MouseButtons.Left)
            {
                var (track, thumb) = GetHBarRects();
                int thumbW  = thumb.Width;
                int range   = track.Width - thumbW;
                if (range > 0)
                {
                    int pos       = Math.Clamp(e.X - track.X - _hBarDragOffset, 0, range);
                    int displayW  = DisplayRectangle.Width;
                    int maxScroll = Math.Max(0, displayW - ClientSize.Width);
                    AutoScrollPosition = new Point((int)((float)pos / range * maxScroll), -AutoScrollPosition.Y);
                }
                redraw = true;
            }
            else
            {
                bool newV = _showVBar && GetVBarRects().track.Contains(e.Location);
                bool newH = _showHBar && GetHBarRects().track.Contains(e.Location);
                if (newV != _vBarHovered || newH != _hBarHovered)
                {
                    _vBarHovered = newV;
                    _hBarHovered = newH;
                    redraw = true;
                }
            }

            if (redraw) InvalidateScrollBars();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_vBarDragging || _hBarDragging)
            {
                _vBarDragging = false;
                _hBarDragging = false;
                InvalidateScrollBars();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _vBarHovered = false;
            _hBarHovered = false;
            InvalidateScrollBars();
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            InvalidateScrollBars();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            InvalidateScrollBars();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            UpdateScrollPadding();
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRegion();
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Region oldRegion = this.Region;
                this.Region = null;
                oldRegion?.Dispose();

                _cachedPathSurface?.Dispose();
                _cachedPathSurface = null;
                _cachedPathBorder?.Dispose();
                _cachedPathBorder = null;
            }
            base.Dispose(disposing);
        }

        // ── Scroll helpers ────────────────────────────────────────────────────────

        private void ScrollBy(bool vertical, int direction)
        {
            if (vertical)
            {
                int max   = Math.Max(0, GetVirtualContentHeight() - ClientSize.Height);
                int newY  = Math.Clamp(-AutoScrollPosition.Y + direction * ClientSize.Height / 3, 0, max);
                AutoScrollPosition = new Point(-AutoScrollPosition.X, newY);
            }
            else
            {
                int max   = Math.Max(0, GetVirtualContentWidth() - ClientSize.Width);
                int newX  = Math.Clamp(-AutoScrollPosition.X + direction * ClientSize.Width / 3, 0, max);
                AutoScrollPosition = new Point(newX, -AutoScrollPosition.Y);
            }
            InvalidateScrollBars();
        }

        // ── Border helpers ────────────────────────────────────────────────────────

        private GraphicsPath GetFigurePath(Rectangle rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;
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
