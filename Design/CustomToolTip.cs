using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VRCGalleryManager.Design
{
    public class CustomToolTip : ToolTip
    {
        private int borderRadius = 10;
        private int borderSize = 2;
        private Color borderColor = Color.PaleVioletRed;
        private Color toolTipBackColor = Color.MediumSlateBlue;
        private Color toolTipForeColor = Color.White;

        [Category("VRCGalleryManager")]
        public int BorderRadius
        {
            get => borderRadius;
            set => borderRadius = value;
        }

        [Category("VRCGalleryManager")]
        public int BorderSize
        {
            get => borderSize;
            set => borderSize = value;
        }

        [Category("VRCGalleryManager")]
        public Color BorderColor
        {
            get => borderColor;
            set => borderColor = value;
        }

        [Category("VRCGalleryManager")]
        public Color ToolTipBackColor
        {
            get => toolTipBackColor;
            set => toolTipBackColor = value;
        }

        [Category("VRCGalleryManager")]
        public Color ToolTipForeColor
        {
            get => toolTipForeColor;
            set => toolTipForeColor = value;
        }

        public CustomToolTip()
        {
            OwnerDraw = true;
            Draw += CustomToolTip_Draw;
            Popup += CustomToolTip_Popup;
        }

        private void CustomToolTip_Popup(object sender, PopupEventArgs e)
        {
        }

        private void CustomToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = e.Bounds;
            rect.Inflate(-1, -1);
            int effectiveRadius = Math.Min(borderRadius, Math.Min(rect.Width, rect.Height) / 2);
            using (GraphicsPath path = GetRoundedRectanglePath(rect, effectiveRadius))
            {
                using (SolidBrush brush = new SolidBrush(toolTipBackColor))
                {
                    e.Graphics.FillPath(brush, path);
                }
                using (Pen pen = new Pen(borderColor, borderSize))
                {
                    e.Graphics.DrawPath(pen, path);
                }
                TextRenderer.DrawText(
                    e.Graphics,
                    e.ToolTipText,
                    e.Font,
                    rect,
                    toolTipForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
            path.AddArc(arcRect, 180, 90);
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
