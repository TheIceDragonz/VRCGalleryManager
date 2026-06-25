using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace VRCGalleryManager.Design
{
    public class CustomContextMenuStrip : ContextMenuStrip
    {
        // Colors
        private Color primaryColor = Color.FromArgb(106, 227, 249);
        private Color backgroundColor = Color.FromArgb(25, 25, 25);
        private Color textColor = Color.White;
        private Color hoverColor = Color.FromArgb(45, 45, 45);
        private Color borderColor = Color.FromArgb(50, 50, 50);
        private int borderRadius = 10;
        private int borderSize = 1;

        [Category("VRCGalleryManager")]
        public int BorderSize
        {
            get { return borderSize; }
            set { borderSize = value; Invalidate(); }
        }

        [Category("VRCGalleryManager")]
        public int BorderRadius
        {
            get { return borderRadius; }
            set { borderRadius = value; UpdateRegion(); Invalidate(); }
        }

        [Category("VRCGalleryManager")]
        public Color PrimaryColor
        {
            get { return primaryColor; }
            set { primaryColor = value; UpdateRenderer(); }
        }

        [Category("VRCGalleryManager")]
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; UpdateRenderer(); }
        }

        [Category("VRCGalleryManager")]
        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; UpdateRenderer(); }
        }

        [Category("VRCGalleryManager")]
        public Color HoverColor
        {
            get { return hoverColor; }
            set { hoverColor = value; UpdateRenderer(); }
        }

        [Category("VRCGalleryManager")]
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; UpdateRenderer(); }
        }

        public CustomContextMenuStrip()
        {
            this.Renderer = new MenuRenderer(new MenuColorTable(primaryColor, backgroundColor, textColor, hoverColor, borderColor));
            this.BackColor = backgroundColor;
            this.ForeColor = textColor;
            this.ShowImageMargin = false; // Hide image margin by default for modern look
            this.Font = new Font("Segoe UI", 9F);
            this.Cursor = Cursors.Hand; // Show hand cursor on hover
        }

        private void UpdateRenderer()
        {
            this.Renderer = new MenuRenderer(new MenuColorTable(primaryColor, backgroundColor, textColor, hoverColor, borderColor));
            this.BackColor = backgroundColor;
            this.ForeColor = textColor;
        }

        protected override void OnItemAdded(ToolStripItemEventArgs e)
        {
            base.OnItemAdded(e);
            e.Item.ForeColor = this.TextColor;
            if (e.Item.Font.Name == "Segoe UI" && e.Item.Font.Size == 9F)
            {
                e.Item.Font = new Font("Segoe UI", 10F); // slightly larger font
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateRegion();
        }

        private void UpdateRegion()
        {
            if (this.Width > 0 && this.Height > 0)
            {
                using (GraphicsPath path = GetRoundedPath(new Rectangle(0, 0, this.Width, this.Height), borderRadius))
                {
                    this.Region = new Region(path);
                }
            }
        }

        public GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;
            if (curveSize > 0)
            {
                path.StartFigure();
                path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
                path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
                path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
                path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
                path.CloseFigure();
            }
            else
            {
                path.AddRectangle(rect);
            }
            return path;
        }
    }

    internal class MenuRenderer : ToolStripProfessionalRenderer
    {
        private Color primaryColor;
        private Color textColor;

        public MenuRenderer(MenuColorTable colorTable) : base(colorTable)
        {
            this.primaryColor = colorTable.PrimaryColor;
            this.textColor = colorTable.TextColor;
            this.RoundedEdges = false;
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = textColor;
            if (e.Item.TextAlign == ContentAlignment.MiddleCenter)
            {
                e.TextFormat &= ~TextFormatFlags.Left;
                e.TextFormat |= TextFormatFlags.HorizontalCenter;
                e.TextFormat &= ~TextFormatFlags.Right;
                e.TextRectangle = new Rectangle(Point.Empty, e.Item.Size);
            }
            base.OnRenderItemText(e);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = textColor;
            base.OnRenderArrow(e);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is CustomContextMenuStrip customMenu)
            {
                int radius = customMenu.BorderRadius;
                if (radius > 0)
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    var rect = new Rectangle(0, 0, customMenu.Width - 1, customMenu.Height - 1);
                    using (GraphicsPath path = customMenu.GetRoundedPath(rect, radius))
                    using (Pen pen = new Pen(customMenu.BorderColor, customMenu.BorderSize))
                    {
                        pen.Alignment = PenAlignment.Inset;
                        e.Graphics.DrawPath(pen, path);
                    }
                    return; // Skip default border
                }
            }
            base.OnRenderToolStripBorder(e);
        }
    }

    internal class MenuColorTable : ProfessionalColorTable
    {
        private Color primaryColor;
        private Color backgroundColor;
        private Color textColor;
        private Color hoverColor;
        private Color borderColor;

        public MenuColorTable(Color primaryColor, Color backgroundColor, Color textColor, Color hoverColor, Color borderColor)
        {
            this.primaryColor = primaryColor;
            this.backgroundColor = backgroundColor;
            this.textColor = textColor;
            this.hoverColor = hoverColor;
            this.borderColor = borderColor;
            this.UseSystemColors = false;
        }

        public Color PrimaryColor => primaryColor;
        public Color BackgroundColor => backgroundColor;
        public Color TextColor => textColor;

        // Background
        public override Color ToolStripDropDownBackground => backgroundColor;
        public override Color MenuBorder => borderColor;
        public override Color MenuItemBorder => hoverColor;

        // Hovered item
        public override Color MenuItemSelected => hoverColor;
        public override Color MenuItemSelectedGradientBegin => hoverColor;
        public override Color MenuItemSelectedGradientEnd => hoverColor;

        // Pressed item
        public override Color MenuItemPressedGradientBegin => hoverColor;
        public override Color MenuItemPressedGradientEnd => hoverColor;
        public override Color MenuItemPressedGradientMiddle => hoverColor;

        // Image Margin
        public override Color ImageMarginGradientBegin => backgroundColor;
        public override Color ImageMarginGradientMiddle => backgroundColor;
        public override Color ImageMarginGradientEnd => backgroundColor;

        // Separator
        public override Color SeparatorDark => borderColor;
        public override Color SeparatorLight => backgroundColor;
    }
}
