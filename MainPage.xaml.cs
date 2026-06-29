using VRCGalleryManager.Core;

namespace VRCGalleryManager;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
#if ANDROID
        await RequestAndroidPermissions();
#endif
    }

#if ANDROID
    private async Task RequestAndroidPermissions()
    {
        try
        {
            // 1. First, request standard media permissions
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Photos>();
                if (status != PermissionStatus.Granted)
                {
                    await Permissions.RequestAsync<Permissions.Photos>();
                }
            }
            else
            {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                if (status != PermissionStatus.Granted)
                {
                    await Permissions.RequestAsync<Permissions.StorageRead>();
                }
            }

            // 2. Then, request full file access if needed (Android 11+)
            if (OperatingSystem.IsAndroidVersionAtLeast(30))
            {
                if (!Android.OS.Environment.IsExternalStorageManager)
                {
                    bool answer = await Application.Current.MainPage.DisplayAlert(
                        "Permissions Required", 
                        "To read and display images from your VRChat gallery, the app needs full file access (MANAGE_EXTERNAL_STORAGE). We will redirect you to the settings to enable it.", 
                        "Go to Settings", "Cancel");
                        
                    if (answer)
                    {
                        var intent = new Android.Content.Intent(Android.Provider.Settings.ActionManageAppAllFilesAccessPermission);
                        intent.AddCategory("android.intent.category.DEFAULT");
                        intent.SetData(Android.Net.Uri.Parse($"package:{Android.App.Application.Context.PackageName}"));
                        Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.StartActivity(intent);
                    }
                }
            }
        }
        catch (Exception ex)
        { 
            System.Diagnostics.Debug.WriteLine($"Perm Error: {ex.Message}");
        }
    }
#endif

    private FileDropService GetFileDropService()
    {
        return Handler?.MauiContext?.Services.GetService<FileDropService>() 
            ?? Application.Current?.Windows.FirstOrDefault()?.Page?.Handler?.MauiContext?.Services.GetService<FileDropService>();
    }

    private bool? _isCurrentDragValid = null;

    private async void OnDrop(object sender, DropEventArgs e)
    {
        _isCurrentDragValid = null;
        var service = GetFileDropService();
        if (service == null || !service.IsDragDropEnabled) return;

        var filePaths = new List<string>();

#if WINDOWS
        if (e.PlatformArgs is not null && e.PlatformArgs.DragEventArgs.DataView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
        {
            var items = await e.PlatformArgs.DragEventArgs.DataView.GetStorageItemsAsync();
            foreach (var item in items)
            {
                if (item is Windows.Storage.StorageFile file)
                {
                    filePaths.Add(file.Path);
                }
            }
        }
#endif

        if (filePaths.Count > 0)
        {
            service.NotifyFileDropped(filePaths.ToArray());
        }
        service.NotifyDragLeave();
    }

    private async void OnDragOver(object sender, DragEventArgs e)
    {
        var service = GetFileDropService();
        if (service == null || !service.IsDragDropEnabled)
        {
            e.AcceptedOperation = DataPackageOperation.None;
            return;
        }

#if WINDOWS
        if (_isCurrentDragValid == null)
        {
            _isCurrentDragValid = false;
            try
            {
                if (e.PlatformArgs is not null && e.PlatformArgs.DragEventArgs.DataView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
                {
                    var deferral = e.PlatformArgs.DragEventArgs.GetDeferral();
                    var items = await e.PlatformArgs.DragEventArgs.DataView.GetStorageItemsAsync();
                    bool isValid = false;
                    foreach (var item in items)
                    {
                        if (item is Windows.Storage.StorageFile file)
                        {
                            string ext = file.FileType.ToLower();
                            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".webp" || ext == ".gif")
                            {
                                isValid = true;
                                break;
                            }
                        }
                    }
                    _isCurrentDragValid = isValid;
                    deferral.Complete();
                }
            }
            catch { }
        }

        if (_isCurrentDragValid == true)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            service.NotifyDragEnter();
        }
        else
        {
            e.AcceptedOperation = DataPackageOperation.None;
            service.NotifyDragLeave(); // Hide overlay if invalid format
        }
#else
        e.AcceptedOperation = DataPackageOperation.Copy;
        service.NotifyDragEnter();
#endif
    }

    private void OnDragLeave(object sender, DragEventArgs e)
    {
        _isCurrentDragValid = null;
        var service = GetFileDropService();
        if (service != null && service.IsDragDropEnabled)
        {
            service.NotifyDragLeave();
        }
    }
}
