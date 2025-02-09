using System.Windows.Forms;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Forms.Panels;

namespace VRCGalleryManager.Forms
{
    public partial class Create : Form
    {
        private static string MASK_TAG = "square";
        private static string ANIMATION_STYLE = "";

        private readonly GifToSpriteSheetConverter converter;
        SpriteSheetViewer viewer;

        private ApiRequest apiRequest;

        private string gifPath;
        private Bitmap spriteSheet;

        private int imageframes = 0;

        public Create(VRCAuth auth)
        {
            apiRequest = new ApiRequest(auth);

            InitializeComponent();

            converter = new GifToSpriteSheetConverter();

            TypePanel.LoadEmojiType(createOpenTypePanel, createTypePanel);

            trackBarFPS.LabelText = trackBarFPS.Value.ToString();
        }

        private void File_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null || files.Length == 0) return;

            gifPath = files[0];
            previewGif.ImageLocation = gifPath;

            try
            {
                var (spriteSheetBitmap, frameCount) = converter.ConvertGifToSpriteSheet(gifPath);

                spriteSheet = spriteSheetBitmap;
                previewSS.Image = spriteSheet;

                imageframes = frameCount;

                buttonSave.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void File_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                e.Effect = Path.GetExtension(files[0]).ToLower() == ".gif" ? DragDropEffects.Copy : DragDropEffects.None;
            }
        }

        private void buttonLocalSave_Click(object sender, EventArgs e)
        {
            if (spriteSheet != null)
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Title = "Save Sprite Sheet";
                    saveDialog.Filter = "PNG Image|*.png";
                    saveDialog.FileName = Path.GetFileNameWithoutExtension(gifPath) + ".png";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            converter.SaveSpriteSheet(saveDialog.FileName);
                            MessageBox.Show("Sprite sheet saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            NotificationManager.ShowNotification(ex.Message, "Error", NotificationType.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No sprite sheet available to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void creatorUpload_Click(object sender, EventArgs e)
        {
            if (spriteSheet != null)
            {
                if (!createOpenTypePanel.Text.Contains("Type"))
                {
                    try
                    {
                        string image = converter.SaveTempSpriteSheet();

                        ANIMATION_STYLE = createOpenTypePanel.Text.ToLower();

                        ApiRequest.ApiData emoji = await apiRequest.UploadImage(image, MASK_TAG, TagType.EmojiAnimated, ANIMATION_STYLE, imageframes, trackBarFPS.Value);

                        NotificationManager.ShowNotification("File uploaded successfully", "File upload", NotificationType.Success);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error during file upload", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    DialogMessage.ShowMissingTypeDialog(this);
                }
            }
            else
            {
                DialogMessage.ShowMissingSpriteSheet(this);
            }
        }


        private void trackBarFPS_ValueChanged(object sender, EventArgs e)
        {
            trackBarFPS.LabelText = trackBarFPS.Value.ToString();
            if (viewer != null) viewer.UpdateFPS(trackBarFPS.Value);
        }

        private void createOpenTypePanel_Click(object sender, EventArgs e)
        {
            createTypePanel.Visible = !createTypePanel.Visible;
        }

        private async void pasteButton_Click(object sender, EventArgs e)
        {
            try
            {
                pasteButton.Enabled = false;

                (string gifPath, Bitmap spriteSheet, int frameCount) = await converter.ProcessGifFromClipboard();

                this.gifPath = gifPath;
                this.spriteSheet = spriteSheet;
                previewGif.ImageLocation = gifPath;
                previewSS.Image = spriteSheet;
                imageframes = frameCount;

                VRChatPreview();

                NotificationManager.ShowNotification(
                    "GIF processed and sprite sheet generated successfully!",
                    "Paste Success",
                    NotificationType.Success
                );
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification(
                    ex.Message,
                    "Error processing GIF",
                    NotificationType.Error
                );
            }
            finally
            {
                pasteButton.Enabled = true;
            }
        }

        private async void VRChatPreview()
        {
            if (spriteSheet != null)
            {
                viewer = new SpriteSheetViewer(previewVRChat);
                await viewer.LoadSpriteSheetAsync(spriteSheet, imageframes, trackBarFPS.Value);
                viewer.StartAnimation();
            }
        }
    }
}
