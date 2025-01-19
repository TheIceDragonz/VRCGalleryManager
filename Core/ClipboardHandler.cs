using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VRCGalleryManager.Core;

namespace VRCGalleryManager.Core
{
    public class ClipboardHandler
    {
        public static void ClipboardDataImage(Button pasteButton, Action<string> UploadImage)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data != null)
            {
                if (data.GetDataPresent(DataFormats.Bitmap))
                {
                    var image = (Image)data.GetData(DataFormats.Bitmap);

                    string directoryPath = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
                    Directory.CreateDirectory(directoryPath);
                    string tempPath = Path.Combine(directoryPath, $"Pasted-Image_{Guid.NewGuid()}.png");
                    image.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);
                    UploadImage(tempPath);
                    NotificationManager.ShowNotification("Image pasted and saved successfully!", "Paste Image", NotificationType.Success);
                }
                else if (data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])data.GetData(DataFormats.FileDrop);
                    if (files.Length > 0)
                    {
                        pasteButton.Enabled = false;

                        UploadImage(files[0]);

                        pasteButton.Enabled = true;
                    }
                }
                else
                {
                    NotificationManager.ShowNotification("No image or file found in the clipboard!", "Error", NotificationType.Error);
                }
            }
            else
            {
                NotificationManager.ShowNotification("Clipboard is empty!", "Error", NotificationType.Error);
            }
        }
    }
}
