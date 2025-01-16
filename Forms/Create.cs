using System.Drawing.Imaging;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;

namespace VRCGalleryManager.Forms
{
    public partial class Create : Form
    {
        private static string MASK_TAG = "square";
        private static string ANIMATION_STYLE = "";

        private ApiRequest apiRequest;

        private string gifPath;
        private Bitmap spriteSheet;

        private int imageframes = 0;

        public Create(VRCAuth auth)
        {
            apiRequest = new ApiRequest(auth);

            InitializeComponent();

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(pictureSS_DragEnter);
            this.DragDrop += new DragEventHandler(pictureSS_DragDrop);

            LoadEmojiType();
        }

        private void pictureSS_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            gifPath = files[0];  // Salva il percorso della GIF
            previewGif.ImageLocation = gifPath;

            if (gifPath != null)
            {
                string tempOutputPath = Path.Combine(Path.GetTempPath(), gifPath + "spritesheet_temp.png");
                ConvertGifToSpriteSheets(gifPath, tempOutputPath);
                previewSS.Image = spriteSheet;
                buttonSave.Enabled = true;
            }
        }

        private void pictureSS_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (Path.GetExtension(files[0]).ToLower() == ".gif")
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        private void ConvertGifToSpriteSheets(string gifPath, string tempOutputPath)
        {
            Image gifImage = Image.FromFile(gifPath);
            FrameDimension dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
            int frameCount = gifImage.GetFrameCount(dimension);

            if (frameCount < 64) imageframes = frameCount;
            else imageframes = 64;

            int maxTextureSize = 1024;
            int squareSize;
            int cols, rows;

            if (frameCount <= 16)
            {
                squareSize = 256; // Dimensione per 4x4 griglia
                cols = rows = 4;
            }
            else
            {
                squareSize = 128; // Dimensione per 8x8 griglia
                cols = rows = 8;
            }

            int spriteSheetWidth = cols * squareSize;
            int spriteSheetHeight = rows * squareSize;

            if (spriteSheetWidth > maxTextureSize || spriteSheetHeight > maxTextureSize)
            {
                int maxCols = maxTextureSize / squareSize;
                int maxRows = maxTextureSize / squareSize;

                cols = Math.Min(frameCount, maxCols * maxRows);
                rows = (int)Math.Ceiling((double)frameCount / cols);

                spriteSheetWidth = Math.Min(cols * squareSize, maxTextureSize);
                spriteSheetHeight = Math.Min(rows * squareSize, maxTextureSize);

                if (spriteSheetWidth > maxTextureSize || spriteSheetHeight > maxTextureSize)
                {
                    NotificationManager.ShowNotification("It is not possible to fit all the frames within the maximum allowed size.", "Errore", NotificationType.Error);
                    return;
                }
            }

            spriteSheet = new Bitmap(spriteSheetWidth, spriteSheetHeight);

            using (Graphics g = Graphics.FromImage(spriteSheet))
            {
                g.Clear(Color.Transparent);

                for (int i = 0; i < frameCount; i++)
                {
                    gifImage.SelectActiveFrame(dimension, i);

                    // Ritaglia il frame in un quadrato
                    Bitmap squareFrame = CropToSquare(gifImage, squareSize);

                    int col = i % cols;
                    int row = i / cols;

                    g.DrawImage(squareFrame, col * squareSize, row * squareSize, squareSize, squareSize);
                }
            }
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

        private void buttonLocalSave_Click(object sender, EventArgs e)
        {
            if (spriteSheet != null)
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    string FileName = Path.GetFileNameWithoutExtension(gifPath);

                    saveDialog.Title = "Salva la sprite sheet";
                    saveDialog.Filter = "PNG Image|*.png";
                    saveDialog.FileName = FileName + ".png";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        string outputFilePath = saveDialog.FileName;

                        if (Path.GetExtension(outputFilePath).ToLower() != ".png")
                        {
                            outputFilePath = Path.ChangeExtension(outputFilePath, ".png");
                        }

                        spriteSheet.Save(outputFilePath, ImageFormat.Png);
                    }
                }
            }
            else
            {
                DialogMessage.ShowMissingGif(this);
            }
        }
        private string SaveTemp()
        {
            if (spriteSheet != null)
            {
                string outputFilePath = "VRCGalleryManager-spritesheet_temp.png";

                if (Path.GetExtension(outputFilePath).ToLower() != ".png")
                {
                    outputFilePath = Path.ChangeExtension(outputFilePath, ".png");
                }

                spriteSheet.Save(Path.GetTempPath() + outputFilePath, ImageFormat.Png);

                return Path.GetTempPath() + outputFilePath;
            }
            else
            {
                DialogMessage.ShowMissingGif(this);
            }
            return null;
        }

        private async Task<string> DownloadImageFromUrl(string url)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), "VRCGalleryManager-downloaded_image.gif" + url.Length);

            using (HttpClient client = new HttpClient())
            {
                byte[] imageBytes = await client.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(tempFilePath, imageBytes);
            }

            return tempFilePath;
        }

        private async void urlToSpriteSheet(object sender, EventArgs e)
        {
            string url = urlToSpriteSheetText.Text;

            if (!string.IsNullOrWhiteSpace(url))
            {
                if (urlToSpriteSheetText.Text.Contains("gif"))
                {
                    string downloadedFilePath = await DownloadImageFromUrl(url);
                    gifPath = downloadedFilePath;
                    previewGif.ImageLocation = gifPath;

                    string tempOutputPath = Path.Combine(Path.GetTempPath(), "VRCGalleryManager-spritesheet_temp.png" + url.Length);
                    ConvertGifToSpriteSheets(gifPath, tempOutputPath);
                    previewSS.Image = spriteSheet;
                }
                else
                {
                    DialogMessage.ShowValidURL(this);
                }
            }
            urlToSpriteSheetText.Text = "";
        }

        private async void creatorUpload_Click(object sender, EventArgs e)
        {
            if (spriteSheet != null)
            {
                if (!createOpenTypePanel.Text.Contains("Type"))
                {
                    try
                    {
                        string image = SaveTemp();

                        ANIMATION_STYLE = createOpenTypePanel.Text.ToLower();

                        ApiRequest.ApiData emoji = await apiRequest.UploadImage(image, MASK_TAG, TagType.EmojiAnimated, ANIMATION_STYLE, imageframes, trackBarFPS.Value);

                        NotificationManager.ShowNotification("File uploaded successfully", "File upload", NotificationType.Success);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error during file upload", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    DialogMessage.ShowMissingTypeDialog(this);
                }
            }
            else
            {
                DialogMessage.ShowMissingSpriteSheet(this);
            }
        }

        private void trackBarFPS_ValueChanged(object sender, EventArgs e)
        {
            labelFPS.Text = "FPS :" + trackBarFPS.Value.ToString();
        }

        private void createOpenTypePanel_Click(object sender, EventArgs e)
        {
            createTypePanel.Visible = !createTypePanel.Visible;
        }
    }
}
