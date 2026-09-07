using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace VRCGalleryManager.Core
{
    public static class FileHelper
    {
#if WINDOWS
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public uint wFunc;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pFrom;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string? pTo;
            public ushort fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string? lpszProgressTitle;
        }

        private const uint FO_DELETE = 0x0003;
        private const ushort FOF_ALLOWUNDO = 0x0040;        // Move to Recycle Bin
        private const ushort FOF_NOCONFIRMATION = 0x0010;   // Don't show Windows confirmation dialog
        private const ushort FOF_NOERRORUI = 0x0400;        // Don't show system error UI
        private const ushort FOF_SILENT = 0x0004;           // Don't show progress window

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);
#endif

        /// <summary>
        /// Deletes a file by sending it to the Windows Recycle Bin if supported, or falling back to standard File.Delete.
        /// </summary>
        /// <param name="filePath">Full or relative path to the file.</param>
        /// <returns>True if successfully deleted or recycled, false otherwise.</returns>
        public static bool DeleteToRecycleBin(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return false;

#if WINDOWS
            try
            {
                string fullPath = Path.GetFullPath(filePath);

                var fileOp = new SHFILEOPSTRUCT
                {
                    hwnd = IntPtr.Zero,
                    wFunc = FO_DELETE,
                    pFrom = fullPath + "\0", // Double null-terminated when marshaled
                    pTo = null,
                    fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION | FOF_NOERRORUI | FOF_SILENT,
                    fAnyOperationsAborted = false,
                    hNameMappings = IntPtr.Zero,
                    lpszProgressTitle = null
                };

                int result = SHFileOperation(ref fileOp);
                if (result == 0 && !fileOp.fAnyOperationsAborted)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Recycle bin error: {ex.Message}");
            }
#endif

            // Fallback for Android or non-Windows / unhandled drives
            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"File.Delete error: {ex.Message}");
                return false;
            }
        }

        private static readonly HttpClient _downloadHttpClient = new HttpClient();

        /// <summary>
        /// Downloads raw image bytes directly from a URL with VRChat auth cookies if needed,
        /// avoiding heavy ImageSharp decoding and preventing memory/arithmetic overflow.
        /// </summary>
        public static async Task<(byte[] Bytes, string Extension)> DownloadBytesFromUrlAsync(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("User-Agent", "VRCGalleryManager/1.0.0 contact@vrcgallerymanager.com");
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Origin", "https://vrchat.com");

            if (url.Contains("vrchat.cloud"))
            {
                var authConfig = VRCAuth.Instance()?.Config;
                if (authConfig != null && authConfig.DefaultHeaders.TryGetValue("Cookie", out var cookieValue))
                {
                    request.Headers.Add("Cookie", cookieValue);
                }
            }

            using var res = await _downloadHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (!res.IsSuccessStatusCode)
            {
                string errorBody = await res.Content.ReadAsStringAsync();
                throw new Exception($"HTTP {res.StatusCode}: {errorBody}");
            }

            string? ct = res.Content.Headers.ContentType?.MediaType;
            string ext = ct?.ToLowerInvariant() switch
            {
                "image/jpeg" or "image/jpg" => ".jpg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/webp" => ".webp",
                _ => ""
            };

            if (string.IsNullOrEmpty(ext))
            {
                string cleanUrl = url.Split('?')[0];
                string urlExt = Path.GetExtension(cleanUrl);
                if (!string.IsNullOrEmpty(urlExt) && urlExt.Length <= 5)
                {
                    ext = urlExt.ToLowerInvariant();
                }
                else
                {
                    ext = ".png";
                }
            }

            byte[] bytes = await res.Content.ReadAsByteArrayAsync();
            return (bytes, ext);
        }

        /// <summary>
        /// Saves a file directly to the system Downloads folder on Android, or prompts via FileSaver on desktop/other platforms.
        /// </summary>
        /// <param name="fileName">Desired file name.</param>
        /// <param name="bytes">File content bytes.</param>
        /// <param name="extension">File extension (e.g. .png, .jpg).</param>
        /// <returns>Tuple indicating success and saved destination/path.</returns>
