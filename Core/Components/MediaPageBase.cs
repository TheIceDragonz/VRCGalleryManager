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

        protected bool isLoading = true;
        protected bool isRefreshing = false;
        protected int imageCount = 0;
        
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
            
            await LoadInitialDataAsync();

            if (!string.IsNullOrEmpty(uploadPath) && File.Exists(uploadPath))
            {
                await LoadLocalFileForEditing(uploadPath);
            }
        }

        protected abstract Task LoadInitialDataAsync();
        
        protected abstract Task RefreshList();
        
        protected abstract int MaxImageCount { get; }

        protected async Task OnDropFile(InputFileChangeEventArgs e)
        {
            isDragging = false;
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

        protected async Task LoadFileForEditing(IBrowserFile file)
        {
            try
            {
                // Resize image natively to avoid OutOfMemoryException with large base64 strings
                var resizedFile = await file.RequestImageFileAsync("image/png", 2048, 2048);
                
                using var stream = resizedFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                
                if (file.ContentType.Contains("gif", StringComparison.OrdinalIgnoreCase))
                {
                    NotificationService.Show("Le GIF sono supportate solo per le Emoji. L'immagine è stata convertita in formato statico.", "Formato GIF convertito", NotificationType.Info);
                }
                
                byte[] bytes = memoryStream.ToArray();
                string base64String = Convert.ToBase64String(bytes);
                editingBase64Image = $"data:image/png;base64,{base64String}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                NotificationService.Show($"Error loading image: {ex.Message}", "Error", NotificationType.Error);
            }
        }

        protected async Task LoadLocalFileForEditing(string filePath)
        {
            try
            {
                byte[] bytes = await File.ReadAllBytesAsync(filePath);
                string contentType = GetContentType(filePath);

                if (contentType.Contains("gif", StringComparison.OrdinalIgnoreCase))
                {
                    NotificationService.Show("Le GIF sono supportate solo per le Emoji. L'immagine è stata convertita in formato statico.", "Formato GIF convertito", NotificationType.Info);
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

        protected async Task HandleViewerDeleteAsync<T>(int index, List<T> itemList, Func<T, Task> deleteAction, VRCGalleryManager.Components.ImageViewer imageViewer)
        {
            if (index >= 0 && index < itemList.Count)
            {
                var item = itemList[index];
                await deleteAction(item);
                if (!itemList.Contains(item))
                {
                    imageViewer?.Close();
                }
            }
        }

        public virtual void Dispose()
        {
            FileDropService.OnDragEnter -= HandleDragEnter;
            FileDropService.OnDragLeave -= HandleDragLeave;
            FileDropService.OnFileDropped -= HandleFileDropped;
        }

        protected void HandleDragEnter()
        {
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
    }
}
