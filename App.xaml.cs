namespace VRCGalleryManager;

public partial class App : Application
{
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
#endif
        return window;
    }
}
