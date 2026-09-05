using System;
using System.IO;
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
    }
}
