using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VRCGalleryManager.Design
{
    public class ScrollBarHelper : NativeWindow
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, [MarshalAs(UnmanagedType.Bool)] bool bShow);

        private const int SB_VERT = 1;
        private const int WM_NCCALCSIZE = 0x0083;

        private readonly ScrollableControl _control;
        private readonly ModernVScrollBar _scrollBar;
        private bool _isUpdating = false;

        public ScrollBarHelper(ScrollableControl control)
        {
            _control = control ?? throw new ArgumentNullException(nameof(control));
            
            _scrollBar = new ModernVScrollBar();
            _scrollBar.Width = 6; // Thin and elegant!
            _scrollBar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;

            // Wire events
            _scrollBar.Scroll += CustomScrollBar_Scroll;
            _control.Scroll += Control_Scroll;
            _control.Layout += Control_Layout;
            _control.SizeChanged += Control_SizeChanged;
            _control.ControlAdded += (s, e) => UpdateScrollBarValues();
            _control.ControlRemoved += (s, e) => UpdateScrollBarValues();
            _control.MouseWheel += Control_MouseWheel;

            if (_control.Parent != null)
            {
                AddToParent();
            }
            else
            {
                _control.ParentChanged += (s, e) => AddToParent();
            }

            // Bind to HandleCreated to hide native scrollbar immediately and hook messages
            if (_control.IsHandleCreated)
            {
                HideNativeScrollBar();
                AssignHandle(_control.Handle);
            }
            
            _control.HandleCreated += (s, e) => {
                HideNativeScrollBar();
                AssignHandle(_control.Handle);
            };

            _control.HandleDestroyed += (s, e) => {
                ReleaseHandle();
            };

            UpdateScrollBarValues();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCCALCSIZE)
            {
                HideNativeScrollBar();
            }
            base.WndProc(ref m);
        }

        private void AddToParent()
        {
            if (_control.Parent == null || _scrollBar.Parent == _control.Parent) return;

            _control.Parent.Controls.Add(_scrollBar);
            _scrollBar.BringToFront();
            PositionScrollBar();
        }

        private void PositionScrollBar()
        {
            if (_control.Parent == null) return;
            
            _scrollBar.Location = new Point(
                _control.Right - _scrollBar.Width - 2, 
                _control.Top + 2
            );
            _scrollBar.Height = _control.Height - 4;
        }

        private void CustomScrollBar_Scroll(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            _isUpdating = true;
            try
            {
                _control.AutoScrollPosition = new Point(0, _scrollBar.Value);
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void Control_Scroll(object sender, ScrollEventArgs e)
        {
            UpdatePositionFromControl();
        }

        private void Control_MouseWheel(object sender, MouseEventArgs e)
        {
            _control.BeginInvoke(new Action(UpdatePositionFromControl));
        }

        private void Control_Layout(object sender, LayoutEventArgs e)
        {
            UpdateScrollBarValues();
        }

        private void Control_SizeChanged(object sender, EventArgs e)
        {
            PositionScrollBar();
            UpdateScrollBarValues();
        }

        private void UpdatePositionFromControl()
        {
            if (_isUpdating) return;
            _isUpdating = true;
            try
            {
                _scrollBar.Value = -_control.AutoScrollPosition.Y;
            }
            finally
            {
                _isUpdating = false;
            }
            HideNativeScrollBar();
        }

        private void UpdateScrollBarValues()
        {
            if (_isUpdating) return;
            _isUpdating = true;
            try
            {
                int clientHeight = _control.ClientSize.Height;
                int displayHeight = _control.DisplayRectangle.Height;

                if (displayHeight > clientHeight && clientHeight > 0)
                {
                    _scrollBar.Visible = true;
                    _scrollBar.Minimum = 0;
                    _scrollBar.Maximum = displayHeight;
                    _scrollBar.LargeChange = clientHeight;
                    _scrollBar.Value = -_control.AutoScrollPosition.Y;
                }
                else
                {
                    _scrollBar.Visible = false;
                }
            }
            finally
            {
                _isUpdating = false;
            }
            HideNativeScrollBar();
            PositionScrollBar();
        }

        private void HideNativeScrollBar()
        {
            try
            {
                if (_control.IsHandleCreated)
                {
                    ShowScrollBar(_control.Handle, SB_VERT, false);
                }
            }
            catch { }
        }

        public static ScrollBarHelper Attach(ScrollableControl control)
        {
            return new ScrollBarHelper(control);
        }
    }
}
