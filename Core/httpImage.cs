using System.Net;
using VRCEmojiManager.Forms;

namespace VRCEmojiManager.Core
{
    public static class httpImage
    {
        public static async Task<HttpWebRequest> CreateWebRequestAsync(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "VRCEmojiManager/0.0.1 (Testing)";
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
