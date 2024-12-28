using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace VRCGalleryManager.Core
{
    public class SpriteSheetViewer
    {
        private System.Windows.Forms.Timer animationTimer;
        private Bitmap spriteSheet;
        private int frameWidth;
        private int frameHeight;
        private int currentFrame;
        private int totalFrames;
        private int cols;
        private Action<Bitmap> renderFrame;

        public SpriteSheetViewer(string spriteSheetPath, int frameWidth, int frameHeight, int frameDelay, Action<Bitmap> renderFrame)
        {
            if (string.IsNullOrEmpty(spriteSheetPath) || !File.Exists(spriteSheetPath) || Path.GetExtension(spriteSheetPath).ToLower() != ".png")
            {
                throw new ArgumentException("Percorso non valido o file non supportato.");
            }

            spriteSheet = new Bitmap(spriteSheetPath);
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            cols = spriteSheet.Width / frameWidth;
            totalFrames = (spriteSheet.Width / frameWidth) * (spriteSheet.Height / frameHeight);
            currentFrame = 0;
            this.renderFrame = renderFrame;

            animationTimer = new System.Windows.Forms.Timer
            {
                Interval = frameDelay
            };
            animationTimer.Tick += OnAnimationTick;
            animationTimer.Start();
        }

        private void OnAnimationTick(object sender, EventArgs e)
        {
            int row = currentFrame / cols;
            int col = currentFrame % cols;

            Rectangle frameRect = new Rectangle(col * frameWidth, row * frameHeight, frameWidth, frameHeight);
            Bitmap frame = new Bitmap(frameWidth, frameHeight);

            using (Graphics g = Graphics.FromImage(frame))
            {
                g.DrawImage(spriteSheet, new Rectangle(0, 0, frameWidth, frameHeight), frameRect, GraphicsUnit.Pixel);
            }

            renderFrame?.Invoke(frame);

            currentFrame = (currentFrame + 1) % totalFrames;
        }
    }
}
