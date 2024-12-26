using VRCEmojiManager.Core;
using VRCEmojiManager.Forms;

namespace VRCEmojiManager
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

            _forms = new Form[] { new Emoji(Auth), new Create(Auth), new Settings(Auth) };
            foreach (var form in _forms)
            {
                form.TopLevel = false;
                form.Dock = DockStyle.Fill;
                this.FormsPanel.Controls.Add(form);
                form.Hide();
            }

            ShowForm(2);
        }

        private void ShowForm(int index)
        {
            foreach (var form in _forms) form.Hide();

            _forms[index].Show();

            _switchEmoji.BorderSize = index == 0 ? 2 : 0;
            _switchCreate.BorderSize = index == 1 ? 2 : 0;
            _switchSettings.BorderSize = index == 2 ? 2 : 0;
        }

        private void _switchEmoji_Click(object sender, EventArgs e) => ShowForm(0);
        private void _switchCreate_Click(object sender, EventArgs e) => ShowForm(1);
        private void _switchSettings_Click(object sender, EventArgs e) => ShowForm(2);
    }
}
