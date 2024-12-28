using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCGalleryManager.Core
{
    public static class SpriteSheetViewer
    {
        public static async Task SpriteSheet(string spriteSheetPath, string fps, string? delay, PictureBox pictureBox)
        {
            if (string.IsNullOrEmpty(spriteSheetPath) || !File.Exists(spriteSheetPath) || Path.GetExtension(spriteSheetPath).ToLower() != ".png")
            {
                throw new ArgumentException("Percorso non valido o file non supportato.");
            }

            int fpsInt = int.Parse(fps);
            int? delayInt = int.Parse(delay);

            Bitmap spriteSheet = new Bitmap(spriteSheetPath);

            // Calcolo delle colonne e righe basato sugli FPS
            int cols = fpsInt <= 16 ? 4 : 8; // 4x4 per <=16 FPS, 8x8 per >16 FPS
            int rows = spriteSheet.Height / (spriteSheet.Width / cols);

            int frameWidth = spriteSheet.Width / cols;
            int frameHeight = spriteSheet.Height / rows;

            int totalFrames = cols * rows;
            int frameDelay = delayInt ?? (1000 / fpsInt);

            await Task.Run(async () =>
            {
                int currentFrame = 0;
                while (true)
                {
                    int row = currentFrame / cols;
                    int col = currentFrame % cols;

                    Rectangle frameRect = new Rectangle(col * frameWidth, row * frameHeight, frameWidth, frameHeight);
                    Bitmap frame = new Bitmap(frameWidth, frameHeight);

                    using (Graphics g = Graphics.FromImage(frame))
                    {
                        g.DrawImage(spriteSheet, new Rectangle(0, 0, frameWidth, frameHeight), frameRect, GraphicsUnit.Pixel);
                    }

                    // Aggiorna la PictureBox
                    pictureBox.Invoke(new Action(() =>
                    {
                        pictureBox.Image?.Dispose(); // Pulisce l'immagine precedente
                        pictureBox.Image = frame;
                    }));

                    currentFrame = (currentFrame + 1) % totalFrames;

                    await Task.Delay(frameDelay);
                }
            });
        }
    }
}
