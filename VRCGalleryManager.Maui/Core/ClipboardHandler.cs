using Bitmap = System.Drawing.Bitmap;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Graphics = System.Drawing.Graphics;
using Image = System.Drawing.Image;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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
                Console.WriteLine($"Could not create temp directory: {ex.Message}");
            }
        }

        public static async Task<string> ClipboardDataImageOrLink(Action<string> uploadImage = null)
        {
            return await Microsoft.Maui.ApplicationModel.MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.Default.HasText)
                {
                    string text = await Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.Default.GetTextAsync();
                    if (!string.IsNullOrEmpty(text) && await IsValidImageLinkAsync(text))
                    {
                        string savedPath = await SaveImageFromUrlAsync(text);
                        if (!string.IsNullOrEmpty(savedPath))
                        {
                            uploadImage?.Invoke(savedPath);
                            return savedPath;
                        }
                    }
                }

#if WINDOWS
                try
                {
                    var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();

                    if (dataPackageView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
                    {
                        var items = await dataPackageView.GetStorageItemsAsync();
                        foreach (var item in items)
                        {
                            if (item is Windows.Storage.StorageFile file)
                            {
                                string ext = file.FileType.ToLower();
                                if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".webp" || ext == ".gif")
                                {
                                    uploadImage?.Invoke(file.Path);
                                    return file.Path;
                                }
                            }
                        }
                    }

                    if (dataPackageView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Bitmap))
                    {
                        var imageStreamRef = await dataPackageView.GetBitmapAsync();
                        using var ras = await imageStreamRef.OpenReadAsync();
                        using var stream = ras.AsStreamForRead();
                        
                        using var src = Image.FromStream(stream, true, true);
                        using var bmp = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
                        using (var g = Graphics.FromImage(bmp))
                        {
                            g.CompositingMode = CompositingMode.SourceCopy;
                            g.DrawImage(src, 0, 0);
                        }
                        string filePath = GetTempFilePath("Clipboard-Image");
                        bmp.Save(filePath, ImageFormat.Png);

                        uploadImage?.Invoke(filePath);
                        return filePath;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Native Clipboard Error: {ex.Message}");
                }
#endif
                return null;
            });
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

        public static async Task<string> SaveImageFromUrlAsync(string url)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent", "VRCGalleryManager/1.0"); // VRChat API requires User-Agent
                
                if (url.Contains("vrchat.cloud"))
                {
                    var authConfig = VRCAuth.Instance().Config;
                    if (authConfig != null && authConfig.DefaultHeaders.TryGetValue("Cookie", out var cookieValue))
                    {
                        request.Headers.Add("Cookie", cookieValue);
                    }
                }

                var res = await HttpClient.SendAsync(request);
                string ct = res.Content.Headers.ContentType?.MediaType;
                
                if (!res.IsSuccessStatusCode)
                {
                    string errorBody = await res.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Download failed! Status: {res.StatusCode}, Body: {errorBody}");
                    return null;
                }
                
                if (ct == null || !ct.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine($"Download failed! Invalid content type: {ct}");
                    return null;
                }

                string cleanUrl = url.Split('?')[0];
                bool isGif = ct.Equals("image/gif", StringComparison.OrdinalIgnoreCase) ||
                             cleanUrl.EndsWith(".gif", StringComparison.OrdinalIgnoreCase);

                if (isGif)
                {
                    string filePath = Path.Combine(TempDirectory, $"Downloaded-Image_{Guid.NewGuid():N}.gif");
                    using (var stream = await res.Content.ReadAsStreamAsync())
                    using (var fileStream = File.Create(filePath))
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                    return filePath;
                }
                else
                {
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
                    return filePath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading image: {ex.Message}");
                return null;
            }
        }

        private static string GetTempFilePath(string prefix)
            => Path.Combine(TempDirectory, $"{prefix}_{Guid.NewGuid():N}.png");
    }
}
