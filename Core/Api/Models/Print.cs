using System;
using System.Text.Json.Serialization;

namespace VRCGalleryManager.Core.Api.Models
{
    public class PrintFiles
    {
        [JsonPropertyName("fileId")]
        public string? FileId { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }
    }

    public class Print
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("authorId")]
        public string? AuthorId { get; set; }

        [JsonPropertyName("authorName")]
        public string? AuthorName { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("files")]
        public PrintFiles? Files { get; set; }

        [JsonPropertyName("note")]
        public string? Note { get; set; }

        [JsonPropertyName("ownerId")]
        public string? OwnerId { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("worldId")]
        public string? WorldId { get; set; }

        [JsonPropertyName("worldName")]
        public string? WorldName { get; set; }
    }
}
