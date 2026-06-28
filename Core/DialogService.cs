using System;
using System.Threading.Tasks;

namespace VRCGalleryManager.Core
{
    public class DialogRequest
    {
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string ConfirmText { get; set; } = "Yes";
        public string CancelText { get; set; } = "No";
        public TaskCompletionSource<bool>? Tcs { get; set; }
    }

    public class DialogService
    {
        public event Action<DialogRequest>? OnShow;
        public event Action? OnHide;

        public Task<bool> ShowConfirmAsync(string title, string message, string confirmText = "Yes", string cancelText = "No")
        {
            var tcs = new TaskCompletionSource<bool>();

            var request = new DialogRequest
            {
                Title = title,
                Message = message,
                ConfirmText = confirmText,
                CancelText = cancelText,
                Tcs = tcs
            };

            OnShow?.Invoke(request);
            return tcs.Task;
        }

        public void Hide()
        {
            OnHide?.Invoke();
        }
    }
}
