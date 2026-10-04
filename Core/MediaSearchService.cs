using System;
using System.Threading.Tasks;

namespace VRCGalleryManager.Core
{
    public enum MediaSearchMode
    {
        Gif,
        Image,
        Sticker = Image
    }

    public class MediaSearchOptions
    {
        public string Title { get; set; } = "Search Media";
        public string InitialQuery { get; set; } = "";
        public MediaSearchMode Mode { get; set; } = MediaSearchMode.Gif;
        public Func<string, Task>? OnMediaSelected { get; set; }
        public Action? OnClosed { get; set; }
    }

    public class MediaSearchService
    {
        public event Action<MediaSearchOptions>? OnOpen;
        public event Action? OnClose;

        public bool IsOpen { get; private set; }

        public void Open(Func<string, Task> onMediaSelected, string title = "Search Media", string initialQuery = "", MediaSearchMode mode = MediaSearchMode.Gif)
        {
            var options = new MediaSearchOptions
            {
                Title = title,
                InitialQuery = initialQuery,
                Mode = mode,
                OnMediaSelected = onMediaSelected
            };
            Open(options);
        }

        public void Open(MediaSearchOptions options)
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
