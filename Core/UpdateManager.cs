using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRCGalleryManager.Core;

namespace VRCGalleryManager
{
    public class UpdateManager
    {
        private static readonly string apiUrl = "https://api.github.com/repos/TheIceDragonz/VRCGalleryManager/releases/latest";

        public async Task<bool> IsUpdateAvailableAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("VRCGalleryManager");
            string jsonResponse = await client.GetStringAsync(apiUrl);
            using var doc = JsonDocument.Parse(jsonResponse);
            var root = doc.RootElement;
            string latestVersion = Regex.Replace(root.GetProperty("tag_name").GetString() ?? "", @"[^\d\.]", "");
            string localVersion = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyFileVersionAttribute>()?
                .Version ?? throw new InvalidOperationException("Local version not found");

            return !string.Equals(localVersion, latestVersion, StringComparison.OrdinalIgnoreCase);
        }

        public async Task CheckForUpdatesAsync(Button checkUpdateButton)
        {
            string latestVersion;
            string localVersion;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("VRCGalleryManager");

            string jsonResponse = await client.GetStringAsync(apiUrl);
            using (var doc = JsonDocument.Parse(jsonResponse))
            {
                var root = doc.RootElement;
                latestVersion = Regex.Replace(root.GetProperty("tag_name").GetString() ?? "", @"[^\d\.]", "");
                localVersion = Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyFileVersionAttribute>()?
                    .Version ?? throw new InvalidOperationException("Local version not found");
            }

            bool isAvailable = !string.Equals(localVersion, latestVersion, StringComparison.OrdinalIgnoreCase);

            if (!isAvailable)
            {
                NotificationManager.ShowNotification(
                    "You are already using the latest version.",
                    "No Update",
                    NotificationType.Info);
                return;
            }

            var result = MessageBox.Show(
                $"A new version ({latestVersion}) is available.\n" +
                $"You are currently on version {localVersion}.\n\n" +
                "Do you want to download and install it?",
                "Update Available",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                checkUpdateButton.Enabled = false;
                checkUpdateButton.Text = "Updating (0%)";

                await DownloadAndInstallUpdateAsync(client, checkUpdateButton);
            }
        }

        private async Task DownloadAndInstallUpdateAsync(HttpClient client, Button checkUpdateButton)
        {
            string jsonResponse = await client.GetStringAsync(apiUrl);
            using var doc = JsonDocument.Parse(jsonResponse);
            var releaseJson = doc.RootElement;

            string installerUrl = "";
            if (releaseJson.TryGetProperty("assets", out var assets))
            {
                foreach (var asset in assets.EnumerateArray())
                {
                    var name = asset.GetProperty("name").GetString() ?? "";
                    if (name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        installerUrl = asset.GetProperty("browser_download_url").GetString()!;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(installerUrl))
            {
                NotificationManager.ShowNotification(
                    "Update installer not found among assets.",
                    "Error",
                    NotificationType.Error);
                return;
            }

            string tempFolder = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(tempFolder);
            string tempFilePath = Path.Combine(tempFolder, Path.GetFileName(installerUrl)!);

            using var response = await client.GetAsync(installerUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var totalBytes = response.Content.Headers.ContentLength ?? -1L;

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
                        checkUpdateButton.Invoke(new Action(() =>
                        {
                            checkUpdateButton.Text = $"Updating ({progress}%)";
                        }));
                    }
                }
            }

            Process.Start(new ProcessStartInfo(tempFilePath)
            {
                UseShellExecute = true
            });
            Application.Exit();
        }
    }
}
