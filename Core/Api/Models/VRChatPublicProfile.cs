using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VRCGalleryManager.Core.Api.Models
{
    public class PublicProfileBadge : Badge
    {
    }

    public class VRChatPublicProfile
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

        [JsonPropertyName("iconType")]
        public string? IconType { get; set; }

        [JsonPropertyName("currentAvatarImageUrl")]
        public string? CurrentAvatarImageUrl { get; set; }

        [JsonPropertyName("currentAvatarThumbnailImageUrl")]
        public string? CurrentAvatarThumbnailImageUrl { get; set; }

        [JsonPropertyName("currentAvatarName")]
        public string? CurrentAvatarName { get; set; }

        [JsonPropertyName("currentAvatarAuthorName")]
        public string? CurrentAvatarAuthorName { get; set; }

        private string? _bannerCustomUrl;
        [JsonPropertyName("bannerCustomUrl")]
        public string? BannerCustomUrl
        {
            get => !string.IsNullOrEmpty(_bannerCustomUrl) ? _bannerCustomUrl : (!string.IsNullOrEmpty(_profilePicOverride) ? _profilePicOverride : BannerUrl);
            set => _bannerCustomUrl = value;
        }

        private string? _profilePicOverride;
        [JsonPropertyName("profilePicOverride")]
        public string? ProfilePicOverride
        {
            get => !string.IsNullOrEmpty(_profilePicOverride) ? _profilePicOverride : (BannerCustomUrl ?? BannerUrl);
            set => _profilePicOverride = value;
        }

        private string? _profilePicOverrideThumbnail;
        [JsonPropertyName("profilePicOverrideThumbnail")]
        public string? ProfilePicOverrideThumbnail
        {
            get => !string.IsNullOrEmpty(_profilePicOverrideThumbnail) ? _profilePicOverrideThumbnail : (BannerCustomUrl ?? BannerUrl ?? ProfilePicOverride);
            set => _profilePicOverrideThumbnail = value;
        }

        [JsonPropertyName("bannerUrl")]
        public string? BannerUrl { get; set; }

        [JsonPropertyName("bannerType")]
        public string? BannerType { get; set; }

        [JsonPropertyName("bannerColor")]
        public string? BannerColor { get; set; }

        [JsonPropertyName("nameplateEffect")]
        public string? NameplateEffect { get; set; }

        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

        [JsonPropertyName("bioLinks")]
        public List<string>? BioLinks { get; set; }

        [JsonPropertyName("pronouns")]
        public string? Pronouns { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("statusDescription")]
        public string? StatusDescription { get; set; }

        [JsonPropertyName("backgroundType")]
        public string? BackgroundType { get; set; }

        [JsonPropertyName("backgroundGradientTop")]
        public string? BackgroundGradientTop { get; set; }

        [JsonPropertyName("backgroundGradientBottom")]
        public string? BackgroundGradientBottom { get; set; }

        [JsonPropertyName("backgroundTextureId")]
        public string? BackgroundTextureId { get; set; }

        [JsonPropertyName("backgroundTemplateId")]
        public string? BackgroundTemplateId { get; set; }

        [JsonPropertyName("profileEffect")]
        public string? ProfileEffect { get; set; }

        [JsonPropertyName("iconFrame")]
        public string? IconFrame { get; set; }

        [JsonPropertyName("themeButtonColor")]
        public string? ThemeButtonColor { get; set; }

        [JsonPropertyName("themeIconColor")]
        public string? ThemeIconColor { get; set; }

        [JsonPropertyName("themeSubtextColor")]
        public string? ThemeSubtextColor { get; set; }

        [JsonPropertyName("themeId")]
        public string? ThemeId { get; set; }

        [JsonPropertyName("badges")]
        public List<Badge>? Badges { get; set; }

        [JsonPropertyName("isEconomyCreator")]
        public bool IsEconomyCreator { get; set; }

        [JsonPropertyName("hasVrcPlus")]
        public bool? HasVrcPlus { get; set; }
    }
}
