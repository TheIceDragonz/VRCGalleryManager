using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VRCGalleryManager.Design
{
    /// <summary>
    /// FlowLayoutPanel con scrollbar personalizzata in stile dark/cyan.
    /// Le barre sono disegnate direttamente in OnPaint — nessun controllo
    /// figlio aggiuntivo, zero interferenza con DisplayRectangle e AutoScroll.
    ///
    /// Utilizzo: imposta AutoScroll = true e UseCustomScrollBar = true (default).
    /// </summary>
    public class ModernFlowPanel : FlowLayoutPanel
    {
        // ── WinAPI ───────────────────────────────────────────────────────────────
        [DllImport("user32.dll")]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
        private const int SB_BOTH       = 3;
        private const int WM_NCPAINT    = 0x0085;
        private const int WM_NCCALCSIZE = 0x0083;
        private const int WM_VSCROLL    = 0x0115;
        private const int WM_HSCROLL    = 0x0114;

        // ── Scrollbar visual state ────────────────────────────────────────────────
        private bool _useCustomScrollBar = true;
        private bool _showVBar           = false;
        private bool _showHBar           = false;
        private bool _vBarDragging       = false;
        private bool _hBarDragging       = false;
        private int  _vBarDragOffset     = 0;
        private int  _hBarDragOffset     = 0;
        private bool _vBarHovered        = false;
        private bool _hBarHovered        = false;

        private const int BAR_W      = 6;
        private const int BAR_MARGIN = 3;

        private static readonly Color TrackColor      = Color.FromArgb(7,  36,  43);
        private static readonly Color ThumbColor      = Color.FromArgb(106, 227, 249);
        private static readonly Color ThumbHoverColor = Color.FromArgb(160, 240, 255);

        // ── Properties ───────────────────────────────────────────────────────────

        [Category("VRCGalleryManager")]
        [DefaultValue(true)]
        [Description("Sostituisce le scrollbar native con quelle in stile dark/cyan.")]
        public bool UseCustomScrollBar
        {
            get => _useCustomScrollBar;
            set { _useCustomScrollBar = value; UpdateScrollPadding(); Invalidate(); }
        }

        // ── Constructor ──────────────────────────────────────────────────────────

        public ModernFlowPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint  |
                     ControlStyles.ResizeRedraw, true);
        }

        // ── Padding per lo spazio barre ──────────────────────────────────────────

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateScrollPadding();
        }

        private void UpdateScrollPadding()
        {
            if (!_useCustomScrollBar || DesignMode)
            {
                Padding = Padding.Empty;
                return;
            }

            int reserve = BAR_W + BAR_MARGIN * 2;
            // Determina automaticamente su quale lato riservare spazio
            // in base alla direzione del flow e al WrapContents
            bool needRight  = NeedsVBar();
            bool needBottom = NeedsHBar();

            int right  = needRight  || (!WrapContents) ? reserve : 0;
            int bottom = needBottom || (!WrapContents && FlowDirection == FlowDirection.LeftToRight) ? reserve : 0;

            if (Padding.Right != right || Padding.Bottom != bottom)
                Padding = new Padding(Padding.Left, Padding.Top, right, bottom);
        }

        private bool NeedsVBar() =>
            IsHandleCreated && DisplayRectangle.Height > ClientSize.Height && ClientSize.Height > 0;

        private bool NeedsHBar() =>
            IsHandleCreated && DisplayRectangle.Width > ClientSize.Width && ClientSize.Width > 0;

        // ── WndProc ──────────────────────────────────────────────────────────────

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
        }

        // ── Geometry ─────────────────────────────────────────────────────────────

        private (Rectangle track, Rectangle thumb) GetVBarRects()
        {
            int clientH  = ClientSize.Height;
            int displayH = DisplayRectangle.Height;
            int trackX   = ClientSize.Width - BAR_W - BAR_MARGIN;
            int trackH   = clientH - BAR_MARGIN * 2 - (_showHBar ? BAR_W + BAR_MARGIN : 0);
            var track    = new Rectangle(trackX, BAR_MARGIN, BAR_W, Math.Max(0, trackH));

            float ratio  = displayH > 0 ? Math.Min(1f, (float)clientH / displayH) : 1f;
            int thumbH   = Math.Max(20, (int)(track.Height * ratio));
            int maxScr   = displayH - clientH;
            int range    = track.Height - thumbH;
            int thumbY   = (maxScr > 0 && range > 0)
                               ? (int)((float)(-AutoScrollPosition.Y) / maxScr * range)
                               : 0;
            return (track, new Rectangle(track.X, track.Y + thumbY, track.Width, thumbH));
        }

        private (Rectangle track, Rectangle thumb) GetHBarRects()
        {
            int clientW  = ClientSize.Width;
            int displayW = DisplayRectangle.Width;
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
            return (track, new Rectangle(track.X + thumbX, track.Y, thumbW, track.Height));
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!_useCustomScrollBar || !AutoScroll || DesignMode) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int clientH  = ClientSize.Height;
            int clientW  = ClientSize.Width;
            int displayH = DisplayRectangle.Height;
            int displayW = DisplayRectangle.Width;

            _showVBar = displayH > clientH && clientH > 0;
            _showHBar = displayW > clientW && clientW > 0;

            // Disegna le barre nel client area (sopra i figli, in coda alla paint)
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
            using (var b = new SolidBrush(TrackColor))
                g.FillPath(b, trackPath);
            using (var b = new SolidBrush(hovered ? ThumbHoverColor : ThumbColor))
                g.FillPath(b, thumbPath);
        }

        // ── Mouse ────────────────────────────────────────────────────────────────

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
                    else ScrollBy(vertical: true, direction: e.Y < thumb.Y ? -1 : 1);
                    Invalidate();
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
                    else ScrollBy(vertical: false, direction: e.X < thumb.X ? -1 : 1);
                    Invalidate();
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
                int range = track.Height - thumb.Height;
                if (range > 0)
                {
                    int pos    = Math.Clamp(e.Y - track.Y - _vBarDragOffset, 0, range);
                    int maxScr = Math.Max(0, DisplayRectangle.Height - ClientSize.Height);
                    AutoScrollPosition = new Point(-AutoScrollPosition.X, (int)((float)pos / range * maxScr));
                }
                redraw = true;
            }
            else if (_hBarDragging && e.Button == MouseButtons.Left)
            {
                var (track, thumb) = GetHBarRects();
                int range = track.Width - thumb.Width;
                if (range > 0)
                {
                    int pos    = Math.Clamp(e.X - track.X - _hBarDragOffset, 0, range);
                    int maxScr = Math.Max(0, DisplayRectangle.Width - ClientSize.Width);
                    AutoScrollPosition = new Point((int)((float)pos / range * maxScr), -AutoScrollPosition.Y);
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

            if (redraw) Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_vBarDragging || _hBarDragging)
            {
                _vBarDragging = false;
                _hBarDragging = false;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _vBarHovered = false;
            _hBarHovered = false;
            Invalidate();
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            Invalidate();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (_useCustomScrollBar && !DesignMode)
                try { if (IsHandleCreated) ShowScrollBar(Handle, SB_BOTH, false); } catch { }
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            Invalidate();
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            Invalidate();
        }

        // ── Scroll helpers ────────────────────────────────────────────────────────

        private void ScrollBy(bool vertical, int direction)
        {
            if (vertical)
            {
                int max  = Math.Max(0, DisplayRectangle.Height - ClientSize.Height);
                int newY = Math.Clamp(-AutoScrollPosition.Y + direction * ClientSize.Height / 3, 0, max);
                AutoScrollPosition = new Point(-AutoScrollPosition.X, newY);
            }
            else
            {
                int max  = Math.Max(0, DisplayRectangle.Width - ClientSize.Width);
                int newX = Math.Clamp(-AutoScrollPosition.X + direction * ClientSize.Width / 3, 0, max);
                AutoScrollPosition = new Point(newX, -AutoScrollPosition.Y);
            }
            Invalidate();
        }
    }
}
