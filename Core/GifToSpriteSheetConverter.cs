using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace VRCGalleryManager.Core
{
    public class GifToSpriteSheetConverter
    {
        private Image gifImage;
        public Bitmap SpriteSheet { get; private set; }
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
            var matches = System.Text.RegularExpressions.Regex.Matches(htmlContent, pattern);

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    yield return match.Value;
                }
            }
        }

        public (Bitmap spriteSheet, int frameCount) ConvertGifToSpriteSheet(string gifPath)
        {
            int textureSize = 1024;
            gifImage?.Dispose();
            gifImage = Image.FromFile(gifPath);
            FrameDimension dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
            frameCount = Math.Min(gifImage.GetFrameCount(dimension), 64);

            int squareSize, cols, rows;

            if (frameCount == 1)
            {
                throw new ArgumentException("The GIF must contain more than one frame.");
            }

            if (frameCount <= 16)
            {
                squareSize = textureSize / 4;
                cols = rows = 4;
            }
            else
            {
                squareSize = textureSize / 8;
                cols = rows = 8;
            }

            SpriteSheet = new Bitmap(textureSize, 1024);

            using (Graphics g = Graphics.FromImage(SpriteSheet))
            {
                g.Clear(Color.Transparent);

                for (int i = 0; i < frameCount; i++)
                {
                    gifImage.SelectActiveFrame(dimension, i);
                    Bitmap squareFrame = CropToSquare(gifImage, squareSize);

                    int col = i % cols;
                    int row = i / cols;

                    g.DrawImage(squareFrame, col * squareSize, row * squareSize, squareSize, squareSize);
                }
            }

            return (SpriteSheet, frameCount);
        }

        public string SaveSpriteSheet(string outputPath)
        {
            if (SpriteSheet == null)
                throw new InvalidOperationException("No sprite sheet to save.");

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            SpriteSheet.Save(outputPath, ImageFormat.Png);

            return outputPath;
        }

        public string SaveTempSpriteSheet()
        {
            if (SpriteSheet == null)
                throw new InvalidOperationException("No sprite sheet to save.");

            string tempPath = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(tempPath);

            string outputPath = Path.Combine(tempPath, $"spritesheet_{Guid.NewGuid()}.png");
            SpriteSheet.Save(outputPath, ImageFormat.Png);

            return outputPath;
        }

        public async Task<(string gifPath, Bitmap spriteSheet, int frameCount)> ProcessGifFromClipboard()
        {
            IDataObject data = Clipboard.GetDataObject();

            if (data == null)
                throw new Exception("Clipboard is empty!");

            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])data.GetData(DataFormats.FileDrop);
                string gifFile = files.FirstOrDefault(f => Path.GetExtension(f).ToLower() == ".gif");

                if (gifFile == null)
                    throw new Exception("No valid GIF file found in the clipboard!");

                var (spriteSheet, frameCount) = ConvertGifToSpriteSheet(gifFile);
                return (gifFile, spriteSheet, frameCount);
            }

            if (data.GetDataPresent(DataFormats.Bitmap))
            {
                Image clipboardImage = (Image)data.GetData(DataFormats.Bitmap);
                string tempPath = Path.Combine(Path.GetTempPath(), $"clipboard_image_{Guid.NewGuid()}.gif");
                clipboardImage.Save(tempPath, ImageFormat.Gif);

                var (spriteSheet, frameCount) = ConvertGifToSpriteSheet(tempPath);
                return (tempPath, spriteSheet, frameCount);
            }

            if (data.GetDataPresent(DataFormats.Text))
            {
                string url = (string)data.GetData(DataFormats.Text);

                if (string.IsNullOrWhiteSpace(url))
                    throw new Exception("Invalid URL in the clipboard!");

                string gifPath = await DownloadImageFromUrl(url);

                var (spriteSheet, frameCount) = ConvertGifToSpriteSheet(gifPath);
                return (gifPath, spriteSheet, frameCount);
            }

            throw new Exception("No valid GIF data found in the clipboard!");
        }

        private Bitmap CropToSquare(Image img, int size)
        {
            int maxSize = Math.Max(img.Width, img.Height);
            Bitmap squareImage = new Bitmap(size, size);

            using (Graphics g = Graphics.FromImage(squareImage))
            {
                g.Clear(Color.Transparent);
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

                g.DrawImage(img, new Rectangle(0, 0, size, size), srcRect, GraphicsUnit.Pixel);
            }

            return squareImage;
        }
    }
}
