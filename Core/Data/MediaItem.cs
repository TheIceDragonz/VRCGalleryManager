using System;
using System.Collections.Generic;

namespace VRCGalleryManager.Core.Data
{
    public class MediaItem
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Ups { get; set; }
        public int NumComments { get; set; }
        public string Permalink { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string? MediaUrl { get; set; }
        public string? Mp4PreviewUrl { get; set; }
        public string? StaticPreviewUrl { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double CreatedUtc { get; set; }
        public bool IsOver18 { get; set; }
        public bool HasMedia => !string.IsNullOrEmpty(MediaUrl);
    }

    public class MediaSearchResult
    {
        public List<MediaItem> Items { get; set; } = new();
        public string? After { get; set; }
        public int Dist { get; set; }
    }
}
