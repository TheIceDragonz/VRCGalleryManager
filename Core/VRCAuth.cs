using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using VRCGalleryManager.Core.Api;
using VRCGalleryManager.Core.Api.Models;

namespace VRCGalleryManager.Core
{
    public enum VRCAuthStatus
    {
        Success,
        RequiresEmail2FA,
        RequiresApp2FA,
        Error
    }

    public class VRCAuth
    {
        private static VRCAuth? instance;

        public VRChatApiClient ApiClient { get; }
        public AuthConfigShim Config { get; }
        public VRCAuth AuthApi => this;

        public bool LoggedIn = false;
        public bool CookieLoaded = false;
        public CurrentUser? CurrentUser { get; set; }
        public event Action? OnAuthStateChanged;

        public bool Is2FARequired = false;
        public bool IsEmail2FA = false;
        public event Action? On2FARequiredEvent;
        public void Notify2FARequired() => On2FARequiredEvent?.Invoke();

        public string LastErrorMessage { get; private set; } = "";
        public string CookieHeader => ApiClient.CookieHeader;

        private Task? _initTask;

        public Task EnsureInitializedAsync()
        {
            if (_initTask == null)
            {
                _initTask = LoadCookiesAsync();
            }
            return _initTask;
        }

        private VRCAuth()
        {
            ApiClient = new VRChatApiClient();
            Config = new AuthConfigShim(ApiClient);

            ApiClient.OnCookiesUpdated += () =>
            {
                _ = SaveCookiesAsync();
            };

            _initTask = LoadCookiesAsync();
        }

        public static VRCAuth Instance()
        {
            if (instance == null) instance = new VRCAuth();
            return instance;
        }

        public async Task<VRCAuthStatus> LoginAsync(string usernameVRC, string passwordVRC, bool isManualLogin = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usernameVRC) || string.IsNullOrWhiteSpace(passwordVRC))
                {
                    LastErrorMessage = "Username and password cannot be empty.";
                    return VRCAuthStatus.Error;
                }

                var (user, raw, statusCode) = await ApiClient.LoginRawAsync(usernameVRC.Trim(), passwordVRC, clearTwoFactorCookie: isManualLogin);

                try
                {
                    using var doc = JsonDocument.Parse(raw);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("requiresTwoFactorAuth", out var twoFactorProp) && twoFactorProp.ValueKind == JsonValueKind.Array)
                    {
                        var factors = twoFactorProp.EnumerateArray()
                            .Select(x => x.GetString() ?? "")
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

                        Is2FARequired = true;

                        if (factors.Contains("emailOtp") && !factors.Contains("totp") && !factors.Contains("otp"))
                        {
                            IsEmail2FA = true;
                            return VRCAuthStatus.RequiresEmail2FA;
                        }
                        else
                        {
                            IsEmail2FA = false;
                            return VRCAuthStatus.RequiresApp2FA;
                        }
                    }

