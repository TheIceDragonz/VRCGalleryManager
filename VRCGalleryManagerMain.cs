using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.Helpers;
using VRCGalleryManager.Forms;

namespace VRCGalleryManager
{
    public partial class MainPanel : Form
    {
        private static Mutex mutex;

        private readonly ApiConnectedForm[] _forms = new ApiConnectedForm[8];
        private readonly Func<ApiConnectedForm>[] _formFactories;
        private ImageEditorForm _editorForm;
        private EmojiEditorForm _emojiEditorForm;
        private int _previousFormIndex = 7; // index of the form visible before the editor

        // Track current editor session handlers so we can safely detach them
        private Action<string, string> _currentHandleSave;
        private Action _currentHandleCancel;
        // Track pending cancel callback so ShowForm() can invoke it if editor is interrupted
        private Action _currentEditorCancel;

        // Track emoji editor session handlers
        private Action<string, bool, string, int, int> _currentEmojiHandleSave;
        private Action _currentEmojiHandleCancel;
        private Action _currentEmojiEditorCancel;

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

            _formFactories = new Func<ApiConnectedForm>[]
            {
                () => new Icons(Auth),
                () => new Photos(Auth),
                () => new Emoji(Auth),
                () => new Sticker(Auth),
                () => new Prints(Auth),

                () => new Picflow(Auth),
                () => new Gallery(Auth),
                () => new Settings(Auth, this)
            };

            ShowForm(7);

            ApplyRecolorBar();
        }

        private void ApplyRecolorBar()
        {
            Color c = Color.FromArgb(15, 17, 19);
            int color = ColorTranslator.ToWin32(c);
            DwmSetWindowAttribute(this.Handle, 35, ref color, sizeof(int));
        }

        public async Task SetCurrentName()
        {
            userNameLabel.Text = Settings.UserName;
            userNameLabel.ForeColor = Settings.MeColor;
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
            profileIcon.Visible = true;
            profileIcon.BringToFront();
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
            userNameLabel.Text = "";
        }

        private void ShowForm(int index)
        {
            // If the editor is currently open, treat navigation as a cancel
            if (_editorForm != null && _editorForm.Visible)
            {
                // Detach handlers first to prevent double-fire
                if (_currentHandleSave != null) _editorForm.OnSave -= _currentHandleSave;
                if (_currentHandleCancel != null) _editorForm.OnCancel -= _currentHandleCancel;
                _currentHandleSave = null;
                _currentHandleCancel = null;

                // Notify the originating form so it can re-enable its buttons
                var pendingCancel = _currentEditorCancel;
                _currentEditorCancel = null;
                pendingCancel?.Invoke();

                _editorForm.Hide();
            }
            else if (_emojiEditorForm != null && _emojiEditorForm.Visible)
            {
                // Detach handlers first to prevent double-fire
                if (_currentEmojiHandleSave != null) _emojiEditorForm.OnSave -= _currentEmojiHandleSave;
                if (_currentEmojiHandleCancel != null) _emojiEditorForm.OnCancel -= _currentEmojiHandleCancel;
                _currentEmojiHandleSave = null;
                _currentEmojiHandleCancel = null;

                // Notify the originating form
                var pendingCancel = _currentEmojiEditorCancel;
                _currentEmojiEditorCancel = null;
                pendingCancel?.Invoke();

                _emojiEditorForm.Hide();
            }
            else
            {
                foreach (var form in _forms) form?.Hide();
                _editorForm?.Hide();
                _emojiEditorForm?.Hide();
            }

            GetForm(index).Show();
            _previousFormIndex = index;

            _switchIcons.BorderColor   = index == 0 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
            _switchPhotos.BorderColor  = index == 1 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
            _switchEmoji.BorderColor   = index == 2 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
            _switchSticker.BorderColor = index == 3 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
            _switchPrints.BorderColor  = index == 4 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);

