using System.Net;
using VRCGalleryManager.Forms;

namespace VRCGalleryManager.Core
{
    public static class HttpImage
    {
        public static async Task<HttpWebRequest> CreateWebRequestAsync(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "VRCGalleryManager/0.0.1 (Testing)";
            request.AllowAutoRedirect = true;
            return request;
        }

        public static async Task<string> GetFinalUrlAsync(string url)
        {
            try
            {
                var request = await CreateWebRequestAsync(url);
                using (var response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    return response.ResponseUri.ToString();
                }
            }
            catch
            {
                return "imageNotFound";
            }
        }
    }
}