                    if (statusCode >= 400 || root.TryGetProperty("error", out _))
                    {
                        LastErrorMessage = VRChatApiClient.ExtractErrorMessage(raw, statusCode == 401 ? "Invalid username or password." : $"Login failed (HTTP {statusCode}).");
                        return VRCAuthStatus.Error;
                    }
                }
                catch (JsonException)
                {
                    if (raw.Contains("emailOtp"))
                    {
                        Is2FARequired = true;
                        IsEmail2FA = true;
                        return VRCAuthStatus.RequiresEmail2FA;
                    }
                    else if (raw.Contains("totp") || raw.Contains("otp"))
                    {
                        Is2FARequired = true;
                        IsEmail2FA = false;
                        return VRCAuthStatus.RequiresApp2FA;
                    }
                }

                if (statusCode >= 400)
                {
                    LastErrorMessage = VRChatApiClient.ExtractErrorMessage(raw, statusCode == 401 ? "Invalid username or password." : $"Login failed (HTTP {statusCode}).");
                    return VRCAuthStatus.Error;
                }

                if (user == null || string.IsNullOrEmpty(user.Id))
                {
                    LastErrorMessage = "Authentication failed: invalid user data received.";
                    return VRCAuthStatus.Error;
                }

                await SaveCookiesAsync();
                LoggedIn = true;
                CookieLoaded = true;
                Is2FARequired = false;
                CurrentUser = user;
                Console.WriteLine("Logged in as: {0}", CurrentUser.DisplayName);

                OnAuthStateChanged?.Invoke();
                return VRCAuthStatus.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine("API Error: {0}", ex.Message);
                LastErrorMessage = ex.Message;
                return VRCAuthStatus.Error;
            }
        }

        public static bool CheckHasVRCPlus(CurrentUser? user)
        {
            if (user == null) return false;

            if (user.Badges != null)
            {
                foreach (var badge in user.Badges)
                {
                    if (badge.BadgeId == "bdg_754f9935-0f97-49d8-b857-95afb9b673fa" ||
                        (!string.IsNullOrEmpty(badge.BadgeName) && (badge.BadgeName.Contains("Plus", StringComparison.OrdinalIgnoreCase) || badge.BadgeName.Contains("Supporter", StringComparison.OrdinalIgnoreCase))) ||
                        (!string.IsNullOrEmpty(badge.BadgeId) && (badge.BadgeId.Contains("supporter", StringComparison.OrdinalIgnoreCase) || badge.BadgeId.Contains("vrcplus", StringComparison.OrdinalIgnoreCase))))
                    {
                        return true;
                    }
                }
            }

            if (user.Tags != null)
            {
                if (user.Tags.Any(t => t.Equals("system_supporter", StringComparison.OrdinalIgnoreCase) ||
                                      t.Contains("supporter", StringComparison.OrdinalIgnoreCase) ||
                                      t.Contains("vrc_plus", StringComparison.OrdinalIgnoreCase) ||
                                      t.Contains("vrcplus", StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasVRCPlus => CurrentUser != null && CheckHasVRCPlus(CurrentUser);

        public async Task<VRCAuthStatus> Verify2FAAsync(string code, bool isEmail)
        {
            try
            {
                string cleanCode = (code ?? "").Trim().Replace(" ", "").Replace("-", "");
                if (string.IsNullOrEmpty(cleanCode))
                {
                    LastErrorMessage = "2FA code cannot be empty.";
                    return VRCAuthStatus.Error;
                }

                bool isVerified = false;
                if (isEmail)
                {
                    isVerified = await ApiClient.VerifyEmail2FAAsync(cleanCode);
                }
                else
                {
                    isVerified = await ApiClient.VerifyTotp2FAAsync(cleanCode);
                }

                if (!isVerified)
                {
                    LastErrorMessage = "Invalid 2FA code.";
                    return VRCAuthStatus.Error;
                }

                CurrentUser? user = null;
                try
                {
                    user = await ApiClient.GetCurrentUserAsync();
                }
                catch
                {
                    await Task.Delay(300);
                    user = await ApiClient.GetCurrentUserAsync();
                }

                if (user == null || string.IsNullOrEmpty(user.Id))
                {
                    LastErrorMessage = "Authentication failed: unable to fetch user profile.";
                    return VRCAuthStatus.Error;
                }

                await SaveCookiesAsync();
                LoggedIn = true;
                CookieLoaded = true;
                CurrentUser = user;
                Is2FARequired = false;
                Console.WriteLine("Logged in as: {0}", CurrentUser.DisplayName);

                OnAuthStateChanged?.Invoke();
                return VRCAuthStatus.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine("2FA Verification Error: {0}", ex.Message);
                LastErrorMessage = ex.Message;
                return VRCAuthStatus.Error;
            }
        }

        public async Task<CurrentUser> GetCurrentUserAsync()
        {
            var user = await ApiClient.GetCurrentUserAsync();
            CurrentUser = user;
            return user;
        }

        public async Task SaveCookiesAsync()
        {
            try
            {
                var cookieString = ApiClient.CookieHeader;
                if (!string.IsNullOrEmpty(cookieString))
                {
                    await SecureStorage.Default.SetAsync("auth_cookie", cookieString);
                    CookieLoaded = true;
                    Console.WriteLine("Session cookies saved securely.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save error: {ex.Message}");
            }
        }

        public void SaveCookies()
        {
            _ = SaveCookiesAsync();
        }

        public async Task<bool> LoadCookiesAsync()
        {
            try
            {
                string cookieString = await SecureStorage.Default.GetAsync("auth_cookie");

                if (!string.IsNullOrEmpty(cookieString))
                {
                    ApiClient.SetCookieHeader(cookieString);
                    CookieLoaded = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load error: {ex.Message}");
                CookieLoaded = false;
            }
            return false;
        }

        public void LoadCookies()
        {
            _ = LoadCookiesAsync();
        }

        public async Task SaveCredentialsAsync(string username, string password)
        {
            try
            {
                if (!string.IsNullOrEmpty(username))
                {
                    await SecureStorage.Default.SetAsync("auth_username", username);
                }
                if (!string.IsNullOrEmpty(password))
                {
                    await SecureStorage.Default.SetAsync("auth_password", password);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save credentials error: {ex.Message}");
            }
        }

        public void SaveCredentials(string username, string password)
        {
            _ = SaveCredentialsAsync(username, password);
        }

        public async Task<(string username, string password)?> LoadCredentialsAsync()
        {
            try
            {
                string username = await SecureStorage.Default.GetAsync("auth_username");
                string password = await SecureStorage.Default.GetAsync("auth_password");

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    return (username, password);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load credentials error: {ex.Message}");
            }
            return null;
        }

        public (string username, string password)? LoadCredentials()
        {
            try
            {
                string username = Task.Run(async () => await SecureStorage.Default.GetAsync("auth_username")).GetAwaiter().GetResult();
                string password = Task.Run(async () => await SecureStorage.Default.GetAsync("auth_password")).GetAwaiter().GetResult();

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    return (username, password);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load credentials error: {ex.Message}");
            }
            return null;
        }

        public void ClearAuthCookieOnly()
        {
            ApiClient.ClearAuthCookieOnly();
        }

        private readonly SemaphoreSlim _reloginSemaphore = new(1, 1);
        private DateTime _lastReloginAttempt = DateTime.MinValue;

        public async Task<bool> TryAutoReloginAsync()
        {
            var creds = await LoadCredentialsAsync();
            if (creds == null) return false;

            await _reloginSemaphore.WaitAsync();
            try
            {
                if (LoggedIn && (DateTime.UtcNow - _lastReloginAttempt).TotalSeconds < 5)
                {
                    return true;
                }

                _lastReloginAttempt = DateTime.UtcNow;

                // Clear only auth cookie, preserving twoFactorAuth cookie so 2FA may be bypassed
                ClearAuthCookieOnly();

                var status = await LoginAsync(creds.Value.username, creds.Value.password, isManualLogin: false);
                if (status == VRCAuthStatus.Success)
                {
                    Is2FARequired = false;
                    return true;
                }
                else if (status == VRCAuthStatus.RequiresEmail2FA || status == VRCAuthStatus.RequiresApp2FA)
                {
                    Is2FARequired = true;
                    IsEmail2FA = status == VRCAuthStatus.RequiresEmail2FA;
                    Notify2FARequired();
                    return false;
                }
            }
            catch
            {
            }
            finally
            {
                _reloginSemaphore.Release();
            }
            return false;
        }

        public async Task LogoutAsync(bool keepCredentials = false)
        {
            try
            {
                await ApiClient.LogoutAsync();
            }
            catch { }

            Logout(keepCredentials);
        }

        public void Logout(bool keepCredentials = false)
        {
            SecureStorage.Default.Remove("auth_cookie");

            if (!keepCredentials)
            {
                SecureStorage.Default.Remove("auth_username");
                SecureStorage.Default.Remove("auth_password");
            }

            LoggedIn = false;
            CookieLoaded = false;
            CurrentUser = null;
            Is2FARequired = false;

            ApiClient.ClearCookies();

            OnAuthStateChanged?.Invoke();
        }

        public void NotifyAuthStateChanged()
        {
            OnAuthStateChanged?.Invoke();
        }

        public class AuthConfigShim
        {
            private readonly VRChatApiClient _client;
            public AuthConfigShim(VRChatApiClient client) => _client = client;

            public string BasePath => VRChatApiClient.DefaultBaseUrl;
            public string UserAgent { get; set; } = VRChatApiClient.DefaultUserAgent;

            public Dictionary<string, string> DefaultHeaders
            {
                get
                {
                    var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    var ch = _client.CookieHeader;
                    if (!string.IsNullOrEmpty(ch))
                    {
                        dict["Cookie"] = ch;
                    }
                    return dict;
                }
            }
        }
    }
}