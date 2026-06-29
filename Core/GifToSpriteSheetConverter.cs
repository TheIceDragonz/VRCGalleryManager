using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using Color = SixLabors.ImageSharp.Color;
using Point = SixLabors.ImageSharp.Point;
using Image = SixLabors.ImageSharp.Image;
using System.Collections.Generic;

namespace VRCGalleryManager.Core
{
    public class GifToSpriteSheetConverter : IDisposable
    {
        private Image<Rgba32> gifImage;
        public Image<Rgba32> SpriteSheet { get; private set; }
        public int frameCount { get; private set; }

        public async Task<string> DownloadImageFromUrl(string url)
        {
            string directoryPath = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(directoryPath);

            string tempFilePath = Path.Combine(directoryPath, $"downloaded_image_{Guid.NewGuid()}.gif");

            using (HttpClient client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true }))
            {
                if (url.ToLower().EndsWith(".gif"))
                {
                    byte[] imageBytes = await client.GetByteArrayAsync(url);
                    await File.WriteAllBytesAsync(tempFilePath, imageBytes);
                    return tempFilePath;
                }

                string htmlContent = await client.GetStringAsync(url);

                var gifUrls = ExtractGifUrlsFromEmbed(htmlContent);

                foreach (var gifUrl in gifUrls)
                {
                    try
                    {
                        byte[] imageBytes = await client.GetByteArrayAsync(gifUrl);
                        await File.WriteAllBytesAsync(tempFilePath, imageBytes);
                        return tempFilePath;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            throw new Exception("No valid GIF URLs found in the page!");
        }

        private IEnumerable<string> ExtractGifUrlsFromEmbed(string htmlContent)
        {
            string pattern = @"https?:\/\/[^\s""'<>]+\.gif";
            var matches = Regex.Matches(htmlContent, pattern);

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    yield return match.Value;
                }
            }
        }

        private string loadedGifPath = null;

        public void Dispose()
        {
            gifImage?.Dispose();
            gifImage = null;
            loadedGifPath = null;
            SpriteSheet?.Dispose();
            SpriteSheet = null;
        }

        public (Image<Rgba32> spriteSheet, int frameCount) ConvertGifToSpriteSheet(string gifPath)
        {
            if (loadedGifPath != gifPath || gifImage == null)
            {
                gifImage?.Dispose();
                gifImage = Image.Load<Rgba32>(gifPath);
                loadedGifPath = gifPath;
            }
            int count = gifImage.Frames.Count;
            int maxFrames = Math.Min(count, 64);
            return ConvertGifToSpriteSheet(gifPath, 0, maxFrames - 1);
        }

        public (Image<Rgba32> spriteSheet, int frameCount) ConvertGifToSpriteSheet(string gifPath, int startFrame, int endFrame)
        {
            int textureSize = 1024;
            if (loadedGifPath != gifPath || gifImage == null)
            {
                gifImage?.Dispose();
                gifImage = Image.Load<Rgba32>(gifPath);
                loadedGifPath = gifPath;
            }
            int count = gifImage.Frames.Count;

            int framesToUse = endFrame - startFrame + 1;
            if (framesToUse <= 0 || framesToUse > 64)
            {
                throw new ArgumentException("Selected frame range is invalid or exceeds 64 frames.");
            }

            int squareSize, cols, rows;

            if (framesToUse <= 16)
            {
                squareSize = textureSize / 4;
                cols = rows = 4;
            }
            else
            {
                squareSize = textureSize / 8;
                cols = rows = 8;
            }

            SpriteSheet?.Dispose();
            SpriteSheet = new Image<Rgba32>(textureSize, 1024, Color.Transparent);

            for (int i = 0; i < framesToUse; i++)
            {
                var frame = gifImage.Frames.CloneFrame(startFrame + i);
                using (var squareFrame = CropToSquare(frame, squareSize))
                {
                    int col = i % cols;
                    int row = i / cols;
                    
                    SpriteSheet.Mutate(x => x.DrawImage(squareFrame, new Point(col * squareSize, row * squareSize), 1f));
                }
            }

            frameCount = framesToUse;
            return (SpriteSheet, framesToUse);
        }

        public string SaveSpriteSheet(string outputPath)
        {
            if (SpriteSheet == null)
                throw new InvalidOperationException("No sprite sheet to save.");

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            SpriteSheet.SaveAsPng(outputPath);

            return outputPath;
        }

        public string SaveTempSpriteSheet()
        {
            if (SpriteSheet == null)
                throw new InvalidOperationException("No sprite sheet to save.");

            string tempPath = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(tempPath);

            string outputPath = Path.Combine(tempPath, $"spritesheet_{Guid.NewGuid()}.png");
            SpriteSheet.SaveAsPng(outputPath);

            return outputPath;
        }

        private Image<Rgba32> CropToSquare(Image<Rgba32> img, int size)
        {
            int maxSize = Math.Max(img.Width, img.Height);
            var squareImage = new Image<Rgba32>(size, size, Color.Transparent);
            
            Rectangle srcRect;

            if (img.Width > img.Height)
            {
                int offset = (img.Width - img.Height) / 2;
                srcRect = new Rectangle(offset, 0, img.Height, img.Height);
            }
            else
            {
                int offset = (img.Height - img.Width) / 2;
                srcRect = new Rectangle(0, offset, img.Width, img.Width);
            }

            // Crop
            var cropped = img.Clone(x => x.Crop(srcRect));
            // Resize to target size
            cropped.Mutate(x => x.Resize(size, size, KnownResamplers.Bicubic));
            
            squareImage.Mutate(x => x.DrawImage(cropped, new Point(0, 0), 1f));
            cropped.Dispose();
            
            return squareImage;
        }
    }
}
