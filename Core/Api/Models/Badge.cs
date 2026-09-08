using System;
using System.Text.Json.Serialization;

namespace VRCGalleryManager.Core.Api.Models
{
    public class Badge
    {
        [JsonPropertyName("badgeId")]
        public string BadgeId { get; set; } = "";

        [JsonPropertyName("badgeName")]
        public string BadgeName { get; set; } = "";

        [JsonPropertyName("badgeImageUrl")]
        public string BadgeImageUrl { get; set; } = "";

        [JsonPropertyName("badgeDescription")]
        public string BadgeDescription { get; set; } = "";

        [JsonPropertyName("showcased")]
        public bool Showcased { get; set; }

        [JsonPropertyName("hidden")]
        public bool? Hidden { get; set; }

        [JsonPropertyName("assignedAt")]
        public DateTime? AssignedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}
