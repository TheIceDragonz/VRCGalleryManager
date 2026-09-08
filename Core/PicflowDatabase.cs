using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace VRCGalleryManager.Core
{
    public class CachedItem
    {
        public string ItemId { get; set; } = "";
        public string Username { get; set; } = "";
        public string UserId { get; set; } = "";
        public string Url { get; set; } = "";
        public int CategoryInt { get; set; }
    }

    public static class PicflowDatabase
    {
        private static readonly string Folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "VRCGalleryManager");
        private static readonly string PathDbJson = Path.Combine(Folder, "picflow_db.json");
        
        private static Dictionary<string, CachedItem> _db = new Dictionary<string, CachedItem>();
        private static readonly object _lock = new object();
        private static System.Threading.Timer? _debounceTimer;
        private static readonly object _saveLock = new object();
        private static bool _isDirty = false;

        static PicflowDatabase()
        {
            Load();
        }

        private static void EnsureFolderExists()
        {
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);
        }

        public static void Load()
        {
            lock (_lock)
            {
                try
                {
                    if (File.Exists(PathDbJson))
                    {
                        var json = File.ReadAllText(PathDbJson);
                        _db = JsonSerializer.Deserialize<Dictionary<string, CachedItem>>(json) ?? new Dictionary<string, CachedItem>();
                    }
                    else
                    {
                        _db = new Dictionary<string, CachedItem>();
                    }
                }
                catch
                {
                    _db = new Dictionary<string, CachedItem>();
                }
            }
        }

        public static void ScheduleSave()
        {
            lock (_saveLock)
            {
                _isDirty = true;
                _debounceTimer?.Dispose();
                _debounceTimer = new System.Threading.Timer(_ => Flush(), null, 500, System.Threading.Timeout.Infinite);
            }
        }

        public static void Flush()
        {
            lock (_saveLock)
            {
                if (!_isDirty) return;
                _isDirty = false;
                _debounceTimer?.Dispose();
                _debounceTimer = null;
            }
            Save();
        }

        public static void Save()
        {
            lock (_lock)
            {
                try
                {
                    EnsureFolderExists();
                    var options = new JsonSerializerOptions { WriteIndented = true }; 
                    var jsonString = JsonSerializer.Serialize(_db, options);
                    File.WriteAllText(PathDbJson, jsonString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error saving Picflow DB: " + ex.Message);
                }
            }
        }

        public static CachedItem GetItem(string itemId)
        {
            lock (_lock)
            {
                if (_db.TryGetValue(itemId, out var item))
                {
                    return item;
                }
                return null;
            }
        }

        public static void SaveItem(string itemId, string username, string userId, string url, int category)
        {
            bool isNewOrUpdated = false;
            lock (_lock)
            {
                if (!_db.ContainsKey(itemId) || _db[itemId].Url != url)
                {
                    _db[itemId] = new CachedItem
                    {
                        ItemId = itemId,
                        Username = username,
                        UserId = userId,
                        Url = url,
                        CategoryInt = category
                    };
                    isNewOrUpdated = true;
                }
            }

            if (isNewOrUpdated)
            {
                ScheduleSave();
            }
        }

        public static Dictionary<string, CachedItem> GetAll()
        {
            lock (_lock)
            {
                return new Dictionary<string, CachedItem>(_db);
            }
        }

        public static void Clear()
        {
            lock (_lock)
            {
                _db.Clear();
            }
            Flush();
        }
    }
}
