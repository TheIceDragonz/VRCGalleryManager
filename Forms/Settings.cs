using VRCGalleryManager.Core;
using System.Text.Json;
using VRChat.API.Model;

namespace VRCGalleryManager.Forms
{
    public partial class Settings : Form
    {
        private VRCAuth Auth;
        public static int MaxDataPages = 10;
        private string[] _allFiles = new string[0];

        public Settings(VRCAuth Auth)
        {
            InitializeComponent();

            this.Auth = Auth;

            checkToken();
        }

        private void LoginButton(object sender, EventArgs e)
        {
            if (Auth.LoggedIn || Auth.CookieLoaded)
            {
                Auth.AuthApi.Logout();
                System.IO.File.Delete(VRCAuth.tokenFilePath);

                _username.Enabled = true;
                _password.Enabled = true;

                _username.Text = "";
                _password.Text = "";

                _loginButton.Text = "Login";
                _loginButton.BackColor = Color.DimGray;

                Auth.LoggedIn = false;
                Auth.CookieLoaded = false;
            }

            if (!(Auth.LoggedIn || Auth.CookieLoaded))
            {
                Auth.VRCAuthentication(_username.Text, _password.Text);
                checkToken();
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
                    _loginButton.BackColor = Color.FromArgb(128, 64, 64);

                    CurrentUser currentUsers = Auth.AuthApi.GetCurrentUser();

                    _username.Text = currentUsers.DisplayName;
                    _password.Text = "password";
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
