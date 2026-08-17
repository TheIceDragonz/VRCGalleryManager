using Microsoft.Maui.Storage;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;
using System.Threading;

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
        private static VRCAuth instance;

        public Configuration Config;
        public ApiClient ApiClient;
        public AuthenticationApi AuthApi;

        public bool LoggedIn = false;
        public bool CookieLoaded = false;
        public CurrentUser CurrentUser { get; set; }
        public event Action OnAuthStateChanged;

        public bool Is2FARequired = false;
        public bool IsEmail2FA = false;
        public event Action On2FARequiredEvent;
        public void Notify2FARequired() => On2FARequiredEvent?.Invoke();



        public string LastErrorMessage { get; private set; }

        private VRCAuth()
        {
            Config = new Configuration();
            Config.UserAgent = "VRCGalleryManager";

            ApiClient = new ApiClient(Config.BasePath);

            LoadCookies();

            AuthApi = new AuthenticationApi(ApiClient, ApiClient, Config);
        }

        public static VRCAuth Instance()
        {
            if (instance == null) instance = new VRCAuth();
            return instance;
        }

        public async Task<VRCAuthStatus> LoginAsync(string usernameVRC, string passwordVRC)
        {
            Config.ApiKey.Clear();
            Config.ApiKeyPrefix.Clear();
            Config.DefaultHeaders.Clear();
            Config.UserAgent = "VRCGalleryManager";
            Config.Username = usernameVRC;
            Config.Password = passwordVRC;
            if (AuthApi.Configuration is Configuration authConfig)
            {
                authConfig.ApiKey.Clear();
                authConfig.ApiKeyPrefix.Clear();
                authConfig.DefaultHeaders.Clear();
                authConfig.UserAgent = "VRCGalleryManager";
                authConfig.Username = usernameVRC;
                authConfig.Password = passwordVRC;
            }

            try
            {
                ApiResponse<CurrentUser> currentUserResp = await AuthApi.GetCurrentUserWithHttpInfoAsync();
                ExtractAuthCookie(currentUserResp);

                if (requiresEmail2FA(currentUserResp))
                {
                    return VRCAuthStatus.RequiresEmail2FA;
                }
                else if (currentUserResp.RawContent != null && currentUserResp.RawContent.Contains("totp"))
                {
                    return VRCAuthStatus.RequiresApp2FA;
                }
                else if (currentUserResp.RawContent != null && currentUserResp.RawContent.Contains("\"error\""))
                {
                    LastErrorMessage = ExtractErrorMessage(currentUserResp.RawContent);
                    return VRCAuthStatus.Error;
                }
                else if (currentUserResp.Data == null || string.IsNullOrEmpty(currentUserResp.Data.Id))
                {
                    LastErrorMessage = "Invalid credentials or API error.";
                    return VRCAuthStatus.Error;
                }

                var user = (currentUserResp.Data != null && !string.IsNullOrEmpty(currentUserResp.Data.Id))
                    ? currentUserResp.Data
                    : await AuthApi.GetCurrentUserAsync();
                
                await SaveCookiesAsync();
                LoggedIn = true;
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

        private string ExtractErrorMessage(string rawContent)
        {
            try
            {
                var root = System.Text.Json.JsonDocument.Parse(rawContent);
                if (root.RootElement.TryGetProperty("error", out var errorEl) && errorEl.TryGetProperty("message", out var msgEl))
                {
                    return msgEl.GetString();
                }
            }
            catch { }
            return "Invalid credentials or API error.";
        }

        public static bool CheckHasVRCPlus(CurrentUser user)
        {
            if (user == null) return false;

            if (user.Badges != null)
            {
                foreach (var badge in user.Badges)
                {
                    if (badge.BadgeId == "bdg_754f9935-0f97-49d8-b857-95afb9b673fa" ||
                        (badge.BadgeName != null && (badge.BadgeName.Contains("Plus", StringComparison.OrdinalIgnoreCase) || badge.BadgeName.Contains("Supporter", StringComparison.OrdinalIgnoreCase))) ||
                        (badge.BadgeId != null && (badge.BadgeId.Contains("supporter", StringComparison.OrdinalIgnoreCase) || badge.BadgeId.Contains("vrcplus", StringComparison.OrdinalIgnoreCase))))
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
                string cleanCode = (code ?? "").Trim();
                if (string.IsNullOrEmpty(cleanCode))
                {
                    LastErrorMessage = "2FA code cannot be empty.";
                    return VRCAuthStatus.Error;
                }

                bool isVerified = false;
                if (isEmail)
                {
                    var resp2fa = await AuthApi.Verify2FAEmailCodeWithHttpInfoAsync(new TwoFactorEmailCode(cleanCode));
                    ExtractAuthCookie(resp2fa);
                    if (resp2fa.Data != null && resp2fa.Data.Verified)
                    {
                        isVerified = true;
                    }
                    else if (resp2fa.RawContent != null && resp2fa.RawContent.Contains("\"verified\":true"))
                    {
                        isVerified = true;
                    }
                    else if (resp2fa.RawContent != null && resp2fa.RawContent.Contains("\"error\""))
                    {
                        LastErrorMessage = ExtractErrorMessage(resp2fa.RawContent);
                    }
                    else
                    {
                        LastErrorMessage = "Invalid 2FA email code.";
                    }
                }
                else
                {
                    var resp2fa = await AuthApi.Verify2FAWithHttpInfoAsync(new TwoFactorAuthCode(cleanCode));
                    ExtractAuthCookie(resp2fa);
                    if (resp2fa.Data != null && resp2fa.Data.Verified)
                    {
                        isVerified = true;
                    }
                    else if (resp2fa.RawContent != null && resp2fa.RawContent.Contains("\"verified\":true"))
                    {
                        isVerified = true;
                    }
                    else if (resp2fa.RawContent != null && resp2fa.RawContent.Contains("\"error\""))
                    {
                        LastErrorMessage = ExtractErrorMessage(resp2fa.RawContent);
                    }
                    else
                    {
                        LastErrorMessage = "Invalid 2FA code.";
                    }
                }

                if (!isVerified)
                {
                    return VRCAuthStatus.Error;
                }

                var userResp = await AuthApi.GetCurrentUserWithHttpInfoAsync();
                ExtractAuthCookie(userResp);

                var user = (userResp.Data != null && !string.IsNullOrEmpty(userResp.Data.Id))
                    ? userResp.Data
                    : await AuthApi.GetCurrentUserAsync();

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

        private void ExtractAuthCookie<T>(ApiResponse<T> resp)
        {
            if (resp == null || resp.Headers == null) return;

            var cookiesDict = new Dictionary<string, string>();
            
            if (Config.DefaultHeaders.TryGetValue("Cookie", out string existingCookies))
            {
                var parts = existingCookies.Split(';');
                foreach(var p in parts)
                {
                    var kv = p.Trim().Split(new[] { '=' }, 2);
                    if (kv.Length == 2)
                        cookiesDict[kv[0]] = kv[1];
                }
            }

            foreach (var key in resp.Headers.Keys)
            {
                if (key.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
                {
                    var cookies = resp.Headers[key];
                    foreach (var cookieHeader in cookies)
                    {
                        var parts = cookieHeader.Split(';');
                        var firstPart = parts[0].Trim();
                        var kv = firstPart.Split(new[] { '=' }, 2);
                        if (kv.Length == 2)
                        {
                            cookiesDict[kv[0]] = kv[1];
                        }
                    }
                }
            }

            if (cookiesDict.Count > 0)
            {
                var cookieString = string.Join("; ", cookiesDict.Select(kv => $"{kv.Key}={kv.Value}"));
                if (Config.DefaultHeaders.ContainsKey("Cookie"))
                    Config.DefaultHeaders["Cookie"] = cookieString;
                else
                    Config.DefaultHeaders.Add("Cookie", cookieString);
                
                Config.ApiKeyPrefix.Clear();

                if (cookiesDict.TryGetValue("auth", out string authTok))
                {
                    if (Config.ApiKey.ContainsKey("auth"))
                        Config.ApiKey["auth"] = authTok;
                    else
                        Config.AddApiKey("auth", authTok);
                }

                if (cookiesDict.TryGetValue("twoFactorAuth", out string twoFaTok))
                {
                    if (Config.ApiKey.ContainsKey("twoFactorAuth"))
                        Config.ApiKey["twoFactorAuth"] = twoFaTok;
                    else
                        Config.AddApiKey("twoFactorAuth", twoFaTok);
                }
                
                Console.WriteLine("Cookies extracted and updated.");
            }
        }

        private bool requiresEmail2FA(ApiResponse<CurrentUser> resp)
        {
            return resp.RawContent != null && resp.RawContent.Contains("emailOtp");
        }

        public async Task SaveCookiesAsync()
        {
            try
            {
                if (Config.DefaultHeaders.TryGetValue("Cookie", out string cookieString))
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
                    if (!cookieString.Contains("="))
                    {
                        cookieString = $"auth={cookieString}";
                    }

                    if (Config.DefaultHeaders.ContainsKey("Cookie"))
                        Config.DefaultHeaders["Cookie"] = cookieString;
                    else
                        Config.DefaultHeaders.Add("Cookie", cookieString);

                    Config.ApiKeyPrefix.Clear();

                    var parts = cookieString.Split(';');
                    foreach (var p in parts)
                    {
                        var kv = p.Trim().Split(new[] { '=' }, 2);
                        if (kv.Length == 2)
                        {
                            string key = kv[0].Trim();
                            string val = kv[1].Trim();
                            if (key == "auth" || key == "twoFactorAuth")
                            {
                                if (Config.ApiKey.ContainsKey(key))
                                    Config.ApiKey[key] = val;
                                else
                                    Config.AddApiKey(key, val);
                            }
                        }
                    }

                    CookieLoaded = true;
                    AuthApi = new AuthenticationApi(ApiClient, ApiClient, Config);
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
            try
            {
                string cookieString = Task.Run(async () => await SecureStorage.Default.GetAsync("auth_cookie")).GetAwaiter().GetResult();

                if (!string.IsNullOrEmpty(cookieString))
                {
                    if (!cookieString.Contains("="))
                    {
                        cookieString = $"auth={cookieString}";
                    }

                    if (Config.DefaultHeaders.ContainsKey("Cookie"))
                        Config.DefaultHeaders["Cookie"] = cookieString;
                    else
                        Config.DefaultHeaders.Add("Cookie", cookieString);

                    Config.ApiKeyPrefix.Clear();

                    var parts = cookieString.Split(';');
                    foreach (var p in parts)
                    {
                        var kv = p.Trim().Split(new[] { '=' }, 2);
                        if (kv.Length == 2)
                        {
                            string key = kv[0].Trim();
                            string val = kv[1].Trim();
                            if (key == "auth" || key == "twoFactorAuth")
                            {
                                if (Config.ApiKey.ContainsKey(key))
                                    Config.ApiKey[key] = val;
                                else
                                    Config.AddApiKey(key, val);
                            }
                        }
                    }

                    CookieLoaded = true;
                    AuthApi = new AuthenticationApi(ApiClient, ApiClient, Config);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load error: {ex.Message}");
                CookieLoaded = false;
            }
        }

        public async Task SaveCredentialsAsync(string username, string password)
        {
            try
            {
                await SecureStorage.Default.SetAsync("auth_username", username);
                await SecureStorage.Default.SetAsync("auth_password", password);
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
            if (Config.DefaultHeaders.TryGetValue("Cookie", out string cookieString))
            {
                var parts = cookieString.Split(';');
                var newParts = parts.Where(p => !string.IsNullOrWhiteSpace(p) && !p.Trim().StartsWith("auth=")).Select(p => p.Trim()).ToList();
                if (newParts.Count > 0)
                {
                    Config.DefaultHeaders["Cookie"] = string.Join("; ", newParts);
                }
                else
                {
                    Config.DefaultHeaders.Remove("Cookie");
                }
            }
            Config.ApiKey.Remove("auth");
            Config.ApiKeyPrefix.Remove("auth");
        }

        private SemaphoreSlim _reloginSemaphore = new SemaphoreSlim(1, 1);
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

                ClearAuthCookieOnly();
                
                var status = await LoginAsync(creds.Value.username, creds.Value.password);
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

            Config.Username = null;
            Config.Password = null;
            Config.ApiKey.Clear();
            Config.ApiKeyPrefix.Clear();
            Config.DefaultHeaders.Clear();
            Config.UserAgent = "VRCGalleryManager";

            ApiClient = new ApiClient(Config.BasePath);
            AuthApi = new AuthenticationApi(ApiClient, ApiClient, Config);

            OnAuthStateChanged?.Invoke();
        }

        public void NotifyAuthStateChanged()
        {
            OnAuthStateChanged?.Invoke();
        }
    }
}