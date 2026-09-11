using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using ZXing;
using ZXing.Common;
using Image = SixLabors.ImageSharp.Image;
using Size = SixLabors.ImageSharp.Size;
using ResizeMode = SixLabors.ImageSharp.Processing.ResizeMode;

namespace VRCGalleryManager.Core
{
    public class QrCodeItem
    {
        public string Text { get; set; } = "";
        public bool IsUrl { get; set; }
        public string? Url { get; set; }
        public string Category { get; set; } = "Text";
        public string DisplayTitle { get; set; } = "";

        public static QrCodeItem Parse(string rawText)
        {
            var trimmed = rawText.Trim();
            var item = new QrCodeItem { Text = trimmed };

            // Check if it's a VRChat ID directly (wrld_, avtr_, usr_)
            if (trimmed.StartsWith("wrld_", StringComparison.OrdinalIgnoreCase))
            {
                item.IsUrl = true;
                item.Url = $"https://vrchat.com/home/world/{trimmed}";
                item.Category = "VRChat World";
                item.DisplayTitle = $"World ({trimmed})";
                return item;
            }
            if (trimmed.StartsWith("avtr_", StringComparison.OrdinalIgnoreCase))
            {
                item.IsUrl = true;
                item.Url = $"https://vrchat.com/home/avatar/{trimmed}";
                item.Category = "VRChat Avatar";
                item.DisplayTitle = $"Avatar ({trimmed})";
                return item;
            }
            if (trimmed.StartsWith("usr_", StringComparison.OrdinalIgnoreCase))
            {
                item.IsUrl = true;
                item.Url = $"https://vrchat.com/home/user/{trimmed}";
                item.Category = "VRChat User";
                item.DisplayTitle = $"User ({trimmed})";
                return item;
            }

            // Check if it's a standard URL
            if (Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                item.IsUrl = true;
                item.Url = uri.ToString();
                string host = uri.Host.ToLowerInvariant();

                if (host.Contains("vrchat.com"))
                {
                    string path = uri.AbsolutePath.ToLowerInvariant();
                    if (path.Contains("/world/"))
                    {
                        item.Category = "VRChat World";
                        item.DisplayTitle = "VRChat World Link";
                    }
                    else if (path.Contains("/avatar/"))
                    {
                        item.Category = "VRChat Avatar";
                        item.DisplayTitle = "VRChat Avatar Link";
                    }
                    else if (path.Contains("/user/"))
                    {
                        item.Category = "VRChat User";
                        item.DisplayTitle = "VRChat User Profile";
                    }
                    else
                    {
                        item.Category = "VRChat";
                        item.DisplayTitle = "VRChat Link";
                    }
                }
                else if (host.Contains("discord.gg") || host.Contains("discord.com"))
                {
                    item.Category = "Discord";
                    item.DisplayTitle = "Discord Server / Invite";
                }
                else if (host.Contains("booth.pm"))
                {
                    item.Category = "Booth";
                    item.DisplayTitle = "Booth Item / Shop";
                }
                else if (host.Contains("gumroad.com"))
                {
                    item.Category = "Gumroad";
                    item.DisplayTitle = "Gumroad Store";
                }
                else if (host.Contains("twitter.com") || host.Contains("x.com"))
                {
                    item.Category = "X / Twitter";
                    item.DisplayTitle = "X / Twitter Profile";
                }
                else if (host.Contains("youtube.com") || host.Contains("youtu.be"))
                {
                    item.Category = "YouTube";
                    item.DisplayTitle = "YouTube Video / Channel";
                }
                else if (host.Contains("github.com"))
                {
                    item.Category = "GitHub";
                    item.DisplayTitle = "GitHub Repository";
                }
                else
                {
                    item.Category = "Web Link";
                    item.DisplayTitle = uri.Host;
                }

                return item;
            }

            // Raw text
            item.Category = "Text";
            item.DisplayTitle = trimmed.Length > 30 ? trimmed.Substring(0, 27) + "..." : trimmed;
            return item;
        }
    }

    public class QrCodeService
    {
        private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(15) };

        /// <summary>
        /// Scans an image file from a local path or an HTTP URL for QR codes.
        /// </summary>
        public async Task<List<QrCodeItem>> ScanAsync(string sourcePathOrUrl, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(sourcePathOrUrl))
                return new List<QrCodeItem>();

            if (sourcePathOrUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                sourcePathOrUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return await ScanUrlAsync(sourcePathOrUrl, ct);
            }

            return await ScanFileAsync(sourcePathOrUrl, ct);
        }

        /// <summary>
        /// Scans a local image file.
        /// </summary>
        public async Task<List<QrCodeItem>> ScanFileAsync(string filePath, CancellationToken ct = default)
        {
            if (!File.Exists(filePath))
                return new List<QrCodeItem>();

            return await Task.Run(() =>
            {
                try
                {
                    using var image = Image.Load<Rgb24>(filePath);
                    return DecodeImage(image);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[QrCodeService] Error scanning file: {ex.Message}");
                    return new List<QrCodeItem>();
                }
            }, ct);
        }

        /// <summary>
        /// Scans an image from raw bytes.
        /// </summary>
        public async Task<List<QrCodeItem>> ScanBytesAsync(byte[] imageBytes, CancellationToken ct = default)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return new List<QrCodeItem>();

            return await Task.Run(() =>
            {
                try
                {
                    using var image = Image.Load<Rgb24>(imageBytes);
                    return DecodeImage(image);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[QrCodeService] Error scanning bytes: {ex.Message}");
                    return new List<QrCodeItem>();
                }
            }, ct);
        }

        /// <summary>
        /// Downloads an image from an HTTP URL and scans it.
        /// </summary>
        public async Task<List<QrCodeItem>> ScanUrlAsync(string url, CancellationToken ct = default)
        {
            try
            {
                var bytes = await HttpClient.GetByteArrayAsync(url, ct);
                return await ScanBytesAsync(bytes, ct);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[QrCodeService] Error downloading URL: {ex.Message}");
                return new List<QrCodeItem>();
            }
        }

        private static List<QrCodeItem> DecodeImage(Image<Rgb24> image)
        {
            var foundItems = new List<QrCodeItem>();
            var seenTexts = new HashSet<string>(StringComparer.Ordinal);

            // If image is colossal (e.g. 8K screenshot), downscale slightly to speed up decoding
            if (image.Width > 3840 || image.Height > 3840)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(3840, 3840)
                }));
            }

            byte[] rgbBytes = new byte[image.Width * image.Height * 3];
            image.CopyPixelDataTo(rgbBytes);

            var luminanceSource = new RGBLuminanceSource(rgbBytes, image.Width, image.Height);
            var reader = new BarcodeReaderGeneric
            {
                AutoRotate = true,
                Options = new DecodingOptions
                {
                    PossibleFormats = new[] { BarcodeFormat.QR_CODE },
                    TryHarder = true
                }
            };

            var results = reader.DecodeMultiple(luminanceSource);
            if (results != null)
            {
                foreach (var res in results)
                {
                    if (!string.IsNullOrWhiteSpace(res.Text) && seenTexts.Add(res.Text.Trim()))
                    {
                        foundItems.Add(QrCodeItem.Parse(res.Text));
                    }
                }
            }

            return foundItems;
        }
    }
}
