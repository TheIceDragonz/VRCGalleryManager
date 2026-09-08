using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

#if WINDOWS
using System.Runtime.InteropServices;
using System.Threading;
#endif

namespace VRCGalleryManager.Core
{
    public static class FolderPickerHelper
    {
        /// <summary>
        /// Attempts to automatically discover the default VRChat screenshot directory on the PC.
        /// Searches standard Windows known folders and localized names across languages and OneDrive.
        /// </summary>
        public static string? GetDefaultVRChatPhotoPath()
        {
            var candidates = new List<string>();

            // 1. MyPictures SpecialFolder (takes into account customized Windows folder mappings)
            try
            {
                var myPics = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                if (!string.IsNullOrWhiteSpace(myPics))
                {
                    candidates.Add(Path.Combine(myPics, "VRChat"));
                }
            }
            catch { }

            // 2. UserProfile localized folder names
            try
            {
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                if (!string.IsNullOrWhiteSpace(userProfile))
                {
                    string[] subNames = { "Pictures", "Immagini", "Images", "Bilder", "Imágenes", "Fotos" };
                    foreach (var name in subNames)
                    {
                        candidates.Add(Path.Combine(userProfile, name, "VRChat"));
                        candidates.Add(Path.Combine(userProfile, "OneDrive", name, "VRChat"));
                    }
                }
            }
            catch { }

            // Prioritize candidate path that actually exists and has files or directories
            foreach (var path in candidates)
            {
                try
                {
                    if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
                    {
                        return path;
                    }
                }
                catch { }
            }

            // If none has contents, check if any candidate exists
            foreach (var path in candidates)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        return path;
                    }
                }
                catch { }
            }

            return null;
        }

        /// <summary>
        /// Displays a native folder picker dialog to select a folder.
        /// On Windows, uses native Win32 IFileOpenDialog running in an STA thread for 100% reliability.
        /// </summary>
        public static async Task<string?> PickFolderAsync(string? initialPath = null, string? title = "Select VRChat Screenshots Folder")
        {
#if WINDOWS
            try
            {
                var tcs = new TaskCompletionSource<string?>();

                var thread = new Thread(() =>
                {
                    try
                    {
                        var dialog = (IFileOpenDialog)new FileOpenDialogRCW();

                        dialog.GetOptions(out var options);
                        options |= FOS.FOS_PICKFOLDERS | FOS.FOS_FORCEFILESYSTEM | FOS.FOS_PATHMUSTEXIST;
                        dialog.SetOptions(options);

                        if (!string.IsNullOrWhiteSpace(title))
                        {
                            dialog.SetTitle(title);
                        }

                        if (!string.IsNullOrWhiteSpace(initialPath) && Directory.Exists(initialPath))
                        {
                            try
                            {
                                var riid = new Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"); // IShellItem GUID
                                if (SHCreateItemFromParsingName(initialPath, IntPtr.Zero, ref riid, out var shellItem) == 0 && shellItem != null)
                                {
                                    dialog.SetFolder(shellItem);
                                    dialog.SetDefaultFolder(shellItem);
                                }
                            }
                            catch { }
                        }

                        IntPtr hwnd = GetForegroundWindow();
                        int hr = dialog.Show(hwnd);

                        // 0 = S_OK
                        if (hr == 0)
                        {
                            dialog.GetResult(out var resultItem);
                            if (resultItem != null)
                            {
                                resultItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out var pickedPath);
                                if (!string.IsNullOrWhiteSpace(pickedPath))
                                {
                                    tcs.TrySetResult(pickedPath);
                                    return;
                                }
                            }
                        }

                        tcs.TrySetResult(null);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Win32 FolderPicker error: {ex}");
                        tcs.TrySetException(ex);
                    }
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;
                thread.Start();

                var selected = await tcs.Task;
                return selected;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Win32 FolderPicker error: {ex}");
                return null;
            }
#else
            await Task.CompletedTask;
            return null;
#endif
        }

#if WINDOWS
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            IntPtr pbc,
            [In] ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

        [ComImport]
        [Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")]
        [ClassInterface(ClassInterfaceType.None)]
        [TypeLibType(TypeLibTypeFlags.FCanCreate)]
        private class FileOpenDialogRCW { }

        [ComImport]
        [Guid("42f85136-db7e-439c-85f1-e4075d135fc8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IFileOpenDialog
        {
            [PreserveSig] int Show([In] IntPtr parent);
            void SetFileTypes([In] uint cFileTypes, [In] IntPtr rgFilterSpec);
            void SetFileTypeIndex([In] uint iFileType);
            void GetFileTypeIndex(out uint piFileType);
            void Advise([In, MarshalAs(UnmanagedType.Interface)] IntPtr pfde, out uint pdwCookie);
            void Unadvise([In] uint dwCookie);
            void SetOptions([In] FOS fos);
            void GetOptions(out FOS fos);
            void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);
            void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);
            void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
            void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
            void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
            void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
            void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);
            void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
            void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
            void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, int fdap);
            void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
            void Close([MarshalAs(UnmanagedType.Error)] int hr);
            void SetClientGuid([In] ref Guid guid);
            void ClearClientData();
            void SetFilter([In, MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
            void GetResults([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenum);
            void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppsai);
        }

        [ComImport]
        [Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItem
        {
            void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv);
            void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);
            void GetDisplayName([In] SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
            void GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);
            void Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
        }

        [Flags]
        private enum FOS : uint
        {
            FOS_PICKFOLDERS = 0x00000020,
            FOS_FORCEFILESYSTEM = 0x00000040,
            FOS_PATHMUSTEXIST = 0x00000800,
            FOS_FILEMUSTEXIST = 0x00001000,
            FOS_NOCHANGEDIR = 0x00000008,
            FOS_DONTADDTORECENT = 0x02000000,
        }

        private enum SIGDN : uint
        {
            SIGDN_FILESYSPATH = 0x80058000,
        }
#endif
    }
}
