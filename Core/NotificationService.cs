using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VRCGalleryManager.Core
{
    public enum NotificationType
    {
        Success,
        Info,
        Error
    }

    public class NotificationService
    {
        private readonly List<NotificationItem> _notifications = new();
        public IReadOnlyList<NotificationItem> Notifications => _notifications;

        public event Action? OnChanged;

        public void Show(string message, string title, NotificationType type, int durationMs = 4000)
        {
            var item = new NotificationItem
            {
                Id = Guid.NewGuid(),
                Title = title,
                Message = message,
                Type = type,
                IsLeaving = false
            };

            _notifications.Add(item);
            OnChanged?.Invoke();

            _ = AutoDismiss(item, durationMs);
        }

        private async Task AutoDismiss(NotificationItem item, int durationMs)
        {
            await Task.Delay(durationMs);
            await BeginDismiss(item);
        }

        public async Task BeginDismiss(NotificationItem item)
        {
            if (!_notifications.Contains(item)) return;
            item.IsLeaving = true;
            OnChanged?.Invoke();

            // Wait for CSS slide-out animation to complete
            await Task.Delay(400);
            _notifications.Remove(item);
            OnChanged?.Invoke();
        }
    }

    public class NotificationItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public NotificationType Type { get; set; }
        public bool IsLeaving { get; set; }
    }
}
