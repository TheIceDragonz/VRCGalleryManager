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
#if ANDROID
using Android.Content;
using AndroidX.Core.Content;
#endif

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

        public static string GetCurrentVersion()
        {
            var asm = typeof(UpdateManager).Assembly;
            var fileVersion = asm.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
            if (!string.IsNullOrWhiteSpace(fileVersion))
                return fileVersion;

            var infoVersion = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion?.Split('+')[0];
            if (!string.IsNullOrWhiteSpace(infoVersion))
                return infoVersion;

            var asmVersion = asm.GetName().Version;
            if (asmVersion != null)
                return asmVersion.Build >= 0 
                    ? $"{asmVersion.Major}.{asmVersion.Minor}.{asmVersion.Build}" 
                    : $"{asmVersion.Major}.{asmVersion.Minor}";

#if ANDROID
            try
            {
                var v = Microsoft.Maui.ApplicationModel.AppInfo.Current.VersionString;
                if (!string.IsNullOrWhiteSpace(v))
                    return v;
            }
            catch { }
#endif

            return "";
        }

        public static bool IsNewerVersion(string latestVersion, string localVersion)
        {
            if (string.IsNullOrWhiteSpace(latestVersion) || string.IsNullOrWhiteSpace(localVersion))
                return false;

            if (Version.TryParse(latestVersion, out var latest) && Version.TryParse(localVersion, out var local))
            {
                return latest > local;
            }

            return !string.Equals(localVersion, latestVersion, StringComparison.OrdinalIgnoreCase);
        }

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
                string localVersion = GetCurrentVersion();

                bool isAvailable = IsNewerVersion(latestVersion, localVersion);
                _cachedUpdate = (isAvailable, latestVersion, localVersion);
                _lastCheckTime = DateTime.UtcNow;
                return (isAvailable, latestVersion, localVersion);
            }
            catch
            {
                return (false, "", "");
            }
        }

        public async Task<string> GetLatestApkUrlAsync()
        {
            try
            {
                string jsonResponse = await _httpClient.GetStringAsync(apiUrl);
                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;
                if (root.TryGetProperty("assets", out var assets))
                {
                    foreach (var asset in assets.EnumerateArray())
                    {
                        var name = asset.GetProperty("name").GetString() ?? "";
                        if (name.EndsWith(".apk", StringComparison.OrdinalIgnoreCase))
                        {
                            var downloadUrl = asset.GetProperty("browser_download_url").GetString();
                            if (!string.IsNullOrEmpty(downloadUrl))
                                return downloadUrl;
                        }
                    }
                }

                if (root.TryGetProperty("tag_name", out var tag))
                {
                    var tagName = tag.GetString() ?? "";
                    if (!string.IsNullOrEmpty(tagName))
                    {
                        return $"https://github.com/TheIceDragonz/VRCGalleryManager/releases/download/{tagName}/VRCGalleryManager.apk";
                    }
                }
            }
            catch
            {
            }

            return "https://github.com/TheIceDragonz/VRCGalleryManager/releases/latest/download/VRCGalleryManager.apk";
        }

        public Task DownloadAndroidApkAsync(NotificationService notificationService, string version)
        {
            return DownloadAndroidApkAsync(notificationService, null, version);
        }

#if ANDROID
        public static void InstallApk(string filePath)
        {
            var context = Android.App.Application.Context;
            var file = new Java.IO.File(filePath);
            if (!file.Exists())
            {
                throw new FileNotFoundException("APK file not found.", filePath);
            }

            string authority = $"{context.PackageName}.fileprovider";
            var apkUri = AndroidX.Core.Content.FileProvider.GetUriForFile(context, authority, file);

            var intent = new Android.Content.Intent(Android.Content.Intent.ActionView);
            intent.SetDataAndType(apkUri, "application/vnd.android.package-archive");
            intent.AddFlags(Android.Content.ActivityFlags.GrantReadUriPermission);
            intent.AddFlags(Android.Content.ActivityFlags.NewTask);

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O && !context.PackageManager!.CanRequestPackageInstalls())
            {
                try
                {
                    var manageIntent = new Android.Content.Intent(
                        Android.Provider.Settings.ActionManageUnknownAppSources,
                        Android.Net.Uri.Parse($"package:{context.PackageName}"));
                    manageIntent.AddFlags(Android.Content.ActivityFlags.NewTask);

                    var act = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
                    if (act != null)
                        act.StartActivity(manageIntent);
                    else
                        context.StartActivity(manageIntent);
                    return;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to open unknown sources settings: {ex.Message}");
                }
            }

            var currentActivity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            if (currentActivity != null)
            {
                currentActivity.StartActivity(intent);
            }
            else
            {
                context.StartActivity(intent);
            }
        }
