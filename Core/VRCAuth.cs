using Microsoft.VisualBasic;
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

        public static readonly string tokenFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VRCGalleryManagerMain", "authToken.txt");

        public static Uri uriTarget = new Uri("https://vrchat.com");

        private VRCAuth()
        {
            Config = new Configuration();
            ApiClient = new ApiClient();
            Config.UserAgent = "VRCGalleryManager/0.0.1 (Testing)";
        }

        public static VRCAuth Instance()
        {
            if (instance == null) instance = new VRCAuth();
            return instance;
        }

        public void VRCAuthentication(string usernameVRC, string passworldVRC)
        {
            Config.Username = usernameVRC;
            Config.Password = passworldVRC;
            Config.UserAgent = "VRCGalleryManager/0.0.1 (Testing)";

            AuthApi = new AuthenticationApi(ApiClient, ApiClient, Config);

            try
            {
                ApiResponse<CurrentUser> currentUserResp = AuthApi.GetCurrentUserWithHttpInfo();

                if (requiresEmail2FA(currentUserResp)) // If the API wants us to send an Email OTP code
                {
                    string imputAuth = Interaction.InputBox("Insert the 2FA Code", "2FA Authentication", "");
                    if (imputAuth != null)
                    {
                        AuthApi.Verify2FAEmailCode(new TwoFactorEmailCode(imputAuth));
                    }
                }
                else
                {
                    // requiresEmail2FA returned false, so we use secret-based 2fa verification
                    // authApi.VerifyRecoveryCode(new TwoFactorAuthCode("12345678")); // To Use a Recovery Code

                    //authApi.Verify2FA(new TwoFactorAuthCode("123456"));
                }

                SaveCookies();

                LoggedIn = true;

                CurrentUser currentUser = AuthApi.GetCurrentUser();
                Console.WriteLine("Logged in as {0}", currentUser.DisplayName, currentUser.DateJoined, currentUser.Id);

            }
            catch (ApiException ex)
            {
                // Catch any exceptions write to console, helps w debugging :D
                Console.WriteLine("Exception when calling API: {0}", ex.Message);
                Console.WriteLine("Status Code: {0}", ex.ErrorCode);
                Console.WriteLine(ex.ToString());

                MessageBox.Show("Incorrect Credentials");
            }

            // Function that determines if the api expects email2FA from an ApiResponse
            bool requiresEmail2FA(ApiResponse<CurrentUser> resp)
            {
                // We can just use a super simple string.Contains() check
                if (resp.RawContent.Contains("emailOtp"))
                {
                    return true;
                }
                return false;
            }
        }

        public void SaveCookies()
        {
            var cookies = ApiClient.CookieContainer.GetAllCookies();
            string cookieString = string.Join(";", cookies.Select(c => $"{c.Name}={c.Value}"));
            System.IO.File.WriteAllText(tokenFilePath, cookieString);
        }

        public void LoadCookies()
        {
            try
            {
                string cookieString = System.IO.File.ReadAllText(tokenFilePath);
                Config.DefaultHeaders.Add("Cookie", cookieString);
                AuthApi = new AuthenticationApi(ApiClient, ApiClient, Config);
                CookieLoaded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                CookieLoaded = false;
            }
        }
    }
}