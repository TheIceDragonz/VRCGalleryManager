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
            FormsPanel = new RoundedPanel();
            SwitchPanel = new RoundedPanel();
            _switchPhotos = new RoundedButton();
            _switchIcons = new RoundedButton();
            _switchCreate = new RoundedButton();
            _switchPrints = new RoundedButton();
            _switchSettings = new RoundedButton();
            _switchSticker = new RoundedButton();
            _switchEmoji = new RoundedButton();
            profileBanner = new RoundedPictureBox();
            profileIcon = new RoundedPictureBox();
            ((System.ComponentModel.ISupportInitialize)bannerIcon).BeginInit();
            SwitchPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)profileBanner).BeginInit();
            ((System.ComponentModel.ISupportInitialize)profileIcon).BeginInit();
            SuspendLayout();
            // 
            // bannerIcon
            // 
            bannerIcon.Image = Properties.Resources.Icon;
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
            FormsPanel.BackgroundColor = Color.FromArgb(5, 5, 5);
            FormsPanel.BorderColor = Color.FromArgb(80, 80, 80);
            FormsPanel.BorderRadius = 15;
            FormsPanel.BorderSize = 0;
            FormsPanel.Location = new Point(182, 12);
            FormsPanel.Name = "FormsPanel";
            FormsPanel.Size = new Size(828, 558);
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
            SwitchPanel.Controls.Add(_switchPhotos);
            SwitchPanel.Controls.Add(_switchIcons);
            SwitchPanel.Controls.Add(_switchCreate);
            SwitchPanel.Controls.Add(_switchPrints);
            SwitchPanel.Controls.Add(_switchSettings);
            SwitchPanel.Controls.Add(_switchSticker);
            SwitchPanel.Controls.Add(_switchEmoji);
            SwitchPanel.Location = new Point(12, 170);
            SwitchPanel.Margin = new Padding(5);
            SwitchPanel.Name = "SwitchPanel";
            SwitchPanel.Padding = new Padding(5);
            SwitchPanel.Size = new Size(157, 400);
            SwitchPanel.TabIndex = 6;
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
            _switchPhotos.Location = new Point(5, 205);
            _switchPhotos.Name = "_switchPhotos";
            _switchPhotos.Size = new Size(147, 40);
            _switchPhotos.TabIndex = 5;
            _switchPhotos.Text = "Photos";
            _switchPhotos.TextColor = Color.FromArgb(106, 227, 249);
            _switchPhotos.UseVisualStyleBackColor = false;
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
            _switchIcons.Location = new Point(5, 165);
            _switchIcons.Name = "_switchIcons";
            _switchIcons.Size = new Size(147, 40);
            _switchIcons.TabIndex = 4;
            _switchIcons.Text = "Icons";
            _switchIcons.TextColor = Color.FromArgb(106, 227, 249);
            _switchIcons.UseVisualStyleBackColor = false;
            // 
            // _switchCreate
            // 
            _switchCreate.BackColor = Color.FromArgb(7, 36, 43);
            _switchCreate.BackgroundColor = Color.FromArgb(7, 36, 43);
            _switchCreate.BorderColor = Color.FromArgb(5, 55, 66);
            _switchCreate.BorderRadius = 10;
            _switchCreate.BorderSize = 2;
            _switchCreate.Dock = DockStyle.Top;
            _switchCreate.FlatAppearance.BorderSize = 0;
            _switchCreate.FlatStyle = FlatStyle.Flat;
            _switchCreate.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _switchCreate.ForeColor = Color.FromArgb(106, 227, 249);
            _switchCreate.Location = new Point(5, 125);
            _switchCreate.Name = "_switchCreate";
            _switchCreate.Size = new Size(147, 40);
            _switchCreate.TabIndex = 3;
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
            _switchPrints.FlatAppearance.BorderSize = 0;
            _switchPrints.FlatStyle = FlatStyle.Flat;
            _switchPrints.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _switchPrints.ForeColor = Color.FromArgb(106, 227, 249);
            _switchPrints.Location = new Point(5, 85);
            _switchPrints.Name = "_switchPrints";
            _switchPrints.Size = new Size(147, 40);
            _switchPrints.TabIndex = 2;
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
            _switchSettings.Location = new Point(5, 355);
            _switchSettings.Name = "_switchSettings";
            _switchSettings.Size = new Size(147, 40);
            _switchSettings.TabIndex = 2;
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
            _switchSticker.FlatAppearance.BorderSize = 0;
            _switchSticker.FlatStyle = FlatStyle.Flat;
            _switchSticker.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _switchSticker.ForeColor = Color.FromArgb(106, 227, 249);
            _switchSticker.Location = new Point(5, 45);
            _switchSticker.Name = "_switchSticker";
            _switchSticker.Size = new Size(147, 40);
            _switchSticker.TabIndex = 1;
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
            _switchEmoji.FlatAppearance.BorderSize = 0;
            _switchEmoji.FlatStyle = FlatStyle.Flat;
            _switchEmoji.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _switchEmoji.ForeColor = Color.FromArgb(106, 227, 249);
            _switchEmoji.Location = new Point(5, 5);
            _switchEmoji.Name = "_switchEmoji";
            _switchEmoji.Size = new Size(147, 40);
            _switchEmoji.TabIndex = 0;
            _switchEmoji.Text = "Emoji";
            _switchEmoji.TextColor = Color.FromArgb(106, 227, 249);
            _switchEmoji.UseVisualStyleBackColor = false;
            _switchEmoji.Click += _switchEmoji_Click;
            // 
            // profileBanner
            // 
            profileBanner.BackColor = Color.FromArgb(7, 36, 43);
            profileBanner.BackgroundColor = Color.FromArgb(7, 36, 43);
            profileBanner.BorderColor = Color.FromArgb(5, 55, 66);
            profileBanner.BorderRadiusBottomLeft = 20;
            profileBanner.BorderRadiusBottomRight = 20;
            profileBanner.BorderRadiusTopLeft = 20;
            profileBanner.BorderRadiusTopRight = 20;
            profileBanner.BorderSize = 0;
            profileBanner.ImageLocation = "";
            profileBanner.Location = new Point(12, 63);
            profileBanner.Name = "profileBanner";
            profileBanner.Size = new Size(157, 89);
            profileBanner.SizeMode = PictureBoxSizeMode.StretchImage;
            profileBanner.TabIndex = 7;
            profileBanner.TabStop = false;
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
            profileIcon.Location = new Point(21, 105);
            profileIcon.Margin = new Padding(5);
            profileIcon.Name = "profileIcon";
            profileIcon.Size = new Size(55, 55);
            profileIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            profileIcon.TabIndex = 8;
            profileIcon.TabStop = false;
            // 
            // MainPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 17, 19);
            ClientSize = new Size(1022, 582);
            Controls.Add(profileIcon);
            Controls.Add(profileBanner);
            Controls.Add(SwitchPanel);
            Controls.Add(FormsPanel);
            Controls.Add(bannerIcon);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(750, 600);
            Name = "MainPanel";
            Text = "VRCGalleryManager";
            ((System.ComponentModel.ISupportInitialize)bannerIcon).EndInit();
            SwitchPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)profileBanner).EndInit();
            ((System.ComponentModel.ISupportInitialize)profileIcon).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox bannerIcon;
        private RoundedPanel FormsPanel;
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
    }
}
