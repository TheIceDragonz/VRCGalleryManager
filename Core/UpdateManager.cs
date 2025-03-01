using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using VRCGalleryManager.Core;

namespace VRCGalleryManager
{
    public class UpdateManager
    {
        private static readonly string apiUrl = "https://api.github.com/repos/TheIceDragonz/VRCGalleryManager/releases/latest";

        public async Task CheckForUpdatesAsync(Button _checkUpdate)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("VRCGalleryManager");
                    string jsonResponse = await client.GetStringAsync(apiUrl);
                    using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                    {
                        JsonElement root = doc.RootElement;
                        string latestVersion = root.GetProperty("tag_name").GetString();
                        string localVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                        if (!string.Equals(localVersion, latestVersion, StringComparison.OrdinalIgnoreCase))
                        {
                            DialogResult result = MessageBox.Show(
                                $"A new version ({latestVersion}) is available.\nYou are currently using version {localVersion}.\n\nDo you want to download and install the update?",
                                "Update Available",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);

                            if (result == DialogResult.Yes)
                            {
                                _checkUpdate.Enabled = false;
                                _checkUpdate.Text = "Updating (0%)";
                                await DownloadAndInstallUpdateAsync(client, root, _checkUpdate);
                            }
                        }
                        else
                        {
                            NotificationManager.ShowNotification("You are using the latest version.", "No Update", NotificationType.Info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification("Error checking for updates:\n" + ex.Message, "Error", NotificationType.Error);
            }
            finally
            {
                if (!_checkUpdate.Enabled)
                {
                    _checkUpdate.Text = "Check Update";
                    _checkUpdate.Enabled = true;
                }
            }
        }

        private async Task DownloadAndInstallUpdateAsync(HttpClient client, JsonElement releaseJson, Button _checkUpdate)
        {
            string installerUrl = "";
            if (releaseJson.TryGetProperty("assets", out JsonElement assets))
            {
                foreach (JsonElement asset in assets.EnumerateArray())
                {
                    string assetName = asset.GetProperty("name").GetString();
                    if (assetName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        installerUrl = asset.GetProperty("browser_download_url").GetString();
                        break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(installerUrl))
            {
                string tempFolder = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }
                string tempFilePath = Path.Combine(tempFolder, Path.GetFileName(installerUrl));
                using (var response = await client.GetAsync(installerUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    var totalBytes = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1L;
                    using (var installerStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        var buffer = new byte[8192];
                        long totalRead = 0;
                        int bytesRead;
                        while ((bytesRead = await installerStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalRead += bytesRead;
                            if (totalBytes > 0)
                            {
                                int progress = (int)((totalRead * 100) / totalBytes);
                                _checkUpdate.Invoke(new Action(() =>
                                {
                                    _checkUpdate.Text = $"Updating ({progress}%)";
                                }));
                            }
                        }
                    }
                }
                Process.Start(new ProcessStartInfo(tempFilePath)
                {
                    UseShellExecute = true
                });
                Application.Exit();
            }
            else
            {
                NotificationManager.ShowNotification("Update installer not found among assets.", "Error", NotificationType.Error);
            }
        }
    }
}
