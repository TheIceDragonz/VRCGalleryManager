using Microsoft.VisualBasic;
using VRCGalleryManager.Core.Helpers;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;

namespace VRCGalleryManager.Core
{
    public class VRCAuth
    {
        private static VRCAuth instance;

        public Configuration Config;
        public ApiClient ApiClient;
        public AuthenticationApi AuthApi;

        public bool LoggedIn = false;
        public bool CookieLoaded = false;

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

        public void VRCAuthentication(string usernameVRC, string passwordVRC)
        {
            Config.Username = usernameVRC;
            Config.Password = passwordVRC;

            try
            {
                ApiResponse<CurrentUser> currentUserResp = AuthApi.GetCurrentUserWithHttpInfo();
                ExtractAuthCookie(currentUserResp);

                if (requiresEmail2FA(currentUserResp))
                {
                    string inputAuth = Interaction.InputBox("Enter the code received via email", "Email Authentication", "");
                    if (!string.IsNullOrEmpty(inputAuth))
                    {
                        var resp2fa = AuthApi.Verify2FAEmailCodeWithHttpInfo(new TwoFactorEmailCode(inputAuth));
                        ExtractAuthCookie(resp2fa);
                    }
                }

                else if (currentUserResp.RawContent != null && currentUserResp.RawContent.Contains("totp"))
                {
                    string inputAuth = Interaction.InputBox("Enter the 2FA code (Authenticator)", "2FA Authentication", "");
                    if (!string.IsNullOrEmpty(inputAuth))
                    {
                        var resp2fa = AuthApi.Verify2FAWithHttpInfo(new TwoFactorAuthCode(inputAuth));
                        ExtractAuthCookie(resp2fa);
                    }
                }

                SaveCookies();

                LoggedIn = true;
                CurrentUser currentUser = AuthApi.GetCurrentUser();
                Console.WriteLine("Logged in as: {0}", currentUser.DisplayName);
            }
            catch (ApiException ex)
            {
                Console.WriteLine("API Error: {0}", ex.Message);
                MessageBox.Show("Invalid credentials or connection error.");
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

            Config.ApiKey.Clear();
            Config.DefaultHeaders.Clear();

            ApiClient = new ApiClient(Config.BasePath);
            AuthApi = new AuthenticationApi(ApiClient, ApiClient, Config);
        }
    }
}