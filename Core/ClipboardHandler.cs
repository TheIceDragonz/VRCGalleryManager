using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCGalleryManager.Core
{
    public class ClipboardHandler
    {
        public static async void ClipboardDataImageOrLink(Button pasteButton, Action<string> UploadImage)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data != null)
            {
                if (data.GetDataPresent(DataFormats.Bitmap))
                {
                    string savedPath = HandleBitmap(data);
                    if (!string.IsNullOrEmpty(savedPath))
                    {
                        UploadImage(savedPath);
                    }
                }
                else if (data.GetDataPresent(DataFormats.FileDrop))
                {
                    HandleFileDrop(data, pasteButton, UploadImage);
                }
                else if (data.GetDataPresent(DataFormats.Text))
                {
                    string clipboardText = Clipboard.GetText();
                    if (await IsValidImageLink(clipboardText))
                    {
                        string savedPath = await SaveImageFromUrlAsync(clipboardText);
                        if (!string.IsNullOrEmpty(savedPath))
                        {
                            UploadImage(savedPath);
                        }
                    }
                    else
                    {
                        NotificationManager.ShowNotification("The link does not contain a valid image!", "Error", NotificationType.Error);
                    }
                }
                else
                {
                    NotificationManager.ShowNotification("No image, file, or link found in the clipboard!", "Error", NotificationType.Error);
                }
            }
            else
            {
                NotificationManager.ShowNotification("Clipboard is empty!", "Error", NotificationType.Error);
            }
        }

        private static string HandleBitmap(IDataObject data)
        {
            try
            {
                var image = (Image)data.GetData(DataFormats.Bitmap);

                string directoryPath = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
                Directory.CreateDirectory(directoryPath);
                string tempPath = Path.Combine(directoryPath, $"Pasted-Image_{Guid.NewGuid()}.png");

                image.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);
                NotificationManager.ShowNotification("Image pasted and saved successfully!", "Paste Image", NotificationType.Success);
                return tempPath;
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification($"Error saving image: {ex.Message}", "Error", NotificationType.Error);
                return null;
            }
        }

        private static void HandleFileDrop(IDataObject data, Button pasteButton, Action<string> UploadImage)
        {
            string[] files = (string[])data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                pasteButton.Enabled = false;
                UploadImage(files[0]); // Passa direttamente il file
                pasteButton.Enabled = true;
            }
        }

        private static async Task<bool> IsValidImageLink(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                    if (response.IsSuccessStatusCode && response.Content.Headers.ContentType != null)
                    {
                        string mimeType = response.Content.Headers.ContentType.MediaType;
                        return mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            catch
            {
                // Ignorare errori come timeout o URL non valido
            }
            return false;
        }

        private static async Task<string> SaveImageFromUrlAsync(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode && response.Content.Headers.ContentType != null)
                    {
                        string mimeType = response.Content.Headers.ContentType.MediaType;
                        if (mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                        {
                            string directoryPath = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
                            Directory.CreateDirectory(directoryPath);
                            string tempPath = Path.Combine(directoryPath, $"Downloaded-Image_{Guid.NewGuid()}.png");

                            using (var stream = await response.Content.ReadAsStreamAsync())
                            {
                                using (var image = Image.FromStream(stream))
                                {
                                    image.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }

                            NotificationManager.ShowNotification("Image downloaded and saved successfully!", "Download Image", NotificationType.Success);
                            return tempPath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification($"Error downloading image: {ex.Message}", "Error", NotificationType.Error);
            }
            return null;
        }
    }
}
