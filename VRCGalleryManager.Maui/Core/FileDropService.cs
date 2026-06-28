using System;

namespace VRCGalleryManager.Maui.Core;

public class FileDropService
{
    public event Action<string[]>? OnFileDropped;
    public event Action? OnDragEnter;
    public event Action? OnDragLeave;

    public bool IsDragDropEnabled => OnFileDropped != null;

    public void NotifyFileDropped(string[] filePaths)
    {
        OnFileDropped?.Invoke(filePaths);
    }

    public void NotifyDragEnter()
    {
        OnDragEnter?.Invoke();
    }

    public void NotifyDragLeave()
    {
        OnDragLeave?.Invoke();
    }
}
