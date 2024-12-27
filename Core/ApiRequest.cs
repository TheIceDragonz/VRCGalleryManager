using VRCEmojiManager.Forms;
using System.Diagnostics;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;

namespace VRCEmojiManager.Core
{
    public class ApiRequest
    {
        private VRCAuth Auth;
        private FilesApi filesApi;

        private string tag = "emoji";

        public ApiRequest(VRCAuth Auth)
        {
            this.Auth = Auth;
            filesApi = new FilesApi(Auth.ApiClient, Auth.ApiClient, Auth.Config);
        }

        public class ApiData
        {
            public List<string> IdEmoji { get; set; } = new List<string>();
            public string CountEmoji { get; set; }
        }

        public async Task<ApiData> GetApiData()
        {

            ApiData apiData = new ApiData();

            try
            {
                var icons = filesApi.GetFiles(tag);

                foreach (var icon in icons)
                {
                    Debug.WriteLine(icon.Id);

                    apiData.IdEmoji.Add(icon.Id);
                }
            }
            catch (ApiException ex)
            {
                if (ex.ErrorCode == 404 && ex.Message.Contains("Emoji Not Found"))
                {
                    //apiData.Icon = "";
                }
                else
                {
                    Console.WriteLine($"Errore: {ex.Message}");
                }
            }

            return apiData;
        }
    }
}
