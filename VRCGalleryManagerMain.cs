using VRCGalleryManager.Core;
using VRCGalleryManager.Design;
using VRCGalleryManager.Forms;
using VRCGalleryManager.Helpers;

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

            if (Auth.LoggedIn || Auth.CookieLoaded)
            {
                ProfileImage();
            }

            ShowForm(6);
        }

        private async void ProfileImage()
        {
            Settings images = new Settings(Auth);
            profileIcon.LoadAsync(await HttpImage.GetFinalUrlAsync(images.UserIconImage));
            profileBanner.LoadAsync(await HttpImage.GetFinalUrlAsync(images.UserBannerImage));
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
    }
}
