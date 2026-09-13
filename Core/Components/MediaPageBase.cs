using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Threading.Tasks;
using VRCGalleryManager.Core.DTO;

namespace VRCGalleryManager.Core.Components
{
    public abstract class MediaPageBase : ComponentBase, IDisposable
    {
        [Inject] protected VRCAuth Auth { get; set; }
        [Inject] protected IJSRuntime JSRuntime { get; set; }
        [Inject] protected NotificationService NotificationService { get; set; }
        [Inject] protected DialogService dialogService { get; set; }
        [Inject] protected FileDropService FileDropService { get; set; }
        [Inject] protected ApiRequest apiRequest { get; set; }
        [Inject] protected MediaCacheService CacheService { get; set; }
        [Inject] protected NetworkStatusService NetworkStatus { get; set; }
        [Inject] protected ImageViewerService ViewerService { get; set; } = default!;

        protected bool isLoading = true;
        protected bool isRefreshing = false;
        protected bool hasNetworkError = false;
        protected string errorMessage = "";
        private int _imageCount = 0;
        protected int imageCount
        {
            get => _imageCount;
            set
            {
                _imageCount = value;
                UpdateDropAllowedState();
            }
        }
        
        protected string editingBase64Image = null;
        protected bool isDragging = false;

        [Parameter]
        [SupplyParameterFromQuery]
        public string uploadPath { get; set; }

        protected override async Task OnInitializedAsync()
        {
            FileDropService.OnDragEnter += HandleDragEnter;
            FileDropService.OnDragLeave += HandleDragLeave;
            FileDropService.OnFileDropped += HandleFileDropped;
            NetworkStatus.OnNetworkStatusChanged += HandleBaseNetworkStatusChanged;
            
            await LoadInitialDataAsync();
            UpdateDropAllowedState();

            if (!string.IsNullOrEmpty(uploadPath) && File.Exists(uploadPath))
            {
                if (imageCount >= MaxImageCount)
                {
                    NotificationService.Show(
                        $"This category is full ({imageCount}/{MaxImageCount}). Please delete an existing item before uploading a new one.",
                        "Limit Reached",
                        NotificationType.Error,
                        6000);
                }
                else
                {
                    await LoadLocalFileForEditing(uploadPath);
                }
            }
        }

        private void HandleBaseNetworkStatusChanged(bool isOnline)
        {
            if (isOnline && hasNetworkError)
            {
                InvokeAsync(async () =>
                {
                    hasNetworkError = false;
                    errorMessage = "";
                    await RefreshList();
                });
            }
        }

        public virtual async Task RetryLoadAsync()
        {
            hasNetworkError = false;
            errorMessage = "";
            await RefreshList();
        }

        protected abstract Task LoadInitialDataAsync();
        
        protected abstract Task RefreshList();
        
        protected abstract int MaxImageCount { get; }
        protected virtual bool SupportsAnimatedGif => false;

        protected async Task OnDropFile(InputFileChangeEventArgs e)
        {
            isDragging = false;
            if (imageCount >= MaxImageCount) return;
            await OnInputFileChange(e);
        }

        protected async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            if (imageCount >= MaxImageCount) return;
            
            var file = e.File;
            if (file != null && file.ContentType.StartsWith("image/"))
            {
                await LoadFileForEditing(file);
            }
        }

        protected async Task PasteFromClipboard()
        {
            if (imageCount >= MaxImageCount) return;
            
            try
            {
                string savedPath = await ClipboardHandler.ClipboardDataImageOrLink();
                if (!string.IsNullOrEmpty(savedPath) && File.Exists(savedPath))
                {
                    await LoadLocalFileForEditing(savedPath);
                }
                else
                {
                    NotificationService.Show("No image found or invalid link in clipboard.", "Clipboard Empty", NotificationType.Info);
                }
            }
            catch (Exception ex)
            {
                NotificationService.Show(ex.Message, "Paste Error", NotificationType.Error);
            }
        }

        protected virtual async Task LoadFileForEditing(IBrowserFile file)
        {
            try
            {
                bool isGif = file.ContentType.Contains("gif", StringComparison.OrdinalIgnoreCase) || 
                             file.Name.EndsWith(".gif", StringComparison.OrdinalIgnoreCase);

                if (SupportsAnimatedGif && isGif)
                {
                    using var stream = file.OpenReadStream(maxAllowedSize: 20 * 1024 * 1024);
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    byte[] bytes = memoryStream.ToArray();
                    string base64String = Convert.ToBase64String(bytes);
                    editingBase64Image = $"data:image/gif;base64,{base64String}";
                    return;
                }

                // Resize image natively to avoid OutOfMemoryException with large base64 strings
                var resizedFile = await file.RequestImageFileAsync("image/png", 2048, 2048);
                
                using var stream2 = resizedFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
                using var memoryStream2 = new MemoryStream();
                await stream2.CopyToAsync(memoryStream2);
                
                if (isGif)
                {
                    NotificationService.Show("GIFs are only supported for Emojis. The image was converted to a static format.", "GIF Converted", NotificationType.Info);
                }
                
                byte[] resizedBytes = memoryStream2.ToArray();
                string resizedBase64 = Convert.ToBase64String(resizedBytes);
                editingBase64Image = $"data:image/png;base64,{resizedBase64}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                NotificationService.Show($"Error loading image: {ex.Message}", "Error", NotificationType.Error);
            }
        }

