using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Buffers;
using VRCGalleryManager.Core.Helpers;

namespace VRCGalleryManager.Core
{
    static public class MetaDataImageReader
    {
        public class PhotoMetadata
        {
            [JsonPropertyName("application")]
            public string Application { get; set; } = "VRCGalleryManager";

            [JsonPropertyName("version")]
            public int Version { get; set; } = 1;

            [JsonPropertyName("author")]
            public AuthorInfo Author { get; set; } = new();

            [JsonPropertyName("world")]
            public WorldInfo World { get; set; } = new();

            [JsonPropertyName("players")]
            public List<PlayerInfo> Players { get; set; } = new();
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

        public static PhotoMetadata? ExtractPhotoMetadata(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return null;

            // Fast path: clean iTXt chunk parser
            try
            {
                string? itxtJson = PngMetadataWriter.ReadTextChunk(filePath, "Description");
                if (!string.IsNullOrEmpty(itxtJson))
                {
                    var parsed = JsonSerializer.Deserialize<PhotoMetadata>(itxtJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (parsed != null) return parsed;
                }
            }
            catch { }

            // Fallback path: span-based byte scanner for modded / legacy files
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                
                int maxRead = (int)Math.Min(stream.Length, 512 * 1024);
                byte[] buffer = ArrayPool<byte>.Shared.Rent(maxRead);

                try
                {
                    int bytesRead = stream.Read(buffer, 0, maxRead);
                    var data = TryParsePhotoMetadataJson(buffer.AsSpan(0, bytesRead));
                    if (data != null) return data;

                    // If not found in header and file is larger, check tail
                    if (stream.Length > maxRead)
                    {
                        int tailSize = (int)Math.Min(stream.Length - maxRead, 64 * 1024);
                        stream.Seek(-tailSize, SeekOrigin.End);
                        int tailRead = stream.Read(buffer, 0, tailSize);
                        data = TryParsePhotoMetadataJson(buffer.AsSpan(0, tailRead));
                        if (data != null) return data;
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static bool InjectMetadata(string filePath, PhotoMetadata data)
        {
            return PngMetadataWriter.InjectPhotoMetadata(filePath, data);
        }

        private static PhotoMetadata? TryParsePhotoMetadataJson(ReadOnlySpan<byte> span)
        {
            int searchStart = 0;
            ReadOnlySpan<byte> vrcxMarker = "VRCX"u8;
            ReadOnlySpan<byte> gmMarker = "VRCGalleryManager"u8;
            ReadOnlySpan<byte> appMarker = "application"u8;

            while (searchStart < span.Length)
            {
                int braceIdx = span.Slice(searchStart).IndexOf((byte)'{');
                if (braceIdx < 0) break;

                int jsonStart = searchStart + braceIdx;
                
                if (span.Slice(jsonStart).IndexOf(vrcxMarker) < 0 && 
                    span.Slice(jsonStart).IndexOf(gmMarker) < 0 &&
                    span.Slice(jsonStart).IndexOf(appMarker) < 0)
                {
                    break;
                }

                try
                {
                    var reader = new Utf8JsonReader(span.Slice(jsonStart), isFinalBlock: false, state: default);
                    using var doc = JsonDocument.ParseValue(ref reader);
                    string raw = doc.RootElement.GetRawText();
                    if (raw.Contains("\"application\"", StringComparison.OrdinalIgnoreCase))
                    {
                        return JsonSerializer.Deserialize<PhotoMetadata>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                }
                catch
                {
                    // Continue search if JSON was malformed at this point
                }

                searchStart = jsonStart + 1;
            }

            return null;
        }
    }
}

