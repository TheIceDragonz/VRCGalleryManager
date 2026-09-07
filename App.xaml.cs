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

        // Configure native TitleBar (.NET 9) with styling matching .about-update-badge
        var updateBadgeButton = new Button
        {
            Text = "Update Available",
            FontSize = 10,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#ffc107"),
            BackgroundColor = Color.FromRgba(255, 193, 7, 38),
            BorderColor = Color.FromRgba(255, 193, 7, 89),
            BorderWidth = 1,
            CornerRadius = 4,
            HeightRequest = 20,
            MinimumHeightRequest = 0,
            MinimumWidthRequest = 0,
            Padding = new Thickness(6, 0),
            Margin = new Thickness(0),
            IsVisible = false
        };

        bool isCursorConfigured = false;
        void ApplyHandCursor()
        {
            try
            {
                if (updateBadgeButton.Handler?.PlatformView is Microsoft.UI.Xaml.UIElement winElement)
                {
                    var handCursor = Microsoft.UI.Input.InputSystemCursor.Create(Microsoft.UI.Input.InputSystemCursorShape.Hand);
                    var arrowCursor = Microsoft.UI.Input.InputSystemCursor.Create(Microsoft.UI.Input.InputSystemCursorShape.Arrow);
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Public);

                    prop?.SetValue(winElement, handCursor);

                    if (!isCursorConfigured)
                    {
                        isCursorConfigured = true;
                        winElement.PointerEntered += (s, e) =>
                        {
                            try { prop?.SetValue(winElement, handCursor); } catch { }
                        };
                        winElement.PointerExited += (s, e) =>
                        {
                            try { prop?.SetValue(winElement, arrowCursor); } catch { }
                        };
                    }
                }
            }
            catch { }
        }

        updateBadgeButton.Loaded += (s, e) => ApplyHandCursor();
        updateBadgeButton.HandlerChanged += (s, e) =>
        {
            isCursorConfigured = false;
            ApplyHandCursor();
        };

        var pointerGesture = new PointerGestureRecognizer();
        pointerGesture.PointerEntered += (s, e) =>
        {
            updateBadgeButton.BackgroundColor = Color.FromRgba(255, 193, 7, 60);
            ApplyHandCursor();
        };
        pointerGesture.PointerExited += (s, e) =>
        {
            updateBadgeButton.BackgroundColor = Color.FromRgba(255, 193, 7, 38);
        };
        updateBadgeButton.GestureRecognizers.Add(pointerGesture);

        updateBadgeButton.Clicked += (s, e) =>
        {
            AppEvents.NavigateToSettings(promptUpdate: true);
            BringWindowToFront(window);
        };

        AppEvents.UpdateStatusChanged += (isAvailable, latestVersion) =>
        {
            Application.Current?.Dispatcher.Dispatch(() =>
            {
                if (isAvailable)
                {
                    updateBadgeButton.Text = string.IsNullOrWhiteSpace(latestVersion) ? "Update Available" : $"Update v{latestVersion}";
                    updateBadgeButton.IsVisible = true;
                    ApplyHandCursor();
                }
                else
                {
                    updateBadgeButton.IsVisible = false;
                }
            });
        };

        var titleBar = new TitleBar
        {
            BackgroundColor = Color.FromArgb("#111418"),
            ForegroundColor = Color.FromArgb("#E2E8F0"),
            LeadingContent = new HorizontalStackLayout
            {
                Spacing = 8, // Distanza personalizzabile tra logo e testo (in pixel)
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(10, 0, 0, 0),
                InputTransparent = true,
                Children =
                {
                    new Image
                    {
                        Source = "icon.ico",
                        WidthRequest = 16,
                        HeightRequest = 16,
                        VerticalOptions = LayoutOptions.Center,
                        InputTransparent = true
                    },
                    new Label
                    {
                        Text = "VRCGalleryManager",
                        TextColor = Color.FromArgb("#E2E8F0"),
                        VerticalOptions = LayoutOptions.Center,
                        FontSize = 12,
                        InputTransparent = true
                    }
                }
            },
            TrailingContent = new HorizontalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 0, 8, 0),
                Children = { updateBadgeButton }
            }
        };
        titleBar.PassthroughElements.Add(updateBadgeButton);

        window.TitleBar = titleBar;

        window.Created += (s, e) =>
        {
            try
            {
                var winuiWin = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
                if (winuiWin != null)
                {
                    var appWin = GetAppWindow(winuiWin);
                    if (appWin != null)
                    {
                        appWin.SetIcon("icon.ico");
                        if (appWin.TitleBar != null)
                        {
                            appWin.TitleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
                            appWin.TitleBar.ButtonForegroundColor = Microsoft.UI.ColorHelper.FromArgb(255, 226, 232, 240);
                            appWin.TitleBar.ButtonHoverBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(255, 33, 38, 45);
                            appWin.TitleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.White;
                            appWin.TitleBar.ButtonPressedBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(255, 48, 54, 61);
                            appWin.TitleBar.ButtonPressedForegroundColor = Microsoft.UI.Colors.White;
                            appWin.TitleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
                            appWin.TitleBar.ButtonInactiveForegroundColor = Microsoft.UI.ColorHelper.FromArgb(255, 110, 118, 129);
                        }
                    }
                }
            }
            catch { }
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
