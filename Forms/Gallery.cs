using Microsoft.WindowsAPICodePack.Shell;
using System.Diagnostics;
using VRCGalleryManager.Core;
using VRCGalleryManager.Design;

namespace VRCGalleryManager.Forms
{
    public partial class Gallery : ApiConnectedForm
    {
        private string currentRootFolder;
        private string currentFolder;
        private CancellationTokenSource _cts;
        MetaDataImageReader.VrcxData vrcxData;
        private string selectedImagePath;

        public Gallery(VRCAuth auth)
        {
            InitializeComponent();
            ScrollBarHelper.Attach(userInfoPanel);
            InitApiRequest(auth);

            this.Shown += (s, e) =>
            {
                var getpath = Config.Get("PathGallery");
                if (!string.IsNullOrEmpty(getpath))
                {
                    currentRootFolder = getpath;
                    currentFolder = getpath;
                }

                if (string.IsNullOrEmpty(currentRootFolder))
                    FolderImage();
            };

            Shown += Gallery_Shown;

            _refreshButton.Click -= _refreshButton_Click;
            _refreshButton.Click += _refreshButton_Click;

            folderBack.Click -= folderBack_Click;
            folderBack.Click += folderBack_Click;
        }

        private async void Gallery_Shown(object sender, EventArgs e)
        {
            if (galleryPanel.Controls.Count == 0)
                await RefreshGalleryAsync();
        }

        private async void _refreshButton_Click(object sender, EventArgs e)
        {
            await RefreshGalleryAsync();
        }

        private async void folderBack_Click(object sender, EventArgs e)
        {
            galleryInfoPanel.Visible = false;

            if (string.IsNullOrEmpty(currentFolder)) return;

            string normalizedRoot = Path.GetFullPath(currentRootFolder).TrimEnd(Path.DirectorySeparatorChar);
            string normalizedCurrent = Path.GetFullPath(currentFolder).TrimEnd(Path.DirectorySeparatorChar);

            // se sei già in root → non fare nulla
            if (string.Equals(normalizedCurrent, normalizedRoot, StringComparison.OrdinalIgnoreCase))
                return;

            var parent = Directory.GetParent(currentFolder);
            if (parent != null)
            {
                currentFolder = parent.FullName;
                await RefreshGalleryAsync();
            }
        }


        private async Task RefreshGalleryAsync()
        {
            selectedImagePath = null;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            _refreshButton.Enabled = false;

            if (string.IsNullOrEmpty(currentFolder) || !Directory.Exists(currentFolder))
            {
                _refreshButton.Enabled = false;
                folderBack.Visible = false;
                return;
            }

            try
            {
                await ShowFoldersAndImagesAsync(currentFolder, token);
            }
            catch (OperationCanceledException) { }

            _refreshButton.Enabled = true;
        }

        private class ThumbnailLoadRequest
        {
            public bool IsFolder { get; set; }
            public string Path { get; set; }
            public PictureBox PictureBox { get; set; }
            public Label Label { get; set; }
        }

        private async Task ShowFoldersAndImagesAsync(string folderPath, CancellationToken token)
        {
            PrepareGallery();

            var loadRequests = new List<ThumbnailLoadRequest>();

            var dirs = Directory.GetDirectories(folderPath);
            foreach (var dir in dirs)
            {
                token.ThrowIfCancellationRequested();
                var panel = CreateFolderPanel(dir, out var pictureBox, out var label);
                galleryPanel.Controls.Add(panel);
                loadRequests.Add(new ThumbnailLoadRequest
                {
                    IsFolder = true,
                    Path = dir,
                    PictureBox = pictureBox,
                    Label = label
                });
            }

            var files = GetImageFiles(folderPath);
            foreach (var file in files)
            {
                token.ThrowIfCancellationRequested();
                var box = CreateImageBox(file);
                galleryPanel.Controls.Add(box);
                loadRequests.Add(new ThumbnailLoadRequest
                {
                    IsFolder = false,
                    Path = file,
                    PictureBox = box
                });
            }

            // Start sequential background loading
            _ = Task.Run(() => ProcessLoadQueue(loadRequests, token), token);

            await Task.CompletedTask;
        }

