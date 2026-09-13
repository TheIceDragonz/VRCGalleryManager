using System;

namespace VRCGalleryManager.Core;

public class FileDropService
{
    public event Action<string[]>? OnFileDropped;
    public event Action? OnDragEnter;
    public event Action? OnDragLeave;

    public bool IsDropAllowed { get; set; } = true;

    public bool IsDragDropEnabled => OnFileDropped != null && IsDropAllowed;

    public void NotifyFileDropped(string[] filePaths)
    {
        if (!IsDragDropEnabled) return;
        OnFileDropped?.Invoke(filePaths);
    }

    public void NotifyDragEnter()
    {
        if (!IsDragDropEnabled) return;
        OnDragEnter?.Invoke();
    }

    public void NotifyDragLeave()
    {
        OnDragLeave?.Invoke();
    }
}
