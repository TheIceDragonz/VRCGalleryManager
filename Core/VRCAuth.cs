using Microsoft.Maui.Storage;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;

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
            Config.Username = usernameVRC;
            Config.Password = passwordVRC;

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
                    return VRCAuthStatus.Error;
                }
                else if (currentUserResp.Data == null || string.IsNullOrEmpty(currentUserResp.Data.Id))
                {
                    return VRCAuthStatus.Error;
                }

                SaveCookies();

                LoggedIn = true;
                CurrentUser = await AuthApi.GetCurrentUserAsync();
                Console.WriteLine("Logged in as: {0}", CurrentUser.DisplayName);
                
                OnAuthStateChanged?.Invoke();

                return VRCAuthStatus.Success;
            }
            catch (ApiException ex)
            {
                Console.WriteLine("API Error: {0}", ex.Message);
                return VRCAuthStatus.Error;
            }
        }

        public async Task<VRCAuthStatus> Verify2FAAsync(string code, bool isEmail)
        {
            try
            {
                if (isEmail)
                {
                    var resp2fa = await AuthApi.Verify2FAEmailCodeWithHttpInfoAsync(new TwoFactorEmailCode(code));
                    ExtractAuthCookie(resp2fa);
                }
                else
                {
                    var resp2fa = await AuthApi.Verify2FAWithHttpInfoAsync(new TwoFactorAuthCode(code));
                    ExtractAuthCookie(resp2fa);
                }

                SaveCookies();

                LoggedIn = true;
                CurrentUser = await AuthApi.GetCurrentUserAsync();
                Console.WriteLine("Logged in as: {0}", CurrentUser.DisplayName);
                
                OnAuthStateChanged?.Invoke();

                return VRCAuthStatus.Success;
            }
            catch (ApiException ex)
            {
                Console.WriteLine("2FA Verification Error: {0}", ex.Message);
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
                
                if (cookiesDict.TryGetValue("auth", out string authTok))
                {
                    if (Config.ApiKey.ContainsKey("auth"))
                        Config.ApiKey["auth"] = authTok;
                    else
                        Config.AddApiKey("auth", authTok);
                    Config.AddApiKeyPrefix("auth", "auth");
                }
                
                Console.WriteLine("Cookies extracted and updated.");
            }
        }

        private bool requiresEmail2FA(ApiResponse<CurrentUser> resp)
        {
            return resp.RawContent != null && resp.RawContent.Contains("emailOtp");
        }

        public void SaveCookies()
        {
            try
            {
                if (Config.DefaultHeaders.TryGetValue("Cookie", out string cookieString))
                {
                    Task.Run(async () => await SecureStorage.Default.SetAsync("auth_cookie", cookieString)).Wait();
                    CookieLoaded = true;
                    Console.WriteLine("Session cookies saved securely.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save error: {ex.Message}");
            }
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

                    var authPart = cookieString.Split(';').FirstOrDefault(p => p.Trim().StartsWith("auth="));
                    if (authPart != null)
                    {
                        string token = authPart.Trim().Substring(5);
                        if (Config.ApiKey.ContainsKey("auth"))
                            Config.ApiKey["auth"] = token;
                        else
                            Config.AddApiKey("auth", token);
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

        public void SaveCredentials(string username, string password)
        {
            try
            {
                Task.Run(async () => {
                    await SecureStorage.Default.SetAsync("auth_username", username);
                    await SecureStorage.Default.SetAsync("auth_password", password);
                }).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save credentials error: {ex.Message}");
            }
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
                var newParts = parts.Where(p => !p.Trim().StartsWith("auth=")).ToList();
                Config.DefaultHeaders["Cookie"] = string.Join(";", newParts);
            }
            Config.ApiKey.Remove("auth");
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

            Config.ApiKey.Clear();
            Config.DefaultHeaders.Clear();

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