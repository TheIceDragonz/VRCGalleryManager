using System;

namespace VRCGalleryManager.Core
{
    public static class AppEvents
    {
        /// <summary>
        /// Request navigation to Settings page inside Blazor.
        /// </summary>
        public static event Action<bool>? RequestNavigateToSettings;

        public static void NavigateToSettings(bool promptUpdate = false)
        {
            RequestNavigateToSettings?.Invoke(promptUpdate);
        }

        /// <summary>
        /// Request showing the update install confirmation dialog.
        /// </summary>
        public static event Action? RequestPromptUpdate;

        public static void PromptUpdate()
        {
            RequestPromptUpdate?.Invoke();
        }

        /// <summary>
        /// Raised when update availability status is determined.
        /// </summary>
        public static event Action<bool, string>? UpdateStatusChanged;

        public static void NotifyUpdateStatus(bool isAvailable, string latestVersion)
        {
            UpdateStatusChanged?.Invoke(isAvailable, latestVersion);
        }

        /// <summary>
        /// Raised when VRChat Profile Theme changes or is toggled.
        /// (bool isEnabled, string? buttonColor, string? iconColor)
        /// </summary>
        public static event Action<bool, string?, string?>? ProfileThemeChanged;

        public static void NotifyProfileThemeChanged(bool isEnabled, string? buttonColor, string? iconColor)
        {
            ProfileThemeChanged?.Invoke(isEnabled, buttonColor, iconColor);
        }
    }
}
