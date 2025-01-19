using System.Diagnostics;
using VRCGalleryManager.Core;
using VRCGalleryManager.Design;
using VRCGalleryManager.Forms.UIComponents;

namespace VRCGalleryManager.Forms.Panels
{
    public class ImagePanel
    {
        static public async void AddImagePanel(FlowLayoutPanel mainPanel, ApiRequest apiRequest, string imageId, string tags, string frames, string framesOverTime)
        {
            //* IMAGE Animated PANEL
            RoundedPictureBox pictureBox = new RoundedPictureBox
            {
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(24, 27, 31),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderRadiusBottomLeft = 10,
                BorderRadiusBottomRight = 10,
                BorderRadiusTopLeft = 10,
                BorderRadiusTopRight = 10,
                BorderColor = Color.FromArgb(24, 27, 31),
                BorderSize = 5,
                Padding = new Padding(5)
            };
            string image = tags.Contains("animated")
                ? $"https://api.vrchat.cloud/api/1/file/{imageId}/1/file"
                : $"https://api.vrchat.cloud/api/1/image/{imageId}/1/256";

            string finalaviImage = await HttpImage.GetFinalUrlAsync(image);

            if (!finalaviImage.Contains("imageNotFound"))
            {
                if (pictureBox.Controls["btn_open"] == null)
                {
                    CircularButton btn_open = CircularButtonTools.CreateButton("open", (sender, e) =>
                    {
                        Process.Start("explorer.exe", $"https://api.vrchat.cloud/api/1/file/{imageId}/1/file");
                    });

                    btn_open.Location = new Point(pictureBox.Size.Width - 60, pictureBox.Size.Height - 35);

                    if (tags.Contains("animated"))
                    {
                        SpriteSheetViewer viewer = new SpriteSheetViewer(pictureBox);
                        await viewer.LoadSpriteSheetAsync(finalaviImage, int.Parse(frames), int.Parse(framesOverTime));
                        viewer.StartAnimation();
                    }
                    else
                    {
                        pictureBox.LoadAsync(finalaviImage);
                    }

                    pictureBox.Controls.Add(btn_open);
                }
            }

            if (pictureBox.Controls["btn_delete"] == null)
            {
                CircularButton btn_delete = CircularButtonTools.CreateButton("delete", async (sender, e) =>
                {
                    DialogResult result = DialogMessage.ShowDeleteFileDialog(imageId);

                    if (result == DialogResult.Yes)
                    {
                        Debug.WriteLine("Delete: " + imageId);
                        await apiRequest.DeleteApiData(imageId);
                        mainPanel.Controls.Remove(pictureBox);
                    }
                });
                btn_delete.Location = new Point(pictureBox.Size.Width - 35, pictureBox.Size.Height - 35);
                pictureBox.Controls.Add(btn_delete);
            }

            mainPanel.Controls.Add(pictureBox);
        }
        static public async void AddImagePanel(FlowLayoutPanel mainPanel, ApiRequest apiRequest, string imageId)
        {
            Size size = new Size(150, 150);
            if (mainPanel.Name.Contains("prints")) size = new Size(190, 150);
            if (mainPanel.Name.Contains("gallery")) size = new Size(250, 150);

            int value = 10;
            if (mainPanel.Name.Contains("icons")) value = 100; //Max Rounded

            //* IMAGE Static PANEL
            RoundedPictureBox pictureBox = new RoundedPictureBox
            {
                Size = size,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(24, 27, 31),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderRadiusBottomLeft = value,
                BorderRadiusBottomRight = 10,
                BorderRadiusTopLeft = value,
                BorderRadiusTopRight = value,
                BorderColor = Color.FromArgb(24, 27, 31),
                BorderSize = 5,
                Padding = new Padding(5)
            };
            string image = $"https://api.vrchat.cloud/api/1/image/{imageId}/1/256";
            string finalaviImage = await HttpImage.GetFinalUrlAsync(image);

            if (!finalaviImage.Contains("imageNotFound"))
            {
                if (pictureBox.Controls["btn_open"] == null)
                {
                    CircularButton btn_open = CircularButtonTools.CreateButton("open", (sender, e) =>
                    {
                        Process.Start("explorer.exe", $"https://api.vrchat.cloud/api/1/file/{imageId}/1/file");
                    });

                    btn_open.Location = new Point(pictureBox.Size.Width - 60, pictureBox.Size.Height - 35);

                    pictureBox.LoadAsync(finalaviImage);

                    pictureBox.Controls.Add(btn_open);
                }
            }
            if (pictureBox.Controls["btn_delete"] == null)
            {
                CircularButton btn_delete = CircularButtonTools.CreateButton("delete", async (sender, e) =>
                {
                    DialogResult result = DialogMessage.ShowDeleteFileDialog(imageId);

                    if (result == DialogResult.Yes)
                    {
                        Debug.WriteLine("Delete: " + imageId);
                        await apiRequest.DeleteApiData(imageId);
                        mainPanel.Controls.Remove(pictureBox);
                    }
                });
                btn_delete.Location = new Point(pictureBox.Size.Width - 35, pictureBox.Size.Height - 35);
                pictureBox.Controls.Add(btn_delete);
            }

            mainPanel.Controls.Add(pictureBox);
        }
    }
}
