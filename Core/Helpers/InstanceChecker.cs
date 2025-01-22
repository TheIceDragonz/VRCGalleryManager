using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VRCGalleryManager.Core.Helpers
{
    public static class InstanceChecker
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(nint hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(nint hWnd, int nCmdShow);

        private const int SW_SHOWNORMAL = 1;

        public static bool IsAlreadyRunning(string appName)
        {
            var currentProcess = Process.GetCurrentProcess();
            var existingProcess = Process.GetProcessesByName(currentProcess.ProcessName)
                                         .FirstOrDefault(p => p.Id != currentProcess.Id);

            if (existingProcess != null)
            {
                nint hWnd = existingProcess.MainWindowHandle;
                if (hWnd != nint.Zero)
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
