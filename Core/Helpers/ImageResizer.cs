using System.Drawing.Imaging;

namespace VRCGalleryManager.Core.Helpers
{
    public class ImageResizer
    {
        public static string ResizeImage1024x1024(string imagePath)
        {
            using var input = Image.FromFile(imagePath);
            int target = 1024;
            if (input.Width == input.Height)
            {
                return imagePath;
            }
            int newSize = Math.Max(input.Width, input.Height);
            var tempDir = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(tempDir);
            var outPath = Path.Combine(tempDir, $"resized_{Guid.NewGuid()}.png");
            using var bmp = new Bitmap(newSize, newSize, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                int offsetX = (newSize - input.Width) / 2;
                int offsetY = (newSize - input.Height) / 2;
                g.DrawImage(input, offsetX, offsetY, input.Width, input.Height);
            }
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
