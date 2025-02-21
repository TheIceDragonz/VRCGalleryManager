using VRCGalleryManager.Design;
using Timer = System.Windows.Forms.Timer;

namespace VRCGalleryManager.Core
{
    public static class NotificationManager
    {
        private static readonly List<RoundedPanel> ActiveNotifications = new List<RoundedPanel>();
        private const int PanelWidth = 350;
        private const int PanelHeight = 100;
        private const int Spacing = 10;

        public static void ShowNotification(string message, string title, NotificationType type)
        {
            Form mainForm = Application.OpenForms["MainPanel"];

            // Seleziona i colori in base al tipo di notifica
            Color borderColor;
            Color textColor;
            switch (type)
            {
                case NotificationType.Success:
                    borderColor = Color.LightGreen;
                    textColor = Color.LightGreen;
                    break;
                case NotificationType.Info:
                    borderColor = Color.LightBlue;
                    textColor = Color.LightBlue;
                    break;
                case NotificationType.Error:
                default:
                    borderColor = Color.LightCoral;
                    textColor = Color.LightCoral;
                    break;
            }

            RoundedPanel notificationPanel = new RoundedPanel
            {
                BorderRadius = 10,
                BorderSize = 3,
                BorderColor = borderColor,
                Size = new Size(PanelWidth, PanelHeight),
                BackColor = Color.FromArgb(5, 5, 5),
                Location = new Point(-PanelWidth, mainForm.ClientSize.Height - (PanelHeight + Spacing)),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = textColor,
                AutoSize = true,
                Location = new Point(10, 10)
            };
            notificationPanel.Controls.Add(titleLabel);

            Label messageLabel = new Label
            {
                Text = message,
                Font = new Font("Arial", 10),
                ForeColor = textColor,
                AutoSize = true,
                MaximumSize = new Size(PanelWidth - 20, 0),
                Location = new Point(10, 40)
            };
            notificationPanel.Controls.Add(messageLabel);

            mainForm.Controls.Add(notificationPanel);
            notificationPanel.BringToFront();

            lock (ActiveNotifications)
            {
                ActiveNotifications.Add(notificationPanel);
                AdjustNotificationPositions();
            }

            Timer slideInTimer = new Timer { Interval = 5 };
            slideInTimer.Tick += (s, e) =>
            {
                if (notificationPanel.Location.X < 10)
                    notificationPanel.Location = new Point(notificationPanel.Location.X + 20, notificationPanel.Location.Y);
                else
                    slideInTimer.Stop();
            };
            slideInTimer.Start();

            Timer closeTimer = new Timer { Interval = 3000 };
            closeTimer.Tick += (s, e) =>
            {
                closeTimer.Stop();
                RemoveNotification(notificationPanel);
            };
            closeTimer.Start();
        }

        private static void RemoveNotification(RoundedPanel notificationPanel)
        {
            Timer slideOutTimer = new Timer { Interval = 5 };
            slideOutTimer.Tick += (s, e) =>
            {
                if (notificationPanel.Location.X > -PanelWidth)
                    notificationPanel.Location = new Point(notificationPanel.Location.X - 20, notificationPanel.Location.Y);
                else
                {
                    slideOutTimer.Stop();
                    lock (ActiveNotifications)
                    {
                        ActiveNotifications.Remove(notificationPanel);
                        notificationPanel.Parent.Controls.Remove(notificationPanel);
                        notificationPanel.Dispose();
                        AdjustNotificationPositions();
                    }
                }
            };
            slideOutTimer.Start();
        }

        private static void AdjustNotificationPositions()
        {
            lock (ActiveNotifications)
            {
                int currentY = Application.OpenForms["MainPanel"].ClientSize.Height - (PanelHeight + Spacing);
                foreach (var panel in ActiveNotifications)
                {
                    panel.Location = new Point(panel.Location.X, currentY);
                    currentY -= (PanelHeight + Spacing);
                }
            }
        }
    }

    public enum NotificationType
    {
        Success,
        Info,
        Error
    }
}