        private void ProcessLoadQueue(List<ThumbnailLoadRequest> requests, CancellationToken token)
        {
            foreach (var req in requests)
            {
                if (token.IsCancellationRequested)
                    break;

                try
                {
                    if (req.IsFolder)
                    {
                        var files = GetImageFiles(req.Path).ToList();
                        var firstImage = files.LastOrDefault();
                        if (string.IsNullOrEmpty(firstImage))
                        {
                            if (!this.IsDisposed && this.IsHandleCreated)
                            {
                                this.BeginInvoke(new Action(() =>
                                {
                                    if (!token.IsCancellationRequested && !req.PictureBox.IsDisposed)
                                    {
                                        req.PictureBox.Visible = false;
                                        req.Label.Dock = DockStyle.Fill;
                                    }
                                }));
                            }
                            continue;
                        }

                        var thumb = LoadThumbnail(firstImage);
                        if (token.IsCancellationRequested)
                        {
                            thumb?.Dispose();
                            break;
                        }

                        if (thumb != null)
                        {
                            if (!this.IsDisposed && this.IsHandleCreated)
                            {
                                this.BeginInvoke(new Action(() =>
                                {
                                    if (!token.IsCancellationRequested && !req.PictureBox.IsDisposed)
                                    {
                                        req.PictureBox.Image = thumb;
                                    }
                                    else
                                    {
                                        thumb.Dispose();
                                    }
                                }));
                            }
                            else
                            {
                                thumb.Dispose();
                            }
                        }
                        else
                        {
                            if (!this.IsDisposed && this.IsHandleCreated)
                            {
                                this.BeginInvoke(new Action(() =>
                                {
                                    if (!token.IsCancellationRequested && !req.PictureBox.IsDisposed)
                                    {
                                        req.PictureBox.Visible = false;
                                        req.Label.Dock = DockStyle.Fill;
                                    }
                                }));
                            }
                        }
                    }
                    else
                    {
                        var thumb = LoadThumbnail(req.Path);
                        if (token.IsCancellationRequested)
                        {
                            thumb?.Dispose();
                            break;
                        }

                        if (thumb != null)
                        {
                            if (!this.IsDisposed && this.IsHandleCreated)
                            {
                                this.BeginInvoke(new Action(() =>
                                {
                                    if (!token.IsCancellationRequested && !req.PictureBox.IsDisposed)
                                    {
                                        req.PictureBox.Image = thumb;
                                    }
                                    else
                                    {
                                        thumb.Dispose();
                                    }
                                }));
                            }
                            else
                            {
                                thumb.Dispose();
                            }
                        }
                    }
                }
                catch
                {
                    // Ignore exceptions for individual items, proceed with the queue
                }
            }
        }

