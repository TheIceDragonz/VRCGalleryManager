using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace VRCGalleryManager.Core.Helpers
{
    public static class ImageHelper
    {
        public static string[] allowedExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tif", ".tiff", ".ico" };

        public static void SetOpenFileDialogFilter(OpenFileDialog openFileDialog)
        {
            string filterExtensions = string.Join(";", allowedExtensions.Select(ext => "*" + ext));
            openFileDialog.Filter = $"Image Files|{filterExtensions}";
        }

        public static bool IsImageFile(string filePath)
        {
            string ext = Path.GetExtension(filePath)?.ToLower();
            return allowedExtensions.Contains(ext);
        }

        public static DragDropEffects ProcessDragEnter(DragEventArgs e)
        {
            var files = e.Data.GetDataPresent(DataFormats.FileDrop)
                        ? (string[])e.Data.GetData(DataFormats.FileDrop)
                        : null;
            return (files?.Length > 0 && IsImageFile(files[0])) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        public static void ProcessDragDrop(DragEventArgs e, Action<string> uploadImage)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            foreach (var file in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                if (!IsImageFile(file)) continue;
                try
                {
                    string directoryPath = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
                    Directory.CreateDirectory(directoryPath);

                    var dest = Path.Combine(directoryPath, $"dragdrop_image_{Guid.NewGuid()}{Path.GetExtension(file)}");
                    File.Copy(file, dest);
                    uploadImage(dest);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Errore: " + ex.Message);
                }
            }
        }
    }
}
