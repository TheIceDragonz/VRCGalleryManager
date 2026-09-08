using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace VRCGalleryManager.Core
{
    public static class Config
    {
        private static readonly string Folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "VRCGalleryManager");
        private static readonly string PathConfigJson = Path.Combine(Folder, "config.json");
        private static Dictionary<string, string> _settings;

        static Config()
        {
            LoadAllSettings();
        }

        private static void EnsureFolderExists()
        {
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);
        }

        private static void LoadAllSettings()
        {
            try
            {
                if (File.Exists(PathConfigJson))
                {
                    var options = new JsonSerializerOptions
                    {
                        AllowTrailingCommas = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        PropertyNameCaseInsensitive = true
                    };
                    var json = File.ReadAllText(PathConfigJson);
                    _settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json, options)
                                ?? new Dictionary<string, string>();
                }
                else
                {
                    _settings = new Dictionary<string, string>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading config.json: {ex.Message}");
                _settings = new Dictionary<string, string>();
            }
        }

        private static void SaveAllSettings()
        {
            EnsureFolderExists();
            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(_settings, options);
            File.WriteAllText(PathConfigJson, jsonString);
        }

        public static void Set(string key, string value)
        {
            _settings[key] = value;
            SaveAllSettings();
        }

        public static string Get(string key, string defaultValue = "")
        {
            return _settings.TryGetValue(key, out var value)
                 ? value
                 : defaultValue;
        }

        public static string GetPath(string key, string defaultValue = "")
        {
            var raw = Get(key, defaultValue);
            if (string.IsNullOrWhiteSpace(raw))
                return raw;

            return raw.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }
    }
}
