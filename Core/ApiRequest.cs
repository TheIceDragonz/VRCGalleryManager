﻿using System.Diagnostics;
using VRCGalleryManager.Core.DTO;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;
using VRCGalleryManager.Forms;
using System.Data;

namespace VRCGalleryManager.Core
{
    public class ApiRequest
    {
        private VRCAuth Auth;
        private FilesApi filesApi;
        private PrintsApi printsApi;
        private UsersApi usersApi;
        private WorldsApi worldApi;

        public ApiRequest(VRCAuth Auth)
        {
            this.Auth = Auth;
            filesApi = new FilesApi(Auth.ApiClient, Auth.ApiClient, Auth.Config);
            printsApi = new PrintsApi(Auth.ApiClient, Auth.ApiClient, Auth.Config);
            usersApi = new UsersApi(Auth.ApiClient, Auth.ApiClient, Auth.Config);
            worldApi = new WorldsApi(Auth.ApiClient, Auth.ApiClient, Auth.Config);
        }

        public class ApiData
        {
            public List<string> JsonImage { get; set; } = new List<string>();

            public string CountImages { get; set; } = new string("");
            public string Tags { get; set; } = new string("");
            public string AnimationStyle { get; set; } = new string("");
            public string Frames { get; set; } = new string("");
            public string FramesOverTime { get; set; } = new string("");
            public string LoopStyle { get; set; } = new string("");
            public string MaskTag { get; set; } = new string("");
            public string IdImageUploaded { get; set; } = new string("");
        }

        public class ApiDataWorld
        {
            public string Id { get; set; } = new string("");
            public string Name { get; set; } = new string("");
            public string Description { get; set; } = new string("");
            public string Author { get; set; } = new string("");
            public string AuthorId { get; set; } = new string("");
            public string ImageUrl { get; set; } = new string("");
            public string ThumbnailUrl { get; set; } = new string("");
        }

        public async Task<ApiData> GetApiData(string tag)
        {
            ApiData apiData = new ApiData();

            try
            {
                if (!tag.Contains("print"))
                {
                    var images = filesApi.GetFiles(tag, null, 64);

                    foreach (var image in images)
                    {
                        apiData.JsonImage.Add(image.ToJson());
                    }
                }
                else
                {
                    var images = await printsApi.GetUserPrintsAsync(Settings.UserId, 100, 0);
                    

                    foreach (var image in images)
                    {
                        apiData.JsonImage.Add(image.ToJson());
                    }
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

            return apiData;
        }

        public async Task<ApiData> UploadPrint(string path, string note)
        {
            ApiData apiData = new ApiData();

            //var response = await filesApi.UploadImageAsync(path, maskType, tag);
            //
            ImageUploadPayload imageUploadPayload = new ImageUploadPayload(path);
            imageUploadPayload.FileType = FileType.Print;
            imageUploadPayload.Note = note;

            var response = await filesApi.UploadImageAsync(imageUploadPayload);

            apiData.IdImageUploaded = response.Data.Id;

            return apiData;
        }

        public async Task<ApiData> UploadImage(string path, string maskTag, TagType tag, string animationStyle)
        {
            ApiData apiData = new ApiData();

            ImageUploadPayload imageUploadPayload = new ImageUploadPayload(path);

            imageUploadPayload.MaskTag = maskTag;
            imageUploadPayload.Tag = tag;
            imageUploadPayload.AnimationStyle = animationStyle;

            var response = await filesApi.UploadImageAsync(imageUploadPayload);

            apiData.IdImageUploaded = response.Data.Id;

            return apiData;
        }
        public async Task<ApiData> UploadImage(string path, string maskTag, TagType tag, string animationStyle, int frames, int framesOverTime)
        {
            ApiData apiData = new ApiData();

            ImageUploadPayload imageUploadPayload = new ImageUploadPayload(path);

            imageUploadPayload.MaskTag = maskTag;
            imageUploadPayload.Tag = tag;
            imageUploadPayload.AnimationStyle = animationStyle;
            imageUploadPayload.Frames = frames;
            imageUploadPayload.FramesOverTime = framesOverTime;

            var response = await filesApi.UploadImageAsync(imageUploadPayload);

            apiData.IdImageUploaded = response.Data.Id;

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

        public async Task<ApiData> DeleteApiDataPrint(string id)
        {
            ApiData apiData = new ApiData();

            try
            {
                await printsApi.DeletePrintAsync(id);
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Errore: {ex.Message}");
            }

            return apiData;
        }


        public async Task SetProfileIcon(string urlImage)
        {
            try
            {
                await usersApi.UpdateUserAsync(Settings.UserId, new  UpdateUserRequest { UserIcon = urlImage });
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Errore: {ex.Message}");
            }
        }
        public async Task SetProfilePicture(string urlImage)
        {
            try
            {
                await usersApi.UpdateUserAsync(Settings.UserId, new UpdateUserRequest { ProfilePicOverride = urlImage });
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Errore: {ex.Message}");
            }
        }

        public async Task<ApiDataWorld> GetWorldInfo(string worldId)
        {
            ApiDataWorld apiDataWorld = new ApiDataWorld();

            try
            {
                var world = await worldApi.GetWorldAsync(worldId);
                apiDataWorld.Id = world.Id;
                apiDataWorld.Name = world.Name;
                apiDataWorld.Description = world.Description;
                apiDataWorld.AuthorId = world.AuthorId;
                apiDataWorld.ImageUrl = world.ImageUrl;
                apiDataWorld.ThumbnailUrl = world.ThumbnailImageUrl;

                return apiDataWorld;
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Errore: {ex.Message}");
                return null;
            }
        }
    }
}
