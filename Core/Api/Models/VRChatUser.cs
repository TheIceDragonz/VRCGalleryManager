using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VRCGalleryManager.Core.Api.Models
{
    public class VRChatUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = "";

        private string? _userIcon;
        [JsonPropertyName("userIcon")]
        public string? UserIcon
        {
            get => !string.IsNullOrEmpty(_userIcon) ? _userIcon : IconUrl;
            set => _userIcon = value;
        }

        [JsonPropertyName("iconUrl")]
        public string? IconUrl { get; set; }

        private string? _profilePicOverride;
        [JsonPropertyName("profilePicOverride")]
        public string? ProfilePicOverride
        {
            get => !string.IsNullOrEmpty(_profilePicOverride) ? _profilePicOverride : BannerUrl;
            set => _profilePicOverride = value;
        }

        [JsonPropertyName("bannerUrl")]
        public string? BannerUrl { get; set; }

        [JsonPropertyName("bannerType")]
        public string? BannerType { get; set; }

        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

        [JsonPropertyName("bioLinks")]
        public List<string>? BioLinks { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("statusDescription")]
        public string? StatusDescription { get; set; }

        [JsonPropertyName("tags")]
        public List<string>? Tags { get; set; }

        [JsonPropertyName("pronouns")]
        public string? Pronouns { get; set; }
    }

    public class User : VRChatUser
    {
    }
}
