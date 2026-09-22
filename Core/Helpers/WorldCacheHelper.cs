using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VRCGalleryManager.Core.Api;
using VRCGalleryManager.Core.Api.Models;

namespace VRCGalleryManager.Core.Helpers
{
    public class CachedWorldInfo
    {
        [JsonPropertyName("worldId")]
        public string WorldId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("authorName")]
        public string AuthorName { get; set; } = string.Empty;

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [JsonPropertyName("thumbnailImageUrl")]
        public string ThumbnailImageUrl { get; set; } = string.Empty;

        [JsonPropertyName("cachedAtUtc")]
        public DateTime CachedAtUtc { get; set; } = DateTime.UtcNow;
    }

    public static class WorldCacheHelper
    {
        private static readonly ConcurrentDictionary<string, CachedWorldInfo> _cache = new(StringComparer.OrdinalIgnoreCase);
        private static bool _isLoaded = false;
        private static readonly object _fileLock = new();

        private static string BaseFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "VRCGalleryManager");

        private static string DbFilePath => Path.Combine(BaseFolder, "worlds_info_db.json");
        private static string LegacyCacheFilePath => Path.Combine(BaseFolder, "world_info_cache.json");

        public static void EnsureLoaded()
        {
            if (_isLoaded) return;
            lock (_fileLock)
            {
                if (_isLoaded) return;
                try
                {
                    string path = DbFilePath;
                    if (!File.Exists(path) && File.Exists(LegacyCacheFilePath))
                    {
                        try
                        {
                            File.Move(LegacyCacheFilePath, path, true);
                        }
                        catch { }
                    }

                    if (File.Exists(path))
                    {
                        var json = File.ReadAllText(path);
                        var list = JsonSerializer.Deserialize<List<CachedWorldInfo>>(json);
                        if (list != null)
                        {
                            foreach (var item in list)
                            {
                                if (!string.IsNullOrEmpty(item.WorldId))
                                {
                                    _cache[item.WorldId] = item;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[WorldCacheHelper] Failed to load cache: {ex.Message}");
                }
                finally
                {
                    _isLoaded = true;
                }
            }
        }

        public static void SaveCache()
        {
            try
            {
                string path = DbFilePath;
                string dir = Path.GetDirectoryName(path)!;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var list = new List<CachedWorldInfo>(_cache.Values);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(list, options);

                lock (_fileLock)
                {
                    File.WriteAllText(path, json);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WorldCacheHelper] Failed to save cache: {ex.Message}");
            }
        }

        public static CachedWorldInfo? Get(string worldId)
        {
            if (string.IsNullOrEmpty(worldId)) return null;
            EnsureLoaded();
            if (_cache.TryGetValue(worldId, out var info))
            {
                return info;
            }
            return null;
        }

        public static void Put(CachedWorldInfo info)
        {
            if (string.IsNullOrEmpty(info?.WorldId)) return;
            EnsureLoaded();
            _cache[info.WorldId] = info;
        }

        public static async Task<CachedWorldInfo?> FetchAndCacheAsync(string worldId, VRChatApiClient apiClient, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(worldId) || !worldId.StartsWith("wrld_", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            EnsureLoaded();
            if (_cache.TryGetValue(worldId, out var cached) &&
                (!string.IsNullOrEmpty(cached.AuthorName) || cached.ImageUrl == "unavailable") &&
                !string.IsNullOrEmpty(cached.ImageUrl) &&
                !cached.ImageUrl.Contains("/1/500"))
            {
                return cached;
            }

            try
            {
                var vrcWorld = await apiClient.GetWorldAsync(worldId, ct).ConfigureAwait(false);
                if (vrcWorld != null)
                {
                    string bestImageUrl = ResolveBestWorldImageUrl(vrcWorld.ImageUrl, vrcWorld.ThumbnailImageUrl);
                    var entry = new CachedWorldInfo
                    {
                        WorldId = worldId,
                        Name = !string.IsNullOrEmpty(vrcWorld.Name) ? vrcWorld.Name : (cached?.Name ?? ""),
                        AuthorName = !string.IsNullOrEmpty(vrcWorld.AuthorName) ? vrcWorld.AuthorName : (cached?.AuthorName ?? ""),
                        ImageUrl = !string.IsNullOrEmpty(bestImageUrl) ? bestImageUrl : "unavailable",
                        ThumbnailImageUrl = vrcWorld.ThumbnailImageUrl ?? "",
                        CachedAtUtc = DateTime.UtcNow
                    };

                    _cache[worldId] = entry;
                    SaveCache();
                    return entry;
                }
                else
                {
                    var notFoundEntry = new CachedWorldInfo
                    {
                        WorldId = worldId,
                        Name = cached?.Name ?? "",
                        AuthorName = cached?.AuthorName ?? "",
                        ImageUrl = "unavailable",
                        CachedAtUtc = DateTime.UtcNow
                    };
                    _cache[worldId] = notFoundEntry;
                    SaveCache();
                    return notFoundEntry;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WorldCacheHelper] Failed to fetch world {worldId}: {ex.Message}");
                var failedEntry = new CachedWorldInfo
                {
                    WorldId = worldId,
                    Name = cached?.Name ?? "",
                    AuthorName = cached?.AuthorName ?? "",
                    ImageUrl = "unavailable",
                    CachedAtUtc = DateTime.UtcNow
                };
                _cache[worldId] = failedEntry;
                SaveCache();
                return failedEntry;
            }

            return cached;
        }

        public static string ResolveBestWorldImageUrl(string? imageUrl, string? thumbnailImageUrl)
        {
            // Pick the latest image URL from VRChat (prefer imageUrl as primary)
            string sourceUrl = !string.IsNullOrEmpty(imageUrl) ? imageUrl : (thumbnailImageUrl ?? "");
            if (string.IsNullOrEmpty(sourceUrl)) return string.Empty;

            // Extract fileId and version (e.g. /file_xxx/4/file or /file_xxx/4/256)
            var match = Regex.Match(sourceUrl, @"(file_[a-f0-9\-]+)(?:/(\d+))?", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string fileId = match.Groups[1].Value;
                string version = "1";
                if (match.Groups[2].Success && !string.IsNullOrEmpty(match.Groups[2].Value))
                {
                    version = match.Groups[2].Value;
                }
                else if (!string.IsNullOrEmpty(thumbnailImageUrl))
                {
                    var thumbMatch = Regex.Match(thumbnailImageUrl, @"(file_[a-f0-9\-]+)(?:/(\d+))?", RegexOptions.IgnoreCase);
                    if (thumbMatch.Success && thumbMatch.Groups[2].Success && !string.IsNullOrEmpty(thumbMatch.Groups[2].Value))
                    {
                        version = thumbMatch.Groups[2].Value;
                    }
                }

                return $"https://api.vrchat.cloud/api/1/image/{fileId}/{version}/500";
            }

            return sourceUrl;
        }
    }
}
