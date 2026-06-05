using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace VRCGalleryManager.Core.Helpers
{
    public enum AdaptationMode
    {
        Fit,
        Fill,
        Stretch,
        Center
    }

    public static class ImageEditor
    {
        /// <summary>
        /// Calculates the drawing bounds of the image on the canvas based on the adaptation mode.
        /// </summary>
        public static void CalculateBaseBounds(
            AdaptationMode mode,
            float imageW,
            float imageH,
            float canvasW,
            float canvasH,
            out float baseScale,
            out float baseOffsetX,
            out float baseOffsetY)
        {
            switch (mode)
            {
                case AdaptationMode.Stretch:
                    // Scale X and Y independently. Handled separately in rendering,
                    // but for base calculations, we assume scale = 1 and fit bounds to canvas.
                    baseScale = 1f;
                    baseOffsetX = 0f;
                    baseOffsetY = 0f;
                    break;

                case AdaptationMode.Fit:
                    baseScale = Math.Min(canvasW / imageW, canvasH / imageH);
                    baseOffsetX = (canvasW - (imageW * baseScale)) / 2f;
                    baseOffsetY = (canvasH - (imageH * baseScale)) / 2f;
                    break;

                case AdaptationMode.Fill:
                    baseScale = Math.Max(canvasW / imageW, canvasH / imageH);
                    baseOffsetX = (canvasW - (imageW * baseScale)) / 2f;
                    baseOffsetY = (canvasH - (imageH * baseScale)) / 2f;
                    break;

                case AdaptationMode.Center:
                default:
                    baseScale = 1f;
                    baseOffsetX = (canvasW - imageW) / 2f;
                    baseOffsetY = (canvasH - imageH) / 2f;
                    break;
            }
        }

        /// <summary>
        /// Renders the image on a canvas Bitmap using the provided parameters.
        /// </summary>
        public static Bitmap RenderImage(
            Image originalImage,
            int canvasW,
            int canvasH,
            AdaptationMode mode,
            float zoomFactor,
            float panOffsetX,
            float panOffsetY,
            int rotationAngle,
            Color backgroundColor)
        {
            // Create target bitmap
            Bitmap result = new Bitmap(canvasW, canvasH, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(result))
            {
                // Set high quality options
                g.Clear(backgroundColor);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                float imageW = originalImage.Width;
                float imageH = originalImage.Height;
                if (rotationAngle == 90 || rotationAngle == 270)
                {
                    imageW = originalImage.Height;
                    imageH = originalImage.Width;
                }

                float baseScale = 1f;
                if (mode == AdaptationMode.Fit)
                {
                    baseScale = Math.Min(canvasW / imageW, canvasH / imageH);
                }
                else if (mode == AdaptationMode.Fill)
                {
                    baseScale = Math.Max(canvasW / imageW, canvasH / imageH);
                }
                else if (mode == AdaptationMode.Center)
                {
                    baseScale = 1f;
                }

                // Translate to canvas center + offsets (this makes zoom and pan centered!)
                g.TranslateTransform(canvasW / 2f + panOffsetX, canvasH / 2f + panOffsetY);

                if (mode == AdaptationMode.Stretch)
                {
                    float scaleX = (canvasW / imageW) * zoomFactor;
                    float scaleY = (canvasH / imageH) * zoomFactor;
                    g.ScaleTransform(scaleX, scaleY);
                }
                else
                {
                    float finalScale = baseScale * zoomFactor;
                    g.ScaleTransform(finalScale, finalScale);
                }

                g.RotateTransform(rotationAngle);

                // Draw centered
                float drawW = originalImage.Width;
                float drawH = originalImage.Height;
                g.TranslateTransform(-drawW / 2f, -drawH / 2f);

                g.DrawImage(originalImage, 0, 0, drawW, drawH);
            }

            return result;
        }

        /// <summary>
        /// Saves the edited image to a temporary file.
        /// </summary>
        public static string SaveTempProcessedImage(Bitmap bmp)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(tempDir);
            string outPath = Path.Combine(tempDir, $"edited_{Guid.NewGuid():N}.png");
            bmp.Save(outPath, ImageFormat.Png);
            return outPath;
        }
    }
}
