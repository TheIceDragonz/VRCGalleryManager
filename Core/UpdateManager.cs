using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using VRCGalleryManager.Core;

namespace VRCGalleryManager
{
    public class UpdateManager
    {
        private static readonly string apiUrl = "https://api.github.com/repos/TheIceDragonz/VRCGalleryManager/releases/latest";
        private static readonly HttpClient _httpClient = CreateHttpClient();

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("VRCGalleryManager");
            return client;
        }

        private (bool isAvailable, string latestVersion, string localVersion)? _cachedUpdate;
        private DateTime _lastCheckTime = DateTime.MinValue;

        public async Task<(bool isAvailable, string latestVersion, string localVersion)> IsUpdateAvailableAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && _cachedUpdate.HasValue && (DateTime.UtcNow - _lastCheckTime).TotalMinutes < 5)
            {
                return _cachedUpdate.Value;
            }

            try
            {
                string jsonResponse = await _httpClient.GetStringAsync(apiUrl);
                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;
                string latestVersion = Regex.Replace(root.GetProperty("tag_name").GetString() ?? "", @"[^\d\.]", "");
                string localVersion = Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyFileVersionAttribute>()?
                    .Version ?? "1.0.0";

                bool isAvailable = !string.Equals(localVersion, latestVersion, StringComparison.OrdinalIgnoreCase);
                _cachedUpdate = (isAvailable, latestVersion, localVersion);
                _lastCheckTime = DateTime.UtcNow;
                return (isAvailable, latestVersion, localVersion);
            }
            catch
            {
                return (false, "", "");
            }
        }

        public async Task CheckForUpdatesAsync(NotificationService notificationService, DialogService dialogService, Action<string, int, bool> updateProgressState, bool silentIfNotAvailable = false)
        {
            if (!OperatingSystem.IsWindows())
            {
                if (!silentIfNotAvailable)
                {
                    updateProgressState("Updates are only supported on Windows.", 0, true);
                    notificationService.Show("Updates are currently only available for the Windows version.", "Unsupported OS", NotificationType.Info);
                }
                return;
            }

            string latestVersion;
            string localVersion;

            string jsonResponse = await _httpClient.GetStringAsync(apiUrl);
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
                if (!silentIfNotAvailable)
                {
                    notificationService.Show(
                        "You are already using the latest version.",
                        "No Update",
                        NotificationType.Info);
                }
                return;
            }

            var result = await dialogService.ShowConfirmAsync(
                "Update Available",
                $"A new version ({latestVersion}) is available.\n" +
                $"You are currently on version {localVersion}.\n\n" +
                "Do you want to download and install it?",
                "Yes", "No");

            if (result)
            {
                updateProgressState("Updating (0%)", 0, false);
                await DownloadAndInstallUpdateAsync(notificationService, updateProgressState);
            }
        }

        public async Task DownloadAndInstallUpdateAsync(NotificationService notificationService, Action<string, int, bool> updateProgressState)
        {
            string jsonResponse = await _httpClient.GetStringAsync(apiUrl);
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
                notificationService.Show(
                    "Update installer not found among assets.",
                    "Error",
                    NotificationType.Error);
                return;
            }

            string tempFolder = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(tempFolder);
            string tempFilePath = Path.Combine(tempFolder, Path.GetFileName(installerUrl)!);

            using var response = await _httpClient.GetAsync(installerUrl, HttpCompletionOption.ResponseHeadersRead);
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
                        Application.Current.Dispatcher.Dispatch(() =>
                        {
                            updateProgressState($"Updating ({progress}%)", progress, false);
                        });
                    }
                }
            }

            Process.Start(new ProcessStartInfo(tempFilePath)
            {
                UseShellExecute = true
            });
            Application.Current.Quit();
        }
    }
}
