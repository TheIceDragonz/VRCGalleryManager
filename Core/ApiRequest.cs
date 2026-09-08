using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using VRCGalleryManager.Core.Api;
using VRCGalleryManager.Core.Api.Models;
using VRCGalleryManager.Core.DTO;

namespace VRCGalleryManager.Core
{
    public class ApiRequest
    {
        private readonly VRCAuth Auth;

        public ApiRequest(VRCAuth auth)
        {
            Auth = auth;
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

        /// <summary>Returns the standard VRChat 256px thumbnail URL for a file ID.</summary>
        public static string ImageThumbnailUrl(string fileId) =>
            $"https://api.vrchat.cloud/api/1/image/{fileId}/1/256";

        /// <summary>Maps a file extension to its MIME type.</summary>
        private static string GetMimeType(string path) =>
            Path.GetExtension(path).ToLowerInvariant() switch
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
            catch (VRChatApiException ex) when (ex.StatusCode == 401)
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
            catch (VRChatApiException ex) when (ex.StatusCode == 401)
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

        public async Task<List<VRChatFile>> GetFilesAsync(string tag)
        {
            var files = await ExecuteWithReloginAsync(() => Auth.ApiClient.GetFilesAsync(tag, 100));
            return files ?? new List<VRChatFile>();
        }

        public async Task<List<Print>> GetPrintsAsync()
        {
            var user = Auth.CurrentUser ?? await ExecuteWithReloginAsync(() => Auth.GetCurrentUserAsync());
            if (user == null || string.IsNullOrEmpty(user.Id))
            {
                bool relogged = await Auth.TryAutoReloginAsync();
                if (relogged)
                {
                    user = Auth.CurrentUser ?? await Auth.GetCurrentUserAsync();
                }
            }

            if (user == null || string.IsNullOrEmpty(user.Id))
            {
                throw new VRChatApiException(401, "User is not logged in or session expired.");
            }

            var prints = await ExecuteWithReloginAsync(() => Auth.ApiClient.GetUserPrintsAsync(user.Id, 100, 0));
            return prints ?? new List<Print>();
        }

        public async Task<List<InventoryItem>> GetStickersAsync()
        {
            var inventory = await ExecuteWithReloginAsync(() => Auth.ApiClient.GetInventoryAsync(
                itemType: "sticker",
                tags: "Custom Sticker",
                flags: "ugc",
                archived: false,
                order: "newest_created",
                n: 100,
                offset: 0
            ));

            return inventory?.Data ?? new List<InventoryItem>();
        }

        public async Task<ApiData> UploadImage(string path, string maskTag, TagType tag, string? animationStyle, int frames = 0, int framesOverTime = 0)
        {
            var apiData = new ApiData();

            string vrcTag = tag switch
            {
                TagType.Icon => "icon",
                TagType.Gallery => "gallery",
                TagType.Emoji => "emoji",
                TagType.EmojiAnimated => "emojianimated",
                TagType.Sticker => "sticker",
                TagType.Print => "gallery",
                _ => "gallery"
            };

            try
            {
                using var stream = System.IO.File.OpenRead(path);
                var response = await ExecuteWithReloginAsync(() => Auth.ApiClient.UploadImageAsync(
                    stream,
                    Path.GetFileName(path),
                    GetMimeType(path),
                    vrcTag,
                    maskTag: string.IsNullOrEmpty(maskTag) ? null : maskTag,
                    animationStyle: string.IsNullOrEmpty(animationStyle) ? null : animationStyle,
                    frames: frames > 0 ? frames : null,
                    framesOverTime: framesOverTime > 0 ? framesOverTime : null
                ));

                apiData.IdImageUploaded = response.Id;
                apiData.Tags = response.Tags != null ? string.Join(", ", response.Tags) : "";
                if (tag == TagType.EmojiAnimated && !apiData.Tags.Contains("animated"))
                {
                    apiData.Tags = string.IsNullOrEmpty(apiData.Tags) ? "animated" : apiData.Tags + ", animated";
                }
                apiData.Frames = response.Frames.ToString();
                apiData.FramesOverTime = response.FramesOverTime.ToString();
                apiData.AnimationStyle = response.AnimationStyle ?? "";
                apiData.MaskTag = response.MaskTag ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading image: {ex.Message}");
                throw;
            }

            return apiData;
        }

        public async Task<ApiDataPrint> UploadPrint(string path, string note)
        {
            var apiData = new ApiDataPrint();
            try
            {
                using var stream = System.IO.File.OpenRead(path);
                var response = await ExecuteWithReloginAsync(() => Auth.ApiClient.UploadPrintAsync(
                    stream,
                    Path.GetFileName(path),
                    GetMimeType(path),
                    DateTime.UtcNow,
                    note: note
                ));

                apiData.IdImageUploaded = response.Id;
                apiData.AuthorId = response.AuthorId ?? "";
                apiData.AuthorName = response.AuthorName ?? "";
                apiData.FileId = response.Files?.FileId ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return apiData;
        }

        public async Task<ApiDataPrint> GetPrintInfo(string printId)
        {
            var apiData = new ApiDataPrint();
            try
            {
                var response = await ExecuteWithReloginAsync(() => Auth.ApiClient.GetPrintAsync(printId));

                apiData.IdImageUploaded = response.Id;
                apiData.AuthorId = response.AuthorId ?? "";
                apiData.AuthorName = response.AuthorName ?? "";
                apiData.FileId = response.Files?.FileId ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching print info: {ex.Message}");
            }
            return apiData;
        }

        public async Task<ApiData> DeleteApiData(string id)
        {
            var apiData = new ApiData();

            try
            {
                if (id.StartsWith("inv_"))
                {
                    await ExecuteWithReloginAsync(() => Auth.ApiClient.DeleteInventoryItemAsync(id));
                }
                else
                {
                    await ExecuteWithReloginAsync(() => Auth.ApiClient.DeleteFileAsync(id));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return apiData;
        }

        public async Task<ApiData> DeleteApiDataPrint(string id)
        {
            var apiData = new ApiData();

            try
            {
                await ExecuteWithReloginAsync(() => Auth.ApiClient.DeletePrintAsync(id));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return apiData;
        }

        public async Task SetProfileIcon(string urlImage)
        {
            try
            {
                var user = await ExecuteWithReloginAsync(() => Auth.GetCurrentUserAsync());
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

                await ExecuteWithReloginAsync(() => Auth.ApiClient.UpdateUserAsync(user.Id, updateRequest));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public async Task SetProfilePicture(string urlImage)
        {
            try
            {
                var user = await ExecuteWithReloginAsync(() => Auth.GetCurrentUserAsync());
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

                await ExecuteWithReloginAsync(() => Auth.ApiClient.UpdateUserAsync(user.Id, updateRequest));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public async Task<ApiDataWorld?> GetWorldInfo(string worldId)
        {
            try
            {
                var world = await ExecuteWithReloginAsync(() => Auth.ApiClient.GetWorldAsync(worldId));
                return new ApiDataWorld
                {
                    Id = world.Id,
                    Name = world.Name,
                    Description = world.Description,
                    AuthorId = world.AuthorId,
                    ImageUrl = world.ImageUrl,
                    ThumbnailUrl = world.ThumbnailImageUrl
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiDataInventory?> GetInventoryInfo(string userId, string inventoryId)
        {
            try
            {
                var inventory = await ExecuteWithReloginAsync(() => Auth.ApiClient.GetUserInventoryItemAsync(userId, inventoryId));
                if (inventory == null) return null;

                return new ApiDataInventory
                {
                    Collections = inventory.Collections,
                    CreatedAt = inventory.CreatedAt,
                    Description = inventory.Description,
                    ExpiryDate = inventory.ExpiryDate,
                    Flags = inventory.Flags,
                    HolderId = inventory.HolderId,
                    Id = inventory.Id,
                    ImageUrl = inventory.ImageUrl,
                    IsArchived = inventory.IsArchived,
                    IsSeen = inventory.IsSeen,
                    ItemType = inventory.ItemType,
                    ItemTypeLabel = inventory.ItemTypeLabel,
                    Metadata = inventory.Metadata ?? new InventoryMetadata(),
                    Name = inventory.Name,
                    Tags = inventory.Tags,
                    TemplateId = inventory.TemplateId ?? "",
                    TemplateCreatedAt = inventory.TemplateCreatedAt,
                    TemplateUpdatedAt = inventory.TemplateUpdatedAt,
                    UpdatedAt = inventory.UpdatedAt
                };
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
            if (_userNameCache.TryGetValue(userId, out string? name)) return name;

            try
            {
                var user = await ExecuteWithReloginAsync(() => Auth.ApiClient.GetUserAsync(userId));
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
