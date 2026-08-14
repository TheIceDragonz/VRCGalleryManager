using VRCGalleryManager.Core;

namespace VRCGalleryManager;

public partial class App : Application
{
#if WINDOWS
    private H.NotifyIcon.TaskbarIcon trayIcon;
#endif

	public App()
	{
		InitializeComponent();

		MainPage = new MainPage();
	}

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
        window.Title = "VRCGalleryManager";
#if WINDOWS
        window.MinimumWidth = 930;
        window.MinimumHeight = 800;

        window.Created += (s, e) =>
        {
            try
            {
                var menuFlyout = new MenuFlyout();

                var openItem = new MenuFlyoutItem 
                { 
                    Text = "Open", 
                    Command = new Command(() => BringWindowToFront(window)) 
                };
                menuFlyout.Add(openItem);

                var exitItem = new MenuFlyoutItem 
                { 
                    Text = "Quit", 
                    Command = new Command(() => 
                    {
                        Application.Current?.Dispatcher.Dispatch(() =>
                        {
                            trayIcon?.Dispose();
                            Application.Current.Quit();
                        });
                    }) 
                };
                menuFlyout.Add(exitItem);

                // Init tray icon
                trayIcon = new H.NotifyIcon.TaskbarIcon
                {
                    ToolTipText = "VRCGalleryManager",
                    IconSource = new FileImageSource { File = "icon.ico" },
                    NoLeftClickDelay = true,
                    LeftClickCommand = new Command(() =>
                    {
                        BringWindowToFront(window);
                    })
                };

                FlyoutBase.SetContextFlyout(trayIcon, menuFlyout);
                trayIcon.ForceCreate();
            }
            catch { }

            // Handle start in background
            bool startInBackground = Config.Get("StartInBackground", "false") == "true";
            string[] args = Environment.GetCommandLineArgs();
            bool hasBackgroundFlag = args.Contains("--background");

            // Handle minimize to tray and background start
            var winuiWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            if (winuiWindow != null)
            {
                var appWindow = GetAppWindow(winuiWindow);
                if (appWindow != null)
                {
                    appWindow.Closing += (sender, args) =>
                    {
                        if (Config.Get("MinimizeToTray", "true") == "true")
                        {
                            args.Cancel = true;
                            appWindow.Hide();
                        }
                    };

                    if (startInBackground && hasBackgroundFlag)
                    {
                        appWindow.Hide();
                    }
                }
            }
        };

        window.Destroying += (s, e) =>
        {
            trayIcon?.Dispose();
        };
#endif
        return window;
    }

#if WINDOWS
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    private const int SW_RESTORE = 9;

    private void BringWindowToFront(Window window)
    {
        Application.Current?.Dispatcher.Dispatch(() =>
        {
            var winuiWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            if (winuiWindow != null)
            {
                var appWindow = GetAppWindow(winuiWindow);
                if (appWindow != null)
                {
                    appWindow.Show();
                    winuiWindow.Activate();
                    
                    IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(winuiWindow);
                    ShowWindow(hwnd, SW_RESTORE);
                    SetForegroundWindow(hwnd);
                }
            }
        });
    }

    private Microsoft.UI.Windowing.AppWindow GetAppWindow(Microsoft.UI.Xaml.Window window)
    {
        IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
        return Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
    }
#endif
}