            _switchPicflow.BorderColor  = index == 5 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
            _switchGallery.BorderColor  = index == 6 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
            _switchSettings.BorderColor = index == 7 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
        }

        /// <summary>
        /// Mostra l'image editor inline nel FormsPanel.
        /// Chiamato dai form child (Photos, Sticker, Emoji, Prints) al posto di ShowDialog().
        /// </summary>
        /// <param name="imagePath">Path dell'immagine da editare.</param>
        /// <param name="ratio">Ratio di default: "1:1" o "16:9".</param>
        /// <param name="onSave">Callback invocato con il path del file risultante quando l'utente clicca Applica.</param>
        /// <param name="onCancel">Callback invocato quando l'utente clicca Annulla.</param>
        public void ShowEditor(string imagePath, string ratio, Action<string> onSave, Action onCancel)
        {
            ShowEditor(imagePath, ratio, (path, note) => onSave?.Invoke(path), onCancel, showNote: false);
        }

        /// <summary>
        /// Mostra l'image editor inline nel FormsPanel con supporto per note (es. per Prints).
        /// </summary>
        public void ShowEditor(string imagePath, string ratio, Action<string, string> onSave, Action onCancel, bool showNote)
        {
            if (_editorForm == null)
            {
                _editorForm = new ImageEditorForm();
                _editorForm.TopLevel = false;
                _editorForm.FormBorderStyle = FormBorderStyle.None;
                _editorForm.Dock = DockStyle.Fill;
                this.FormsPanel.Controls.Add(_editorForm);
                _editorForm.Hide();
            }

            // Safely detach any lingering handlers from a previous session
            if (_currentHandleSave != null)   _editorForm.OnSave   -= _currentHandleSave;
            if (_currentHandleCancel != null) _editorForm.OnCancel -= _currentHandleCancel;

            void HandleSave(string resultPath, string note)
            {
                _editorForm.OnSave   -= _currentHandleSave;
                _editorForm.OnCancel -= _currentHandleCancel;
                _currentHandleSave   = null;
                _currentHandleCancel = null;
                _currentEditorCancel = null;
                HideEditor();
                onSave?.Invoke(resultPath, note);
            }

            void HandleCancel()
            {
                _editorForm.OnSave   -= _currentHandleSave;
                _editorForm.OnCancel -= _currentHandleCancel;
                _currentHandleSave   = null;
                _currentHandleCancel = null;
                _currentEditorCancel = null;
                HideEditor();
                onCancel?.Invoke();
            }

            _currentHandleSave   = HandleSave;
            _currentHandleCancel = HandleCancel;
            _currentEditorCancel = onCancel;

            _editorForm.OnSave   += HandleSave;
            _editorForm.OnCancel += HandleCancel;

            // Load the image into the editor
            _editorForm.LoadImage(imagePath, ratio, showNote);

            // Switch to editor view
            foreach (var form in _forms) form?.Hide();
            _emojiEditorForm?.Hide();
            _editorForm.Show();
            _editorForm.BringToFront();
        }

        private void HideEditor()
        {
            _editorForm?.Hide();
            // Restore the previously visible form
            if (_previousFormIndex >= 0 && _previousFormIndex < _forms.Length)
                GetForm(_previousFormIndex).Show();
        }

        /// <summary>
        /// Mostra l'emoji editor inline nel FormsPanel.
        /// </summary>
        public void ShowEmojiEditor(string imagePath, Action<string, bool, string, int, int> onSave, Action onCancel)
        {
            if (_emojiEditorForm == null)
            {
                _emojiEditorForm = new EmojiEditorForm();
                _emojiEditorForm.TopLevel = false;
                _emojiEditorForm.FormBorderStyle = FormBorderStyle.None;
                _emojiEditorForm.Dock = DockStyle.Fill;
                this.FormsPanel.Controls.Add(_emojiEditorForm);
                _emojiEditorForm.Hide();
            }

            if (_currentEmojiHandleSave != null)   _emojiEditorForm.OnSave   -= _currentEmojiHandleSave;
            if (_currentEmojiHandleCancel != null) _emojiEditorForm.OnCancel -= _currentEmojiHandleCancel;

            void HandleSave(string resultPath, bool isAnimated, string style, int frames, int fps)
            {
                _emojiEditorForm.OnSave   -= _currentEmojiHandleSave;
                _emojiEditorForm.OnCancel -= _currentEmojiHandleCancel;
                _currentEmojiHandleSave   = null;
                _currentEmojiHandleCancel = null;
                _currentEmojiEditorCancel = null;
                HideEmojiEditor();
                onSave?.Invoke(resultPath, isAnimated, style, frames, fps);
            }

            void HandleCancel()
            {
                _emojiEditorForm.OnSave   -= _currentEmojiHandleSave;
                _emojiEditorForm.OnCancel -= _currentEmojiHandleCancel;
                _currentEmojiHandleSave   = null;
                _currentEmojiHandleCancel = null;
                _currentEmojiEditorCancel = null;
                HideEmojiEditor();
                onCancel?.Invoke();
            }

            _currentEmojiHandleSave   = HandleSave;
            _currentEmojiHandleCancel = HandleCancel;
            _currentEmojiEditorCancel = onCancel;

            _emojiEditorForm.OnSave   += HandleSave;
            _emojiEditorForm.OnCancel += HandleCancel;

            // Load the image/gif into the emoji editor
            _emojiEditorForm.LoadImage(imagePath);

            // Switch to emoji editor view
            foreach (var form in _forms) form?.Hide();
            _editorForm?.Hide();
            _emojiEditorForm.Show();
            _emojiEditorForm.BringToFront();
        }

        private void HideEmojiEditor()
        {
            _emojiEditorForm.Hide();
            if (_previousFormIndex >= 0 && _previousFormIndex < _forms.Length)
                GetForm(_previousFormIndex).Show();
        }

        private ApiConnectedForm GetForm(int index)
        {
            if (_forms[index] == null)
            {
                var form = _formFactories[index]();
                form.TopLevel = false;
                form.Dock = DockStyle.Fill;
                this.FormsPanel.Controls.Add(form);
                form.Hide();
                _forms[index] = form;
            }
            return _forms[index];
        }

        private void _switchIcons_Click(object sender, EventArgs e) => ShowForm(0);
        private void _switchPhotos_Click(object sender, EventArgs e) => ShowForm(1);
        private void _switchEmoji_Click(object sender, EventArgs e) => ShowForm(2);
        private void _switchSticker_Click(object sender, EventArgs e) => ShowForm(3);
        private void _switchPrints_Click(object sender, EventArgs e) => ShowForm(4);

        private void _switchPicflow_Click(object sender, EventArgs e) => ShowForm(5);
        private void _switchGallery_Click(object sender, EventArgs e) => ShowForm(6);
        private void _switchSettings_Click(object sender, EventArgs e) => ShowForm(7);

        public void SetFeatureControlsEnabled(bool enabled)
        {
            _switchIcons.Enabled = enabled;
            _switchPhotos.Enabled = enabled;
            _switchEmoji.Enabled = enabled;
            _switchSticker.Enabled = enabled;
            _switchPrints.Enabled = enabled;
            _switchPicflow.Enabled = enabled;
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

        private void userNameLabel_Click(object sender, EventArgs e)
        {   
            Process.Start(new ProcessStartInfo
            {
                FileName = $"https://vrchat.com/home/user/{Settings.UserId}",
                UseShellExecute = true
            });
        }
    }
}
