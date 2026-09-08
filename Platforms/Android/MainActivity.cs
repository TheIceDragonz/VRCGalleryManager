using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;

namespace VRCGalleryManager;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity, IOnApplyWindowInsetsListener
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (Window != null)
        {
            WindowCompat.SetDecorFitsSystemWindows(Window, false);

            if (OperatingSystem.IsAndroidVersionAtLeast(28) && Window.Attributes != null)
            {
                var lp = Window.Attributes;
                lp.LayoutInDisplayCutoutMode = Android.Views.LayoutInDisplayCutoutMode.ShortEdges;
                Window.Attributes = lp;
            }

            Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
            Window.SetNavigationBarColor(Android.Graphics.Color.ParseColor("#0a0b0d"));

            if (OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                Window.StatusBarContrastEnforced = false;
                Window.NavigationBarContrastEnforced = false;
            }

            var contentView = Window.DecorView.FindViewById(Android.Resource.Id.Content);
            if (contentView != null)
            {
                ViewCompat.SetOnApplyWindowInsetsListener(contentView, this);
            }

            var insetsController = WindowCompat.GetInsetsController(Window, Window.DecorView);
            if (insetsController != null)
            {
                insetsController.AppearanceLightStatusBars = false;
                insetsController.AppearanceLightNavigationBars = false;
            }
        }
    }

    public WindowInsetsCompat OnApplyWindowInsets(Android.Views.View v, WindowInsetsCompat insets)
    {
        var navBarInsets = insets.GetInsets(WindowInsetsCompat.Type.NavigationBars());

        int left = navBarInsets.Left;
        int right = navBarInsets.Right;
        int bottom = navBarInsets.Bottom;

        v.SetPadding(left, 0, right, bottom);
        return insets;
    }
}
