using System.Diagnostics;
using System.Net;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Design;
using VRCGalleryManager.Forms.UIComponents;

namespace VRCGalleryManager.Forms.Panels
{
    public class ImagePanel
    {
        static public async void AddImagePanel(FlowLayoutPanel mainPanel, ApiRequest apiRequest, string imageId, string tags, string frames, string framesOverTime, Action<string> UpdateCounter)
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

                        UpdateCounter("Remove");
                    }
                });
                btn_delete.Location = new Point(pictureBox.Size.Width - 35, pictureBox.Size.Height - 35);
                pictureBox.Controls.Add(btn_delete);
            }

            mainPanel.Controls.Add(pictureBox);
        }

        private static List<RoundedPictureBox> pictureIconList = new List<RoundedPictureBox>();
        private static List<RoundedPictureBox> pictureGalleryList = new List<RoundedPictureBox>();

        static public async void AddImagePanel(FlowLayoutPanel mainPanel, ApiRequest apiRequest, string imageId, Action<string> UpdateCounter)
        {
            Size size = new Size(150, 150);
            if (mainPanel.Name.Contains("prints")) size = new Size(190, 150);
            if (mainPanel.Name.Contains("gallery")) size = new Size(250, 150);

            int value = 10;
            if (mainPanel.Name.Contains("icons")) value = 100; //Max Rounded

            string imageFull = $"https://api.vrchat.cloud/api/1/file/{imageId}/1/file";
            string image256 = $"https://api.vrchat.cloud/api/1/image/{imageId}/1/256";
            string finalaviImage = await HttpImage.GetFinalUrlAsync(image256);

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

            if (mainPanel.Name.Contains("icons"))
            {
                if (Settings.UserIconImage.Contains(imageId)) pictureBox.BorderColor = Color.FromArgb(106, 227, 249);

                pictureBox.Click += async (sender, e) =>
                {
                    await apiRequest.SetProfileIcon(imageFull);

                    foreach (var pb in pictureIconList) pb.BorderColor = Color.FromArgb(24, 27, 31);
                    pictureBox.BorderColor = Color.FromArgb(106, 227, 249);

                    NotificationManager.ShowNotification("Profile Icon", "Profile Icon Updated", NotificationType.Success);
                };
                pictureIconList.Add(pictureBox);
            }
            if (mainPanel.Name.Contains("gallery"))
            {
                if (Settings.UserBannerImage.Contains(imageId)) pictureBox.BorderColor = Color.FromArgb(106, 227, 249);
                pictureBox.Click += async (sender, e) =>
                {
                    await apiRequest.SetProfilePicture(imageFull);

                    foreach (var pb in pictureGalleryList) pb.BorderColor = Color.FromArgb(24, 27, 31);
                    pictureBox.BorderColor = Color.FromArgb(106, 227, 249);

                    NotificationManager.ShowNotification("Profile Picture", "Profile Picture Updated", NotificationType.Success);
                };
                pictureGalleryList.Add(pictureBox);
            }

            if (!finalaviImage.Contains("imageNotFound"))
            {
                if (pictureBox.Controls["btn_open"] == null)
                {
                    CircularButton btn_open = CircularButtonTools.CreateButton("open", (sender, e) =>
                    {
                        Process.Start("explorer.exe", imageFull);
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

                        UpdateCounter("Remove");
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

            string imageFull = $"https://api.vrchat.cloud/api/1/file/{imageId}/1/file";
            string image256 = $"https://api.vrchat.cloud/api/1/image/{imageId}/1/256";
            string finalaviImage = await HttpImage.GetFinalUrlAsync(image256);

            //* IMAGE Static PANEL
            RoundedPictureBox pictureBox = new RoundedPictureBox
            {
                Size = size,
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

            if (!finalaviImage.Contains("imageNotFound"))
            {
                if (pictureBox.Controls["btn_open"] == null)
                {
                    CircularButton btn_open = CircularButtonTools.CreateButton("open", (sender, e) =>
                    {
                        Process.Start("explorer.exe", imageFull);
                    });

                    btn_open.Location = new Point(pictureBox.Size.Width - 60, pictureBox.Size.Height - 35);

                    pictureBox.LoadAsync(finalaviImage);

                    pictureBox.Controls.Add(btn_open);
                }

                if (pictureBox.Controls["btn_picflowupload"] == null)
                {
                    CircularButton btn_picflowupload = CircularButtonTools.CreateButton("picflowupload", async (sender, e) =>
                    {
                        Debug.WriteLine("PicflowUpload: " + imageId);

                        try
                        {
                            string tempFolder = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
                            Directory.CreateDirectory(tempFolder);

                            string localImagePath = Path.Combine(tempFolder, $"downloaded_image_{Guid.NewGuid()}.png");

                            using (WebClient client = new WebClient())
                            {
                                client.Headers.Add("User-Agent", "VRCGalleryManager");
                                client.DownloadFile(new Uri(imageFull), localImagePath);
                            }

                            ApiRequest.ApiData sticker = await apiRequest.UploadImage(localImagePath, "sticker", TagType.Sticker);
                            NotificationManager.ShowNotification("Sticker uploaded successfully", "Sticker uploaded", NotificationType.Success);
                        }
                        catch (Exception ex)
                        {
                            NotificationManager.ShowNotification(ex.Message, "Error during file upload", NotificationType.Error);
                        }
                    });
                    btn_picflowupload.Location = new Point(pictureBox.Size.Width - 35, pictureBox.Size.Height - 35);
                    pictureBox.Controls.Add(btn_picflowupload);
                }
            }

            mainPanel.Controls.Add(pictureBox);
        }
    }
}