        private void PrepareGallery()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(PrepareGallery));
                return;
            }

            ClearGalleryPanel();

            string normalizedRoot = Path.GetFullPath(currentRootFolder).TrimEnd(Path.DirectorySeparatorChar);
            string normalizedCurrent = Path.GetFullPath(currentFolder).TrimEnd(Path.DirectorySeparatorChar);

            folderBack.Visible = !string.Equals(normalizedCurrent, normalizedRoot, StringComparison.OrdinalIgnoreCase);
        }


        private RoundedPanel CreateFolderPanel(string path, out PictureBox outPictureBox, out Label outLabel)
        {
            var panel = new RoundedPanel
            {
                Size = new Size(200, 150),
                Margin = new Padding(10),
                BackColor = Color.FromArgb(24, 27, 31),
                Cursor = Cursors.Hand,
                BorderRadius = 15,
                Padding = new Padding(7)
            };

            var pictureBox = new RoundedPictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(5, 5, 5),
                SizeMode = PictureBoxSizeMode.CenterImage,
                Cursor = Cursors.Hand,
                Image = null,
                BorderRadiusBottomLeft = 10,
                BorderRadiusBottomRight = 10,
                BorderRadiusTopLeft = 10,
                BorderRadiusTopRight = 10
            };

            var label = new Label
            {
                Text = Path.GetFileName(path),
                Dock = DockStyle.Bottom,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };

            void OpenFolder(object s, EventArgs e)
            {
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                currentFolder = path;
                _ = RefreshGalleryAsync();
            }

            pictureBox.Click += OpenFolder;
            label.Click += OpenFolder;
            panel.Click += OpenFolder;

            panel.Controls.Add(pictureBox);
            panel.Controls.Add(label);

            outPictureBox = pictureBox;
            outLabel = label;

            return panel;
        }

        private PictureBox CreateImageBox(string file)
        {
            var pb = new PictureBox
            {
                Size = new Size(150, 150),
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = new Padding(10),
                Cursor = Cursors.Hand,
                Image = null,
                BackColor = Color.FromArgb(5, 5, 5)
            };

            pb.DoubleClick += (s, e) =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = file,
                    UseShellExecute = true
                };
                Process.Start(psi);
            };

            pb.Click += async (s, e) =>
            {
                selectedImagePath = file;
                vrcxData = MetaDataImageReader.ExtractVrcxData(file);
                if (vrcxData != null)
                {
                    MetaDataImageReader.ApiWorldInfo(vrcxData, apiRequest, worldImage, worldNameLabel);
                    var players = vrcxData.Players;
                    var playerLabels = new List<(RoundedLabel label, int priority)>();
                    for (int i = 0; i < players.Count; i++)
                    {
                        (RoundedLabel label, bool isFriend, bool isMe) = MetaDataImageReader.UsersInfo(players[i]);
                        int priority = isMe ? 0 : (isFriend ? 1 : 2);
                        playerLabels.Add((label, priority));
                    }

                    var sortedLabels = playerLabels.OrderByDescending(p => p.priority).Select(p => p.label).ToArray();
                    userInfoPanel.Controls.Clear();
                    userInfoPanel.Controls.AddRange(sortedLabels);

                    detailsPanel.Visible = true;

                    // Force child controls to be visible inside detailsPanel
                    worldImage.Visible = true;
                    worldNameLabel.Visible = true;
                    userInfoPanel.Visible = true;
                }
                else
                {
                    detailsPanel.Visible = false;
                }

                // Show high-quality preview of the clicked image
                try
                {
                    imagePreview.Image?.Dispose();
                    using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (var img = Image.FromStream(stream))
                    {
                        imagePreview.Image = new Bitmap(img);
                    }
                }
                catch { }

                galleryInfoPanel.Visible = true;
            };

            return pb;
        }

        private Image LoadThumbnail(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return null;
            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var original = Image.FromStream(stream, false, false))
                {
                    int width = 150;
                    int height = 150;

                    float ratioX = (float)width / original.Width;
                    float ratioY = (float)height / original.Height;
                    float ratio = Math.Min(ratioX, ratioY);

                    int newWidth = Math.Max(1, (int)(original.Width * ratio));
                    int newHeight = Math.Max(1, (int)(original.Height * ratio));

                    Bitmap thumb = new Bitmap(newWidth, newHeight);
                    using (Graphics g = Graphics.FromImage(thumb))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                        g.DrawImage(original, 0, 0, newWidth, newHeight);
                    }
                    return thumb;
                }
            }
            catch
            {
                try
                {
                    var shellFile = ShellFile.FromFilePath(path);
                    using var bmp = shellFile.Thumbnail.LargeBitmap;
                    return new Bitmap(bmp);
                }
                catch { return null; }
            }
        }

        private static bool IsImageFile(string filename)
        {
            if (filename.Length < 4) return false;
            return filename.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                   filename.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                   filename.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase);
        }

        private IEnumerable<string> GetImageFiles(string dir)
        {
            return Directory.EnumerateFiles(dir).Where(IsImageFile);
        }

        private void ClearGalleryPanel()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ClearGalleryPanel));
                return;
            }
            galleryPanel.SuspendLayout();
            var controls = galleryPanel.Controls.Cast<Control>().ToArray();
            foreach (var ctl in controls)
            {
                if (ctl is PictureBox pb) pb.Image?.Dispose();
                ctl.Dispose();
            }
            galleryPanel.Controls.Clear();
            galleryPanel.ResumeLayout();
        }

        private void worldImage_Click(object sender, EventArgs e)
        {
            if (vrcxData == null) return;
            if (string.IsNullOrEmpty(vrcxData.World.Id)) return;
            Process.Start("explorer.exe", "https://vrchat.com/home/world/" + vrcxData.World.Id);
        }

        private void changeFolder_Click(object sender, EventArgs e)
        {
            FolderImage();
        }

        private async void FolderImage()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select VRChat Images Folder";
                dialog.ShowNewFolderButton = true;
                dialog.RootFolder = Environment.SpecialFolder.MyPictures;

                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                Config.Set("PathGallery", dialog.SelectedPath);
                currentRootFolder = dialog.SelectedPath;
                currentFolder = dialog.SelectedPath;
            }
            await RefreshGalleryAsync();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            var menu = new Design.CustomContextMenuStrip();
            
            // Match btnUpload colors and border size
            menu.BackgroundColor = btnUpload.BackColor;
            menu.BorderColor = btnUpload.BorderColor;
            menu.TextColor = btnUpload.ForeColor;
            menu.PrimaryColor = btnUpload.ForeColor;
            menu.BorderSize = btnUpload.BorderSize;
            
            // Match width
            menu.MinimumSize = new Size(btnUpload.Width, 0);

            menu.Items.Add("Icons", null, (s, ev) => UploadForCategory("Icons", 0));
            menu.Items.Add("Photos", null, (s, ev) => UploadForCategory("Photos", 1));
            menu.Items.Add("Emoji", null, (s, ev) => UploadForCategory("Emoji", 2));
            menu.Items.Add("Sticker", null, (s, ev) => UploadForCategory("Sticker", 3));
            menu.Items.Add("Prints", null, (s, ev) => UploadForCategory("Prints", 4));

            foreach (ToolStripItem item in menu.Items)
            {
                item.AutoSize = false;
                item.Size = new Size(btnUpload.Width - (menu.BorderSize * 2), 35);
                item.TextAlign = ContentAlignment.MiddleCenter;
                item.Font = btnUpload.Font;
            }

            menu.Show(btnUpload, new Point(0, btnUpload.Height));
        }

        private void UploadForCategory(string category, int formIndex)
        {
            string path = selectedImagePath;

            if (string.IsNullOrEmpty(path))
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files|*.*";
                    openFileDialog.Multiselect = false;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        path = openFileDialog.FileName;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            var mainPanel = this.TopLevelControl as MainPanel;
            if (mainPanel == null) return;

            // Switch to the selected category form using reflection
            var method = mainPanel.GetType().GetMethod("ShowForm", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(mainPanel, new object[] { formIndex });

            // Find the newly shown form to trigger its specific upload logic on save
            ApiConnectedForm targetForm = null;
            var formsField = mainPanel.GetType().GetField("_forms", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (formsField != null)
            {
                var forms = formsField.GetValue(mainPanel) as ApiConnectedForm[];
                if (forms != null && formIndex >= 0 && formIndex < forms.Length)
                {
                    targetForm = forms[formIndex];
                }
            }

            if (category == "Emoji")
            {
                mainPanel.ShowEmojiEditor(
                    path,
                    onSave: async (editedImage, isAnimated, style, frames, fps) =>
                    {
                        try
                        {
                            if (isAnimated)
                                await apiRequest.UploadImage(editedImage, "emoji", VRCGalleryManager.Core.DTO.TagType.EmojiAnimated, style, frames, fps);
                            else
                                await apiRequest.UploadImage(editedImage, "emoji", VRCGalleryManager.Core.DTO.TagType.Sticker, null, 0, 0);

                            VRCGalleryManager.Core.NotificationManager.ShowNotification($"{category} uploaded successfully", $"{category} uploaded", VRCGalleryManager.Core.NotificationType.Success);

                            var refreshMethod = targetForm?.GetType().GetMethod("EmojiList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            refreshMethod?.Invoke(targetForm, null);
                        }
                        catch (Exception ex)
                        {
                            VRCGalleryManager.Core.NotificationManager.ShowNotification(ex.Message, "Error during file upload", VRCGalleryManager.Core.NotificationType.Error);
                        }
                        finally
                        {
                            try { File.Delete(editedImage); } catch { }
                        }
                    },
                    onCancel: () => { }
                );
            }
            else
            {
                string ratio = (category == "Photos" || category == "Prints") ? "16:9" : "1:1";
                mainPanel.ShowEditor(
                    path,
                    ratio,
                    onSave: async (editedImage, note) =>
                    {
                        try
                        {
                            if (category == "Icons")
                            {
                                await apiRequest.UploadImage(editedImage, "square", VRCGalleryManager.Core.DTO.TagType.Icon, null, 0, 0);
                            }
                            else if (category == "Photos")
                            {
                                await apiRequest.UploadImage(editedImage, "square", VRCGalleryManager.Core.DTO.TagType.Gallery, null, 0, 0);
                            }
                            else if (category == "Prints")
                            {
                                await apiRequest.UploadPrint(editedImage, note);
                            }
                            else if (category == "Sticker")
                            {
                                await apiRequest.UploadImage(editedImage, "square", VRCGalleryManager.Core.DTO.TagType.Sticker, null, 0, 0);
                            }
                            VRCGalleryManager.Core.NotificationManager.ShowNotification($"{category} uploaded successfully", $"{category} uploaded", VRCGalleryManager.Core.NotificationType.Success);

                            // Refresh the target panel
                            var refreshMethod = targetForm?.GetType().GetMethod("IconsList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) ??
                                                targetForm?.GetType().GetMethod("PhotosList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) ??
                                                targetForm?.GetType().GetMethod("PrintsList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) ??
                                                targetForm?.GetType().GetMethod("StickerList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            refreshMethod?.Invoke(targetForm, null);
                        }
                        catch (Exception ex)
                        {
                            VRCGalleryManager.Core.NotificationManager.ShowNotification(ex.Message, "Error during file upload", VRCGalleryManager.Core.NotificationType.Error);
                        }
                        finally
                        {
                            try { File.Delete(editedImage); } catch { }
                        }
                    },
                    onCancel: () => { },
                    showNote: category == "Prints"
                );
            }
        }
    }
}
