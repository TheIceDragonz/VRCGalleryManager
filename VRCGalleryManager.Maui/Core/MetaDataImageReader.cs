using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace VRCGalleryManager.Core
{
    static public class MetaDataImageReader
    {
        public class VrcxData
        {
            [JsonPropertyName("author")]
            public AuthorInfo Author { get; set; }
            [JsonPropertyName("world")]
            public WorldInfo World { get; set; }
            [JsonPropertyName("players")]
            public List<PlayerInfo> Players { get; set; }
        }

        public class AuthorInfo
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }
            [JsonPropertyName("displayName")]
            public string DisplayName { get; set; }
        }

        public class WorldInfo
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("id")]
            public string Id { get; set; }
            [JsonPropertyName("instanceId")]
            public string InstanceId { get; set; }
            [JsonPropertyName("imageUrl")]
            public string ImageUrl { get; set; }
        }

        public class PlayerInfo
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }
            [JsonPropertyName("displayName")]
            public string DisplayName { get; set; }
        }

        public static VrcxData? ExtractVrcxData(string filePath)
        {
            try
            {
                var bytes = File.ReadAllBytes(filePath);
                int idx = Array.IndexOf(bytes, (byte)'{');
                if (idx < 0) return null;

                var reader = new Utf8JsonReader(bytes.AsSpan(idx), isFinalBlock: true, state: default);
                using var doc = JsonDocument.ParseValue(ref reader);

                string raw = doc.RootElement.GetRawText();
                if (raw.Contains("\"application\":\"VRCX\"") || raw.Contains("\"application\": \"VRCX\""))
                {
                    return JsonSerializer.Deserialize<VrcxData>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
