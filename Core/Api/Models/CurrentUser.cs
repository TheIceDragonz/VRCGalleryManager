using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace VRCGalleryManager.Core.Api.Models
{
    public class UserLanguageItem
    {
        public string Code { get; set; } = "";
        public string DisplayName { get; set; } = "";
    }

    public class JsonLanguage
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = "";

        [JsonPropertyName("value")]
        public string Value { get; set; } = "";
    }

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

        [JsonPropertyName("$isVRCPlus")]
        public bool? IsVRCPlus { get; set; }

        [JsonPropertyName("$languages")]
        public List<JsonLanguage>? JsonLanguages { get; set; }

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

        [JsonPropertyName("ageVerificationStatus")]
        public string? AgeVerificationStatus { get; set; }

        [JsonPropertyName("ageVerified")]
        public bool AgeVerified { get; set; }

        [JsonPropertyName("isEconomyCreator")]
        public bool IsEconomyCreator { get; set; }

        [JsonPropertyName("twoFactorAuthEnabled")]
        public bool TwoFactorAuthEnabled { get; set; }

        [JsonPropertyName("emailVerified")]
        public bool EmailVerified { get; set; }

        [JsonPropertyName("developerType")]
        public string? DeveloperType { get; set; }

        [JsonIgnore]
        public string TrustLevel
        {
            get
            {
                if (Tags == null || Tags.Count == 0) return "Visitor";
                if (Tags.Contains("system_trust_veteran")) return "Trusted User";
                if (Tags.Contains("system_trust_trusted")) return "Known User";
                if (Tags.Contains("system_trust_known")) return "User";
                if (Tags.Contains("system_trust_basic")) return "New User";
                return "Visitor";
            }
        }

        [JsonIgnore]
        public string TrustClass
        {
            get
            {
                return TrustLevel switch
                {
                    "Trusted User" => "pill-trust-veteran",
                    "Known User" => "pill-trust-trusted",
                    "User" => "pill-trust-known",
                    "New User" => "pill-trust-basic",
                    _ => "pill-trust-visitor"
                };
            }
        }

        private static readonly Dictionary<string, string> KnownLanguageNames = new(StringComparer.OrdinalIgnoreCase)
        {
            { "ITA", "Italiano" },
            { "RON", "Română" },
            { "ENG", "English" },
            { "FRA", "Français" },
            { "DEU", "Deutsch" },
            { "SPA", "Español" },
            { "ESP", "Español" },
            { "POR", "Português" },
            { "JPN", "日本語" },
            { "KOR", "한국어" },
            { "ZHO", "中文" },
            { "CHI", "中文" },
            { "RUS", "Русский" },
            { "POL", "Polski" },
            { "NLD", "Nederlands" },
            { "SWE", "Svenska" },
            { "UKR", "Українська" },
            { "VIE", "Tiếng Việt" },
            { "THA", "ไทย" },
            { "IND", "Bahasa Indonesia" },
            { "ARA", "العربية" },
            { "TUR", "Türkçe" },
            { "FIN", "Suomi" },
            { "NOR", "Norsk" },
            { "DAN", "Dansk" },
            { "CES", "Čeština" },
            { "HUN", "Magyar" },
            { "ELL", "Ελληνικά" },
            { "HEB", "עברית" },
            { "HIN", "हिन्दी" },
            { "FIL", "Filipino" },
            { "ZXX", "No linguistic content" }
        };

        [JsonIgnore]
        public List<UserLanguageItem> Languages
        {
            get
            {
                var list = new List<UserLanguageItem>();
                if (JsonLanguages != null && JsonLanguages.Count > 0)
                {
                    foreach (var jl in JsonLanguages)
                    {
                        if (string.IsNullOrEmpty(jl.Key)) continue;
                        var upper = jl.Key.ToUpperInvariant();
                        list.Add(new UserLanguageItem
                        {
                            Code = upper,
                            DisplayName = !string.IsNullOrEmpty(jl.Value) ? jl.Value : (KnownLanguageNames.TryGetValue(upper, out var n) ? n : upper)
                        });
                    }
                }
                else if (Tags != null)
                {
                    foreach (var t in Tags)
                    {
                        if (t.StartsWith("language_", StringComparison.OrdinalIgnoreCase))
                        {
                            var code = t.Substring("language_".Length);
                            if (string.IsNullOrEmpty(code)) continue;
                            var upper = code.ToUpperInvariant();
                            list.Add(new UserLanguageItem
                            {
                                Code = upper,
                                DisplayName = KnownLanguageNames.TryGetValue(upper, out var n) ? n : upper
                            });
                        }
                    }
                }
                return list;
            }
        }
    }
}
