using System.Collections.Generic;
using VRCGalleryManager.Design;
using VRCGalleryManager.Properties;

namespace VRCGalleryManager.Forms.UIComponents
{
    public static class CircularButtonTools
    {
        private static readonly Dictionary<string, string> SvgCache = new();

        public static CircularButton CreateButton(string type, EventHandler onClick)
        {
            CircularButton button = new()
            {
                SvgColor = Color.White,
                SvgSize = new Size(15, 15),
                Width = 25,
                Height = 25,
                Dock = DockStyle.Right,
                BackColor = type switch
                {
                    "open" => Color.FromArgb(96, 159, 255),
                    "delete" => Color.FromArgb(255, 114, 114),
                    "picflowupload" => Color.FromArgb(100, 200, 100),
                    _ => Color.Gray
                },
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            CustomToolTip toolTip = new()
            {
                ToolTipBackColor = Color.FromArgb(7, 36, 43),
                ToolTipForeColor = Color.FromArgb(106, 227, 249),
                BorderSize = 1,
                BorderRadius = 5,
                BorderColor = Color.FromArgb(5, 55, 66)
            };

            toolTip.SetToolTip(button, type switch
            {
                "open" => "Open in browser",
                "delete" => "Delete",
                "picflowupload" => "Upload",
                _ => null
            });

            button.MouseLeave += (sender, args) => toolTip.Hide(button);

            string resourceName = type switch
            {
                "open" => nameof(Resources.open_in_browser_svgrepo_com),
                "delete" => nameof(Resources.delete_svgrepo_com),
                "picflowupload" => nameof(Resources.note_add_svgrepo_com),
                _ => null
            };

            if (!string.IsNullOrEmpty(resourceName))
                button.SvgContent = GetSvgFromCache(resourceName);

            if (onClick != null)
                button.Click += onClick;

            return button;
        }

        private static string GetSvgFromCache(string resourceName)
        {
            if (SvgCache.TryGetValue(resourceName, out var cachedSvg))
                return cachedSvg;

            var resourceObject = Resources.ResourceManager.GetObject(resourceName);
            string svgContent = resourceObject switch
            {
                string content => content,
                byte[] bytes => System.Text.Encoding.UTF8.GetString(bytes),
                _ => null
            };

            if (!string.IsNullOrEmpty(svgContent))
                SvgCache[resourceName] = svgContent;

            return svgContent;
        }
    }
}