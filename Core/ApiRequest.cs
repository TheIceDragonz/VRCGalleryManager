using VRCGalleryManager.Forms;
using System.Diagnostics;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;

namespace VRCGalleryManager.Core
{
    public class ApiRequest
    {
        private VRCAuth Auth;
        private FilesApi filesApi;

        public ApiRequest(VRCAuth Auth)
        {
            this.Auth = Auth;
            filesApi = new FilesApi(Auth.ApiClient, Auth.ApiClient, Auth.Config);
        }

        public class ApiData
        {
            public List<string> IdImage { get; set; } = new List<string>();
            public string CountImage { get; set; }
        }

        public async Task<ApiData> GetApiData(string tag)
        {

            ApiData apiData = new ApiData();

            try
            {
                var icons = filesApi.GetFiles(tag);

                foreach (var icon in icons)
                {
                    apiData.IdImage.Add(icon.Id);

                    Debug.WriteLine(icon.ToJson());
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
