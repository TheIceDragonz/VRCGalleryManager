using VRCGalleryManager.Core.DTO;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;
using System.Collections.Concurrent;
using System.Text.Json;

namespace VRCGalleryManager.Core
{
    public class ApiRequest
    {
        private VRCAuth Auth;
        private FilesApi filesApi;
        private PrintsApi printsApi;
        private UsersApi usersApi;
        private WorldsApi worldApi;
        private InventoryApi inventoryApi;

        public ApiRequest(VRCAuth Auth)
        {
            this.Auth = Auth;
            filesApi = new FilesApi(Auth.ApiClient, Auth.ApiClient, Auth.Config);
            printsApi = new PrintsApi(Auth.ApiClient, Auth.ApiClient, Auth.Config);
            usersApi = new UsersApi(Auth.ApiClient, Auth.ApiClient, Auth.Config);
            worldApi = new WorldsApi(Auth.ApiClient, Auth.ApiClient, Auth.Config);
            inventoryApi = new InventoryApi(Auth.ApiClient, Auth.ApiClient, Auth.Config);
        }

        public class ApiData
        {
            public List<string> JsonImage { get; set; } = new List<string>();

            public string CountImages { get; set; } = "";
            public string Tags { get; set; } = "";
            public string AnimationStyle { get; set; } = "";
            public string Frames { get; set; } = "";
            public string FramesOverTime { get; set; } = "";
            public string LoopStyle { get; set; } = "";
            public string MaskTag { get; set; } = "";
            public string IdImageUploaded { get; set; } = "";
        }

        public class ApiDataPrint
        {
            public string IdImageUploaded { get; set; } = "";
            public string Id { get; set; } = "";
            public string AuthorId { get; set; } = "";
            public string AuthorName { get; set; } = "";
            public string FileId { get; set; } = "";
        }

        public class ApiDataWorld
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public string Author { get; set; } = "";
            public string AuthorId { get; set; } = "";
            public string ImageUrl { get; set; } = "";
            public string ThumbnailUrl { get; set; } = "";
        }

        public class ApiDataInventory
        {
            public List<string> Collections { get; set; } = new List<string>();
            public DateTime CreatedAt { get; set; }
            public string Description { get; set; } = "";
            public DateTime? ExpiryDate { get; set; }
            public List<string> Flags { get; set; } = new List<string>();
            public string HolderId { get; set; } = "";
            public string Id { get; set; } = "";
            public string ImageUrl { get; set; } = "";
            public bool IsArchived { get; set; }
            public bool IsSeen { get; set; }
            public string ItemType { get; set; } = "";
            public string ItemTypeLabel { get; set; } = "";
            public InventoryMetadata Metadata { get; set; } = new InventoryMetadata();
            public string Name { get; set; } = "";
            public List<string> Tags { get; set; } = new List<string>();
            public string TemplateId { get; set; } = "";
            public DateTime TemplateCreatedAt { get; set; }
            public DateTime TemplateUpdatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public class InventoryMetadata
        {
            public string FileId { get; set; } = "";
            public string ImageUrl { get; set; } = "";
            public string MaskTag { get; set; } = "";
            public bool Animated { get; set; }
            public string AnimationStyle { get; set; }
            public int Frames { get; set; }
            public int FramesOverTime { get; set; }
            public string? LoopStyle { get; set; }
        }

        /// <summary>Returns the standard VRChat 256px thumbnail URL for a file ID.</summary>
        public static string ImageThumbnailUrl(string fileId) =>
            $"https://api.vrchat.cloud/api/1/image/{fileId}/1/256";

        /// <summary>Maps a file extension to its MIME type.</summary>
        private static string GetMimeType(string path) =>
            Path.GetExtension(path).ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "image/png"
            };

        public async Task<T> ExecuteWithReloginAsync<T>(Func<Task<T>> apiCall)
        {
            try
            {
                return await apiCall();
            }
            catch (ApiException ex) when (ex.ErrorCode == 401)
            {
                bool reloggedIn = await Auth.TryAutoReloginAsync();
                if (reloggedIn)
                {
                    return await apiCall();
                }
                throw;
            }
        }

        public async Task ExecuteWithReloginAsync(Func<Task> apiCall)
        {
            try
            {
                await apiCall();
            }
            catch (ApiException ex) when (ex.ErrorCode == 401)
            {
                bool reloggedIn = await Auth.TryAutoReloginAsync();
                if (reloggedIn)
                {
                    await apiCall();
                    return;
                }
                throw;
            }
        }

