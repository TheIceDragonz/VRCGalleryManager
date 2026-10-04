using System;
using System.Threading;
using System.Threading.Tasks;
using VRCGalleryManager.Core.Data;

namespace VRCGalleryManager.Core
{
    public interface IMediaProviderService : IDisposable
    {
        Task<MediaSearchResult> SearchGifsAsync(
            string query,
            string sort = "relevance",
            string timeRange = "all",
            string? after = null,
            int limit = 40,
            bool includeNsfw = false,
            CancellationToken cancellationToken = default);

        Task<MediaSearchResult> SearchStickersAsync(
            string query,
            string sort = "relevance",
            string timeRange = "all",
            string? after = null,
            int limit = 40,
            bool includeNsfw = false,
            CancellationToken cancellationToken = default);

        Task<string> DownloadMediaToTempFileAsync(MediaItem item, CancellationToken cancellationToken = default);
    }
}
