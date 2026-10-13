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
        private static readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true });
        private Image<Rgba32> gifImage;
        public Image<Rgba32> SpriteSheet { get; private set; }
        public int frameCount { get; private set; }

        public async Task<string> DownloadImageFromUrl(string url)
        {
            string directoryPath = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(directoryPath);

            string tempFilePath = Path.Combine(directoryPath, $"downloaded_image_{Guid.NewGuid()}.gif");

            if (url.ToLower().EndsWith(".gif"))
            {
                byte[] imageBytes = await _httpClient.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(tempFilePath, imageBytes);
                return tempFilePath;
            }

            string htmlContent = await _httpClient.GetStringAsync(url);

            var gifUrls = ExtractGifUrlsFromEmbed(htmlContent);

            foreach (var gifUrl in gifUrls)
            {
                try
                {
                    byte[] imageBytes = await _httpClient.GetByteArrayAsync(gifUrl);
                    await File.WriteAllBytesAsync(tempFilePath, imageBytes);
                    return tempFilePath;
                }
                catch
                {
                    continue;
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
            return ConvertGifToFinalSpriteSheet(gifPath, 0, maxFrames - 1, true, 0.5f);
        }

        public (Image<Rgba32> spriteSheet, int frameCount) ConvertGifToPreviewSpriteSheet(string gifPath, int startFrame, int endFrame)
        {
            if (loadedGifPath != gifPath || gifImage == null)
            {
                gifImage?.Dispose();
                gifImage = Image.Load<Rgba32>(gifPath);
                loadedGifPath = gifPath;
            }

            int framesToUse = endFrame - startFrame + 1;
            if (framesToUse <= 0 || framesToUse > 64)
            {
                throw new ArgumentException("Selected frame range is invalid or exceeds 64 frames.");
            }

            int cols = framesToUse <= 4 ? 2 : (framesToUse <= 16 ? 4 : 8);
            int rows = cols;
            int maxCellDim = 1024 / cols;

            int origW = gifImage.Width;
            int origH = gifImage.Height;
            int cellW, cellH;

            if (origW >= origH)
            {
                cellW = maxCellDim;
                cellH = Math.Max(1, (int)Math.Round((float)maxCellDim * origH / origW));
            }
            else
            {
                cellH = maxCellDim;
                cellW = Math.Max(1, (int)Math.Round((float)maxCellDim * origW / origH));
            }

            SpriteSheet?.Dispose();
            SpriteSheet = new Image<Rgba32>(cols * cellW, rows * cellH);

            for (int i = 0; i < framesToUse; i++)
            {
                using var frame = gifImage.Frames.CloneFrame(startFrame + i);
                if (frame.Width != cellW || frame.Height != cellH)
                {
                    frame.Mutate(x => x.Resize(cellW, cellH, KnownResamplers.Lanczos3));
                }

                int col = i % cols;
                int row = i / cols;
                SpriteSheet.Mutate(x => x.DrawImage(frame, new Point(col * cellW, row * cellH), 1f));
            }

            frameCount = framesToUse;
            return (SpriteSheet, framesToUse);
        }

        public (Image<Rgba32> spriteSheet, int frameCount) ConvertGifToFinalSpriteSheet(string gifPath, int startFrame, int endFrame, bool isFit, float panOffset = 0.5f)
        {
            int textureSize = 1024;
            if (loadedGifPath != gifPath || gifImage == null)
            {
                gifImage?.Dispose();
                gifImage = Image.Load<Rgba32>(gifPath);
                loadedGifPath = gifPath;
            }

            int framesToUse = endFrame - startFrame + 1;
            if (framesToUse <= 0 || framesToUse > 64)
            {
                throw new ArgumentException("Selected frame range is invalid or exceeds 64 frames.");
            }

            int squareSize, cols, rows;
            if (framesToUse <= 4)
            {
                squareSize = textureSize / 2;
                cols = rows = 2;
            }
            else if (framesToUse <= 16)
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
            SpriteSheet = new Image<Rgba32>(textureSize, textureSize);

            float p = Math.Clamp(panOffset, 0f, 1f);

            for (int i = 0; i < framesToUse; i++)
            {
                using var frame = gifImage.Frames.CloneFrame(startFrame + i);
                int col = i % cols;
                int row = i / cols;
                int originX = col * squareSize;
                int originY = row * squareSize;

                if (isFit)
                {
                    float scale = Math.Min((float)squareSize / frame.Width, (float)squareSize / frame.Height);
                    int targetW = Math.Max(1, (int)Math.Round(frame.Width * scale));
                    int targetH = Math.Max(1, (int)Math.Round(frame.Height * scale));

                    using var resized = frame.Clone(x => x.Resize(targetW, targetH, KnownResamplers.Lanczos3));
                    int destX = (squareSize - targetW) / 2;
                    int destY = (squareSize - targetH) / 2;
                    SpriteSheet.Mutate(x => x.DrawImage(resized, new Point(originX + destX, originY + destY), 1f));
                }
                else
                {
                    int minDim = Math.Min(frame.Width, frame.Height);
                    int cropX = 0;
                    int cropY = 0;

                    if (frame.Width > frame.Height)
                    {
                        int extra = frame.Width - frame.Height;
                        cropX = Math.Clamp((int)Math.Round(extra * p), 0, extra);
                    }
                    else if (frame.Height > frame.Width)
                    {
                        int extra = frame.Height - frame.Width;
                        cropY = Math.Clamp((int)Math.Round(extra * p), 0, extra);
                    }

                    using var cropped = frame.Clone(x =>
                    {
                        x.Crop(new Rectangle(cropX, cropY, minDim, minDim));
                        x.Resize(squareSize, squareSize, KnownResamplers.Lanczos3);
                    });
                    SpriteSheet.Mutate(x => x.DrawImage(cropped, new Point(originX, originY), 1f));
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
    }
}
