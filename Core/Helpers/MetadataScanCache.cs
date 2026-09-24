using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VRCGalleryManager.Core.Helpers
{
    public class CachedPhotoMetadata
    {
        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;

        [JsonPropertyName("lastModified")]
        public DateTime LastModifiedUtc { get; set; }

        [JsonPropertyName("takenAt")]
        public DateTime? TakenAtUtc { get; set; }

        [JsonPropertyName("hasMeta")]
        public bool HasMetadata { get; set; }

        [JsonPropertyName("worldId")]
        public string? WorldId { get; set; }

        [JsonPropertyName("worldName")]
        public string? WorldName { get; set; }

        [JsonPropertyName("authorName")]
        public string? AuthorName { get; set; }
    }

    public static class MetadataScanCache
    {
        private static readonly ConcurrentDictionary<string, CachedPhotoMetadata> _items = new(StringComparer.OrdinalIgnoreCase);
        private static bool _isLoaded = false;
        private static bool _isDirty = false;
        private static readonly object _fileLock = new();

        private static string BaseFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "VRCGalleryManager");

        private static string DbFilePath => Path.Combine(BaseFolder, "metadata_scan_db.json");
        private static string LegacyCacheFilePath => Path.Combine(BaseFolder, "metadata_scan_cache.json");

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
                        var list = JsonSerializer.Deserialize<List<CachedPhotoMetadata>>(json);
                        if (list != null)
                        {
                            foreach (var item in list)
                            {
                                if (!string.IsNullOrEmpty(item.Path))
                                {
                                    _items[item.Path] = item;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[MetadataScanCache] Failed to load db: {ex.Message}");
                }
                finally
                {
                    _isLoaded = true;
                }
            }
        }

        public static bool TryGet(string filePath, out CachedPhotoMetadata? item)
        {
            EnsureLoaded();
            return _items.TryGetValue(filePath, out item);
        }

        public static void Set(string filePath, CachedPhotoMetadata item)
        {
            EnsureLoaded();
            _items[filePath] = item;
            _isDirty = true;
        }

        public static void Remove(string filePath)
        {
            EnsureLoaded();
            if (_items.TryRemove(filePath, out _))
            {
                _isDirty = true;
            }
        }

        public static void Clear()
        {
            EnsureLoaded();
            _items.Clear();
            _isDirty = true;
            Save();
        }

        public static string DatabasePath => DbFilePath;

        public static int Count
        {
            get
            {
                EnsureLoaded();
                return _items.Count;
            }
        }

        public static ICollection<CachedPhotoMetadata> GetAll()
        {
            EnsureLoaded();
            return _items.Values;
        }

        public static List<CachedPhotoMetadata> GetPhotosWithMetadata(string? rootFolder = null)
        {
            EnsureLoaded();
            var query = _items.Values.Where(p => p.HasMetadata);
            if (!string.IsNullOrEmpty(rootFolder))
            {
                string normalizedRoot = Path.GetFullPath(rootFolder).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
                query = query.Where(p =>
                {
                    try
                    {
                        return Path.GetFullPath(p.Path).StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase);
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            return query.ToList();
        }

        public static void SaveIfDirty()
        {
            if (!_isDirty) return;
            Save();
        }

        public static void Save()
        {
            lock (_fileLock)
            {
                try
                {
                    string path = DbFilePath;
                    string dir = Path.GetDirectoryName(path)!;
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    var list = new List<CachedPhotoMetadata>(_items.Values);
                    var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });

                    string tempPath = path + ".tmp";
                    File.WriteAllText(tempPath, json);
                    File.Move(tempPath, path, true);
                    _isDirty = false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[MetadataScanCache] Failed to save db: {ex.Message}");
                }
            }
        }
    }
}
