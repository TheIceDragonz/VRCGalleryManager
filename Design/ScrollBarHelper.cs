using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VRCGalleryManager.Design
{
    /// <summary>
    /// Sostituisce le scrollbar native di qualsiasi ScrollableControl
    /// (Panel, FlowLayoutPanel, ecc.) con la ModernScrollBar personalizzata.
    /// Supporta scroll verticale, orizzontale o entrambi automaticamente.
    /// 
    /// Utilizzo:
    ///   ScrollBarHelper.Attach(myPanel);                        // auto
    ///   ScrollBarHelper.Attach(myPanel, vertical: false);       // solo orizzontale
    ///   ScrollBarHelper.Attach(myPanel, true, true);            // forza entrambe
    /// </summary>
    public class ScrollBarHelper : NativeWindow
    {
        // ── WinAPI ───────────────────────────────────────────────────────────────
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, [MarshalAs(UnmanagedType.Bool)] bool bShow);

        private const int SB_HORZ   = 0;
        private const int SB_VERT   = 1;
        private const int SB_BOTH   = 3;
        private const int WM_NCCALCSIZE = 0x0083;
        private const int WM_NCPAINT   = 0x0085;
        private const int WM_VSCROLL   = 0x0115;
        private const int WM_HSCROLL   = 0x0114;
        private const int WM_MOUSEWHEEL = 0x020A;

        private const int BAR_SIZE = 6; // spessore barra in pixel

        // ── Fields ───────────────────────────────────────────────────────────────
        private readonly ScrollableControl _control;
        private readonly ModernScrollBar   _vBar;   // null se non richiesta
        private readonly ModernScrollBar   _hBar;   // null se non richiesta
        private readonly bool _wantVert;
        private readonly bool _wantHorz;
        private bool _isUpdating = false;

        // ── Constructor ──────────────────────────────────────────────────────────
        private ScrollBarHelper(ScrollableControl control, bool vertical, bool horizontal)
        {
            _control  = control ?? throw new ArgumentNullException(nameof(control));
            _wantVert = vertical;
            _wantHorz = horizontal;

            if (_wantVert)
            {
                _vBar = CreateBar(ScrollOrientation.Vertical);
                _vBar.Scroll += OnBarScrollEvent;
            }

            if (_wantHorz)
            {
                _hBar = CreateBar(ScrollOrientation.Horizontal);
                _hBar.Scroll += OnBarScrollEvent;
            }

            // Wire control events using named methods to prevent leaks
            _control.Scroll        += Control_Scroll;
            _control.Layout        += Control_Layout;
            _control.SizeChanged   += Control_SizeChanged;
            _control.ControlAdded  += Control_ControlAdded;
            _control.ControlRemoved += Control_ControlRemoved;
            _control.MouseWheel    += Control_MouseWheel;
            _control.ParentChanged += Control_ParentChanged;
            _control.HandleCreated   += Control_HandleCreated;
            _control.HandleDestroyed += Control_HandleDestroyed;
            _control.Disposed      += Control_Disposed;

            if (_control.Parent != null)
                AddToParent();

            if (_control.IsHandleCreated)
            {
                HideNativeBars();
                AssignHandle(_control.Handle);
            }

            UpdateScrollBarValues();
        }

        private void OnBarScrollEvent(object sender, EventArgs e)
        {
            OnBarScroll();
        }

        private void Control_Scroll(object sender, ScrollEventArgs e)
        {
            UpdatePositionFromControl();
        }

        private void Control_Layout(object sender, LayoutEventArgs e)
        {
            UpdateScrollBarValues();
        }

        private void Control_SizeChanged(object sender, EventArgs e)
        {
            PositionBars();
            UpdateScrollBarValues();
        }

        private void Control_ControlAdded(object sender, ControlEventArgs e)
        {
            UpdateScrollBarValues();
        }

        private void Control_ControlRemoved(object sender, ControlEventArgs e)
        {
            UpdateScrollBarValues();
        }

        private void Control_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!_control.IsDisposed)
            {
                try
                {
                    _control.BeginInvoke(new Action(UpdatePositionFromControl));
                }
                catch { }
            }
        }

        private void Control_ParentChanged(object sender, EventArgs e)
        {
            AddToParent();
        }

        private void Control_HandleCreated(object sender, EventArgs e)
        {
            HideNativeBars();
            AssignHandle(_control.Handle);
        }

        private void Control_HandleDestroyed(object sender, EventArgs e)
        {
            ReleaseHandle();
        }

        private void Control_Disposed(object sender, EventArgs e)
        {
            // Unsubscribe all events
            _control.Scroll        -= Control_Scroll;
            _control.Layout        -= Control_Layout;
            _control.SizeChanged   -= Control_SizeChanged;
            _control.ControlAdded  -= Control_ControlAdded;
            _control.ControlRemoved -= Control_ControlRemoved;
            _control.MouseWheel    -= Control_MouseWheel;
            _control.ParentChanged -= Control_ParentChanged;
            _control.HandleCreated   -= Control_HandleCreated;
            _control.HandleDestroyed -= Control_HandleDestroyed;
            _control.Disposed      -= Control_Disposed;

            if (_vBar != null)
            {
                _vBar.Scroll -= OnBarScrollEvent;
                if (_vBar.Parent != null)
                {
                    _vBar.Parent.Controls.Remove(_vBar);
                }
                _vBar.Dispose();
            }

            if (_hBar != null)
            {
                _hBar.Scroll -= OnBarScrollEvent;
                if (_hBar.Parent != null)
                {
                    _hBar.Parent.Controls.Remove(_hBar);
                }
                _hBar.Dispose();
            }

            ReleaseHandle();
        }

        // ── WndProc hook ─────────────────────────────────────────────────────────
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCCALCSIZE)
            {
                base.WndProc(ref m);
                HideNativeBars();
                return;
            }

            if (m.Msg == WM_NCPAINT || m.Msg == WM_VSCROLL ||
                m.Msg == WM_HSCROLL || m.Msg == WM_MOUSEWHEEL)
            {
                HideNativeBars();
                base.WndProc(ref m);
                HideNativeBars();
                return;
            }

            base.WndProc(ref m);
        }

        // ── Bar creation ─────────────────────────────────────────────────────────
        private ModernScrollBar CreateBar(ScrollOrientation orientation)
        {
            var bar = new ModernScrollBar
            {
                Orientation = orientation,
                Visible = false
            };

            if (orientation == ScrollOrientation.Vertical)
            {
                bar.Width  = BAR_SIZE;
                bar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            }
            else
            {
                bar.Height = BAR_SIZE;
                bar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            }

            return bar;
        }

        private void AddToParent()
        {
            var parent = _control.Parent;
            if (parent == null) return;

            if (_vBar != null && _vBar.Parent != parent)
            {
                parent.Controls.Add(_vBar);
                _vBar.BringToFront();
            }

            if (_hBar != null && _hBar.Parent != parent)
            {
                parent.Controls.Add(_hBar);
                _hBar.BringToFront();
            }

            PositionBars();
        }

        // ── Positioning ──────────────────────────────────────────────────────────
        private void PositionBars()
        {
            if (_control.Parent == null) return;

            // Vertical bar: along right edge of control
            if (_vBar != null)
            {
                _vBar.Location = new Point(
                    _control.Right - _vBar.Width - 2,
                    _control.Top + 2);
                _vBar.Height = _control.Height - 4;
            }

            // Horizontal bar: along bottom edge of control
            if (_hBar != null)
            {
                _hBar.Location = new Point(
                    _control.Left + 2,
                    _control.Bottom - _hBar.Height - 2);
                _hBar.Width = _control.Width - 4 - (_vBar != null && _vBar.Visible ? _vBar.Width + 2 : 0);
            }
        }

        // ── Sync: bar → control ──────────────────────────────────────────────────
        private void OnBarScroll()
        {
            if (_isUpdating) return;
            _isUpdating = true;
            try
            {
                int x = _hBar != null ? _hBar.Value : -_control.AutoScrollPosition.X;
                int y = _vBar != null ? _vBar.Value : -_control.AutoScrollPosition.Y;
                _control.AutoScrollPosition = new Point(x, y);
            }
            finally { _isUpdating = false; }
        }

        // ── Sync: control → bar ──────────────────────────────────────────────────
        private void UpdatePositionFromControl()
        {
            if (_isUpdating) return;
            _isUpdating = true;
            try
            {
                if (_vBar != null) _vBar.Value = -_control.AutoScrollPosition.Y;
                if (_hBar != null) _hBar.Value = -_control.AutoScrollPosition.X;
            }
            finally
            {
                _isUpdating = false;
                HideNativeBars();
            }
        }

        private void UpdateScrollBarValues()
        {
            if (_isUpdating) return;
            _isUpdating = true;
            try
            {
                int clientH  = _control.ClientSize.Height;
                int displayH = _control.DisplayRectangle.Height;
                int clientW  = _control.ClientSize.Width;
                int displayW = _control.DisplayRectangle.Width;

                if (_vBar != null)
                {
                    bool needV = _wantVert && displayH > clientH && clientH > 0;
                    _vBar.Visible = needV;
                    if (needV)
                    {
                        _vBar.Minimum     = 0;
                        _vBar.Maximum     = displayH;
                        _vBar.LargeChange = clientH;
                        _vBar.Value       = -_control.AutoScrollPosition.Y;
                    }
                }

                if (_hBar != null)
                {
                    bool needH = _wantHorz && displayW > clientW && clientW > 0;
                    _hBar.Visible = needH;
                    if (needH)
                    {
                        _hBar.Minimum     = 0;
                        _hBar.Maximum     = displayW;
                        _hBar.LargeChange = clientW;
                        _hBar.Value       = -_control.AutoScrollPosition.X;
                    }
                }
            }
            finally { _isUpdating = false; }

            HideNativeBars();
            PositionBars();
        }

        private void HideNativeBars()
        {
            try
            {
                if (_control.IsHandleCreated)
                    ShowScrollBar(_control.Handle, SB_BOTH, false);
            }
            catch { }
        }

        // ── Public API ───────────────────────────────────────────────────────────

        /// <summary>
        /// Sostituisce le scrollbar native con le custom.
        /// Rileva automaticamente se serve verticale, orizzontale o entrambe.
        /// </summary>
        public static ScrollBarHelper Attach(ScrollableControl control)
            => new ScrollBarHelper(control, vertical: true, horizontal: true);

        /// <summary>Solo scroll verticale.</summary>
        public static ScrollBarHelper AttachVertical(ScrollableControl control)
            => new ScrollBarHelper(control, vertical: true, horizontal: false);

        /// <summary>Solo scroll orizzontale.</summary>
        public static ScrollBarHelper AttachHorizontal(ScrollableControl control)
            => new ScrollBarHelper(control, vertical: false, horizontal: true);

        /// <summary>Controllo completo su quali assi attivare.</summary>
        public static ScrollBarHelper Attach(ScrollableControl control, bool vertical, bool horizontal)
            => new ScrollBarHelper(control, vertical, horizontal);
    }
}
