using VRCGalleryManager.Design;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace VRCGalleryManager.Core
{
    public static class DialogMessage
    {
        public static DialogResult ShowCreateFolderDialog()
        {
            return MessageBox.Show(
                "The folder does not exist. Do you want to create it?",
                "Create Folder",
                MessageBoxButtons.YesNo
            );
        }

        public static DialogResult ShowReplaceFileDialog()
        {
            return MessageBox.Show(
                "The file already exists. Do you want to replace it?",
                "Replace File",
                MessageBoxButtons.YesNo
            );
        }

        public static DialogResult ShowSaveFileDialog(string apiDataName, string dataID)
        {
            return MessageBox.Show(
                "Do you want to save this file?\n\n" +
                "Name: " + apiDataName + "\n" +
                "ID: " + dataID,
                "Confirm Save !!",
                MessageBoxButtons.YesNo
            );
        }

        public static DialogResult ShowDeleteFileDialog(string dataID)
        {
            return MessageBox.Show(
                "Are you sure to delete this image?\n\n" +
                "ID: " + dataID,
                "Confirm Delete !!",
                MessageBoxButtons.YesNo
            );
        }

        public static void ShowMissingTypeDialog(Form parentForm)
        {
            NotificationManager.ShowNotification(
                "Please select it before uploading.",
                "Missing Type",
                NotificationType.Error
            );
        }

        public static void ShowMissingGif(Form parentForm)
        {
            NotificationManager.ShowNotification(
                "GIF not found",
                "Error",
                NotificationType.Error
            );
        }

        public static void ShowMissingSpriteSheet(Form parentForm)
        {
            NotificationManager.ShowNotification(
                "Sprite sheet not found",
                "Error",
                NotificationType.Error
            );
        }

        public static void ShowValidURL(Form parentForm)
        {
            NotificationManager.ShowNotification(
                "Please enter a valid URL",
                "Error",
                NotificationType.Error
            );
        }
    }
}
