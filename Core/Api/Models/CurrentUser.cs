using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VRCGalleryManager.Core.Api.Models
{
    public class CurrentUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = "";

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("userIcon")]
        public string UserIcon { get; set; } = "";

        [JsonPropertyName("profilePicOverride")]
        public string ProfilePicOverride { get; set; } = "";

        [JsonPropertyName("profilePicOverrideThumbnail")]
        public string ProfilePicOverrideThumbnail { get; set; } = "";

        [JsonPropertyName("currentAvatarThumbnailImageUrl")]
        public string CurrentAvatarThumbnailImageUrl { get; set; } = "";

        [JsonPropertyName("currentAvatarImageUrl")]
        public string CurrentAvatarImageUrl { get; set; } = "";

        [JsonPropertyName("profileEffect")]
        public string? ProfileEffect { get; set; }

        [JsonPropertyName("iconFrame")]
        public string? IconFrame { get; set; }

        [JsonPropertyName("badges")]
        public List<Badge> Badges { get; set; } = new();

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();

        [JsonPropertyName("friends")]
        public List<string> Friends { get; set; } = new();

        [JsonPropertyName("acceptedTOSVersion")]
        public int AcceptedTOSVersion { get; set; }

        [JsonPropertyName("bio")]
        public string Bio { get; set; } = "";

        [JsonPropertyName("bioLinks")]
        public List<string> BioLinks { get; set; } = new();

        [JsonPropertyName("status")]
        public string Status { get; set; } = "";

        [JsonPropertyName("statusDescription")]
        public string StatusDescription { get; set; } = "";

        [JsonPropertyName("pronouns")]
        public string Pronouns { get; set; } = "";

        [JsonPropertyName("requiresTwoFactorAuth")]
        public List<string>? RequiresTwoFactorAuth { get; set; }
    }
}
