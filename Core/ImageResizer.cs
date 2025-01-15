using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace VRCGalleryManager.Core
{
    public class ImageResizer
    {
        public static string ResizeImage1024x1024(string imagePath)
        {
            using (Image inputImage = Image.FromFile(imagePath))
            {
                int targetSize = 1024;
                int newWidth, newHeight;

                float aspectRatio = (float)inputImage.Width / inputImage.Height;
                if (inputImage.Width > inputImage.Height)
                {
                    newWidth = targetSize;
                    newHeight = (int)(targetSize / aspectRatio);
                }
                else
                {
                    newHeight = targetSize;
                    newWidth = (int)(targetSize * aspectRatio);
                }

                using (Bitmap resizedBitmap = new Bitmap(targetSize, targetSize))
                {
                    resizedBitmap.MakeTransparent();

                    using (Graphics graphics = Graphics.FromImage(resizedBitmap))
                    {
                        graphics.Clear(Color.Transparent);
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                        int offsetX = (targetSize - newWidth) / 2;
                        int offsetY = (targetSize - newHeight) / 2;

                        graphics.DrawImage(inputImage, offsetX, offsetY, newWidth, newHeight);
                    }

                    string tempPath = Path.Combine(Path.GetTempPath(), $"resized_{Guid.NewGuid()}.png");
                    resizedBitmap.Save(tempPath, ImageFormat.Png);

                    return tempPath;
                }
            }
        }
    }
}
