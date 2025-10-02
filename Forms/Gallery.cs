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
        private MetaDataImageReader.VrcxData vrcxData;

        public Gallery(VRCAuth auth)
        {
            InitializeComponent();
            InitApiRequest(auth);

            this.Shown += (s, e) =>
            {
                var getpath = Config.Get("PathGallery");
                if (!string.IsNullOrEmpty(getpath))
                {
                    currentRootFolder = NormalizePath(getpath);
                    currentFolder = currentRootFolder;
                }

                if (string.IsNullOrEmpty(currentRootFolder))
                    FolderImage();
            };

            Shown += Gallery_Shown;
            _refreshButton.Click += async (s, e) => await RefreshGalleryAsync();
            folderBack.Click += folderBack_Click;
        }

        private string NormalizePath(string path) =>
            Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        private async void Gallery_Shown(object sender, EventArgs e)
        {
            if (galleryPanel.Controls.Count == 0)
                await RefreshGalleryAsync();
        }

        private async void folderBack_Click(object sender, EventArgs e)
        {
            galleryInfoPanel.Visible = false;

            if (string.IsNullOrEmpty(currentFolder)) return;

            if (currentFolder == currentRootFolder) return;

            var parent = Directory.GetParent(currentFolder);
            if (parent != null)
            {
                currentFolder = NormalizePath(parent.FullName);
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
                folderBack.Visible = false;
                _refreshButton.Enabled = true;
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

            foreach (var dir in Directory.GetDirectories(folderPath))
            {
                token.ThrowIfCancellationRequested();
                galleryPanel.Controls.Add(await Task.Run(() => CreateFolderPanel(dir), token));
            }

            foreach (var file in GetImageFiles(folderPath))
            {
                token.ThrowIfCancellationRequested();
                galleryPanel.Controls.Add(await Task.Run(() => CreateImageBox(file), token));
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
            folderBack.Visible = currentFolder != currentRootFolder;
        }

        private RoundedPanel CreateFolderPanel(string path)
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

            var thumb = LoadThumbnail(GetImageFiles(path).LastOrDefault());

            var pictureBox = new RoundedPictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Cursor = Cursors.Hand,
                Image = thumb,
                BorderRadiusBottomLeft = 10,
                BorderRadiusBottomRight = 10,
                BorderRadiusTopLeft = 10,
                BorderRadiusTopRight = 10,
                Visible = thumb != null
            };

            var label = new Label
            {
                Text = Path.GetFileName(path),
                Dock = thumb == null ? DockStyle.Fill : DockStyle.Bottom,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };

            void OpenFolder(object s, EventArgs e)
            {
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                currentFolder = NormalizePath(path);
                _ = RefreshGalleryAsync();
            }

            pictureBox.Click += OpenFolder;
            label.Click += OpenFolder;
            panel.Click += OpenFolder;

            panel.Controls.Add(pictureBox);
            panel.Controls.Add(label);
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
                Image = LoadThumbnail(file)
            };

            pb.DoubleClick += (s, e) => Process.Start(new ProcessStartInfo { FileName = file, UseShellExecute = true });

            pb.Click += (s, e) =>
            {
                vrcxData = MetaDataImageReader.ExtractVrcxData(file);
                if (vrcxData != null)
                {
                    MetaDataImageReader.ApiWorldInfo(vrcxData, apiRequest, worldImage, worldNameLabel);
                    userInfoPanel.Controls.Clear();
                    foreach (var player in vrcxData.Players)
                    {
                        (RoundedLabel label, bool isFriend) = MetaDataImageReader.UsersInfo(player);
                        userInfoPanel.Controls.Add(label);
                        if (!isFriend) userInfoPanel.Controls.SetChildIndex(label, 0);
                    }
                    galleryInfoPanel.Visible = true;
                }
                else galleryInfoPanel.Visible = false;
            };

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

        private IEnumerable<string> GetImageFiles(string dir) =>
            Directory.EnumerateFiles(dir).Where(f =>
                new[] { ".jpg", ".jpeg", ".png" }.Contains(Path.GetExtension(f).ToLower()));

        private void ClearGalleryPanel()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ClearGalleryPanel));
                return;
            }
            foreach (Control ctl in galleryPanel.Controls)
            {
                if (ctl is PictureBox pb) pb.Image?.Dispose();
                ctl.Dispose();
            }
            galleryPanel.Controls.Clear();
        }

        private void worldImage_Click(object sender, EventArgs e)
        {
            if (vrcxData == null || string.IsNullOrEmpty(vrcxData.World.Id)) return;
            Process.Start("explorer.exe", "https://vrchat.com/home/world/" + vrcxData.World.Id);
        }

        private void changeFolder_Click(object sender, EventArgs e) => FolderImage();

        private async void FolderImage()
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select VRChat Images Folder",
                ShowNewFolderButton = true,
                RootFolder = Environment.SpecialFolder.MyPictures
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            Config.Set("PathGallery", dialog.SelectedPath);
            currentRootFolder = NormalizePath(dialog.SelectedPath);
            currentFolder = currentRootFolder;

            await RefreshGalleryAsync();
        }
    }
}
