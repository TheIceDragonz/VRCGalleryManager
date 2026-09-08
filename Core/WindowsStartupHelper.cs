using System;
using System.Diagnostics;
using System.IO;

namespace VRCGalleryManager.Core
{
    public static class WindowsStartupHelper
    {
#if WINDOWS
        private const string RegistryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "VRCGalleryManager";

        public static void SetStartup(bool enable, bool startInBackground = false)
        {
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true);
                if (key == null) return;

                if (enable)
                {
                    string exePath = Process.GetCurrentProcess().MainModule?.FileName;
                    if (!string.IsNullOrEmpty(exePath))
                    {
                        string command = startInBackground ? $"\"{exePath}\" --background" : $"\"{exePath}\"";
                        key.SetValue(AppName, command);
                    }
                }
                else
                {
                    key.DeleteValue(AppName, false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting startup: {ex.Message}");
            }
        }

        public static bool IsStartupEnabled()
        {
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistryKeyPath, false);
                if (key == null) return false;

                var value = key.GetValue(AppName);
                return value != null;
            }
            catch
            {
                return false;
            }
        }
#else
        public static void SetStartup(bool enable, bool startInBackground = false)
        {
            // Not supported on this platform
        }

        public static bool IsStartupEnabled()
        {
            return false;
        }
#endif
    }
}
