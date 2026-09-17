using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VRCGalleryManager.Core.Api.Models
{
    public class UpdateProfileRequest
    {
        [JsonPropertyName("userIcon")]
        public string? UserIcon { get; set; }

        [JsonPropertyName("bannerType")]
        public string? BannerType { get; set; }

        [JsonPropertyName("bannerColor")]
        public string? BannerColor { get; set; }

        [JsonPropertyName("bannerCustomUrl")]
        public string? BannerCustomUrl { get; set; }

        [JsonPropertyName("profilePicOverride")]
        public string? ProfilePicOverride { get; set; }

        [JsonPropertyName("bannerUrl")]
        public string? BannerUrl { get; set; }

        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

        [JsonPropertyName("bioLinks")]
        public List<string>? BioLinks { get; set; }

        [JsonPropertyName("backgroundType")]
        public string? BackgroundType { get; set; }

        [JsonPropertyName("backgroundTextureId")]
        public string? BackgroundTextureId { get; set; }

        [JsonPropertyName("themeId")]
        public string? ThemeId { get; set; }

        [JsonPropertyName("iconFrame")]
        public string? IconFrame { get; set; }

        [JsonPropertyName("nameplateEffect")]
        public string? NameplateEffect { get; set; }

        [JsonPropertyName("profileEffect")]
        public string? ProfileEffect { get; set; }

        [JsonPropertyName("languages")]
        public List<string>? Languages { get; set; }
    }
}
