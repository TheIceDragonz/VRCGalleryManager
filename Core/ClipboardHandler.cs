using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCGalleryManager.Core
{
    public static class ClipboardHandler
    {
        private static readonly string TempDirectory;

        static ClipboardHandler()
        {
            TempDirectory = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(TempDirectory);
        }

        /// <summary>
        /// Gestisce i dati presenti negli appunti, cercando di riconoscere un'immagine, un file o un link a immagine.
        /// </summary>
        /// <param name="pasteButton">Il bottone che attiva l'operazione, da disabilitare temporaneamente in alcuni casi.</param>
        /// <param name="uploadImage">L'azione da eseguire una volta ottenuto il percorso dell'immagine.</param>
        public static async Task ClipboardDataImageOrLink(Button pasteButton, Action<string> uploadImage)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data == null)
            {
                NotificationManager.ShowNotification("Clipboard is empty!", "Error", NotificationType.Error);
                return;
            }

            if (data.GetDataPresent(DataFormats.Bitmap))
            {
                string savedPath = HandleBitmap(data);
                if (!string.IsNullOrEmpty(savedPath))
                {
                    uploadImage(savedPath);
                }
            }
            else if (data.GetDataPresent(DataFormats.FileDrop))
            {
                HandleFileDrop(data, pasteButton, uploadImage);
            }
            else if (data.GetDataPresent(DataFormats.Text))
            {
                string clipboardText = Clipboard.GetText();
                if (await IsValidImageLinkAsync(clipboardText))
                {
                    string savedPath = await SaveImageFromUrlAsync(clipboardText);
                    if (!string.IsNullOrEmpty(savedPath))
                    {
                        uploadImage(savedPath);
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

        private static string HandleBitmap(IDataObject data)
        {
            try
            {
                if (data.GetData(DataFormats.Bitmap) is Image image)
                {
                    string filePath = GetTempFilePath("Pasted-Image");
                    image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    NotificationManager.ShowNotification("Image pasted and saved successfully!", "Paste Image", NotificationType.Success);
                    return filePath;
                }
                else
                {
                    NotificationManager.ShowNotification("No valid image found in clipboard data!", "Error", NotificationType.Error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification($"Error saving image: {ex.Message}", "Error", NotificationType.Error);
                return null;
            }
        }

        private static void HandleFileDrop(IDataObject data, Button pasteButton, Action<string> uploadImage)
        {
            if (data.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                try
                {
                    pasteButton.Enabled = false;
                    uploadImage(files[0]);
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
                using (HttpClient client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Head, url);
                    HttpResponseMessage response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode && response.Content.Headers.ContentType != null)
                    {
                        string mimeType = response.Content.Headers.ContentType.MediaType;
                        return mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            catch
            {
                // Se si verifica un'eccezione, consideriamo il link non valido.
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
                            string filePath = GetTempFilePath("Downloaded-Image");
                            using (Stream stream = await response.Content.ReadAsStreamAsync())
                            using (Image image = Image.FromStream(stream))
                            {
                                image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                            }
                            NotificationManager.ShowNotification("Image downloaded and saved successfully!", "Download Image", NotificationType.Success);
                            return filePath;
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

        private static string GetTempFilePath(string prefix)
        {
            return Path.Combine(TempDirectory, $"{prefix}_{Guid.NewGuid()}.png");
        }
    }
}
