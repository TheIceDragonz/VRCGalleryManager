using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace VRCGalleryManager.Core.Helpers
{
    public class ImageResizer
    {
        public static string ResizeImage1x1(string imagePath)
        {
            const int maxSize = 2048;
            using var original = Image.FromFile(imagePath);
            int origW = original.Width;
            int origH = original.Height;
            int maxDim = Math.Max(origW, origH);
            float scale = maxDim > maxSize
                ? (float)maxSize / maxDim
                : 1f;
            int resizedW = (int)(origW * scale);
            int resizedH = (int)(origH * scale);
            int side = Math.Max(resizedW, resizedH);
            using var bmp = new Bitmap(side, side, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.CompositingMode = CompositingMode.SourceCopy;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                int offsetX = (side - resizedW) / 2;
                int offsetY = (side - resizedH) / 2;
                g.DrawImage(original, offsetX, offsetY, resizedW, resizedH);
            }
            var tempDir = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(tempDir);
            string outPath = Path.Combine(tempDir, $"resized_{Guid.NewGuid():N}.png");
            bmp.Save(outPath, ImageFormat.Png);
            return outPath;
        }

        public static string ResizeImage16x9(string imagePath)
        {
            using var input = Image.FromFile(imagePath);
            var tempDir = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(tempDir);
            if (input.Width <= 2024 && input.Height <= 2024)
            {
                var outPath = Path.Combine(tempDir, $"converted_{Guid.NewGuid()}.png");
                using var bmp = new Bitmap(input.Width, input.Height, PixelFormat.Format32bppArgb);
                using var g = Graphics.FromImage(bmp);
                g.Clear(Color.Transparent);
                g.DrawImage(input, new Rectangle(0, 0, input.Width, input.Height));
                bmp.Save(outPath, ImageFormat.Png);
                return outPath;
            }
            int canvasW = 2048, canvasH = (int)(canvasW / 16.0 * 9);
            float scale = Math.Min((float)canvasW / input.Width, (float)canvasH / input.Height);
            int newW = (int)(input.Width * scale), newH = (int)(input.Height * scale);
            int offsetX = (canvasW - newW) / 2, offsetY = (canvasH - newH) / 2;
            var outPathResized = Path.Combine(tempDir, $"resized_{Guid.NewGuid()}.png");
            using var bmpResized = new Bitmap(canvasW, canvasH, PixelFormat.Format32bppArgb);
            using var g2 = Graphics.FromImage(bmpResized);
            g2.Clear(Color.Transparent);
            g2.DrawImage(input, new Rectangle(offsetX, offsetY, newW, newH));
            bmpResized.Save(outPathResized, ImageFormat.Png);
            return outPathResized;
        }
    }
}
