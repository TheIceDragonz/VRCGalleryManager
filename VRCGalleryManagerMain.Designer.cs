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
            _switchCreate = new RoundedButton();
            _switchSettings = new RoundedButton();
            _switchSticker = new RoundedButton();
            _switchEmoji = new RoundedButton();
            ((System.ComponentModel.ISupportInitialize)bannerIcon).BeginInit();
            SwitchPanel.SuspendLayout();
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
            SwitchPanel.Controls.Add(_switchCreate);
            SwitchPanel.Controls.Add(_switchSettings);
            SwitchPanel.Controls.Add(_switchSticker);
            SwitchPanel.Controls.Add(_switchEmoji);
            SwitchPanel.Location = new Point(12, 70);
            SwitchPanel.Margin = new Padding(5);
            SwitchPanel.Name = "SwitchPanel";
            SwitchPanel.Padding = new Padding(5);
            SwitchPanel.Size = new Size(157, 500);
            SwitchPanel.TabIndex = 6;
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
            _switchCreate.Location = new Point(5, 85);
            _switchCreate.Name = "_switchCreate";
            _switchCreate.Size = new Size(147, 40);
            _switchCreate.TabIndex = 2;
            _switchCreate.Text = "Create";
            _switchCreate.TextColor = Color.FromArgb(106, 227, 249);
            _switchCreate.UseVisualStyleBackColor = false;
            _switchCreate.Click += _switchCreate_Click;
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
            _switchSettings.Location = new Point(5, 455);
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
            // MainPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 17, 19);
            ClientSize = new Size(1022, 582);
            Controls.Add(SwitchPanel);
            Controls.Add(FormsPanel);
            Controls.Add(bannerIcon);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(750, 500);
            Name = "MainPanel";
            Text = "VRCGalleryManager";
            ((System.ComponentModel.ISupportInitialize)bannerIcon).EndInit();
            SwitchPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PictureBox bannerIcon;
        private RoundedPanel FormsPanel;
        private RoundedPanel SwitchPanel;
        private RoundedButton _switchEmoji;
        private RoundedButton _switchSticker;
        private RoundedButton _switchSettings;
        private RoundedButton _switchCreate;
    }
}