#endif

        public async Task DownloadAndroidApkAsync(NotificationService notificationService, Action<string, int, bool>? updateProgressState = null, string version = "")
        {
            try
            {
                string apkUrl = await GetLatestApkUrlAsync();
                if (string.IsNullOrEmpty(apkUrl))
                {
                    notificationService.Show("APK URL not found.", "Update Error", NotificationType.Error);
                    updateProgressState?.Invoke("Error", 0, true);
                    return;
                }

#if ANDROID
                try
                {
                    var context = Android.App.Application.Context;
                    string fileName = string.IsNullOrEmpty(version)
                        ? "VRCGalleryManager.apk"
                        : $"VRCGalleryManager-v{version}.apk";

                    var externalFilesDir = context.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads) ?? context.CacheDir;
                    if (externalFilesDir != null && !externalFilesDir.Exists())
                    {
                        externalFilesDir.Mkdirs();
                    }

                    string targetFilePath = Path.Combine(externalFilesDir?.AbsolutePath ?? Path.GetTempPath(), fileName);

                    if (File.Exists(targetFilePath))
                    {
                        try { File.Delete(targetFilePath); } catch { }
                    }

                    notificationService.Show("Starting APK download...", "Update", NotificationType.Info, 3000);
                    updateProgressState?.Invoke("Downloading (0%)", 0, false);

                    using (var response = await _httpClient.GetAsync(apkUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        var totalBytes = response.Content.Headers.ContentLength ?? -1L;

                        using var downloadStream = await response.Content.ReadAsStreamAsync();
                        using var fileStream = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                        var buffer = new byte[81920];
                        long totalRead = 0;
                        int bytesRead;
                        int lastReportedProgress = -1;

                        while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalRead += bytesRead;
                            if (totalBytes > 0)
                            {
                                int progress = (int)((totalRead * 100) / totalBytes);
                                if (progress != lastReportedProgress)
                                {
                                    lastReportedProgress = progress;
                                    Microsoft.Maui.Controls.Application.Current?.Dispatcher?.Dispatch(() =>
                                    {
                                        updateProgressState?.Invoke($"Downloading ({progress}%)", progress, false);
                                    });
                                }
                            }
                        }
                    }

                    try
                    {
                        var publicDownloads = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                        if (publicDownloads != null)
                        {
                            if (!publicDownloads.Exists()) publicDownloads.Mkdirs();
                            string publicFilePath = Path.Combine(publicDownloads.AbsolutePath, fileName);
                            File.Copy(targetFilePath, publicFilePath, true);
                            Android.Media.MediaScannerConnection.ScanFile(
                                context,
                                new[] { publicFilePath },
                                new[] { "application/vnd.android.package-archive" },
                                null);
                        }
                    }
                    catch (Exception copyEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Could not copy APK to public downloads: {copyEx.Message}");
                    }

                    notificationService.Show("Download completed! Opening installer...", "Update", NotificationType.Success, 4000);
                    updateProgressState?.Invoke("Ready to install", 100, false);

                    InstallApk(targetFilePath);
                    return;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"In-app APK download/install failed: {ex.Message}");
                    updateProgressState?.Invoke("Error", 0, true);
                    notificationService.Show($"Error during download: {ex.Message}. Falling back to browser.", "Update Error", NotificationType.Error, 6000);
                }
#endif

                await Microsoft.Maui.ApplicationModel.Launcher.Default.OpenAsync(new Uri(apkUrl));
                notificationService.Show("Opening APK download link...", "Update", NotificationType.Info, 5000);
            }
            catch (Exception ex)
            {
                updateProgressState?.Invoke("Error", 0, true);
                notificationService.Show($"Failed to download APK: {ex.Message}", "Download Error", NotificationType.Error);
                await Microsoft.Maui.ApplicationModel.Launcher.Default.OpenAsync(new Uri("https://github.com/TheIceDragonz/VRCGalleryManager/releases/latest"));
            }
        }

        public async Task CheckForUpdatesAsync(NotificationService notificationService, DialogService dialogService, Action<string, int, bool> updateProgressState, bool silentIfNotAvailable = false)
        {
            var (isAvail, latestVersion, localVersion) = await IsUpdateAvailableAsync(true);

            if (!isAvail)
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

            if (!OperatingSystem.IsWindows())
            {
                bool download = await dialogService.ShowConfirmAsync(
                    "Update Available",
                    $"A new version ({latestVersion}) is available.\nYou are currently on version {localVersion}.\n\nDo you want to download and install the new Android APK?",
                    "Install APK", "Later");
                if (download)
                {
                    updateProgressState("Updating (0%)", 0, false);
                    await DownloadAndroidApkAsync(notificationService, updateProgressState, latestVersion);
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
            if (!OperatingSystem.IsWindows())
            {
                await DownloadAndroidApkAsync(notificationService, updateProgressState);
                return;
            }

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
