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

        public static DialogResult ShowMissingTypeDialog()
        {
            return MessageBox.Show(
                "Please select it before uploading.",
                "Missing Type",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
        public static DialogResult ShowMissingGif()
        {
            return MessageBox.Show(
                "GIF not found",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
        public static DialogResult ShowMissingSpriteSheet()
        {
            return MessageBox.Show(
                "Sprite sheet not found",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
        public static DialogResult ShowValidURL()
        {
            return MessageBox.Show(
                "Please enter a valid URL",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
    }
}
