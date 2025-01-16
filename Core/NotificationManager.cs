using VRCGalleryManager.Design;
using Timer = System.Windows.Forms.Timer;

namespace VRCGalleryManager.Core
{
    public static class NotificationManager
    {
        public static void ShowNotification(string message, string title, NotificationType type)
        {
            Form mainForm = Application.OpenForms["MainPanel"];

            int panelWidth = 310;
            int panelHeight = 80;

            RoundedPanel notificationPanel = new RoundedPanel
            {
                BorderRadius = 10,
                Size = new Size(panelWidth, panelHeight),
                BackColor = type == NotificationType.Success ? Color.LightGreen : Color.LightCoral,
                Location = new Point(-panelWidth, mainForm.ClientSize.Height - 90),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(10, 10)
            };
            notificationPanel.Controls.Add(titleLabel);

            Label messageLabel = new Label
            {
                Text = message,
                Font = new Font("Arial", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                MaximumSize = new Size(panelWidth - 20, 0),
                Location = new Point(10, 40)
            };
            notificationPanel.Controls.Add(messageLabel);

            mainForm.Controls.Add(notificationPanel);
            notificationPanel.BringToFront();

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
                Timer slideOutTimer = new Timer { Interval = 5 };
                slideOutTimer.Tick += (s2, e2) =>
                {
                    if (notificationPanel.Location.X > -panelWidth)
                        notificationPanel.Location = new Point(notificationPanel.Location.X - 20, notificationPanel.Location.Y);
                    else
                    {
                        slideOutTimer.Stop();
                        mainForm.Controls.Remove(notificationPanel);
                        notificationPanel.Dispose();
                    }
                };
                slideOutTimer.Start();
            };
            closeTimer.Start();
        }
    }

    public enum NotificationType
    {
        Success,
        Error
    }
}
