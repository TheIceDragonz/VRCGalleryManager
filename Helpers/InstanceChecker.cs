using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace VRCGalleryManager.Helpers
{
    public static class InstanceChecker
    {
        // Importazioni delle API di Windows
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_SHOWNORMAL = 1;

        public static bool IsAlreadyRunning(string appName)
        {
            var currentProcess = Process.GetCurrentProcess();
            var existingProcess = Process.GetProcessesByName(currentProcess.ProcessName)
                                         .FirstOrDefault(p => p.Id != currentProcess.Id);

            if (existingProcess != null)
            {
                IntPtr hWnd = existingProcess.MainWindowHandle;
                if (hWnd != IntPtr.Zero)
                {
                    ShowWindow(hWnd, SW_SHOWNORMAL);
                    SetForegroundWindow(hWnd);
                }
                return true;
            }

            return false;
        }
    }
}
