using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCGalleryManager.Core
{
    public static class ClipboardHandler
    {
        private static readonly string TempDirectory;
        private static readonly HttpClient HttpClient = new HttpClient();

        static ClipboardHandler()
        {
            TempDirectory = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            try
            {
                Directory.CreateDirectory(TempDirectory);
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification(
                    $"Could not create temp directory: {ex.Message}",
                    "Initialization Error",
                    NotificationType.Error
                );
            }
        }

        public static async Task ClipboardDataImageOrLink(Button pasteButton, Action<string> uploadImage)
        {
            var data = Clipboard.GetDataObject();
            if (data == null)
            {
                NotificationManager.ShowNotification("Clipboard is empty!", "Error", NotificationType.Error);
                return;
            }

            string savedPath = null;

            if (data.GetDataPresent("PNG"))
            {
                savedPath = HandlePngFromClipboard(data);
            }
            else if (data.GetDataPresent(DataFormats.FileDrop))
            {
                HandleFileDrop(data, pasteButton, uploadImage);
                return;
            }
            else if (data.GetDataPresent(DataFormats.Text))
            {
                string text = Clipboard.GetText();
                if (await IsValidImageLinkAsync(text))
                    savedPath = await SaveImageFromUrlAsync(text, pasteButton);
                else
                {
                    NotificationManager.ShowNotification("The link does not contain a valid image!", "Error", NotificationType.Error);
                    return;
                }
            }
            else
            {
                NotificationManager.ShowNotification("No image, file or link found in the clipboard!", "Error", NotificationType.Error);
                return;
            }

            if (!string.IsNullOrEmpty(savedPath))
                uploadImage(savedPath);
        }

        private static string HandlePngFromClipboard(IDataObject data)
        {
            try
            {
                const string pngFormat = "PNG";
                if (data.GetData(pngFormat) is MemoryStream pngStream)
                {
                    using var src = Image.FromStream(pngStream, true, true);
                    string filePath = GetTempFilePath("Pasted-Image");
                    src.Save(filePath, ImageFormat.Png);
                    NotificationManager.ShowNotification("Image pasted and saved successfully!", "Paste Image", NotificationType.Info);
                    return filePath;
                }
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification($"Error saving PNG from clipboard: {ex.Message}", "Error", NotificationType.Error);
            }
            return null;
        }

        private static void HandleFileDrop(IDataObject data, Button pasteButton, Action<string> uploadImage)
        {
            if (data.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                string file = files[0];
                string ext = Path.GetExtension(file).ToLowerInvariant();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif")
                {
                    NotificationManager.ShowNotification("Dropped file is not a supported image type!", "Error", NotificationType.Error);
                    return;
                }
                try
                {
                    pasteButton.Enabled = false;
                    uploadImage(file);
                }
                finally
                {
                    pasteButton.Enabled = true;
                }
            }
        }

        private static async Task<bool> IsValidImageLinkAsync(string url)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Head, url);
                var res = await HttpClient.SendAsync(req);
                if (!res.IsSuccessStatusCode)
                {
                    res = await HttpClient.SendAsync(
                        new HttpRequestMessage(HttpMethod.Get, url),
                        HttpCompletionOption.ResponseHeadersRead
                    );
                }
                string ct = res.Content.Headers.ContentType?.MediaType;
                return ct?.StartsWith("image/", StringComparison.OrdinalIgnoreCase) == true;
            }
            catch
            {
                return false;
            }
        }

        private static async Task<string> SaveImageFromUrlAsync(string url, Button pasteButton = null)
        {
            pasteButton?.Invoke((Action)(() => pasteButton.Enabled = false));
            try
            {
                var res = await HttpClient.GetAsync(url);
                string ct = res.Content.Headers.ContentType?.MediaType;
                if (!res.IsSuccessStatusCode || !ct.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    return null;

                using var stream = await res.Content.ReadAsStreamAsync();
                using var src = Image.FromStream(stream, true, true);
                using var bmp = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.DrawImage(src, 0, 0);
                }
                string filePath = GetTempFilePath("Downloaded-Image");
                bmp.Save(filePath, ImageFormat.Png);
                NotificationManager.ShowNotification("Image downloaded and saved successfully!", "Download Image", NotificationType.Info);
                return filePath;
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification($"Error downloading image: {ex.Message}", "Error", NotificationType.Error);
                return null;
            }
            finally
            {
                pasteButton?.Invoke((Action)(() => pasteButton.Enabled = true));
            }
        }

        private static string GetTempFilePath(string prefix)
            => Path.Combine(TempDirectory, $"{prefix}_{Guid.NewGuid():N}.png");
    }
}
