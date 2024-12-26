using VRCEmojiManager.Forms;
using System.Diagnostics;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;

namespace VRCEmojiManager.Core
{
    public class ApiRequestInfo
    {
        private VRCAuth Auth;
        private FilesApi filesApi;

        public ApiRequestInfo(VRCAuth Auth)
        {
            this.Auth = Auth;
            filesApi = new FilesApi(Auth.ApiClient, Auth.ApiClient, Auth.Config);
        }

        public class ApiData
        {
            public string Name { get; set; }
            public string Image { get; set; }
        }

        public async Task<ApiData> GetApiData(string dataID)
        {
            ApiData apiData = new ApiData();

            if (dataID.Contains("avtr_"))
            {
                try
                {
                    //FilesApi avi = await filesApi.GetFilesAsync(dataID);

                    filesApi.CreateFile();

                }
                catch (ApiException ex)
                {
                    if (ex.ErrorCode == 404 && ex.Message.Contains("Avatar Not Found"))
                    {
                        apiData.Name = "";
                    }
                    else
                    {
                        Console.WriteLine($"Errore chiamando GetAvatarAsync: {ex.Message}");
                    }
                }
            }

            return apiData;
        }
    }
}
