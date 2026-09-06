using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VRCGalleryManager.Core
{
    public class ProfileBackgroundItem
    {
        public string Id { get; set; } = "";
        public string Label { get; set; } = "";
        public string Url { get; set; } = "";
        public string Thumbnail { get; set; } = "";
        public bool IsVRCPlus { get; set; }
    }

    public class PublicProfileBadge
    {
        [JsonPropertyName("badgeId")]
        public string? BadgeId { get; set; }

        [JsonPropertyName("badgeName")]
        public string? BadgeName { get; set; }

        [JsonPropertyName("badgeDescription")]
        public string? BadgeDescription { get; set; }

        [JsonPropertyName("badgeImageUrl")]
        public string? BadgeImageUrl { get; set; }

        [JsonPropertyName("showcased")]
        public bool Showcased { get; set; }
    }

    public class VRChatPublicProfile
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = "";

        [JsonPropertyName("userIcon")]
        public string? UserIcon { get; set; }

        [JsonPropertyName("iconUrl")]
        public string? IconUrl { get; set; }

        [JsonPropertyName("currentAvatarImageUrl")]
        public string? CurrentAvatarImageUrl { get; set; }

        [JsonPropertyName("currentAvatarThumbnailImageUrl")]
        public string? CurrentAvatarThumbnailImageUrl { get; set; }

        [JsonPropertyName("profilePicOverride")]
        public string? ProfilePicOverride { get; set; }

        [JsonPropertyName("profilePicOverrideThumbnail")]
        public string? ProfilePicOverrideThumbnail { get; set; }

        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

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

        [JsonPropertyName("bannerColor")]
        public string? BannerColor { get; set; }

        [JsonPropertyName("bannerType")]
        public string? BannerType { get; set; }

        [JsonPropertyName("badges")]
        public List<PublicProfileBadge>? Badges { get; set; }
    }

    public class VRCProfileTheme
    {
        public string ButtonColor { get; set; } = "#6ae3f9";
        public string IconColor { get; set; } = "#6ae3f9";
        public string SubtextColor { get; set; } = "#8b949e";
        public string? BannerColor { get; set; }
        public bool IsCustomTheme { get; set; }
    }

    public class ProfileBackgroundService
    {
        private readonly VRCAuth _auth;
        private readonly HttpClient _httpClient;
        private readonly ConcurrentDictionary<string, (VRChatPublicProfile Profile, DateTime Expiry)> _profileCache = new();

        public event Action? OnSettingsChanged;

        public static readonly IReadOnlyList<ProfileBackgroundItem> BackgroundCatalog = new List<ProfileBackgroundItem>
        {
            new() { Id = "grid", Label = "Grid", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Grid.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Grid_Thumb.png", IsVRCPlus = false },
            new() { Id = "filigree", Label = "Filigree", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Cascade.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Cascade_Thumb.png", IsVRCPlus = true },
            new() { Id = "bit-mountain", Label = "Bit Mountain", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_GRID_BITS.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_GRID_BITS_Thumb.png", IsVRCPlus = true },
            new() { Id = "approach", Label = "Approach", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Approach.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Approach_Thumb.png", IsVRCPlus = true },
            new() { Id = "planet-fall", Label = "Planet Fall", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_PlanetFall.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_PlanetFall_Thumb.png", IsVRCPlus = true },
            new() { Id = "jungle", Label = "Jungle", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Jungle.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Jungle_Thumb.png", IsVRCPlus = true },
            new() { Id = "light-streams", Label = "Light Streams", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_LightStream.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_LightStream_Thumb.png", IsVRCPlus = true },
            new() { Id = "koi", Label = "Koi", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/Koi_Layer.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/Koi_Layer_Thumb.png", IsVRCPlus = true },
            new() { Id = "moonlight", Label = "Moonlight", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Cloud.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Cloud_Thumb.png", IsVRCPlus = true },
            new() { Id = "machine", Label = "Machine", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/TechWallLayer.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/TechWallLayer_Thumb.png", IsVRCPlus = true },
            new() { Id = "topology", Label = "Topology", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/TOPOLOGY.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/TOPOLOGY_Thumb.png", IsVRCPlus = true },
            new() { Id = "circuit", Label = "Circuit", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Circuit.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Circuit_Thumb.png", IsVRCPlus = true },
            new() { Id = "kawaii-cupcakes", Label = "Kawaii Cupcakes", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Kawaii_cupcakes.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Kawaii_cupcakes_Thumb.png", IsVRCPlus = true },
            new() { Id = "bokeh", Label = "Bokeh", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Bokeh.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Bokeh_Thumb.png", IsVRCPlus = true },
            new() { Id = "hologram", Label = "Hologram", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Shatter.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Shatter_Thumb.png", IsVRCPlus = true },
            new() { Id = "flow", Label = "Flow", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Flow_Horizontal.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Flow_Horizontal_Thumb.png", IsVRCPlus = true },
            new() { Id = "jelly", Label = "Jelly", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Jelly.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Jelly_Thumb.png", IsVRCPlus = true },
            new() { Id = "cheese", Label = "Cheese", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Cheese.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Cheese_Thumb.png", IsVRCPlus = true },
            new() { Id = "alchemy", Label = "Alchemy", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Alchemy.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Alchemy_Thumb.png", IsVRCPlus = true },
            new() { Id = "sunset-mountain", Label = "Sunset Mountain", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_MountainSun.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_MountainSun_Thumb.png", IsVRCPlus = true },
            new() { Id = "magic-rocks", Label = "Magic Rocks", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_MagicRocks.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_MagicRocks_Thumb.png", IsVRCPlus = true },
            new() { Id = "city-stillness", Label = "City Stillness", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_FutureCityStillness.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_FutureCityStillness_Thumb.png", IsVRCPlus = true },
            new() { Id = "music", Label = "Music", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Music.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Music_Thumb.png", IsVRCPlus = true },
            new() { Id = "wolf-running", Label = "Wolf Running", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_WolfRunning.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_WolfRunning_Thumb.png", IsVRCPlus = true },
            new() { Id = "dragon-smoke", Label = "Dragon Smoke", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_DragonSmoke.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_DragonSmoke_Thumb.png", IsVRCPlus = true },
            new() { Id = "night-rain", Label = "Night Rain", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_NightRain.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_NightRain_Thumb.png", IsVRCPlus = true },
            new() { Id = "butterfly-zero", Label = "Butterfly Zero", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_ButterFlyZERO.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_ButterFlyZERO_Thumb.png", IsVRCPlus = true },
            new() { Id = "i-am-speed", Label = "I Am Speed", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/void.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/void_Thumb.png", IsVRCPlus = true },
            new() { Id = "ronin", Label = "Ronin", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Ronin.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Ronin_Thumb.png", IsVRCPlus = true },
            new() { Id = "abduction", Label = "Abduction", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Abduction.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Abduction_Thumb.png", IsVRCPlus = true },
            new() { Id = "cat-dream", Label = "Cat Dream", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_CatDream.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_CatDream_Thumb.png", IsVRCPlus = true },
            new() { Id = "virtual-waves", Label = "Virtual Waves", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_VirtualWaves.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_VirtualWaves_Thumb.png", IsVRCPlus = true },
            new() { Id = "bolt-and-snail", Label = "Bolt and Snail", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_BoltNSnail.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_BoltNSnail_Thumb.png", IsVRCPlus = true },
            new() { Id = "escape", Label = "Escape", Url = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Escape.png", Thumbnail = "https://assets.vrchat.com/www/profile_decorations/profile_backgrounds/BG_Escape_Thumb.png", IsVRCPlus = true }
        };

        public ProfileBackgroundService(VRCAuth auth)
        {
            _auth = auth;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.vrchat.cloud/api/1/")
            };
        }

        private VRChatPublicProfile? _currentProfile;
        public string CurrentSidebarBackgroundStyle { get; private set; } = "";
        public string CurrentThemeCss { get; private set; } = "";
        public VRCProfileTheme CurrentTheme { get; private set; } = new();
        public event Action? OnSidebarBackgroundChanged;

        public bool DisplayVRCProfileBackgrounds
        {
            get => Config.Get("DisplayVRCProfileBackgrounds", "false") == "true";
            set
            {
                Config.Set("DisplayVRCProfileBackgrounds", value ? "true" : "false");
                OnSettingsChanged?.Invoke();
                OnSidebarBackgroundChanged?.Invoke();
            }
        }

        public bool DisplayVRCCosmetics
        {
            get => Config.Get("DisplayVRCCosmetics", "true") == "true";
            set
            {
                Config.Set("DisplayVRCCosmetics", value ? "true" : "false");
                OnSettingsChanged?.Invoke();
            }
        }

        public bool DisplayVRCProfileThemes
        {
            get => Config.Get("DisplayVRCProfileThemes", "false") == "true";
            set
            {
                Config.Set("DisplayVRCProfileThemes", value ? "true" : "false");
                RefreshTheme();
                OnSettingsChanged?.Invoke();
                OnSidebarBackgroundChanged?.Invoke();
            }
        }

        public VRCProfileTheme GetProfileTheme(VRChatPublicProfile? profile, bool isDarkMode = true)
        {
            var theme = new VRCProfileTheme();
            if (profile == null || !DisplayVRCProfileThemes)
            {
                return theme;
            }

            bool hasCustom = false;

            if (!string.IsNullOrEmpty(profile.ThemeButtonColor))
            {
                theme.ButtonColor = GetReadableProfileThemeColor(profile.ThemeButtonColor, "#6ae3f9", isDarkMode);
                hasCustom = true;
            }

            if (!string.IsNullOrEmpty(profile.ThemeIconColor))
            {
                theme.IconColor = GetReadableProfileThemeColor(profile.ThemeIconColor, "#6ae3f9", isDarkMode);
                hasCustom = true;
            }

            if (!string.IsNullOrEmpty(profile.ThemeSubtextColor))
            {
                theme.SubtextColor = GetReadableProfileThemeColor(profile.ThemeSubtextColor, "#8b949e", isDarkMode);
                hasCustom = true;
            }

            if (!string.IsNullOrEmpty(profile.BannerColor))
            {
                theme.BannerColor = NormalizeProfileHex(profile.BannerColor);
            }

            theme.IsCustomTheme = hasCustom;
            return theme;
        }

        public void UpdateSidebarBackground(VRChatPublicProfile? profile)
        {
            _currentProfile = profile;
            CurrentSidebarBackgroundStyle = GetBackgroundCssStyle(profile, isDarkMode: true);
            RefreshTheme();
            OnSidebarBackgroundChanged?.Invoke();
        }

        public (double r, double g, double b) IconColorMatrix { get; private set; }
        public (double r, double g, double b) ButtonColorMatrix { get; private set; }

        public void RefreshTheme()
        {
            var profile = _currentProfile;
            if (profile == null && _auth.CurrentUser != null && _profileCache.TryGetValue(_auth.CurrentUser.Id, out var cached))
            {
                profile = cached.Profile;
                _currentProfile = profile;
            }

            CurrentTheme = GetProfileTheme(profile, isDarkMode: true);

            string btnColor = CurrentTheme.ButtonColor;
            string iconColor = string.IsNullOrEmpty(CurrentTheme.IconColor) ? btnColor : CurrentTheme.IconColor;

            var btnRgb = HexToRgb(btnColor) ?? (106, 227, 249);
            var iconRgb = HexToRgb(iconColor) ?? btnRgb;

            ButtonColorMatrix = (iconRgb.r / 255.0, iconRgb.g / 255.0, iconRgb.b / 255.0);
            IconColorMatrix = (iconRgb.r / 255.0, iconRgb.g / 255.0, iconRgb.b / 255.0);

            CurrentThemeCss = GenerateGlobalThemeCss(CurrentTheme);
        }

        public string GenerateGlobalThemeCss(VRCProfileTheme theme)
        {
            if (!DisplayVRCProfileThemes || !theme.IsCustomTheme)
            {
                return "";
            }

            string btnColor = theme.ButtonColor;
            string iconColor = string.IsNullOrEmpty(theme.IconColor) ? btnColor : theme.IconColor;
            string subtextColor = theme.SubtextColor;

            var rgb = HexToRgb(btnColor) ?? (106, 227, 249);
            (byte r, byte g, byte b) darkBase = (7, 10, 14);

            // Harmonized dark solid background (100% opaque, styled like #07242B with the theme tint)
            string btnBg = RgbToHex(MixRgb(darkBase, rgb, 0.14));
            string btnBorder = RgbToHex(MixRgb(darkBase, rgb, 0.28));
            string btnHoverBg = RgbToHex(MixRgb(darkBase, rgb, 0.22));
            string btnHoverBorder = RgbToHex(MixRgb(darkBase, rgb, 0.50));
            string btnActiveBg = RgbToHex(MixRgb(darkBase, rgb, 0.26));
            string focusShadow = $"rgba({rgb.r}, {rgb.g}, {rgb.b}, 0.35)";
            string switchKnobOff = $"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='-4 -4 8 8'%3e%3ccircle r='3' fill='rgba({rgb.r},{rgb.g},{rgb.b},0.55)'/%3e%3c/svg%3e";
            string switchKnobOffHover = $"data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='-4 -4 8 8'%3e%3ccircle r='3' fill='rgba({rgb.r},{rgb.g},{rgb.b},0.85)'/%3e%3c/svg%3e";
            string switchKnobOn = "data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='-4 -4 8 8'%3e%3ccircle r='3' fill='%23ffffff'/%3e%3c/svg%3e";

            return $@"
                :root {{
                    --accent: {iconColor} !important;
                    --accent-hover: {iconColor} !important;
                    --text-muted: {subtextColor} !important;
                    --filter-accent: url(#theme-icon-filter) !important;
                    --theme-rgb: {rgb.r}, {rgb.g}, {rgb.b};
                    --theme-btn-bg: {btnBg};
                    --theme-btn-border: {btnBorder};
                    --theme-btn-hover-bg: {btnHoverBg};
                    --theme-btn-hover-border: {btnHoverBorder};
                    --theme-btn-active-bg: {btnActiveBg};
                    --theme-focus-shadow: {focusShadow};
                    --theme-switch-knob-off: url(""{switchKnobOff}"");
                    --theme-switch-knob-off-hover: url(""{switchKnobOffHover}"");
                    --theme-switch-knob-on: url(""{switchKnobOn}"");
                }}
            ";
        }

        public static string HexToCssFilter(string hex)
        {
            var rgbOpt = HexToRgb(hex);
            if (rgbOpt == null) return "var(--filter-accent)";
            var rgb = rgbOpt.Value;

            double r = rgb.r / 255.0;
            double g = rgb.g / 255.0;
            double b = rgb.b / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double h = 0;
            if (delta > 0.00001)
            {
                if (Math.Abs(max - r) < 0.00001)
                    h = 60.0 * (((g - b) / delta) % 6.0);
                else if (Math.Abs(max - g) < 0.00001)
                    h = 60.0 * (((b - r) / delta) + 2.0);
                else
                    h = 60.0 * (((r - g) / delta) + 4.0);

                if (h < 0) h += 360.0;
            }

            double l = (max + min) / 2.0;
            double s = delta == 0 ? 0 : delta / (1.0 - Math.Abs(2.0 * l - 1.0));

            int invertPct = (int)Math.Round(l * 100);
            int sepiaPct = 100;
            int satPct = (int)Math.Round(Math.Clamp(s * 1200, 300, 3000));
            int hueDeg = (int)Math.Round(h - 40.0);
            if (hueDeg < 0) hueDeg += 360;
            int brightPct = (int)Math.Round(Math.Clamp(l * 125, 80, 200));

            return $"invert({invertPct}%) sepia({sepiaPct}%) saturate({satPct}%) hue-rotate({hueDeg}deg) brightness({brightPct}%)";
        }

        public ProfileBackgroundItem? GetBackground(string? textureId)
        {
            if (string.IsNullOrEmpty(textureId)) return null;
            for (int i = 0; i < BackgroundCatalog.Count; i++)
            {
                if (string.Equals(BackgroundCatalog[i].Id, textureId, StringComparison.OrdinalIgnoreCase))
                {
                    return BackgroundCatalog[i];
                }
            }
            return null;
        }

        public async Task<VRChatPublicProfile?> GetPublicProfileAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;

            if (_profileCache.TryGetValue(userId, out var cached) && cached.Expiry > DateTime.UtcNow)
            {
                return cached.Profile;
            }

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"profile/{userId}");
                request.Headers.TryAddWithoutValidation("User-Agent", Api.VRChatApiClient.DefaultUserAgent);
                if (!string.IsNullOrEmpty(_auth.CookieHeader))
                {
                    request.Headers.TryAddWithoutValidation("Cookie", _auth.CookieHeader);
                }

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var profile = JsonSerializer.Deserialize<VRChatPublicProfile>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (profile != null)
                    {
                        _profileCache[userId] = (profile, DateTime.UtcNow.AddMinutes(10));
                        return profile;
                    }
                }
            }
            catch
            {
                // Fallback / ignore network failures
            }

            return null;
        }

        public string GetBackgroundCssStyle(VRChatPublicProfile? profile, bool isDarkMode = true)
        {
            if (profile == null || !DisplayVRCProfileBackgrounds)
            {
                return "";
            }

            if (string.Equals(profile.BackgroundType, "gradient", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(profile.BackgroundGradientTop) &&
                !string.IsNullOrEmpty(profile.BackgroundGradientBottom))
            {
                string topColor = GetReadableProfileThemeColor(profile.BackgroundGradientTop, "#1f2937", isDarkMode);
                string bottomColor = GetReadableProfileThemeColor(profile.BackgroundGradientBottom, "#111827", isDarkMode);

                return $"background-image: linear-gradient(180deg, {topColor}, {bottomColor}); background-size: cover;";
            }

            if (string.Equals(profile.BackgroundType, "texture", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(profile.BackgroundTextureId))
            {
                var bg = GetBackground(profile.BackgroundTextureId);
                if (bg != null && !string.IsNullOrEmpty(bg.Url))
                {
                    return $"background-image: url('{bg.Url}'); background-size: cover; background-position: top center; background-repeat: no-repeat;";
                }
            }

            return "";
        }

        #region Color Readability Adaptation

        private const double DarkMinLuminance = 0.06;
        private const double LightMaxLuminance = 0.74;

        public static string GetReadableProfileThemeColor(string colorValue, string fallback, bool isDarkMode = true)
        {
            string? normalized = NormalizeProfileHex(colorValue);
            if (normalized == null) return fallback;

            var rgb = HexToRgb(normalized);
            if (rgb == null) return fallback;

            double luminanceThreshold = isDarkMode ? DarkMinLuminance : LightMaxLuminance;
            double luminance = GetRelativeLuminance(rgb.Value);
            bool requiresAdjustment = isDarkMode ? luminance < luminanceThreshold : luminance > luminanceThreshold;

            if (!requiresAdjustment)
            {
                return normalized;
            }

            (byte r, byte g, byte b) targetRgb = isDarkMode ? ((byte)255, (byte)255, (byte)255) : ((byte)0, (byte)0, (byte)0);
            var currentRgb = rgb.Value;

            for (int i = 0; i < 14; i++)
            {
                currentRgb = MixRgb(currentRgb, targetRgb, 0.18);
                luminance = GetRelativeLuminance(currentRgb);
                if ((isDarkMode && luminance >= luminanceThreshold) || (!isDarkMode && luminance <= luminanceThreshold))
                {
                    break;
                }
            }

            return RgbToHex(currentRgb);
        }

        public static string? NormalizeProfileHex(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            string hex = value.Trim().TrimStart('#').ToLowerInvariant();
            if (!Regex.IsMatch(hex, "^[0-9a-f]{6}$"))
            {
                return null;
            }
            return $"#{hex}";
        }

        private static (byte r, byte g, byte b)? HexToRgb(string hex)
        {
            string clean = hex.TrimStart('#');
            if (clean.Length != 6) return null;

            if (byte.TryParse(clean.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte r) &&
                byte.TryParse(clean.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte g) &&
                byte.TryParse(clean.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte b))
            {
                return (r, g, b);
            }
            return null;
        }

        private static string RgbToHex((byte r, byte g, byte b) rgb)
        {
            return $"#{rgb.r:x2}{rgb.g:x2}{rgb.b:x2}";
        }

        private static double GetRelativeLuminance((byte r, byte g, byte b) rgb)
        {
            static double Channel(byte value)
            {
                double normalized = value / 255.0;
                return normalized <= 0.03928
                    ? normalized / 12.92
                    : Math.Pow((normalized + 0.055) / 1.055, 2.4);
            }

            double r = Channel(rgb.r);
            double g = Channel(rgb.g);
            double b = Channel(rgb.b);

            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        private static (byte r, byte g, byte b) MixRgb((byte r, byte g, byte b) from, (byte r, byte g, byte b) to, double weight)
        {
            byte r = (byte)Math.Clamp(Math.Round(from.r + (to.r - from.r) * weight), 0, 255);
            byte g = (byte)Math.Clamp(Math.Round(from.g + (to.g - from.g) * weight), 0, 255);
            byte b = (byte)Math.Clamp(Math.Round(from.b + (to.b - from.b) * weight), 0, 255);
            return (r, g, b);
        }

        #endregion
    }
}
