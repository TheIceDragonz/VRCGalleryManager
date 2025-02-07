using Svg;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace VRCGalleryManager.Design
{
    public class RoundedButton : Button
    {
        private int borderSize = 0;
        public int BorderRadius { get; set; } = 15;
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
            get { return borderSize; }
            set
            {
                if (borderSize != value)
                {
                    borderSize = value;
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                if (borderColor != value)
                {
                    borderColor = value;
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public Color BackgroundColor
        {
            get { return BackColor; }
            set
            {
                if (BackColor != value)
                {
                    BackColor = value;
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        public Color TextColor
        {
            get { return ForeColor; }
            set
            {
                if (ForeColor != value)
                {
                    ForeColor = value;
                    Invalidate();
                }
            }
        }

        [Category("VRCGalleryManager")]
        [TypeConverter(typeof(ResourceNameConverter))]
        public string SvgResource
        {
            get { return svgResource; }
            set
            {
                svgResource = value;
                if (!string.IsNullOrEmpty(svgResource))
                {
                    SvgContent = LoadSvgFromResources(svgResource);
                }
                svgImage = null;
                Refresh();
            }
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
                    Refresh();
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
                    svgImage = null;
                    Refresh();
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
                    svgImage = null;
                    Refresh();
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
                    Refresh();
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
                    Refresh();
                }
            }
        }

        private string LoadSvgFromResources(string resourceName)
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
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            // Clear the entire drawing area
            base.OnPaint(pevent);

            Rectangle rectSurface = ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -borderSize, -borderSize);
            int smoothSize = 2;
            if (borderSize > 0)
                smoothSize = borderSize;

            // Draw rounded button background and border
            if (BorderRadius > 2) //Rounded button
            {
                using (GraphicsPath pathSurface = GetFigurePath(rectSurface, BorderRadius))
                using (GraphicsPath pathBorder = GetFigurePath(rectBorder, BorderRadius - borderSize))
                using (Pen penSurface = new Pen(Parent.BackColor, smoothSize))
                using (Pen penBorder = new Pen(borderColor, borderSize))
                {
                    pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    Region = new Region(pathSurface);

                    pevent.Graphics.DrawPath(penSurface, pathSurface);

                    if (borderSize >= 1)
                        pevent.Graphics.DrawPath(penBorder, pathBorder);
                }
            }
            else //Normal button
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.None;
                Region = new Region(rectSurface);
                if (borderSize >= 1)
                {
                    using (Pen penBorder = new Pen(borderColor, borderSize))
                    {
                        penBorder.Alignment = PenAlignment.Inset;
                        pevent.Graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);
                    }
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

        public RoundedButton()
        {
            SetStyle(ControlStyles.Selectable, false);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Size = new Size(150, 40);
            BackColor = Color.MediumSlateBlue;
            ForeColor = Color.White;
            Resize += Button_Resize;
        }

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

        private void Button_Resize(object sender, EventArgs e)
        {
            if (BorderRadius > Height)
                BorderRadius = Height;
        }
    }

    public class ResourceNameConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var resources = Properties.Resources.ResourceManager
                .GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true)
                .Cast<DictionaryEntry>()
                .Where(r => r.Value is string || r.Value is byte[])
                .Select(r => r.Key.ToString())
                .ToList();

            return new StandardValuesCollection(resources);
        }
    }
}
