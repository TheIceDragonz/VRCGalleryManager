﻿using Microsoft.VisualBasic;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;
using VRCGalleryManager.Core.Helpers;

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
            ApiClient = new ApiClient();
            Config.UserAgent = "VRCGalleryManager";
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
            Config.UserAgent = "VRCGalleryManager";

            AuthApi = new AuthenticationApi(ApiClient, ApiClient, Config);

            try
            {
                ApiResponse<CurrentUser> currentUserResp = AuthApi.GetCurrentUserWithHttpInfo();

                if (requiresEmail2FA(currentUserResp)) // If the API wants us to send an Email OTP code
                {
                    string imputAuth = Interaction.InputBox("Insert the Email Code", "Email Authentication", "");
                    if (imputAuth != null)
                    {
                        AuthApi.Verify2FAEmailCode(new TwoFactorEmailCode(imputAuth));
                    }
                }
                else
                {
                    string imputAuth = Interaction.InputBox("Insert the 2FA Code", "2FA Authentication", "");
                    if (imputAuth != null)
                    {
                        AuthApi.Verify2FA(new TwoFactorAuthCode(imputAuth));
                    }
                }

                SaveCookies();
                LoadCookies();

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
            string encryptedCookies = CryptAuth.Encrypt(cookieString);
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VRCGalleryManager"));
            System.IO.File.WriteAllText(tokenFilePath, encryptedCookies);
        }

        public void LoadCookies()
        {
            try
            {
                string cookieString = CryptAuth.Decrypt(System.IO.File.ReadAllText(tokenFilePath));
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


        public void Logout()
        {
            if (System.IO.File.Exists(tokenFilePath))
            {
                System.IO.File.Delete(tokenFilePath);
            }
            LoggedIn = false;
            CookieLoaded = false;
            // Clear the cookies
            Config.DefaultHeaders.Clear();
            ApiClient.ClearCookieContainer();
        }
    }
}