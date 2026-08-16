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

        protected ApiRequest apiRequest;
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
            
            apiRequest = new ApiRequest(Auth);
            
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
