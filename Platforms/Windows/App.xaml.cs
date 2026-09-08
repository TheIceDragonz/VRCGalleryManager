using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace VRCGalleryManager.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        SetWebView2UserDataFolder();

        this.InitializeComponent();

        var mainInstance = AppInstance.FindOrRegisterForKey("VRCGalleryManagerSingleInstance");
        if (!mainInstance.IsCurrent)
        {
            var args = AppInstance.GetCurrent().GetActivatedEventArgs();
            mainInstance.RedirectActivationToAsync(args).AsTask().Wait();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return;
        }

        mainInstance.Activated += MainInstance_Activated;
        DisableEfficiencyMode();
        VRCGalleryManager.Core.WindowsStartupHelper.SyncStartupPathIfEnabled();
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct PROCESS_POWER_THROTTLING_STATE
    {
        public uint Version;
        public uint ControlMask;
        public uint StateMask;
    }

    private const uint PROCESS_POWER_THROTTLING_CURRENT_VERSION = 1;
    private const uint PROCESS_POWER_THROTTLING_EXECUTION_SPEED = 0x1;
    private const int ProcessPowerThrottling = 13;

    [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetProcessInformation(IntPtr hProcess, int ProcessInformationClass, ref PROCESS_POWER_THROTTLING_STATE ProcessInformation, uint ProcessInformationSize);

    private void DisableEfficiencyMode()
    {
        try
        {
            var state = new PROCESS_POWER_THROTTLING_STATE
            {
                Version = PROCESS_POWER_THROTTLING_CURRENT_VERSION,
                ControlMask = PROCESS_POWER_THROTTLING_EXECUTION_SPEED,
                StateMask = 0
            };
            SetProcessInformation(System.Diagnostics.Process.GetCurrentProcess().Handle, ProcessPowerThrottling, ref state, (uint)System.Runtime.InteropServices.Marshal.SizeOf(state));
        }
        catch { }
    }

    private static void SetWebView2UserDataFolder()
    {
        try
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var userDataFolder = System.IO.Path.Combine(localAppData, "VRCGalleryManager", "WebView2");

            if (!System.IO.Directory.Exists(userDataFolder))
            {
                System.IO.Directory.CreateDirectory(userDataFolder);
            }

            Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", userDataFolder);
        }
        catch { }
    }

    private void MainInstance_Activated(object sender, AppActivationArguments e)
    {
        Microsoft.Maui.Controls.Application.Current?.Dispatcher.Dispatch(() =>
        {
            var window = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            if (window != null)
            {
                IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
                var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                if (appWindow != null)
                {
                    if (appWindow.Position.X < -10000 || appWindow.Position.Y < -10000)
                    {
                        try
                        {
                            var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(appWindow.Id, Microsoft.UI.Windowing.DisplayAreaFallback.Primary);
                            if (displayArea != null)
                            {
                                var centeredPosition = appWindow.Position;
                                centeredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
                                centeredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
                                appWindow.Move(centeredPosition);
                            }
                        }
                        catch { }
                    }

                    appWindow.Show();
                    window.Activate();
                }
            }
        });
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

