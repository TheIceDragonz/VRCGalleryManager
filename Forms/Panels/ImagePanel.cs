using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows.Automation.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Design;
using VRCGalleryManager.Forms.UIComponents;
using static VRCGalleryManager.Core.ApiRequest;

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
        private static List<RoundedPictureBox> picturePhotosList = new List<RoundedPictureBox>();

        static public async void AddImagePanel(FlowLayoutPanel mainPanel, ApiRequest apiRequest, string imageId, Action<string> UpdateCounter)
        {
            Size size = new Size(150, 150);
            int value = 10;

            if (mainPanel.Name.Contains("prints")) size = new Size(190, 150);
            if (mainPanel.Name.Contains("photos")) size = new Size(250, 150);
            if (mainPanel.Name.Contains("icons")) value = 100; //Max Rounded

            string imageFull = $"https://api.vrchat.cloud/api/1/file/{imageId}/1/file";
            string image256 = $"https://api.vrchat.cloud/api/1/image/{imageId}/1/256";
            string finalaviImage = await HttpImage.GetFinalUrlAsync(image256);

            var selectedColor = Color.FromArgb(106, 227, 249);

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
                pictureBox.Cursor = Cursors.Hand;
                pictureBox.MouseEnter += (s, e) => pictureBox.BorderColor = (pictureBox.BorderColor != selectedColor) ? Color.FromArgb(255, 255, 255) : selectedColor;
                pictureBox.MouseLeave += (s, e) => pictureBox.BorderColor = (pictureBox.BorderColor != selectedColor) ? Color.FromArgb(24, 27, 31) : selectedColor;

                if (Settings.UserIconImage.Contains(imageId))
                {
                    pictureBox.BorderColor = selectedColor;
                    pictureBox.Cursor = Cursors.Default;
                }

                pictureBox.Click += async (sender, e) =>
                {
                    await apiRequest.SetProfileIcon(imageFull);

                    Settings.UserBannerImage = imageFull;
                    (Application.OpenForms["MainPanel"] as MainPanel)?.ProfileUpdateIcon(imageFull);

                    foreach (var pb in pictureIconList)
                    {
                        pb.BorderColor = Color.FromArgb(24, 27, 31);
                        pb.Cursor = Cursors.Hand;
                    }

                    pictureBox.BorderColor = Color.FromArgb(106, 227, 249);
                    pictureBox.Cursor = Cursors.Default;

                    NotificationManager.ShowNotification("Icon Changed Successful", "Profile Updated", NotificationType.Success);
                };
                pictureIconList.Add(pictureBox);
            }
            if (mainPanel.Name.Contains("photos"))
            {
                pictureBox.Cursor = Cursors.Hand;
                pictureBox.MouseEnter += (s, e) => pictureBox.BorderColor = (pictureBox.BorderColor != selectedColor) ? Color.FromArgb(255, 255, 255) : selectedColor;
                pictureBox.MouseLeave += (s, e) => pictureBox.BorderColor = (pictureBox.BorderColor != selectedColor) ? Color.FromArgb(24, 27, 31) : selectedColor;

                if (Settings.UserBannerImage.Contains(imageId))
                {
                    pictureBox.BorderColor = selectedColor;
                    pictureBox.Cursor = Cursors.Default;
                }

                pictureBox.Click += async (sender, e) =>
                {
                    await apiRequest.SetProfilePicture(imageFull);

                    Settings.UserBannerImage = imageFull;
                    (Application.OpenForms["MainPanel"] as MainPanel)?.ProfileUpdateBanner(imageFull);

                    foreach (var pb in picturePhotosList)
                    {
                        pb.BorderColor = Color.FromArgb(24, 27, 31);
                        pb.Cursor = Cursors.Hand;
                    }

                    pictureBox.BorderColor = Color.FromArgb(106, 227, 249);
                    pictureBox.Cursor = Cursors.Default;

                    NotificationManager.ShowNotification("Picture Changed Successful", "Profile Updated", NotificationType.Success);
                };
                picturePhotosList.Add(pictureBox);
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

        //Prints
        static public async void AddPrintsPanel(FlowLayoutPanel mainPanel, ApiRequest apiRequest, string printid, string userId, string username, string imageId, Action<string> UpdateCounter)
        {
            Size size = new Size(190, 150);

            string imageFull = $"https://api.vrchat.cloud/api/1/file/{imageId}/1/file";
            string image256 = $"https://api.vrchat.cloud/api/1/image/{imageId}/1/256";
            string finalaviImage = await HttpImage.GetFinalUrlAsync(image256);

            var selectedColor = Color.FromArgb(106, 227, 249);

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

            RoundedLabel authorLabel = new RoundedLabel
            {
                Text = username,
                ForeColor = Color.White,
                Font = new Font("Arial", 8, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(5, 5),
                BackColor = Color.FromArgb(24, 27, 31),
                BorderSize = 0,
            };
            authorLabel.Click += (s, e) => Process.Start(new ProcessStartInfo($"https://vrchat.com/home/user/{userId}") { UseShellExecute = true });
            authorLabel.Cursor = Cursors.Hand;
            pictureBox.Controls.Add(authorLabel);

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
                        Debug.WriteLine("Delete: " + printid);
                        await apiRequest.DeleteApiDataPrint(printid);
                        mainPanel.Controls.Remove(pictureBox);

                        UpdateCounter("Remove");
                    }
                });
                btn_delete.Location = new Point(pictureBox.Size.Width - 35, pictureBox.Size.Height - 35);
                pictureBox.Controls.Add(btn_delete);
            }

            if (Settings.Friends.Contains(userId))
            {
                authorLabel.ForeColor = Color.Orange;
            }

            mainPanel.Controls.Add(pictureBox);
        }

        //PicFlow
        static public async void AddImagePanel(FlowLayoutPanel mainPanel, ApiRequest apiRequest, string userId, string imageId)
        {
            var invData = await apiRequest.GetInventoryInfo(userId, imageId);

            if (invData == null) return;

            Size size = new Size(150, 150);

            string imageFull = $"https://api.vrchat.cloud/api/1/file/{invData.Metadata.FileId}/1/file";
            string image256 = $"https://api.vrchat.cloud/api/1/image/{invData.Metadata.FileId}/1/256";

            string image = invData.Metadata.Animated
                ? $"https://api.vrchat.cloud/api/1/image/{invData.Metadata.FileId}/1/512"
                : $"https://api.vrchat.cloud/api/1/image/{invData.Metadata.FileId}/1/256";

            string finalaviImage = await HttpImage.GetFinalUrlAsync(image);

            if (!finalaviImage.Contains("imageNotFound"))
            {
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

                if (invData.Metadata != null && invData.Metadata.Animated == true)
                {
                    SpriteSheetViewer viewer = new SpriteSheetViewer(pictureBox);
                    await viewer.LoadSpriteSheetAsync(finalaviImage, invData.Metadata.Frames, invData.Metadata.FramesOverTime);
                    viewer.StartAnimation();
                }
                else
                {
                    pictureBox.LoadAsync(finalaviImage);
                }

                RoundedLabel authorLabel = new RoundedLabel
                {
                    Text = invData.Name,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 8, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(5, 5),
                    BackColor = Color.FromArgb(24, 27, 31),
                    BorderSize = 0,
                };
                authorLabel.Click += (s, e) => Process.Start(new ProcessStartInfo($"https://vrchat.com/home/user/{userId}") { UseShellExecute = true });
                authorLabel.Cursor = Cursors.Hand;
                pictureBox.Controls.Add(authorLabel);

                if (pictureBox.Controls["btn_open"] == null && invData.Name.Contains("Custom"))
                {
                    CircularButton btn_open = CircularButtonTools.CreateButton("open", (sender, e) =>
                    {
                        Process.Start("explorer.exe", imageFull);
                    });

                    btn_open.Location = new Point(pictureBox.Size.Width - 60, pictureBox.Size.Height - 35);

                    pictureBox.Controls.Add(btn_open);
                }

                if (pictureBox.Controls["btn_picflowupload"] == null && invData.Name.Contains("Custom"))
                {
                    CircularButton btn_picflowupload = CircularButtonTools.CreateButton("picflowupload", async (sender, e) =>
                    {
                        Debug.WriteLine("PicflowUpload: " + invData.Metadata.FileId);

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

                            if (invData.Name.Contains("Sticker"))
                            {
                                ApiRequest.ApiData sticker = await apiRequest.UploadImage(localImagePath, "sticker", TagType.Sticker);
                                NotificationManager.ShowNotification("Sticker uploaded successfully", "Sticker uploaded", NotificationType.Success);
                            }
                            if (invData.Name.Contains("Emoji"))
                            {
                                if (invData.Metadata.Animated == true)
                                {
                                    ApiRequest.ApiData emoji = await apiRequest.UploadImage(localImagePath, invData.Metadata.MaskTag, TagType.EmojiAnimated, invData.Metadata.AnimationStyle, invData.Metadata.Frames, invData.Metadata.FramesOverTime);
                                }
                                else
                                {
                                    ApiRequest.ApiData emoji = await apiRequest.UploadImage(localImagePath, "emoji", TagType.Sticker);
                                }
                                NotificationManager.ShowNotification("Emoji uploaded successfully", "Emoji uploaded", NotificationType.Success);
                            }
                        }
                        catch (Exception ex)
                        {
                            NotificationManager.ShowNotification(ex.Message, "Error during file upload", NotificationType.Error);
                        }
                    });
                    btn_picflowupload.Location = new Point(pictureBox.Size.Width - 35, pictureBox.Size.Height - 35);
                    pictureBox.Controls.Add(btn_picflowupload);
                }

                if (Settings.Friends.Contains(userId))
                {
                    authorLabel.ForeColor = Color.Orange;
                }

                mainPanel.Controls.Add(pictureBox);
            }
        }
    }
}
