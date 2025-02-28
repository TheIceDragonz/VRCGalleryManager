using Newtonsoft.Json.Linq;
using System;
using System.Runtime.InteropServices;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.Helpers;
using VRCGalleryManager.Forms;

namespace VRCGalleryManager
{
    public partial class MainPanel : Form
    {
        private static Mutex mutex;

        private readonly Form[] _forms;

        private VRCAuth Auth;
        public MainPanel()
        {
            // Check if another instance is already running
            const string appName = "VRCGalleryManager";
            if (InstanceChecker.IsAlreadyRunning(appName))
            {
                Environment.Exit(0);
                return;
            }

            InitializeComponent();

            bannerIcon.Height = 150;

            Auth = VRCAuth.Instance();
            Auth.LoadCookies();

            _forms = new Form[]
            {
                new Icons(Auth),
                new Gallery(Auth),
                new Emoji(Auth),
                new Sticker(Auth),
                new Prints(Auth),

                new Picflow(Auth),
                new Create(Auth),
                new Settings(Auth, this)
            };
            foreach (var form in _forms)
            {
                form.TopLevel = false;
                form.Dock = DockStyle.Fill;
                this.FormsPanel.Controls.Add(form);
                form.Hide();
            }

            ShowForm(7);

            if (Auth.LoggedIn || Auth.CookieLoaded)
            {
                _ = ProfileImage();
            }

            ApplyRecolorBar();
        }
        private void ApplyRecolorBar()
        {
            Color c = Color.FromArgb(15, 17, 19);
            int color = ColorTranslator.ToWin32(c);
            DwmSetWindowAttribute(this.Handle, 35, ref color, sizeof(int));
        }

        public async Task ProfileImage()
        {
            BigBannerIcon(false);

            profileIcon.LoadAsync(await HttpImage.GetFinalUrlAsync(Settings.UserIconImage));
            profileBanner.LoadAsync(await HttpImage.GetFinalUrlAsync(Settings.UserBannerImage));
            var badgeBoxes = new[] { badgeBox3, badgeBox2, badgeBox1 };
            foreach (var (badge, box) in Settings.Badges.Zip(badgeBoxes, (badge, box) => (badge, box)))
            {
                JObject jsonObject = JObject.Parse(badge);
                string imageBadge = jsonObject["badgeImageUrl"]?.ToString();

                box.Visible = !string.IsNullOrEmpty(imageBadge);
                if (box.Visible)
                    box.LoadAsync(imageBadge);
            }
            if (!string.IsNullOrEmpty(Settings.UserIconImage)) profileIcon.Visible = true;
            profileBanner.Visible = true;
        }
        public async Task ProfileUpdateIcon(string IconImage)
        {
            profileIcon.LoadAsync(await HttpImage.GetFinalUrlAsync(IconImage));
        }
        public async Task ProfileUpdateBanner(string BannerImage)
        {
            profileBanner.LoadAsync(await HttpImage.GetFinalUrlAsync(BannerImage));
        }
        public void ProfileImageRemover()
        {
            BigBannerIcon(true);

            profileIcon.Image = null;
            profileBanner.Image = null;
            var badgeBoxes = new[] { badgeBox3, badgeBox2, badgeBox1 };
            foreach (var box in badgeBoxes)
            {
                box.Visible = false;
                box.Image = null;
            }
            profileIcon.Visible = false;
            profileBanner.Visible = false;
        }

        private void ShowForm(int index)
        {
            foreach (var form in _forms) form.Hide();

            _forms[index].Show();

            _switchIcons.BorderColor = index == 0 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
            _switchPhotos.BorderColor = index == 1 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
            _switchEmoji.BorderColor = index == 2 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
            _switchSticker.BorderColor = index == 3 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
            _switchPrints.BorderColor = index == 4 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);

            _switchPicflow.BorderColor = index == 5 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
            _switchCreate.BorderColor = index == 6 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
            _switchSettings.BorderColor = index == 7 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
        }

        private void _switchIcons_Click(object sender, EventArgs e) => ShowForm(0);
        private void _switchPhotos_Click(object sender, EventArgs e) => ShowForm(1);
        private void _switchEmoji_Click(object sender, EventArgs e) => ShowForm(2);
        private void _switchSticker_Click(object sender, EventArgs e) => ShowForm(3);
        private void _switchPrints_Click(object sender, EventArgs e) => ShowForm(4);

        private void _switchPicflow_Click(object sender, EventArgs e) => ShowForm(5);
        private void _switchCreate_Click(object sender, EventArgs e) => ShowForm(6);
        private void _switchSettings_Click(object sender, EventArgs e) => ShowForm(7);

        public void SetFeatureControlsEnabled(bool enabled)
        {
            _switchIcons.Enabled = enabled;
            _switchPhotos.Enabled = enabled;
            _switchEmoji.Enabled = enabled;
            _switchSticker.Enabled = enabled;
            _switchPrints.Enabled = enabled;
            _switchPicflow.Enabled = enabled;
            _switchCreate.Enabled = enabled;
        }

        //Recolor Bar
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        //Banner Icon Animation
        public void BigBannerIcon(bool active)
        {
            int targetHeight = active ? 150 : 45;

            int totalDuration = 300;
            int interval = 10;
            int steps = totalDuration / interval;

            int startHeight = bannerIcon.Height;
            double animHeight = startHeight;
            double increment = (targetHeight - startHeight) / (double)steps;

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = interval;
            timer.Tick += (sender, e) =>
            {
                animHeight += increment;

                if ((increment > 0 && animHeight >= targetHeight) ||
                    (increment < 0 && animHeight <= targetHeight))
                {
                    animHeight = targetHeight;
                    timer.Stop();
                    timer.Dispose();
                }

                bannerIcon.Height = (int)Math.Round(animHeight);
            };
            timer.Start();
        }

        
    }
}
