using VRCGalleryManager.Core.Helpers;
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

        public static readonly string tokenFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VRCGalleryManager", "authToken.txt");

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

            foreach (var key in resp.Headers.Keys)
            {
                if (key.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
                {
                    var cookies = resp.Headers[key];
                    foreach (var cookieHeader in cookies)
                    {
                        if (cookieHeader.StartsWith("auth="))
                        {
                            var parts = cookieHeader.Split(';');
                            var authPart = parts.FirstOrDefault(p => p.Trim().StartsWith("auth="));
                            if (authPart != null)
                            {
                                string token = authPart.Trim().Substring(5);
                                Config.ApiKey["auth"] = token;
                                Config.AddApiKeyPrefix("auth", "auth");
                                Console.WriteLine("Auth cookie extracted.");
                                return;
                            }
                        }
                    }
                }
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
                if (Config.ApiKey.TryGetValue("auth", out string token))
                {
                    string folder = Path.GetDirectoryName(tokenFilePath);
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string encryptedToken = CryptAuth.Encrypt(token);
                    System.IO.File.WriteAllText(tokenFilePath, encryptedToken);

                    CookieLoaded = true;
                    Console.WriteLine("Session token saved successfully.");
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
                if (System.IO.File.Exists(tokenFilePath))
                {
                    string encrypted = System.IO.File.ReadAllText(tokenFilePath);
                    string token = CryptAuth.Decrypt(encrypted);

                    if (!string.IsNullOrEmpty(token))
                    {
                        if (Config.ApiKey.ContainsKey("auth"))
                            Config.ApiKey["auth"] = token;
                        else
                            Config.AddApiKey("auth", token);

                        if (Config.DefaultHeaders.ContainsKey("Cookie"))
                            Config.DefaultHeaders["Cookie"] = $"auth={token}";
                        else
                            Config.DefaultHeaders.Add("Cookie", $"auth={token}");

                        CookieLoaded = true;

                        AuthApi = new AuthenticationApi(ApiClient, ApiClient, Config);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load error: {ex.Message}");
                CookieLoaded = false;
            }
        }

        public void Logout()
        {
            if (System.IO.File.Exists(tokenFilePath))
            {
                System.IO.File.Delete(tokenFilePath);
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