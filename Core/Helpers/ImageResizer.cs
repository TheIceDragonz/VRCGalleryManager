using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using Color = SixLabors.ImageSharp.Color;
using Point = SixLabors.ImageSharp.Point;
using Image = SixLabors.ImageSharp.Image;
using System.IO;
using System;

namespace VRCGalleryManager.Core.Helpers
{
    public class ImageResizer
    {
        public static string ResizeImage1x1(string imagePath)
        {
            const int maxSize = 2048;
            using var original = Image.Load<Rgba32>(imagePath);
            int origW = original.Width;
            int origH = original.Height;
            int maxDim = Math.Max(origW, origH);
            float scale = maxDim > maxSize
                ? (float)maxSize / maxDim
                : 1f;
            int resizedW = (int)(origW * scale);
            int resizedH = (int)(origH * scale);
            int side = Math.Max(resizedW, resizedH);
            
            using var bmp = new Image<Rgba32>(side, side, Color.Transparent);
            original.Mutate(x => x.Resize(resizedW, resizedH, KnownResamplers.Bicubic));
            
            int offsetX = (side - resizedW) / 2;
            int offsetY = (side - resizedH) / 2;
            
            bmp.Mutate(x => x.DrawImage(original, new Point(offsetX, offsetY), 1f));

            var tempDir = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(tempDir);
            string outPath = Path.Combine(tempDir, $"resized_{Guid.NewGuid():N}.png");
            bmp.SaveAsPng(outPath);
            return outPath;
        }

        public static string ResizeImage16x9(string imagePath)
        {
            using var input = Image.Load<Rgba32>(imagePath);
            var tempDir = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(tempDir);
            
            if (input.Width <= 2024 && input.Height <= 2024)
            {
                var outPath = Path.Combine(tempDir, $"converted_{Guid.NewGuid()}.png");
                using var bmp = new Image<Rgba32>(input.Width, input.Height, Color.Transparent);
                bmp.Mutate(x => x.DrawImage(input, new Point(0, 0), 1f));
                bmp.SaveAsPng(outPath);
                return outPath;
            }
            
            int canvasW = 2048, canvasH = (int)(canvasW / 16.0 * 9);
            float scale = Math.Min((float)canvasW / input.Width, (float)canvasH / input.Height);
            int newW = (int)(input.Width * scale), newH = (int)(input.Height * scale);
            int offsetX = (canvasW - newW) / 2, offsetY = (canvasH - newH) / 2;
            
            var outPathResized = Path.Combine(tempDir, $"resized_{Guid.NewGuid()}.png");
            using var bmpResized = new Image<Rgba32>(canvasW, canvasH, Color.Transparent);
            input.Mutate(x => x.Resize(newW, newH, KnownResamplers.Bicubic));
            bmpResized.Mutate(x => x.DrawImage(input, new Point(offsetX, offsetY), 1f));
            bmpResized.SaveAsPng(outPathResized);
            return outPathResized;
        }
    }
}
