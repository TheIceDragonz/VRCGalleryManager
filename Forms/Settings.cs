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
                    foreach (var badge in currentUser.Badges)
                    {
                        Badges.Add(badge.ToJson());
                    }
                    await _mainPanel.ProfileImage();
                    _mainPanel.SetFeatureControlsEnabled(true);
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Contains("expired"))
                    {
                        NotificationManager.ShowNotification("Token expired. Please log in again.", "Token Expired", NotificationType.Error);
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
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("VRCGalleryManager");
                    string apiUrl = "https://api.github.com/repos/TheIceDragonz/VRCGalleryManager/releases/latest";
                    string jsonResponse = await client.GetStringAsync(apiUrl);
                    using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                    {
                        JsonElement root = doc.RootElement;
                        string latestVersion = root.GetProperty("tag_name").GetString();
                        string localVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                        if (!string.Equals(localVersion, latestVersion, StringComparison.OrdinalIgnoreCase))
                        {
                            DialogResult result = MessageBox.Show(
                                $"A new version ({latestVersion}) is available.\nYou are currently using version {localVersion}.\n\nDo you want to open the releases page?",
                                "Update Available",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);
                            if (result == DialogResult.Yes)
                            {
                                Process.Start(new ProcessStartInfo("https://github.com/TheIceDragonz/VRCGalleryManager/releases")
                                {
                                    UseShellExecute = true
                                });
                            }
                        }
                        else
                        {
                            NotificationManager.ShowNotification("You are using the latest version.", "No Update", NotificationType.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification("Error checking for updates:\n" + ex.Message, "Error", NotificationType.Error);
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
            string[] cacheFiles = Directory.GetFiles(cacheFolderPath);

            if(cacheFiles.Length == 0)
            {
                NotificationManager.ShowNotification("No cache files found.", "Error", NotificationType.Error);
                return;
            }

            if (!Directory.Exists(cacheFolderPath))
            {
                NotificationManager.ShowNotification("Cache folder does not exist.", "Error", NotificationType.Error);
                return;
            }
            try
            {
                DialogResult result = MessageBox.Show(
                                $"Do you want to delete All Cache Files?",
                                "Cache Files",
                                MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    foreach (string file in cacheFiles)
                    {
                        File.Delete(file);
                    }
                    NotificationManager.ShowNotification("All cache files have been successfully deleted!", "Success", NotificationType.Success);
                }
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
