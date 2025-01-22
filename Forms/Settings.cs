using VRCGalleryManager.Core;
using VRChat.API.Model;

namespace VRCGalleryManager.Forms
{
    public partial class Settings : Form
    {
        private VRCAuth Auth;
        public static int MaxDataPages = 10;
        private string[] _allFiles = new string[0];

        public static string UserId = "";
        public static string UserIconImage = "";
        public static string UserBannerImage = "";
        public static List<string> Badges = new List<string>();

        public Settings(VRCAuth Auth)
        {
            InitializeComponent();

            this.Auth = Auth;

            checkToken();
        }

        private void LoginButton(object sender, EventArgs e)
        {
            if (!(Auth.LoggedIn || Auth.CookieLoaded))
            {
                Auth.VRCAuthentication(_username.Text, _password.Text);
                checkToken();

                return;
            }

            if (Auth.LoggedIn || Auth.CookieLoaded)
            {
                Auth.AuthApi.Logout();
                System.IO.File.Delete(VRCAuth.tokenFilePath);

                _username.Enabled = true;
                _password.Enabled = true;

                _username.Text = "";
                _password.Text = "";

                _loginButton.Text = "Login";
                _loginButton.TextColor = Color.FromArgb(106, 227, 249);
                _loginButton.BackColor = Color.FromArgb(7, 36, 43);

                Auth.LoggedIn = false;
                Auth.CookieLoaded = false;

                (Application.OpenForms["MainPanel"] as MainPanel)?.ProfileImageRemover();
            }
        }

        private void checkToken()
        {
            if (Auth.LoggedIn || Auth.CookieLoaded)
            {
                try
                {
                    _username.Enabled = false;
                    _password.Enabled = false;

                    _loginButton.Text = "Logout";
                    _loginButton.TextColor = Color.FromArgb(255, 128, 128);

                    CurrentUser currentUser = Auth.AuthApi.GetCurrentUser();

                    _username.Text = currentUser.DisplayName;
                    _password.Text = "password";

                    UserId = currentUser.Id;
                    UserIconImage = currentUser.UserIcon;
                    UserBannerImage = currentUser.ProfilePicOverrideThumbnail;
                    if (string.IsNullOrEmpty(UserBannerImage)) UserBannerImage = currentUser.CurrentAvatarThumbnailImageUrl;
                    foreach (var badge in currentUser.Badges) Badges.Add(badge.ToJson());

                    (Application.OpenForms["MainPanel"] as MainPanel)?.ProfileImage();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());

                    _vrcLogin.BackColor = Color.IndianRed;
                    _vrcLoginLabel.Text = ex.ToString();
                }
            }
        }
    }
}