        protected virtual async Task LoadLocalFileForEditing(string filePath)
        {
            try
            {
                byte[] bytes = await File.ReadAllBytesAsync(filePath);
                string contentType = GetContentType(filePath);

                if (contentType.Contains("gif", StringComparison.OrdinalIgnoreCase) && !SupportsAnimatedGif)
                {
                    NotificationService.Show("GIFs are only supported for Emojis. The image was converted to a static format.", "GIF Converted", NotificationType.Info);
                }

                string base64String = Convert.ToBase64String(bytes);
                editingBase64Image = $"data:{contentType};base64,{base64String}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading local file: {ex.Message}");
            }
        }
        
        protected string GetContentType(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            return ext switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "image/png"
            };
        }

        protected void OnEditorCancel()
        {
            editingBase64Image = null;
        }

        protected async Task<string> SaveBase64ToTempFileAsync(string base64String, string filePrefix = "upload")
        {
            string mimeType = "image/png";
            int semiColonIndex = base64String.IndexOf(";");
            if (base64String.StartsWith("data:") && semiColonIndex > 5)
            {
                mimeType = base64String.Substring(5, semiColonIndex - 5);
            }
            string ext = mimeType == "image/jpeg" ? ".jpg" : ".png";

            int commaIndex = base64String.IndexOf(",");
            string base64Data = commaIndex >= 0 ? base64String.Substring(commaIndex + 1) : base64String;
            byte[] bytes = Convert.FromBase64String(base64Data);

            string tempFolder = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(tempFolder);
            string tempPath = Path.Combine(tempFolder, $"{filePrefix}_{Guid.NewGuid():N}{ext}");

            await File.WriteAllBytesAsync(tempPath, bytes);
            return tempPath;
        }

        protected async Task ConfirmAndDeleteItemAsync<T>(
            T item,
            string itemId,
            string itemName,
            List<T> itemList,
            string cacheKey,
            string countCacheKey,
            Func<string, Task> deleteApiCall)
        {
            bool confirm = await dialogService.ShowConfirmAsync($"Delete {itemName}", $"Are you sure you want to delete this {itemName.ToLower()}?", "Delete", "Cancel");
            if (confirm)
            {
                try
                {
                    await deleteApiCall(itemId);
                    itemList.Remove(item);
                    imageCount--;
                    CacheService.Set(cacheKey, new List<T>(itemList));
                    CacheService.Set(countCacheKey, imageCount);
                    StateHasChanged();
                    NotificationService.Show($"{itemName} deleted successfully.", "Deleted", NotificationType.Success);
                }
                catch (Exception ex)
                {
                    NotificationService.Show(ex.Message, "Delete Failed", NotificationType.Error);
                }
            }
        }

        protected async Task HandleViewerDeleteAsync<T>(int index, List<T> itemList, Func<T, Task> deleteAction, VRCGalleryManager.Components.ImageViewer imageViewer = null)
        {
            if (index >= 0 && index < itemList.Count)
            {
                var item = itemList[index];
                await deleteAction(item);
                if (!itemList.Contains(item))
                {
                    if (imageViewer != null)
                    {
                        imageViewer.Close();
                    }
                    else
                    {
                        ViewerService.Close();
                    }
                }
            }
        }

        protected void UpdateDropAllowedState()
        {
            if (FileDropService != null)
            {
                FileDropService.IsDropAllowed = imageCount < MaxImageCount;
            }
        }

        public virtual void Dispose()
        {
            if (FileDropService != null)
            {
                FileDropService.IsDropAllowed = true;
                FileDropService.OnDragEnter -= HandleDragEnter;
                FileDropService.OnDragLeave -= HandleDragLeave;
                FileDropService.OnFileDropped -= HandleFileDropped;
            }
            NetworkStatus.OnNetworkStatusChanged -= HandleBaseNetworkStatusChanged;
        }

        protected void HandleDragEnter()
        {
            if (imageCount >= MaxImageCount) return;

            InvokeAsync(() =>
            {
                isDragging = true;
                StateHasChanged();
            });
        }

        protected void HandleDragLeave()
        {
            InvokeAsync(() =>
            {
                isDragging = false;
                StateHasChanged();
            });
        }

        protected void HandleFileDropped(string[] files)
        {
            InvokeAsync(async () =>
            {
                isDragging = false;
                if (imageCount >= MaxImageCount)
                {
                    NotificationService.Show(
                        $"This category is full ({imageCount}/{MaxImageCount}). Please delete an existing item before uploading.",
                        "Limit Reached",
                        NotificationType.Error,
                        5000);
                    return;
                }

                if (files != null && files.Length > 0)
                {
                    var filePath = files[0];
                    string ext = Path.GetExtension(filePath).ToLower();
                    if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".webp" || ext == ".gif")
                    {
                        await LoadLocalFileForEditing(filePath);
                    }
                }
                StateHasChanged();
            });
        }

        protected string? activeExpandedCardId = null;

        protected void HandleCardContextMenu(string? id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                activeExpandedCardId = id;
                StateHasChanged();
            }
        }

        protected void HandleCardMouseLeave(string? id)
        {
            if (activeExpandedCardId != null && activeExpandedCardId == id)
            {
                activeExpandedCardId = null;
                StateHasChanged();
            }
        }

        protected async Task CopyImageToClipboardAsync(string source)
        {
            if (string.IsNullOrEmpty(source)) return;
            try
            {
                bool success = false;
                if (source.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    string tempPath = await ClipboardHandler.SaveImageFromUrlAsync(source, false);
                    if (!string.IsNullOrEmpty(tempPath))
                    {
                        success = await ClipboardHandler.CopyImageToClipboardAsync(tempPath);
                    }
                }
                else
                {
                    success = await ClipboardHandler.CopyImageToClipboardAsync(source);
                }

                if (success)
                {
                    NotificationService.Show("Image copied to clipboard.", "Copied", NotificationType.Success);
                }
                else
                {
                    NotificationService.Show("Failed to copy image.", "Error", NotificationType.Error);
                }
            }
            catch (Exception ex)
            {
                NotificationService.Show($"Failed to copy image: {ex.Message}", "Error", NotificationType.Error);
            }
        }

        protected async Task DownloadImageFileAsync(string source, string fileName)
        {
            if (string.IsNullOrEmpty(source)) return;
            try
            {
                if (string.IsNullOrWhiteSpace(fileName)) fileName = "download";

                byte[] fileBytes;
                string ext;

                if (source.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    var (downloadedBytes, downloadedExt) = await FileHelper.DownloadBytesFromUrlAsync(source);
                    if (downloadedBytes == null || downloadedBytes.Length == 0)
                    {
                        NotificationService.Show("Failed to download image file.", "Error", NotificationType.Error);
                        return;
                    }
                    fileBytes = downloadedBytes;
                    ext = downloadedExt;
                }
                else
                {
                    if (!File.Exists(source))
                    {
                        NotificationService.Show("Source file not found.", "Error", NotificationType.Error);
                        return;
                    }
                    ext = Path.GetExtension(source);
                    if (string.IsNullOrEmpty(ext)) ext = ".png";
                    fileBytes = await File.ReadAllBytesAsync(source);
                }

                var (saved, destination) = await FileHelper.SaveImageToDownloadsAsync(fileName, fileBytes, ext);
                if (saved)
                {
                    string successMsg = OperatingSystem.IsAndroid()
                        ? "Image saved directly to Downloads."
                        : "Image saved successfully.";
                    NotificationService.Show(successMsg, "Saved", NotificationType.Success);

                    if (!string.IsNullOrEmpty(destination))
                    {
                        string mime = ext.ToLowerInvariant() switch
                        {
                            ".jpg" or ".jpeg" => "image/jpeg",
                            ".png" => "image/png",
                            ".gif" => "image/gif",
                            ".webp" => "image/webp",
                            _ => "image/*"
                        };
                        await FileHelper.OpenFileAsync(destination, mime);
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Show($"Download failed: {ex.Message}", "Error", NotificationType.Error);
            }
        }

        protected async Task CopyLinkToClipboardAsync(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            try
            {
                bool success = false;
                try
                {
                    success = await JSRuntime.InvokeAsync<bool>("clipboardInterop.writeText", url);
                }
                catch { }

                if (!success)
                {
                    await Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.Default.SetTextAsync(url);
                    success = true;
                }

                if (success)
                {
                    NotificationService.Show("Link copied to clipboard.", "Copied", NotificationType.Success);
                }
            }
            catch (Exception ex)
            {
                NotificationService.Show($"Failed to copy link: {ex.Message}", "Error", NotificationType.Error);
            }
        }
    }
}
