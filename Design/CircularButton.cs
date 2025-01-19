using Svg;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VRCGalleryManager.Design
{
    public class CircularButton : Button
    {
        private int borderSize = 0;
        private Color borderColor = Color.PaleVioletRed;

        private Image svgImage;
        private string svgResource;
        private ContentAlignment svgAlignment = ContentAlignment.MiddleCenter;
        private Color svgColor = Color.Black;
        private Size svgSize = new Size(50, 50);
        private Padding svgPadding = new Padding(0);
        private Point svgOffset = Point.Empty;

        [Category("VRCGalleryManager")]
        public int BorderSize
        {
            get => borderSize;
            set
            {
                borderSize = value;
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        [Category("VRCGalleryManager")]
        public Color BackgroundColor
        {
            get => BackColor;
            set => BackColor = value;
        }

        [Category("VRCGalleryManager")]
        public Color TextColor
        {
            get => ForeColor;
            set => ForeColor = value;
        }

        [Category("VRCGalleryManager")]
        [TypeConverter(typeof(ResourceNameConverter))]
        public string SvgResource
        {
            get => svgResource;
            set
            {
                svgResource = value;
                if (!string.IsNullOrEmpty(svgResource))
                {
                    _ = UpdateSvgContentAsync(svgResource);
                }
                svgImage = null; // Reset the image to force re-rendering
                Refresh(); // Ensure the control repaints itself
            }
        }
        private async Task UpdateSvgContentAsync(string resourceName)
        {
            SvgContent = await LoadSvgFromResourcesAsync(resourceName);
            svgImage = null; // Reset the image to force re-rendering
            Refresh(); // Ensure the control repaints itself
        }

        [Browsable(false)]
        public string SvgContent { get; set; }

        [Category("VRCGalleryManager")]
        public ContentAlignment SvgAlignment
        {
            get { return svgAlignment; }
            set
            {
                if (svgAlignment != value)
                {
                    svgAlignment = value;
                    Refresh(); // Force redraw to apply alignment changes
                }
            }
        }

        [Category("VRCGalleryManager")]
        public Color SvgColor
        {
            get { return svgColor; }
            set
            {
                if (svgColor != value)
                {
                    svgColor = value;
                    svgImage = null; // Reset SVG image to apply color changes
                    Refresh(); // Force redraw
                }
            }
        }

        [Category("VRCGalleryManager")]
        public Size SvgSize
        {
            get { return svgSize; }
            set
            {
                if (svgSize != value)
                {
                    svgSize = value;
                    svgImage = null; // Reset SVG image to apply size changes
                    Refresh(); // Force redraw
                }
            }
        }

        [Category("VRCGalleryManager")]
        public Padding SvgPadding
        {
            get { return svgPadding; }
            set
            {
                if (svgPadding != value)
                {
                    svgPadding = value;
                    Refresh(); // Force redraw to apply padding changes
                }
            }
        }

        [Category("VRCGalleryManager")]
        public Point SvgOffset
        {
            get { return svgOffset; }
            set
            {
                if (svgOffset != value)
                {
                    svgOffset = value;
                    Refresh(); // Force redraw to apply offset changes
                }
            }
        }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            var rectSurface = ClientRectangle;
            var rectBorder = Rectangle.Inflate(rectSurface, -borderSize, -borderSize);

            using (var pathSurface = new GraphicsPath())
            using (var pathBorder = new GraphicsPath())
            using (var penSurface = new Pen(Parent.BackColor, borderSize))
            using (var penBorder = new Pen(borderColor, borderSize))
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                pathSurface.AddEllipse(rectSurface);
                Region = new Region(pathSurface);

                pevent.Graphics.DrawEllipse(penSurface, rectSurface);

                if (borderSize > 0)
                {
                    pathBorder.AddEllipse(rectBorder);
                    pevent.Graphics.DrawPath(penBorder, pathBorder);
                }
            }
            // Draw SVG image
            if (!string.IsNullOrEmpty(SvgContent))
            {
                try
                {
                    if (svgImage == null)
                    {
                        using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(SvgContent)))
                        {
                            var svgDoc = SvgDocument.Open<SvgDocument>(stream);
                            if (SvgColor != Color.Empty)
                            {
                                foreach (var element in svgDoc.Descendants())
                                {
                                    if (element.Fill != null)
                                    {
                                        element.Fill = new SvgColourServer(SvgColor);
                                    }
                                }
                            }
                            svgImage = svgDoc.Draw(SvgSize.Width, SvgSize.Height); // Use SvgSize to scale the SVG
                        }
                    }

                    if (svgImage != null)
                    {
                        Rectangle imageRect = GetAlignedRectangle(SvgSize, ClientRectangle, SvgAlignment);

                        // Apply padding
                        imageRect.X += SvgPadding.Left;
                        imageRect.Y += SvgPadding.Top;
                        imageRect.Width -= (SvgPadding.Left + SvgPadding.Right);
                        imageRect.Height -= (SvgPadding.Top + SvgPadding.Bottom);

                        // Apply offset
                        imageRect.Offset(SvgOffset);

                        // Draw the image
                        pevent.Graphics.DrawImage(svgImage, imageRect);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error rendering SVG: {ex.Message}");
                }
            }
        }

        private async Task<string> LoadSvgFromResourcesAsync(string resourceName)
        {
            return await Task.Run(() =>
            {
                Console.WriteLine($"Loading resource: {resourceName}");

                var resourceManager = Properties.Resources.ResourceManager;
                var resourceSet = resourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);

                foreach (DictionaryEntry entry in resourceSet)
                {
                    if (entry.Key.ToString() == resourceName)
                    {
                        if (entry.Value is string svgContent)
                        {
                            Console.WriteLine($"Loaded SVG content as string: {resourceName}");
                            return svgContent;
                        }
                        else if (entry.Value is byte[] svgBytes)
                        {
                            Console.WriteLine($"Loaded SVG content as byte array: {resourceName}");
                            return System.Text.Encoding.UTF8.GetString(svgBytes);
                        }
                        else
                        {
                            Console.WriteLine($"Resource {resourceName} is not a valid SVG format.");
                        }
                    }
                }

                Console.WriteLine($"Resource {resourceName} not found.");
                return null;
            });
        }


        private Rectangle GetAlignedRectangle(Size imageSize, Rectangle container, ContentAlignment alignment)
        {
            int x = container.X;
            int y = container.Y;

            if (alignment == ContentAlignment.TopCenter || alignment == ContentAlignment.MiddleCenter || alignment == ContentAlignment.BottomCenter)
            {
                x += (container.Width - imageSize.Width) / 2;
            }
            else if (alignment == ContentAlignment.TopRight || alignment == ContentAlignment.MiddleRight || alignment == ContentAlignment.BottomRight)
            {
                x += container.Width - imageSize.Width;
            }

            if (alignment == ContentAlignment.MiddleLeft || alignment == ContentAlignment.MiddleCenter || alignment == ContentAlignment.MiddleRight)
            {
                y += (container.Height - imageSize.Height) / 2;
            }
            else if (alignment == ContentAlignment.BottomLeft || alignment == ContentAlignment.BottomCenter || alignment == ContentAlignment.BottomRight)
            {
                y += container.Height - imageSize.Height;
            }

            return new Rectangle(new Point(x, y), imageSize);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Width = Height;
        }

        public CircularButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Size = new Size(100, 100);
            BackColor = Color.MediumSlateBlue;
            ForeColor = Color.White;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Parent.BackColorChanged += Container_BackColorChanged;
            Cursor = Cursors.Hand;
        }

        private void Container_BackColorChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
