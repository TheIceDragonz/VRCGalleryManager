using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VRCGalleryManager.Core.Api.Models
{
    public class VRChatFile
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("ownerId")]
        public string OwnerId { get; set; } = "";

        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = "";

        [JsonPropertyName("extension")]
        public string Extension { get; set; } = "";

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();

        [JsonPropertyName("frames")]
        public int Frames { get; set; }

        [JsonPropertyName("framesOverTime")]
        public int FramesOverTime { get; set; }

        [JsonPropertyName("animationStyle")]
        public string? AnimationStyle { get; set; }

        [JsonPropertyName("maskTag")]
        public string? MaskTag { get; set; }
    }
}
