using Timer = System.Windows.Forms.Timer;

namespace VRCGalleryManager.Core
{
    public class SpriteSheetViewer
    {
        private Bitmap _spriteSheet;
        private int _frameWidth;
        private int _frameHeight;
        private int _frameCount;
        private int _currentFrame;
        private readonly Timer _frameTimer;
        private readonly PictureBox _pictureBox;

        public SpriteSheetViewer(PictureBox pictureBox)
        {
            _pictureBox = pictureBox ?? throw new ArgumentNullException(nameof(pictureBox));
            _frameTimer = new Timer();
            _frameTimer.Tick += FrameTimer_Tick;
        }

        public async Task LoadSpriteSheetAsync(string spriteSheetUrl, int frameCount, int framesPerSecond)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(spriteSheetUrl);
                response.EnsureSuccessStatusCode();
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var bitmap = new Bitmap(stream);
                    InitializeSpriteSheet(bitmap, frameCount, framesPerSecond);
                }
            }
        }

        public async Task LoadSpriteSheetAsync(Bitmap spriteSheet, int frameCount, int framesPerSecond)
        {
            if (spriteSheet == null)
                throw new ArgumentNullException(nameof(spriteSheet));
            InitializeSpriteSheet(spriteSheet, frameCount, framesPerSecond);
            await Task.CompletedTask;
        }

        private void InitializeSpriteSheet(Bitmap spriteSheet, int frameCount, int framesPerSecond)
        {
            _spriteSheet = spriteSheet;
            if (_spriteSheet.Width != 1024 || _spriteSheet.Height != 1024)
                throw new ArgumentException("La risoluzione dello sprite sheet deve essere 1024x1024.");
            _frameCount = frameCount;
            _currentFrame = 0;
            int squareSize = frameCount <= 16 ? 256 : 128;
            _frameWidth = squareSize;
            _frameHeight = squareSize;
            _frameTimer.Interval = 1000 / framesPerSecond;
            _pictureBox.Image = new Bitmap(_frameWidth, _frameHeight);
        }

        public void StartAnimation()
        {
            if (_spriteSheet == null)
                throw new InvalidOperationException("Sprite sheet non caricato.");
            _frameTimer.Start();
        }

        public void StopAnimation()
        {
            _frameTimer.Stop();
        }

        public void UpdateFPS(int framesPerSecond)
        {
            _frameTimer.Interval = 1000 / framesPerSecond;
        }

        private void FrameTimer_Tick(object sender, EventArgs e)
        {
            if (_spriteSheet == null || _frameCount <= 0)
                return;
            int cols = _spriteSheet.Width / _frameWidth;
            int x = (_currentFrame % cols) * _frameWidth;
            int y = (_currentFrame / cols) * _frameHeight;
            Rectangle srcRect = new Rectangle(x, y, _frameWidth, _frameHeight);
            using (Graphics g = Graphics.FromImage(_pictureBox.Image))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(_spriteSheet, new Rectangle(0, 0, _frameWidth, _frameHeight), srcRect, GraphicsUnit.Pixel);
            }
            _pictureBox.Refresh();
            _currentFrame = (_currentFrame + 1) % _frameCount;
        }
    }
}
