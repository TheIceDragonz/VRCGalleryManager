using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using VRCGalleryManager.Core;

namespace VRCGalleryManager;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
		{
			try
			{
				string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "fatal.log");
				System.IO.File.AppendAllText(path, eventArgs.ExceptionObject.ToString() + "\n\n");
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
        builder.Services.AddSingleton<ApiRequest>();
        builder.Services.AddSingleton<MediaCacheService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
