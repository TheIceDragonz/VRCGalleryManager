using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRCGalleryManager.Core;

namespace VRCGalleryManager
{
    public class UpdateManager
    {
        private static readonly string apiUrl = "https://api.github.com/repos/TheIceDragonz/VRCGalleryManager/releases/latest";

        public async Task CheckForUpdatesAsync()
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
                                await DownloadAndInstallUpdateAsync(client, root);
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
        }

        private async Task DownloadAndInstallUpdateAsync(HttpClient client, JsonElement releaseJson)
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
                string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(installerUrl));
                using (var installerStream = await client.GetStreamAsync(installerUrl))
                using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await installerStream.CopyToAsync(fileStream);
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
