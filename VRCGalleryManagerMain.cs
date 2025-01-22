using Newtonsoft.Json.Linq;
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

            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Scale(new SizeF(Screen.PrimaryScreen.Bounds.Width / 1920f, Screen.PrimaryScreen.Bounds.Height / 1080f));

            Auth = VRCAuth.Instance();
            Auth.LoadCookies();

            _forms = new Form[] { new Icons(Auth), new Gallery(Auth), new Emoji(Auth), new Sticker(Auth), new Prints(Auth), new Create(Auth), new Settings(Auth) };
            foreach (var form in _forms)
            {
                form.TopLevel = false;
                form.Dock = DockStyle.Fill;
                this.FormsPanel.Controls.Add(form);
                form.Hide();
            }

            ShowForm(6);

            if (Auth.LoggedIn || Auth.CookieLoaded)
            {
                ProfileImage();
            }

            //Recolor Bar
            int color = Color.FromArgb(5, 5, 5).ToArgb() & 0xFFFFFF;
            DwmSetWindowAttribute(this.Handle, 35, ref color, 4);
        }

        public async Task ProfileImage()
        {
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
            profileIcon.Visible = true;
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
            profileIcon.Image = null;
            profileBanner.Image = null;
            var badgeBoxes = new[] { badgeBox3, badgeBox2, badgeBox1 };
            foreach (var box in badgeBoxes)
            {
                box.Visible = false;
                box.Image = null;
            }
            profileIcon.Visible = false;
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

            _switchCreate.BorderColor = index == 5 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
            _switchSettings.BorderColor = index == 6 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
        }

        private void _switchIcons_Click(object sender, EventArgs e) => ShowForm(0);
        private void _switchPhotos_Click(object sender, EventArgs e) => ShowForm(1);
        private void _switchEmoji_Click(object sender, EventArgs e) => ShowForm(2);
        private void _switchSticker_Click(object sender, EventArgs e) => ShowForm(3);
        private void _switchPrints_Click(object sender, EventArgs e) => ShowForm(4);

        private void _switchCreate_Click(object sender, EventArgs e) => ShowForm(5);
        private void _switchSettings_Click(object sender, EventArgs e) => ShowForm(6);

        //Recolor Bar
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
    }
}
