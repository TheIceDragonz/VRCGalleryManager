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
                    var prop = typeof(Microsoft.UI.Xaml.UIElement).GetProperty("ProtectedCursor",
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

        var titleTextLabel = new Label
        {
            Text = "VRCGalleryManager",
            TextColor = Color.FromArgb("#E2E8F0"),
            VerticalOptions = LayoutOptions.Center,
            FontSize = 12,
            InputTransparent = true
        };

        var titleBar = new TitleBar
        {
            BackgroundColor = Color.FromArgb("#111418"),
            ForegroundColor = Color.FromArgb("#E2E8F0"),
            LeadingContent = new HorizontalStackLayout
            {
                Spacing = 8,
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
                    titleTextLabel
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

        Microsoft.UI.Windowing.AppWindow? currentAppWindow = null;
        bool isThemeActive = Config.Get("DisplayVRCProfileThemes", "true") == "true" &&
                             Config.Get("CachedThemeIsCustom", "false") == "true";
        string? themeButtonColorHex = Config.Get("CachedThemeButtonColor", "#6ae3f9");
        string? themeIconColorHex = Config.Get("CachedThemeIconColor", "#6ae3f9");

        static Windows.UI.Color ParseHexColor(string? hex, Windows.UI.Color fallback)
        {
            if (string.IsNullOrWhiteSpace(hex)) return fallback;
            try
            {
                string clean = hex.Trim().TrimStart('#');
                if (clean.Length == 6 &&
                    byte.TryParse(clean.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out byte r) &&
                    byte.TryParse(clean.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out byte g) &&
                    byte.TryParse(clean.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out byte b))
                {
                    return Microsoft.UI.ColorHelper.FromArgb(255, r, g, b);
                }
                else if (clean.Length == 8 &&
                    byte.TryParse(clean.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out byte a) &&
                    byte.TryParse(clean.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out byte r8) &&
                    byte.TryParse(clean.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out byte g8) &&
                    byte.TryParse(clean.Substring(6, 2), System.Globalization.NumberStyles.HexNumber, null, out byte b8))
                {
                    return Microsoft.UI.ColorHelper.FromArgb(a, r8, g8, b8);
                }
            }
            catch { }
            return fallback;
        }

        void ApplyTitleBarTheme()
        {
            try
            {
                if (currentAppWindow == null)
                {
                    if (window.Handler?.PlatformView is Microsoft.UI.Xaml.Window w)
                    {
                        currentAppWindow = GetAppWindow(w);
                    }
                }

                if (currentAppWindow?.TitleBar != null)
                {
                    var nativeTitleBar = currentAppWindow.TitleBar;
                    if (isThemeActive)
                    {
                        string? targetHex = !string.IsNullOrEmpty(themeIconColorHex) ? themeIconColorHex : themeButtonColorHex;
                        var themeColor = ParseHexColor(targetHex, Microsoft.UI.ColorHelper.FromArgb(255, 106, 227, 249));

                        nativeTitleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
                        nativeTitleBar.ButtonForegroundColor = themeColor;
                        nativeTitleBar.ButtonHoverBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(60, themeColor.R, themeColor.G, themeColor.B);
                        nativeTitleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.White;
                        nativeTitleBar.ButtonPressedBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(110, themeColor.R, themeColor.G, themeColor.B);
                        nativeTitleBar.ButtonPressedForegroundColor = Microsoft.UI.Colors.White;
                        nativeTitleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
                        nativeTitleBar.ButtonInactiveForegroundColor = Microsoft.UI.ColorHelper.FromArgb(120, themeColor.R, themeColor.G, themeColor.B);
                    }
                    else
                    {
                        var defaultColor = Microsoft.UI.ColorHelper.FromArgb(255, 106, 227, 249);
                        nativeTitleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
                        nativeTitleBar.ButtonForegroundColor = defaultColor;
                        nativeTitleBar.ButtonHoverBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(60, defaultColor.R, defaultColor.G, defaultColor.B);
                        nativeTitleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.White;
                        nativeTitleBar.ButtonPressedBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(110, defaultColor.R, defaultColor.G, defaultColor.B);
                        nativeTitleBar.ButtonPressedForegroundColor = Microsoft.UI.Colors.White;
                        nativeTitleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
                        nativeTitleBar.ButtonInactiveForegroundColor = Microsoft.UI.ColorHelper.FromArgb(120, defaultColor.R, defaultColor.G, defaultColor.B);
                    }
                }
            }
            catch { }
        }

        AppEvents.ProfileThemeChanged += (isEnabled, btnColor, iconColor) =>
        {
            isThemeActive = isEnabled;
            themeButtonColorHex = btnColor;
            themeIconColorHex = iconColor;

            Application.Current?.Dispatcher.Dispatch(ApplyTitleBarTheme);
        };

        window.Created += (s, e) =>
        {
            try
            {
                var winuiWin = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
                if (winuiWin != null)
                {
                    currentAppWindow = GetAppWindow(winuiWin);
                    if (currentAppWindow != null)
                    {
                        currentAppWindow.SetIcon("icon.ico");
                        ApplyTitleBarTheme();
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

            bool startInBackground = Config.Get("StartInBackground", "false") == "true";
            string[] args = Environment.GetCommandLineArgs();
            bool hasBackgroundFlag = args.Contains("--background");

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
                        appWindow.Move(new Windows.Graphics.PointInt32(-32000, -32000));
                        appWindow.Hide();
                    }
                }
            }
        };

        bool isFirstActivation = true;
        window.Activated += (s, e) =>
        {
            if (isFirstActivation)
            {
                isFirstActivation = false;
                bool startInBackground = Config.Get("StartInBackground", "false") == "true";
                string[] args = Environment.GetCommandLineArgs();
                bool hasBackgroundFlag = args.Contains("--background");

                if (startInBackground && hasBackgroundFlag)
                {
                    var winuiWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
                    if (winuiWindow != null)
                    {
                        var appWindow = GetAppWindow(winuiWindow);
                        if (appWindow != null)
                        {
                            appWindow.Hide();
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
