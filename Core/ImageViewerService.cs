using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VRCGalleryManager.Components;

namespace VRCGalleryManager.Core
{
    public class ImageViewerOptions
    {
        public IReadOnlyList<ImageViewer.ViewerItem> Items { get; set; } = Array.Empty<ImageViewer.ViewerItem>();
        public int StartIndex { get; set; } = 0;
        public bool ShowDownloadButton { get; set; } = true;
        public bool ShowInfoButton { get; set; } = false;
        public bool? ShowUploadButton { get; set; }
        public bool? ShowDeleteButton { get; set; }
        public Func<int, Task>? OnDeleteRequested { get; set; }
        public Func<int, Task>? OnUploadRequested { get; set; }
        public Action? OnClosed { get; set; }
    }

    public class ImageViewerService
    {
        public event Action<ImageViewerOptions>? OnOpen;
        public event Action? OnClose;

        public bool IsOpen { get; private set; }

        public void Open(ImageViewerOptions options)
        {
            IsOpen = true;
            OnOpen?.Invoke(options);
        }

        public void Close()
        {
            IsOpen = false;
            OnClose?.Invoke();
        }

        internal void NotifyClosed()
        {
            IsOpen = false;
        }
    }
}
