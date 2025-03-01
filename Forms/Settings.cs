using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using VRCGalleryManager.Core;
using VRChat.API.Model;
using NotificationType = VRCGalleryManager.Core.NotificationType;
using File = System.IO.File;

namespace VRCGalleryManager.Forms
{
    public partial class Settings : Form
    {
        private VRCAuth Auth;
        private MainPanel _mainPanel;

        public static string UserId = "";
        public static string UserIconImage = "";
        public static string UserBannerImage = "";
        public static List<string> Badges = new List<string>();

        public bool VRCPlus = false;

        public Settings(VRCAuth Auth, MainPanel mainPanel)
        {
            InitializeComponent();

            this.Auth = Auth;
            _mainPanel = mainPanel;

            CheckToken();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible)
            {
                UpdateCacheInfo();
            }
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            if (!Auth.LoggedIn && !Auth.CookieLoaded)
            {
                try
                {
                    ToggleLoginFields(true);
                    Auth.VRCAuthentication(_username.Text, _password.Text);
                    CheckToken();
                }
                catch (Exception ex)
                {
                    NotificationManager.ShowNotification("Login error: " + ex.Message, "Login Failed", NotificationType.Error);
                    ToggleLoginFields(true);
                }
            }
            else
            {
                Logout();
            }
        }

        private void Logout()
        {
            try
            {
                Auth.AuthApi.Logout();
                if (File.Exists(VRCAuth.tokenFilePath))
                {
                    File.Delete(VRCAuth.tokenFilePath);
                }
                _mainPanel.SetFeatureControlsEnabled(false);
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification("Logout error: " + ex.Message, "Logout Failed", NotificationType.Error);
            }
            finally
            {
                Auth.LoggedIn = false;
                Auth.CookieLoaded = false;
                _username.Text = "";
                _password.Text = "";
                _vrcLoginLabel.Text = "VRChat Login";
                _vrcLoginLabel.ForeColor = Color.Azure;
                viewPassword.Enabled = true;
                ToggleLoginFields(true);
                UpdateLoginButtonUI(false);
                _mainPanel.ProfileImageRemover();
            }
        }

        private async void CheckToken()
        {
            if (Auth.LoggedIn || Auth.CookieLoaded)
            {
                try
                {
                    ToggleLoginFields(false);
                    UpdateLoginButtonUI(true);
                    CurrentUser currentUser = Auth.AuthApi.GetCurrentUser();
                    _username.Text = currentUser.DisplayName;
                    _password.Text = "password";
                    UserId = currentUser.Id;
                    UserIconImage = currentUser.UserIcon;
                    UserBannerImage = !string.IsNullOrEmpty(currentUser.ProfilePicOverrideThumbnail) ? currentUser.ProfilePicOverrideThumbnail : currentUser.CurrentAvatarThumbnailImageUrl;
                    Badges.Clear();
                    viewPassword.Enabled = false;
                    foreach (var badge in currentUser.Badges)
                    {
                        Badges.Add(badge.ToJson());

                        if (badge.BadgeName == "Supporter") VRCPlus = true;
                    }
                    await _mainPanel.ProfileImage();

                    if (VRCPlus)
                    {
                        _mainPanel.SetFeatureControlsEnabled(true);
                    }
                    else
                    {
                        _vrcLoginLabel.Text = "VRChat Login (You need VRChat Plus to use this program.)";
                        _vrcLoginLabel.ForeColor = Color.Yellow;

                        NotificationManager.ShowNotification("You need VRChat Plus to use this program.", "VRChat Plus Required", NotificationType.Info);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Contains("expired"))
                    {
                        NotificationManager.ShowNotification("Token expired. Please log in again.", "Token Expired", NotificationType.Info);
                        Logout();
                    }
                    else
                    {
                        _vrcLogin.BackColor = Color.IndianRed;
                        _vrcLoginLabel.Text = ex.Message;
                        NotificationManager.ShowNotification("Token error: " + ex.Message, "Authentication Error", NotificationType.Error);
                    }
                }
            }
        }

        private void ToggleLoginFields(bool enabled)
        {
            _username.Enabled = enabled;
            _password.Enabled = enabled;
        }

        private void UpdateLoginButtonUI(bool loggedIn)
        {
            if (loggedIn)
            {
                _loginButton.Text = "Logout";
                _loginButton.TextColor = Color.FromArgb(255, 128, 128);
            }
            else
            {
                _loginButton.Text = "Login";
                _loginButton.TextColor = Color.FromArgb(106, 227, 249);
                _loginButton.BackColor = Color.FromArgb(7, 36, 43);
            }
        }

        private void _openGitHubPage_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/TheIceDragonz/VRCGalleryManager")
            {
                UseShellExecute = true
            });
        }

        private async void _checkUpdate_Click(object sender, EventArgs e)
        {
            var updater = new UpdateManager();
            await updater.CheckForUpdatesAsync(_checkUpdate);
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

        private void _clearAllVRChatLogs_Click(object sender, EventArgs e)
        {
            string vrchatLogPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"),
                "VRChat", "VRChat");
            string[] logFiles = Directory.GetFiles(vrchatLogPath, "output_log_*.txt");

            if (logFiles.Length == 0)
            {
                NotificationManager.ShowNotification("No log files found.", "Error", NotificationType.Error);
                return;
            }

            DialogResult result = MessageBox.Show(
                                $"Do you want to delete \"{logFiles.Length}\" VRChat Logs?",
                                "VRChat Logs",
                                MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                foreach (string file in logFiles)
                {
                    File.Delete(file);
                }
                NotificationManager.ShowNotification("All logs have been successfully deleted!", "Success", NotificationType.Success);
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

            string[] cacheFiles = Directory.GetFiles(cacheFolderPath);
            if (cacheFiles.Length == 0)
            {
                NotificationManager.ShowNotification("No cache files found.", "Error", NotificationType.Error);
                return;
            }

            DialogResult result = MessageBox.Show(
                            $"Do you want to delete All Cache Files?",
                            "Cache Files",
                            MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                foreach (string file in cacheFiles)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                NotificationManager.ShowNotification("All cache files have been successfully deleted!", "Success", NotificationType.Success);
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

        private void viewPassword_MouseDown(object sender, MouseEventArgs e)
        {
            _password.UseSystemPasswordChar = false;
        }

        private void viewPassword_MouseUp(object sender, MouseEventArgs e)
        {
            _password.UseSystemPasswordChar = true;
        }

    }
}
