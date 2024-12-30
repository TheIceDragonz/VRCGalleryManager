using VRCGalleryManager.Core;
using VRCGalleryManager.Forms;

namespace VRCGalleryManager
{
    public partial class MainPanel : Form
    {

        private readonly Form[] _forms;

        private VRCAuth Auth;

        public MainPanel()
        {
            InitializeComponent();

            Auth = VRCAuth.Instance();

            Auth.LoadCookies();

            _forms = new Form[] { new Emoji(Auth), new Sticker(Auth), new Create(Auth), new Settings(Auth) };
            foreach (var form in _forms)
            {
                form.TopLevel = false;
                form.Dock = DockStyle.Fill;
                this.FormsPanel.Controls.Add(form);
                form.Hide();
            }

            ShowForm(3);
        }

        private void ShowForm(int index)
        {
            foreach (var form in _forms) form.Hide();

            _forms[index].Show();

            _switchEmoji.BorderColor = index == 0 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66);
            _switchSticker.BorderColor = index == 1 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66); ;
            _switchCreate.BorderColor = index == 2 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66); ;
            _switchSettings.BorderColor = index == 3 ? Color.FromArgb(255, 255, 255) : Color.FromArgb(5, 55, 66); ;
        }

        private void _switchEmoji_Click(object sender, EventArgs e) => ShowForm(0);
        private void _switchSticker_Click(object sender, EventArgs e) => ShowForm(1);
        private void _switchCreate_Click(object sender, EventArgs e) => ShowForm(2);
        private void _switchSettings_Click(object sender, EventArgs e) => ShowForm(3);

        
    }
}