#if ANDROID
        public static async Task<(bool Success, string? Destination)> SaveImageToDownloadsAsync(string fileName, byte[] bytes, string extension)
        {
            try
            {
                var context = Android.App.Application.Context;
                string ext = extension.StartsWith(".") ? extension : "." + extension;
                string cleanFileName = fileName;
                foreach (char c in Path.GetInvalidFileNameChars())
                {
                    cleanFileName = cleanFileName.Replace(c, '_');
                }

                if (!cleanFileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                {
                    cleanFileName += ext;
                }

                string mimeType = ext.ToLowerInvariant() switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    ".mp4" => "video/mp4",
                    _ => "application/octet-stream"
                };

                // Method 1: On Android 10+ (API 29+), try MediaStore.Downloads
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Q)
                {
                    try
                    {
                        var contentResolver = context.ContentResolver;
                        if (contentResolver != null)
                        {
                            var contentValues = new Android.Content.ContentValues();
                            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.DisplayName, cleanFileName);
                            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, mimeType);
                            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.RelativePath, Android.OS.Environment.DirectoryDownloads + "/");
                            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.IsPending, 1);

                            var uri = contentResolver.Insert(Android.Provider.MediaStore.Downloads.ExternalContentUri, contentValues);
                            if (uri != null)
                            {
                                using (var outputStream = contentResolver.OpenOutputStream(uri))
                                {
                                    if (outputStream != null)
                                    {
                                        await outputStream.WriteAsync(bytes, 0, bytes.Length);
                                        await outputStream.FlushAsync();
                                    }
                                }

                                contentValues.Clear();
                                contentValues.Put(Android.Provider.MediaStore.IMediaColumns.IsPending, 0);
                                contentResolver.Update(uri, contentValues, null, null);

                                return (true, uri.ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"MediaStore save error, attempting direct file fallback: {ex.Message}");
                    }
                }

                // Method 2: Fallback direct write to public Downloads directory
                var downloadsDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                if (downloadsDir != null && !downloadsDir.Exists())
                {
                    downloadsDir.Mkdirs();
                }

                string baseName = Path.GetFileNameWithoutExtension(cleanFileName);
                string targetPath = Path.Combine(downloadsDir!.AbsolutePath, cleanFileName);

                int counter = 1;
                while (File.Exists(targetPath))
                {
                    targetPath = Path.Combine(downloadsDir.AbsolutePath, $"{baseName} ({counter}){ext}");
                    counter++;
                }

                await File.WriteAllBytesAsync(targetPath, bytes);

                Android.Media.MediaScannerConnection.ScanFile(
                    context,
                    new[] { targetPath },
                    new[] { mimeType },
                    null);

                return (true, targetPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Android download save error: {ex}");
                throw;
            }
        }
#else
        public static async Task<(bool Success, string? Destination)> SaveImageToDownloadsAsync(string fileName, byte[] bytes, string extension)
        {
            string ext = extension.StartsWith(".") ? extension : "." + extension;
            string cleanFileName = fileName;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                cleanFileName = cleanFileName.Replace(c, '_');
            }

            if (!cleanFileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
            {
                cleanFileName += ext;
            }

            using var stream = new MemoryStream(bytes);
            var result = await CommunityToolkit.Maui.Storage.FileSaver.Default.SaveAsync(cleanFileName, stream, default);
            if (result.IsSuccessful)
            {
                return (true, result.FilePath);
            }
            return (false, null);
        }
#endif

        /// <summary>
        /// Opens a file or content URI using the platform default application.
        /// </summary>
        public static Task OpenFileAsync(string destinationPathOrUri, string mimeType = "image/*")
        {
            if (string.IsNullOrWhiteSpace(destinationPathOrUri)) return Task.CompletedTask;

#if ANDROID
            try
            {
                var context = Android.App.Application.Context;
                var intent = new Android.Content.Intent(Android.Content.Intent.ActionView);

                if (destinationPathOrUri.StartsWith("content://", StringComparison.OrdinalIgnoreCase))
                {
                    intent.SetDataAndType(Android.Net.Uri.Parse(destinationPathOrUri), mimeType);
                    intent.AddFlags(Android.Content.ActivityFlags.GrantReadUriPermission);
                }
                else if (File.Exists(destinationPathOrUri))
                {
                    var builder = new Android.OS.StrictMode.VmPolicy.Builder();
                    Android.OS.StrictMode.SetVmPolicy(builder.Build());

                    var file = new Java.IO.File(destinationPathOrUri);
                    intent.SetDataAndType(Android.Net.Uri.FromFile(file), mimeType);
                }
                else
                {
                    return Task.CompletedTask;
                }

                intent.AddFlags(Android.Content.ActivityFlags.NewTask);

                var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
                if (activity != null)
                {
                    activity.StartActivity(intent);
                }
                else
                {
                    context.StartActivity(intent);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OpenFileAsync Android error: {ex.Message}");
            }
#else
            try
            {
                if (File.Exists(destinationPathOrUri))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(destinationPathOrUri)
                    {
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OpenFileAsync error: {ex.Message}");
            }
#endif
            return Task.CompletedTask;
        }
    }
}
