using System.Text.Json.Serialization;

namespace VRCGalleryManager.Core.Api.Models
{
    public class VRChatWorld
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("authorId")]
        public string AuthorId { get; set; } = "";

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; } = "";

        [JsonPropertyName("thumbnailImageUrl")]
        public string ThumbnailImageUrl { get; set; } = "";
    }

    public class World : VRChatWorld
    {
    }
}
