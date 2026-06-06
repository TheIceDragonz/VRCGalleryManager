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
            if (mainForm == null)
            {
                Timer waitTimer = new Timer { Interval = 100 };
                waitTimer.Tick += (s, e) =>
                {
                    Form mainFormDelayed = Application.OpenForms["MainPanel"];
                    if (mainFormDelayed != null)
                    {
                        waitTimer.Stop();
                        waitTimer.Dispose();
                        ShowNotificationInternal(mainFormDelayed, message, title, type);
                    }
                };
                waitTimer.Start();
            }
            else
            {
                ShowNotificationInternal(mainForm, message, title, type);
            }
        }

        private static void ShowNotificationInternal(Form mainForm, string message, string title, NotificationType type)
        {
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
            notificationPanel.Tag = slideInTimer;
            slideInTimer.Tick += (s, e) =>
            {
                if (notificationPanel.IsDisposed)
                {
                    slideInTimer.Stop();
                    slideInTimer.Dispose();
                    return;
                }

                if (notificationPanel.Location.X < 10)
                {
                    notificationPanel.Location = new Point(notificationPanel.Location.X + 20, notificationPanel.Location.Y);
                }
                else
                {
                    slideInTimer.Stop();
                    slideInTimer.Dispose();
                    if (notificationPanel.Tag == slideInTimer)
                        notificationPanel.Tag = null;

                    StartCloseTimer(notificationPanel);
                }
            };
            slideInTimer.Start();
        }

        private static void StartCloseTimer(RoundedPanel notificationPanel)
        {
            if (notificationPanel.IsDisposed) return;

            Timer closeTimer = new Timer { Interval = 3000 };
            notificationPanel.Tag = closeTimer;
            closeTimer.Tick += (s, e) =>
            {
                closeTimer.Stop();
                closeTimer.Dispose();
                if (!notificationPanel.IsDisposed)
                {
                    if (notificationPanel.Tag == closeTimer)
                        notificationPanel.Tag = null;
                    RemoveNotification(notificationPanel);
                }
            };
            closeTimer.Start();
        }

        private static void RemoveNotification(RoundedPanel notificationPanel)
        {
            if (notificationPanel.IsDisposed) return;

            // Stop any existing animation or close timer
            if (notificationPanel.Tag is Timer existingTimer)
            {
                existingTimer.Stop();
                existingTimer.Dispose();
                notificationPanel.Tag = null;
            }

            Timer slideOutTimer = new Timer { Interval = 5 };
            notificationPanel.Tag = slideOutTimer;
            slideOutTimer.Tick += (s, e) =>
            {
                if (notificationPanel.IsDisposed)
                {
                    slideOutTimer.Stop();
                    slideOutTimer.Dispose();
                    return;
                }

                if (notificationPanel.Location.X > -PanelWidth)
                {
                    notificationPanel.Location = new Point(notificationPanel.Location.X - 20, notificationPanel.Location.Y);
                }
                else
                {
                    slideOutTimer.Stop();
                    slideOutTimer.Dispose();
                    lock (ActiveNotifications)
                    {
                        ActiveNotifications.Remove(notificationPanel);
                        if (notificationPanel.Parent != null)
                        {
                            notificationPanel.Parent.Controls.Remove(notificationPanel);
                        }
                        notificationPanel.Dispose();
                        AdjustNotificationPositions();
                    }
                }
            };
            slideOutTimer.Start();
        }

        private static void AdjustNotificationPositions()
        {
            Form mainForm = Application.OpenForms["MainPanel"];
            if (mainForm == null) return;

            lock (ActiveNotifications)
            {
                int currentY = mainForm.ClientSize.Height - (PanelHeight + Spacing);
                foreach (var panel in ActiveNotifications)
                {
                    if (panel.IsDisposed) continue;
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
