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
            public string CountImages { get; set; } = new string("");
            public string Tags { get; set; } = new string("");
            public string AnimationStyle { get; set; } = new string("");
            public string Frames { get; set; } = new string("");
            public string FramesOverTime { get; set; } = new string("");
            public string LoopStyle { get; set; } = new string("");
            public string MaskTag { get; set; } = new string("");
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

                    apiData.CountImages = icons.Count.ToString();
                    apiData.Tags = icon.Tags.Count > 0 ? string.Join(",", icon.Tags) : "";
                    apiData.AnimationStyle = icon.AnimationStyle;
                    if (apiData.Tags.Contains("animated"))
                    {
                        apiData.Frames = icon.Frames.ToString();
                        apiData.FramesOverTime = icon.FramesOverTime.ToString();
                        apiData.LoopStyle = icon.LoopStyle;
                    }
                    apiData.MaskTag = icon.MaskTag;
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
