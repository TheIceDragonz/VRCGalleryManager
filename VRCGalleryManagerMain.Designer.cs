using VRCGalleryManager.Design;

namespace VRCGalleryManager
{
    partial class MainPanel
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainPanel));
            bannerIcon = new PictureBox();
            FormsPanel = new Panel();
            SwitchPanel = new RoundedPanel();
            _switchPicflow = new RoundedButton();
            _switchCreate = new RoundedButton();
            _switchPrints = new RoundedButton();
            _switchSettings = new RoundedButton();
            _switchSticker = new RoundedButton();
            _switchEmoji = new RoundedButton();
            _switchPhotos = new RoundedButton();
            _switchIcons = new RoundedButton();
            profileBanner = new RoundedPictureBox();
            profileIcon = new RoundedPictureBox();
            badgeBox1 = new RoundedPictureBox();
            badgeBox2 = new RoundedPictureBox();
            badgeBox3 = new RoundedPictureBox();
            ((System.ComponentModel.ISupportInitialize)bannerIcon).BeginInit();
            SwitchPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)profileBanner).BeginInit();
            ((System.ComponentModel.ISupportInitialize)profileIcon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)badgeBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)badgeBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)badgeBox3).BeginInit();
            SuspendLayout();
            // 
            // bannerIcon
            // 
            bannerIcon.Image = Properties.Resources.VRCGM;
            bannerIcon.ImageLocation = "";
            bannerIcon.Location = new Point(12, 12);
            bannerIcon.Margin = new Padding(10);
            bannerIcon.Name = "bannerIcon";
            bannerIcon.Size = new Size(157, 45);
            bannerIcon.SizeMode = PictureBoxSizeMode.Zoom;
            bannerIcon.TabIndex = 4;
            bannerIcon.TabStop = false;
            // 
            // FormsPanel
            // 
            FormsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            FormsPanel.BackColor = Color.FromArgb(5, 5, 5);
            FormsPanel.Location = new Point(182, 0);
            FormsPanel.Name = "FormsPanel";
            FormsPanel.Size = new Size(833, 589);
            FormsPanel.TabIndex = 5;
            // 
            // SwitchPanel
            // 
            SwitchPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            SwitchPanel.BackColor = Color.FromArgb(5, 5, 5);
            SwitchPanel.BackgroundColor = Color.FromArgb(5, 5, 5);
            SwitchPanel.BorderColor = Color.FromArgb(80, 80, 80);
            SwitchPanel.BorderRadius = 15;
            SwitchPanel.BorderSize = 0;
            SwitchPanel.Controls.Add(_switchPicflow);
            SwitchPanel.Controls.Add(_switchCreate);
            SwitchPanel.Controls.Add(_switchPrints);
            SwitchPanel.Controls.Add(_switchSettings);
            SwitchPanel.Controls.Add(_switchSticker);
            SwitchPanel.Controls.Add(_switchEmoji);
            SwitchPanel.Controls.Add(_switchPhotos);
            SwitchPanel.Controls.Add(_switchIcons);
            SwitchPanel.Location = new Point(12, 170);
            SwitchPanel.Margin = new Padding(5);
            SwitchPanel.Name = "SwitchPanel";
            SwitchPanel.Padding = new Padding(5);
            SwitchPanel.Size = new Size(157, 407);
            SwitchPanel.TabIndex = 6;
            // 
            // _switchPicflow
            // 
            _switchPicflow.BackColor = Color.FromArgb(7, 36, 43);
            _switchPicflow.BackgroundColor = Color.FromArgb(7, 36, 43);
            _switchPicflow.BorderColor = Color.FromArgb(5, 55, 66);
            _switchPicflow.BorderRadius = 10;
            _switchPicflow.BorderSize = 2;
            _switchPicflow.Dock = DockStyle.Bottom;
            _switchPicflow.Enabled = false;
            _switchPicflow.FlatAppearance.BorderSize = 0;
            _switchPicflow.FlatStyle = FlatStyle.Flat;
            _switchPicflow.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _switchPicflow.ForeColor = Color.FromArgb(106, 227, 249);
            _switchPicflow.Location = new Point(5, 282);
            _switchPicflow.Name = "_switchPicflow";
            _switchPicflow.Size = new Size(147, 40);
            _switchPicflow.SvgAlignment = ContentAlignment.MiddleLeft;
            _switchPicflow.SvgColor = Color.FromArgb(106, 227, 249);
            _switchPicflow.SvgContent = resources.GetString("_switchPicflow.SvgContent");
            _switchPicflow.SvgOffset = new Point(10, 0);
            _switchPicflow.SvgPadding = new Padding(0);
            _switchPicflow.SvgResource = "grid_svgrepo_com";
            _switchPicflow.SvgSize = new Size(25, 25);
            _switchPicflow.TabIndex = 8;
            _switchPicflow.Text = "Picflow";
            _switchPicflow.TextColor = Color.FromArgb(106, 227, 249);
            _switchPicflow.UseVisualStyleBackColor = false;
            _switchPicflow.Click += _switchPicflow_Click;
            // 
            // _switchCreate
            // 
            _switchCreate.BackColor = Color.FromArgb(7, 36, 43);
            _switchCreate.BackgroundColor = Color.FromArgb(7, 36, 43);
            _switchCreate.BorderColor = Color.FromArgb(5, 55, 66);
            _switchCreate.BorderRadius = 10;
            _switchCreate.BorderSize = 2;
            _switchCreate.Dock = DockStyle.Bottom;
            _switchCreate.Enabled = false;
            _switchCreate.FlatAppearance.BorderSize = 0;
            _switchCreate.FlatStyle = FlatStyle.Flat;
            _switchCreate.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _switchCreate.ForeColor = Color.FromArgb(106, 227, 249);
            _switchCreate.Location = new Point(5, 322);
            _switchCreate.Name = "_switchCreate";
            _switchCreate.Size = new Size(147, 40);
            _switchCreate.SvgAlignment = ContentAlignment.MiddleLeft;
            _switchCreate.SvgColor = Color.FromArgb(106, 227, 249);
            _switchCreate.SvgContent = resources.GetString("_switchCreate.SvgContent");
            _switchCreate.SvgOffset = new Point(10, 0);
            _switchCreate.SvgPadding = new Padding(0);
            _switchCreate.SvgResource = "gif_svgrepo_com";
            _switchCreate.SvgSize = new Size(25, 25);
            _switchCreate.TabIndex = 6;
            _switchCreate.Text = "Create";
            _switchCreate.TextColor = Color.FromArgb(106, 227, 249);
            _switchCreate.UseVisualStyleBackColor = false;
            _switchCreate.Click += _switchCreate_Click;
            // 
            // _switchPrints
            // 
            _switchPrints.BackColor = Color.FromArgb(7, 36, 43);
            _switchPrints.BackgroundColor = Color.FromArgb(7, 36, 43);
            _switchPrints.BorderColor = Color.FromArgb(5, 55, 66);
            _switchPrints.BorderRadius = 10;
            _switchPrints.BorderSize = 2;
            _switchPrints.Dock = DockStyle.Top;
            _switchPrints.Enabled = false;
            _switchPrints.FlatAppearance.BorderSize = 0;
            _switchPrints.FlatStyle = FlatStyle.Flat;
            _switchPrints.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _switchPrints.ForeColor = Color.FromArgb(106, 227, 249);
            _switchPrints.Location = new Point(5, 165);
            _switchPrints.Name = "_switchPrints";
            _switchPrints.Size = new Size(147, 40);
            _switchPrints.SvgAlignment = ContentAlignment.MiddleLeft;
            _switchPrints.SvgColor = Color.FromArgb(106, 227, 249);
            _switchPrints.SvgContent = resources.GetString("_switchPrints.SvgContent");
            _switchPrints.SvgOffset = new Point(10, 0);
            _switchPrints.SvgPadding = new Padding(0);
            _switchPrints.SvgResource = "camera_svgrepo_com";
            _switchPrints.SvgSize = new Size(25, 25);
            _switchPrints.TabIndex = 5;
            _switchPrints.Text = "Prints";
            _switchPrints.TextColor = Color.FromArgb(106, 227, 249);
            _switchPrints.UseVisualStyleBackColor = false;
            _switchPrints.Click += _switchPrints_Click;
            // 
            // _switchSettings
            // 
            _switchSettings.BackColor = Color.FromArgb(7, 36, 43);
            _switchSettings.BackgroundColor = Color.FromArgb(7, 36, 43);
            _switchSettings.BorderColor = Color.FromArgb(5, 55, 66);
            _switchSettings.BorderRadius = 10;
            _switchSettings.BorderSize = 2;
            _switchSettings.Dock = DockStyle.Bottom;
            _switchSettings.FlatAppearance.BorderSize = 0;
            _switchSettings.FlatStyle = FlatStyle.Flat;
            _switchSettings.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _switchSettings.ForeColor = Color.FromArgb(106, 227, 249);
            _switchSettings.Location = new Point(5, 362);
            _switchSettings.Name = "_switchSettings";
            _switchSettings.Size = new Size(147, 40);
            _switchSettings.SvgAlignment = ContentAlignment.MiddleLeft;
            _switchSettings.SvgColor = Color.FromArgb(106, 227, 249);
            _switchSettings.SvgContent = resources.GetString("_switchSettings.SvgContent");
            _switchSettings.SvgOffset = new Point(10, 0);
            _switchSettings.SvgPadding = new Padding(0);
            _switchSettings.SvgResource = "settings_svgrepo_com";
            _switchSettings.SvgSize = new Size(25, 25);
            _switchSettings.TabIndex = 7;
            _switchSettings.Text = "Settings";
            _switchSettings.TextColor = Color.FromArgb(106, 227, 249);
            _switchSettings.UseVisualStyleBackColor = false;
            _switchSettings.Click += _switchSettings_Click;
            // 
            // _switchSticker
            // 
            _switchSticker.BackColor = Color.FromArgb(7, 36, 43);
            _switchSticker.BackgroundColor = Color.FromArgb(7, 36, 43);
            _switchSticker.BorderColor = Color.FromArgb(5, 55, 66);
            _switchSticker.BorderRadius = 10;
            _switchSticker.BorderSize = 2;
            _switchSticker.Dock = DockStyle.Top;
            _switchSticker.Enabled = false;
            _switchSticker.FlatAppearance.BorderSize = 0;
            _switchSticker.FlatStyle = FlatStyle.Flat;
            _switchSticker.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _switchSticker.ForeColor = Color.FromArgb(106, 227, 249);
            _switchSticker.Location = new Point(5, 125);
            _switchSticker.Name = "_switchSticker";
            _switchSticker.Size = new Size(147, 40);
            _switchSticker.SvgAlignment = ContentAlignment.MiddleLeft;
            _switchSticker.SvgColor = Color.FromArgb(106, 227, 249);
            _switchSticker.SvgContent = resources.GetString("_switchSticker.SvgContent");
            _switchSticker.SvgOffset = new Point(10, 0);
            _switchSticker.SvgPadding = new Padding(0);
            _switchSticker.SvgResource = "sticker_svgrepo_com";
            _switchSticker.SvgSize = new Size(25, 25);
            _switchSticker.TabIndex = 4;
            _switchSticker.Text = "Sticker";
            _switchSticker.TextColor = Color.FromArgb(106, 227, 249);
            _switchSticker.UseVisualStyleBackColor = false;
            _switchSticker.Click += _switchSticker_Click;
            // 
            // _switchEmoji
            // 
            _switchEmoji.BackColor = Color.FromArgb(7, 36, 43);
            _switchEmoji.BackgroundColor = Color.FromArgb(7, 36, 43);
            _switchEmoji.BorderColor = Color.FromArgb(5, 55, 66);
            _switchEmoji.BorderRadius = 10;
            _switchEmoji.BorderSize = 2;
            _switchEmoji.Dock = DockStyle.Top;
            _switchEmoji.Enabled = false;
            _switchEmoji.FlatAppearance.BorderSize = 0;
            _switchEmoji.FlatStyle = FlatStyle.Flat;
            _switchEmoji.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _switchEmoji.ForeColor = Color.FromArgb(106, 227, 249);
            _switchEmoji.Location = new Point(5, 85);
            _switchEmoji.Name = "_switchEmoji";
            _switchEmoji.Size = new Size(147, 40);
            _switchEmoji.SvgAlignment = ContentAlignment.MiddleLeft;
            _switchEmoji.SvgColor = Color.FromArgb(106, 227, 249);
            _switchEmoji.SvgContent = resources.GetString("_switchEmoji.SvgContent");
            _switchEmoji.SvgOffset = new Point(10, 0);
            _switchEmoji.SvgPadding = new Padding(0);
            _switchEmoji.SvgResource = "emoji_svgrepo_com";
            _switchEmoji.SvgSize = new Size(25, 25);
            _switchEmoji.TabIndex = 3;
            _switchEmoji.Text = "Emoji";
            _switchEmoji.TextColor = Color.FromArgb(106, 227, 249);
            _switchEmoji.UseVisualStyleBackColor = false;
            _switchEmoji.Click += _switchEmoji_Click;
            // 
            // _switchPhotos
            // 
            _switchPhotos.BackColor = Color.FromArgb(7, 36, 43);
            _switchPhotos.BackgroundColor = Color.FromArgb(7, 36, 43);
            _switchPhotos.BorderColor = Color.FromArgb(5, 55, 66);
            _switchPhotos.BorderRadius = 10;
            _switchPhotos.BorderSize = 2;
            _switchPhotos.Dock = DockStyle.Top;
            _switchPhotos.Enabled = false;
            _switchPhotos.FlatAppearance.BorderSize = 0;
            _switchPhotos.FlatStyle = FlatStyle.Flat;
            _switchPhotos.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _switchPhotos.ForeColor = Color.FromArgb(106, 227, 249);
            _switchPhotos.Location = new Point(5, 45);
            _switchPhotos.Name = "_switchPhotos";
            _switchPhotos.Size = new Size(147, 40);
            _switchPhotos.SvgAlignment = ContentAlignment.MiddleLeft;
            _switchPhotos.SvgColor = Color.FromArgb(106, 227, 249);
            _switchPhotos.SvgContent = resources.GetString("_switchPhotos.SvgContent");
            _switchPhotos.SvgOffset = new Point(10, 0);
            _switchPhotos.SvgPadding = new Padding(0);
            _switchPhotos.SvgResource = "image_svgrepo_com";
            _switchPhotos.SvgSize = new Size(25, 25);
            _switchPhotos.TabIndex = 2;
            _switchPhotos.Text = "Photos";
            _switchPhotos.TextColor = Color.FromArgb(106, 227, 249);
            _switchPhotos.UseVisualStyleBackColor = false;
            _switchPhotos.Click += _switchPhotos_Click;
            // 
            // _switchIcons
            // 
            _switchIcons.BackColor = Color.FromArgb(7, 36, 43);
            _switchIcons.BackgroundColor = Color.FromArgb(7, 36, 43);
            _switchIcons.BorderColor = Color.FromArgb(5, 55, 66);
            _switchIcons.BorderRadius = 10;
            _switchIcons.BorderSize = 2;
            _switchIcons.Dock = DockStyle.Top;
            _switchIcons.Enabled = false;
            _switchIcons.FlatAppearance.BorderSize = 0;
            _switchIcons.FlatStyle = FlatStyle.Flat;
            _switchIcons.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _switchIcons.ForeColor = Color.FromArgb(106, 227, 249);
            _switchIcons.Location = new Point(5, 5);
            _switchIcons.Name = "_switchIcons";
            _switchIcons.Size = new Size(147, 40);
            _switchIcons.SvgAlignment = ContentAlignment.MiddleLeft;
            _switchIcons.SvgColor = Color.FromArgb(106, 227, 249);
            _switchIcons.SvgContent = resources.GetString("_switchIcons.SvgContent");
            _switchIcons.SvgOffset = new Point(10, 0);
            _switchIcons.SvgPadding = new Padding(0);
            _switchIcons.SvgResource = "scan_svgrepo_com";
            _switchIcons.SvgSize = new Size(25, 25);
            _switchIcons.TabIndex = 1;
            _switchIcons.Text = "Icons";
            _switchIcons.TextColor = Color.FromArgb(106, 227, 249);
            _switchIcons.UseVisualStyleBackColor = false;
            _switchIcons.Click += _switchIcons_Click;
            // 
            // profileBanner
            // 
            profileBanner.BackColor = Color.FromArgb(7, 36, 43);
            profileBanner.BackgroundColor = Color.FromArgb(7, 36, 43);
            profileBanner.BorderColor = Color.FromArgb(5, 55, 66);
            profileBanner.BorderRadiusBottomLeft = 15;
            profileBanner.BorderRadiusBottomRight = 15;
            profileBanner.BorderRadiusTopLeft = 15;
            profileBanner.BorderRadiusTopRight = 15;
            profileBanner.BorderSize = 0;
            profileBanner.ImageLocation = "";
            profileBanner.Location = new Point(12, 63);
            profileBanner.Name = "profileBanner";
            profileBanner.Size = new Size(157, 89);
            profileBanner.SizeMode = PictureBoxSizeMode.StretchImage;
            profileBanner.TabIndex = 7;
            profileBanner.TabStop = false;
            profileBanner.Visible = false;
            // 
            // profileIcon
            // 
            profileIcon.BackColor = Color.FromArgb(7, 36, 43);
            profileIcon.BackgroundColor = Color.FromArgb(7, 36, 43);
            profileIcon.BorderColor = Color.FromArgb(5, 55, 66);
            profileIcon.BorderRadiusBottomLeft = 25;
            profileIcon.BorderRadiusBottomRight = 25;
            profileIcon.BorderRadiusTopLeft = 25;
            profileIcon.BorderRadiusTopRight = 25;
            profileIcon.BorderSize = 0;
            profileIcon.ImageLocation = "";
            profileIcon.Location = new Point(12, 105);
            profileIcon.Name = "profileIcon";
            profileIcon.Size = new Size(55, 55);
            profileIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            profileIcon.TabIndex = 8;
            profileIcon.TabStop = false;
            profileIcon.Visible = false;
            // 
            // badgeBox1
            // 
            badgeBox1.BackColor = Color.Transparent;
            badgeBox1.BackgroundColor = Color.Transparent;
            badgeBox1.BorderColor = Color.PaleVioletRed;
            badgeBox1.BorderRadiusBottomLeft = 75;
            badgeBox1.BorderRadiusBottomRight = 75;
            badgeBox1.BorderRadiusTopLeft = 75;
            badgeBox1.BorderRadiusTopRight = 75;
            badgeBox1.BorderSize = 0;
            badgeBox1.Location = new Point(73, 132);
            badgeBox1.Name = "badgeBox1";
            badgeBox1.Padding = new Padding(2);
            badgeBox1.Size = new Size(30, 30);
            badgeBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            badgeBox1.TabIndex = 0;
            badgeBox1.TabStop = false;
            badgeBox1.Visible = false;
            // 
            // badgeBox2
            // 
            badgeBox2.BackColor = Color.FromArgb(15, 17, 19);
            badgeBox2.BackgroundColor = Color.FromArgb(15, 17, 19);
            badgeBox2.BorderColor = Color.PaleVioletRed;
            badgeBox2.BorderRadiusBottomLeft = 75;
            badgeBox2.BorderRadiusBottomRight = 75;
            badgeBox2.BorderRadiusTopLeft = 75;
            badgeBox2.BorderRadiusTopRight = 75;
            badgeBox2.BorderSize = 0;
            badgeBox2.Location = new Point(109, 132);
            badgeBox2.Name = "badgeBox2";
            badgeBox2.Padding = new Padding(2);
            badgeBox2.Size = new Size(30, 30);
            badgeBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            badgeBox2.TabIndex = 10;
            badgeBox2.TabStop = false;
            badgeBox2.Visible = false;
            // 
            // badgeBox3
            // 
            badgeBox3.BackColor = Color.FromArgb(15, 17, 19);
            badgeBox3.BackgroundColor = Color.FromArgb(15, 17, 19);
            badgeBox3.BorderColor = Color.PaleVioletRed;
            badgeBox3.BorderRadiusBottomLeft = 75;
            badgeBox3.BorderRadiusBottomRight = 75;
            badgeBox3.BorderRadiusTopLeft = 75;
            badgeBox3.BorderRadiusTopRight = 75;
            badgeBox3.BorderSize = 0;
            badgeBox3.Location = new Point(145, 132);
            badgeBox3.Name = "badgeBox3";
            badgeBox3.Padding = new Padding(2);
            badgeBox3.Size = new Size(30, 30);
            badgeBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            badgeBox3.TabIndex = 11;
            badgeBox3.TabStop = false;
            badgeBox3.Visible = false;
            // 
            // MainPanel
            // 
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(15, 17, 19);
            ClientSize = new Size(1015, 589);
            Controls.Add(badgeBox3);
            Controls.Add(badgeBox2);
            Controls.Add(badgeBox1);
            Controls.Add(profileIcon);
            Controls.Add(profileBanner);
            Controls.Add(SwitchPanel);
            Controls.Add(FormsPanel);
            Controls.Add(bannerIcon);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(750, 550);
            Name = "MainPanel";
            Text = "VRCGalleryManager";
            ((System.ComponentModel.ISupportInitialize)bannerIcon).EndInit();
            SwitchPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)profileBanner).EndInit();
            ((System.ComponentModel.ISupportInitialize)profileIcon).EndInit();
            ((System.ComponentModel.ISupportInitialize)badgeBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)badgeBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)badgeBox3).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox bannerIcon;
        private Panel FormsPanel;
        private RoundedPanel SwitchPanel;
        private RoundedButton _switchEmoji;
        private RoundedButton _switchSticker;
        private RoundedButton _switchSettings;
        private RoundedButton _switchPrints;
        private RoundedButton _switchCreate;
        private RoundedButton _switchPhotos;
        private RoundedButton _switchIcons;
        private RoundedPictureBox profileBanner;
        private RoundedPictureBox profileIcon;
        private RoundedPanel badgePanel;
        private RoundedPictureBox badgeBox1;
        private RoundedPictureBox badgeBox2;
        private RoundedPictureBox badgeBox3;
        private RoundedButton _switchPicflow;
    }
}
