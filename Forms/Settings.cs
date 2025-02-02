using System.Diagnostics;
using VRCGalleryManager.Core;
using VRChat.API.Model;
using NotificationType = VRCGalleryManager.Core.NotificationType;

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

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible)
            {
                UpdateCacheInfo();
            }
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

        private void _openCacheFolder_Click(object sender, EventArgs e)
        {
            string cacheFolderPath = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            if (Directory.Exists(cacheFolderPath))
            {
                Process.Start("explorer.exe", cacheFolderPath);
            }
            else
            {
                NotificationManager.ShowNotification("Cache folder does not exist.", "Error", NotificationType.Error);
            }
        }

        private void _openVrchatLogs_Click(object sender, EventArgs e)
        {
            string vrchatLogPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"),
                "VRChat",
                "VRChat");

            if (Directory.Exists(vrchatLogPath))
            {
                Process.Start("explorer.exe", vrchatLogPath);
            }
            else
            {
                NotificationManager.ShowNotification("VRChat logs folder does not exist.", "Error", NotificationType.Error);
            }
        }

        private void _clearAllCacheFiles_Click(object sender, EventArgs e)
        {
            string cacheFolderPath = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");

            if (!Directory.Exists(cacheFolderPath))
            {
                NotificationManager.ShowNotification("Cache folder does not exist.", "Error", NotificationType.Error);
                return;
            }

            try
            {
                foreach (string file in Directory.GetFiles(cacheFolderPath))
                {
                    System.IO.File.Delete(file);
                }
                NotificationManager.ShowNotification("All cache files have been successfully deleted!", "Success", NotificationType.Success);
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification($"Error clearing cache files: {ex.Message}", "Error", NotificationType.Success);
            }

            UpdateCacheInfo();
        }

        private void UpdateCacheInfo()
        {
            string cacheFolder = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            if (!Directory.Exists(cacheFolder))
            {
                infoCacheLabel.Text = "Cache folder not found.";
                return;
            }

            string[] files = Directory.GetFiles(cacheFolder);
            long totalSize = files.Sum(file => new FileInfo(file).Length);
            string formattedSize;
            FormatSize(totalSize, out formattedSize);

            infoCacheLabel.Text = "~ Info Cache ~\n" + 
                                "\n" +
                                $"Files: {files.Length}\n" +
                                $"Total Size: {formattedSize}\n" +
                                $"Cache Path: {cacheFolder}";
        }

        private void FormatSize(long bytes, out string formatted)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double size = bytes;
            int unitIndex = 0;

            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            formatted = $"{size:0.##} {units[unitIndex]}";
        }
    }
}
