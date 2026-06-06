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

        public Gallery(VRCAuth auth)
        {
            InitializeComponent();
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

        private async Task ShowFoldersAndImagesAsync(string folderPath, CancellationToken token)
        {
            PrepareGallery();

            var dirs = Directory.GetDirectories(folderPath);

            foreach (var dir in dirs)
            {
                token.ThrowIfCancellationRequested();
                var panel = CreateFolderPanel(dir, token);
                galleryPanel.Controls.Add(panel);
                await Task.Yield();
            }

            var files = GetImageFiles(folderPath);
            foreach (var file in files)
            {
                token.ThrowIfCancellationRequested();
                var box = CreateImageBox(file, token);
                galleryPanel.Controls.Add(box);
                await Task.Yield();
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


        private RoundedPanel CreateFolderPanel(string path, CancellationToken token)
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

            _ = Task.Run(() =>
            {
                try
                {
                    var files = GetImageFiles(path);
                    var firstImage = files.LastOrDefault();
                    if (string.IsNullOrEmpty(firstImage))
                    {
                        if (!this.IsDisposed && this.IsHandleCreated)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                if (!token.IsCancellationRequested && !pictureBox.IsDisposed)
                                {
                                    pictureBox.Visible = false;
                                    label.Dock = DockStyle.Fill;
                                }
                            }));
                        }
                        return;
                    }

                    var thumb = LoadThumbnail(firstImage);
                    if (thumb != null)
                    {
                        if (token.IsCancellationRequested)
                        {
                            thumb.Dispose();
                            return;
                        }

                        if (!this.IsDisposed && this.IsHandleCreated)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                if (!token.IsCancellationRequested && !pictureBox.IsDisposed)
                                {
                                    pictureBox.Image = thumb;
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
                                if (!token.IsCancellationRequested && !pictureBox.IsDisposed)
                                {
                                    pictureBox.Visible = false;
                                    label.Dock = DockStyle.Fill;
                                }
                            }));
                        }
                    }
                }
                catch { }
            }, token);

            return panel;
        }

        private PictureBox CreateImageBox(string file, CancellationToken token)
        {
            var pb = new PictureBox
            {
                Size = new Size(150, 150),
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = new Padding(10),
                Cursor = Cursors.Hand,
                Image = null,
                BackColor = Color.FromArgb(24, 27, 31)
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
                    galleryInfoPanel.Visible = true;
                }
                else
                {
                    galleryInfoPanel.Visible = false;
                }
            };

            _ = Task.Run(() =>
            {
                try
                {
                    var thumb = LoadThumbnail(file);
                    if (thumb != null)
                    {
                        if (token.IsCancellationRequested)
                        {
                            thumb.Dispose();
                            return;
                        }

                        if (!this.IsDisposed && this.IsHandleCreated)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                if (!token.IsCancellationRequested && !pb.IsDisposed)
                                {
                                    pb.Image = thumb;
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
                catch { }
            }, token);

            return pb;
        }

        private Image LoadThumbnail(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return null;
            try
            {
                var shellFile = ShellFile.FromFilePath(path);
                using var bmp = shellFile.Thumbnail.LargeBitmap;
                return new Bitmap(bmp);
            }
            catch { return null; }
        }

        private IEnumerable<string> GetImageFiles(string dir)
        {
            return Directory.EnumerateFiles(dir)
                .Where(f => new[] { ".jpg", ".jpeg", ".png" }.Contains(Path.GetExtension(f).ToLower()));
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
    }
}
