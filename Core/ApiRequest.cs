using VRCGalleryManager.Forms;
using System.Diagnostics;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;
using File = VRChat.API.Model.File;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using VRCGalleryManager.Core.DTO;

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

            public string IdImageUploaded { get; set; } = new string("");
        }

        public async Task<ApiData> GetApiData(string tag)
        {
            ApiData apiData = new ApiData();
             
            try
            {
                var images = filesApi.GetFiles(tag);

                foreach (var image in images)
                {
                    apiData.IdImage.Add(image.Id);

                    apiData.CountImages = images.Count.ToString();
                    apiData.Tags = image.Tags.Count > 0 ? string.Join(",", image.Tags) : "";
                    apiData.AnimationStyle = image.AnimationStyle;
                    if (apiData.Tags.Contains("animated"))
                    {
                        apiData.Frames = image.Frames.ToString();
                        apiData.FramesOverTime = image.FramesOverTime.ToString();
                        apiData.LoopStyle = image.LoopStyle;
                    }
                    apiData.MaskTag = image.MaskTag;
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
        public async Task<ApiData> UploadApiData(string name, MIMEType mimeType, string extension, List<string> tags)
        {
            ApiData apiData = new ApiData();

            CreateFileRequest createFileRequest = new CreateFileRequest(name, mimeType, extension, tags);

            try
            {
                var imageUploaded = await filesApi.CreateFileAsync(createFileRequest);
                apiData.IdImageUploaded = imageUploaded.Id;

                Debug.WriteLine(imageUploaded);
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Errore: {ex.Message}");
            }

            return apiData;
        }

        public async Task<ApiData> UploadImage(string path, string maskType, TagType tag)
        {
            ApiData apiData = new ApiData();

            //var response = await filesApi.UploadImageAsync(path, maskType, tag);
            //
            ImageUploadPayload imageUploadPayload = new ImageUploadPayload(path);
            imageUploadPayload.MaskTag = maskType;
            imageUploadPayload.Tag = tag;

            var response = await filesApi.UploadImageAsync(imageUploadPayload);

            apiData.IdImageUploaded = response.Data.Id;
            apiData.AnimationStyle = response.Data.AnimationStyle;
            apiData.LoopStyle = response.Data.LoopStyle;
            apiData.MaskTag = response.Data.MaskTag;

            return apiData;
        }

        public async Task<ApiData> UploadImage(string path, string maskTag, TagType tag, string animationStyle/*, int frames, int framesOverTime*/)
        {
            ApiData apiData = new ApiData();
            
            ImageUploadPayload imageUploadPayload = new ImageUploadPayload(path);

            imageUploadPayload.MaskTag = maskTag;
            imageUploadPayload.Tag = tag;
            imageUploadPayload.AnimationStyle = animationStyle;
            //imageUploadPayload.Frames = frames;
            //imageUploadPayload.FramesOverTime = framesOverTime;

            var response = await filesApi.UploadImageAsync(imageUploadPayload);

            apiData.IdImageUploaded = response.Data.Id;
            apiData.AnimationStyle = response.Data.AnimationStyle;
            apiData.LoopStyle = response.Data.LoopStyle;
            apiData.MaskTag = response.Data.MaskTag;
            //apiData.Frames = response.Data.Frames?.ToString();
            //apiData.FramesOverTime = response.Data.FramesOverTime?.ToString();

            return apiData;
        }

        public async Task<ApiData> DeleteApiData(string id)
        {
            ApiData apiData = new ApiData();

            try
            {
                await filesApi.DeleteFileAsync(id);
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Errore: {ex.Message}");
            }

            return apiData;
        }
    }
}
