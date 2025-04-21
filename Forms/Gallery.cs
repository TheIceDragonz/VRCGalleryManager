using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json.Linq;
using VRCGalleryManager.Core;

namespace VRCGalleryManager.Forms
{
    public partial class Gallery : ApiConnectedForm
    {
        private readonly string currentRootFolder;
        private bool isInFolderView;
        private CancellationTokenSource _cts;

        public Gallery(VRCAuth auth)
        {
            InitializeComponent();
            InitApiRequest(auth);

            currentRootFolder = LoadPictureOutputFolder();
            if (string.IsNullOrEmpty(currentRootFolder))
            {
                MessageBox.Show("Unable to read 'picture_output_folder' from config.json.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Enabled = false;
                return;
            }

            Shown += Gallery_Shown;
            _refreshButton.Click += _refreshButton_Click;
            folderBack.Click += folderBack_Click;
        }

        private string LoadPictureOutputFolder()
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow");
                string configPath = Path.Combine(appData, "VRChat", "VRChat", "config.json");
                if (File.Exists(configPath))
                {
                    var json = JObject.Parse(File.ReadAllText(configPath));
                    string picDir = (string)json["picture_output_folder"];
                    if (!string.IsNullOrEmpty(picDir) && Directory.Exists(picDir))
                        return picDir;
                }
            }
            catch { }

            return null;
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
            if (isInFolderView)
            {
                isInFolderView = false;
                await RefreshGalleryAsync();
            }
        }

        private async Task RefreshGalleryAsync()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            _refreshButton.Enabled = false;

            if (!Directory.Exists(currentRootFolder))
            {
                MessageBox.Show($"Directory not found: {currentRootFolder}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _refreshButton.Enabled = true;
                return;
            }

            try
            {
                if (isInFolderView && galleryPanel.Tag is string folderTag)
                    await ShowImagesAsync(folderTag, token);
                else
                    await ShowFoldersAsync(token);
            }
            catch (OperationCanceledException)
            {
                // cancellation requested
            }

            _refreshButton.Enabled = true;
        }

        private async Task ShowFoldersAsync(CancellationToken token)
        {
            PrepareGallery(showBack: false);
            var dirs = Directory.GetDirectories(currentRootFolder);
            foreach (var dir in dirs)
            {
                token.ThrowIfCancellationRequested();
                var panel = await Task.Run(() => CreateFolderPanel(dir), token);
                if (token.IsCancellationRequested) break;
                galleryPanel.Controls.Add(panel);
            }
        }

        private async Task ShowImagesAsync(string folderPath, CancellationToken token)
        {
            PrepareGallery(showBack: true);
            galleryPanel.Tag = folderPath;
            var files = GetImageFiles(folderPath);
            foreach (var file in files)
            {
                token.ThrowIfCancellationRequested();
                var box = await Task.Run(() => CreateImageBox(file), token);
                if (token.IsCancellationRequested) break;
                galleryPanel.Controls.Add(box);
            }
        }

        private void PrepareGallery(bool showBack)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(PrepareGallery), showBack);
                return;
            }

            ClearGalleryPanel();
            folderBack.Visible = showBack;
            isInFolderView = showBack;
        }

        private Panel CreateFolderPanel(string path)
        {
            var panel = new Panel
            {
                Size = new Size(200, 200),
                Margin = new Padding(10),
                BackColor = Color.FromArgb(24, 27, 31),
                Cursor = Cursors.Hand,
                Tag = path
            };

            var firstImage = GetImageFiles(path).FirstOrDefault();
            var thumb = LoadThumbnail(firstImage) ?? Properties.Resources.VRCGM;

            var pictureBox = new PictureBox
            {
                Size = new Size(180, 140),
                Location = new Point(10, 5),
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand,
                Image = thumb
            };

            var label = new Label
            {
                Text = Path.GetFileName(path),
                Dock = DockStyle.Bottom,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };

            void OpenFolder(object s, EventArgs e)
            {
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                isInFolderView = true;
                galleryPanel.Tag = path;
                _ = ShowImagesAsync(path, _cts.Token);
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
            pb.DoubleClick += (s, e) =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = file,
                    UseShellExecute = true
                };
                Process.Start(psi);
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
            catch
            {
                return null;
            }
        }

        private IEnumerable<string> GetImageFiles(string dir)
        {
            return Directory.EnumerateFiles(dir).Where(f => new[] { ".jpg", ".jpeg", ".png"}.Contains(Path.GetExtension(f).ToLower()));
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
    }
}
