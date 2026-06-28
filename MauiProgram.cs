using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core;

namespace VRCGalleryManager;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
		{
			try
			{
				System.IO.File.AppendAllText("crash.log", eventArgs.Exception.ToString() + "\n\n");
			}
			catch { }
		};

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<VRCAuth>(sp => VRCAuth.Instance());
        builder.Services.AddSingleton<NotificationService>();
        builder.Services.AddSingleton<DialogService>();
        builder.Services.AddSingleton<UpdateManager>();
        builder.Services.AddSingleton<FileDropService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