        public async Task<ApiData> GetApiData(string tag)
        {
            ApiData apiData = new ApiData();

            try
            {
                if (tag == "sticker")
                {
                    var inventory = await ExecuteWithReloginAsync(() => inventoryApi.GetInventoryAsync(
                        n: 100,
                        offset: 0,
                        types: InventoryItemType.Sticker,
                        tags: "Custom Sticker",
                        flags: InventoryFlag.Ugc,
                        archived: false,
                        order: "newest_created"
                    ));

                    foreach (var image in inventory.Data)
                    {
                        apiData.JsonImage.Add(image.ToJson());
                    }
                }
                else if (!tag.Contains("print"))
                {
                    var images = await ExecuteWithReloginAsync(() => filesApi.GetFilesAsync(tag, null, 100));

                    foreach (var image in images)
                    {
                        apiData.JsonImage.Add(image.ToJson());
                    }
                }
                else
                {
                    var user = await ExecuteWithReloginAsync(() => Auth.AuthApi.GetCurrentUserAsync());
                    var images = await ExecuteWithReloginAsync(() => printsApi.GetUserPrintsAsync(user.Id, 100, 0));

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
                    Console.WriteLine($"Error: {ex.Message}");
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
                var imageUploaded = await ExecuteWithReloginAsync(() => filesApi.CreateFileAsync(createFileRequest));
                apiData.IdImageUploaded = imageUploaded.Id;
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

            return apiData;
        }

        public async Task<ApiData> UploadImage(string path, string maskTag, TagType tag, string animationStyle, int frames = 0, int framesOverTime = 0)
        {
            ApiData apiData = new ApiData();

            using var stream = System.IO.File.OpenRead(path);
            var fileParam = new FileParameter(Path.GetFileName(path), GetMimeType(path), stream);

            ImagePurpose vrcPurpose = tag switch
            {
                TagType.Icon => ImagePurpose.Icon,
                TagType.Gallery => ImagePurpose.Gallery,
                TagType.Emoji => ImagePurpose.Emoji,
                TagType.EmojiAnimated => ImagePurpose.Emojianimated,
                TagType.Sticker => ImagePurpose.Sticker,
                TagType.Print => ImagePurpose.Gallery,
                _ => ImagePurpose.Gallery
            };

            ImageMask? vrcMask = Enum.TryParse<ImageMask>(maskTag, true, out var m) ? m : (ImageMask?)null;

            ImageAnimationStyle? vrcAnim = Enum.TryParse<ImageAnimationStyle>(animationStyle, true, out var a) ? a : null;

            int? vrcFrames = frames > 0 ? frames : null;
            int? vrcFramesOverTime = framesOverTime > 0 ? framesOverTime : null;

            try
            {
                var response = await ExecuteWithReloginAsync(() => filesApi.UploadImageAsync(
                    fileParam,
                    vrcPurpose,
                    vrcAnim,
                    vrcFrames,
                    vrcFramesOverTime,
                    null,
                    vrcMask
                ));
                apiData.IdImageUploaded = response.Id;
                apiData.Tags = response.Tags != null ? string.Join(", ", response.Tags) : "";
                if (tag == TagType.EmojiAnimated && !apiData.Tags.Contains("animated"))
                {
                    apiData.Tags = string.IsNullOrEmpty(apiData.Tags) ? "animated" : apiData.Tags + ", animated";
                }
                apiData.Frames = response.Frames.ToString();
                apiData.FramesOverTime = response.FramesOverTime.ToString();
                apiData.AnimationStyle = response.AnimationStyle?.ToString() ?? "";
                apiData.MaskTag = response.MaskTag?.ToString() ?? "";
            }
            catch (ApiException ex) 
            { 
                Console.WriteLine($"Error uploading image: {ex.Message}"); 
                throw;
            }

            return apiData;
        }

        public async Task<ApiDataPrint> UploadPrint(string path, string note)
        {
            ApiDataPrint apiData = new ApiDataPrint();
            try
            {
                using var stream = System.IO.File.OpenRead(path);
                var fileParam = new FileParameter(Path.GetFileName(path), GetMimeType(path), stream);

                var response = await ExecuteWithReloginAsync(() => printsApi.UploadPrintAsync(
                    fileParam,
                    DateTime.UtcNow,
                    note
                ));

                apiData.IdImageUploaded = response.Id;
                apiData.AuthorId = response.AuthorId ?? "";
                apiData.AuthorName = response.AuthorName ?? "";
                apiData.FileId = response.Files?.FileId ?? "";
            }
            catch (ApiException ex) 
            { 
                Console.WriteLine(ex.Message); 
                throw;
            }
            return apiData;
        }

        public async Task<ApiDataPrint> GetPrintInfo(string printId)
        {
            ApiDataPrint apiData = new ApiDataPrint();
            try
            {
                var response = await ExecuteWithReloginAsync(() => printsApi.GetPrintAsync(printId));

                apiData.IdImageUploaded = response.Id;
                apiData.AuthorId = response.AuthorId ?? "";
                apiData.AuthorName = response.AuthorName ?? "";
                apiData.FileId = response.Files?.FileId ?? "";
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Error fetching print info: {ex.Message}");
            }
            return apiData;
        }


        public async Task<ApiData> DeleteApiData(string id)
        {
            ApiData apiData = new ApiData();

            try
            {
                if (id.StartsWith("inv_"))
                {
                    await ExecuteWithReloginAsync(() => inventoryApi.DeleteOwnInventoryItemAsync(id));
                }
                else
                {
                    await ExecuteWithReloginAsync(() => filesApi.DeleteFileAsync(id));
                }
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return apiData;
        }

        public async Task<ApiData> DeleteApiDataPrint(string id)
        {
            ApiData apiData = new ApiData();

            try
            {
                await ExecuteWithReloginAsync(() => printsApi.DeletePrintAsync(id));
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return apiData;
        }


        public async Task SetProfileIcon(string urlImage)
        {
            try
            {
                var user = await ExecuteWithReloginAsync(() => Auth.AuthApi.GetCurrentUserAsync());
                
                var updateRequest = new UpdateUserRequest
                {
                    AcceptedTOSVersion = user.AcceptedTOSVersion,
                    Bio = user.Bio,
                    BioLinks = user.BioLinks,
                    Status = user.Status,
                    StatusDescription = user.StatusDescription,
                    Tags = user.Tags,
                    Pronouns = user.Pronouns,
                    ProfilePicOverride = user.ProfilePicOverride,
                    UserIcon = urlImage
                };
                
                await ExecuteWithReloginAsync(() => usersApi.UpdateUserAsync(user.Id, updateRequest));
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        
        public async Task SetProfilePicture(string urlImage)
        {
            try
            {
                var user = await ExecuteWithReloginAsync(() => Auth.AuthApi.GetCurrentUserAsync());
                
                var updateRequest = new UpdateUserRequest
                {
                    AcceptedTOSVersion = user.AcceptedTOSVersion,
                    Bio = user.Bio,
                    BioLinks = user.BioLinks,
                    Status = user.Status,
                    StatusDescription = user.StatusDescription,
                    Tags = user.Tags,
                    Pronouns = user.Pronouns,
                    UserIcon = user.UserIcon,
                    ProfilePicOverride = urlImage
                };
                
                await ExecuteWithReloginAsync(() => usersApi.UpdateUserAsync(user.Id, updateRequest));
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public async Task<ApiDataWorld> GetWorldInfo(string worldId)
        {
            ApiDataWorld apiDataWorld = new ApiDataWorld();

            try
            {
                var world = await ExecuteWithReloginAsync(() => worldApi.GetWorldAsync(worldId));
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
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiDataInventory> GetInventoryInfo(string userId, string inventoryId)
        {
            ApiDataInventory apiInventory = new ApiDataInventory();

            try
            {
                var inventoryResponse = await ExecuteWithReloginAsync(() => inventoryApi.GetUserInventoryItemWithHttpInfoAsync(userId, inventoryId));
                if (inventoryResponse == null || inventoryResponse.Data == null)
                {
                    return null;
                }
                var inventory = inventoryResponse.Data;

                apiInventory.Collections = inventory.Collections;
                apiInventory.CreatedAt = inventory.CreatedAt;
                apiInventory.Description = inventory.Description;
                apiInventory.ExpiryDate = inventory.ExpiryDate;
                apiInventory.Flags = inventory.Flags;
                apiInventory.HolderId = inventory.HolderId;
                apiInventory.Id = inventory.Id;
                apiInventory.ImageUrl = inventory.ImageUrl;
                apiInventory.IsArchived = inventory.IsArchived;
                apiInventory.IsSeen = inventory.IsSeen;
                apiInventory.ItemType = inventory.ItemType.ToString();
                apiInventory.ItemTypeLabel = inventory.ItemTypeLabel;
                apiInventory.Metadata = inventory.Metadata == null
                                    ? new InventoryMetadata()
                                    : JsonSerializer.Deserialize<InventoryMetadata>(
                                      JsonSerializer.Serialize(inventory.Metadata),
                                      new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                                      ) ?? new InventoryMetadata();
                apiInventory.Name = inventory.Name;
                apiInventory.Tags = inventory.Tags;
                apiInventory.TemplateId = inventory.TemplateId;
                apiInventory.TemplateCreatedAt = inventory.TemplateCreatedAt;
                apiInventory.TemplateUpdatedAt = inventory.TemplateUpdatedAt;
                apiInventory.UpdatedAt = inventory.UpdatedAt;

                return apiInventory;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
        private static readonly ConcurrentDictionary<string, string> _userNameCache = new();

        public async Task<string> GetUserName(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return "Unknown User";
            if (_userNameCache.TryGetValue(userId, out string name)) return name;

            try
            {
                var user = await ExecuteWithReloginAsync(() => usersApi.GetUserAsync(userId));
                if (user != null && !string.IsNullOrEmpty(user.DisplayName))
                {
                    _userNameCache[userId] = user.DisplayName;
                    return user.DisplayName;
                }
            }
            catch { }
            return "Unknown User";
        }
    }
}
