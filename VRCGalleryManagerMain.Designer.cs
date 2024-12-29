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
            Icon = new PictureBox();
            FormsPanel = new RoundedPanel();
            SwitchPanel = new RoundedPanel();
            _switchCreate = new RoundedButton();
            _switchSettings = new RoundedButton();
            _switchSticker = new RoundedButton();
            _switchEmoji = new RoundedButton();
            ((System.ComponentModel.ISupportInitialize)Icon).BeginInit();
            SwitchPanel.SuspendLayout();
            SuspendLayout();
            // 
            // Icon
            // 
            Icon.ImageLocation = "E:\\- ProgramLabs\\VRCGalleryManager\\256x256.ico";
            Icon.Location = new Point(12, 12);
            Icon.Margin = new Padding(10);
            Icon.Name = "Icon";
            Icon.Size = new Size(157, 45);
            Icon.SizeMode = PictureBoxSizeMode.Zoom;
            Icon.TabIndex = 4;
            Icon.TabStop = false;
            // 
            // FormsPanel
            // 
            FormsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            FormsPanel.BackColor = Color.FromArgb(64, 64, 64);
            FormsPanel.BackgroundColor = Color.FromArgb(64, 64, 64);
            FormsPanel.BorderColor = Color.FromArgb(80, 80, 80);
            FormsPanel.BorderRadius = 25;
            FormsPanel.BorderSize = 0;
            FormsPanel.Location = new Point(182, 12);
            FormsPanel.Name = "FormsPanel";
            FormsPanel.Size = new Size(828, 558);
            FormsPanel.TabIndex = 5;
            // 
            // SwitchPanel
            // 
            SwitchPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            SwitchPanel.BackColor = Color.FromArgb(64, 64, 64);
            SwitchPanel.BackgroundColor = Color.FromArgb(64, 64, 64);
            SwitchPanel.BorderColor = Color.FromArgb(80, 80, 80);
            SwitchPanel.BorderRadius = 25;
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
            _switchCreate.BackColor = Color.FromArgb(50, 50, 50);
            _switchCreate.BackgroundColor = Color.FromArgb(50, 50, 50);
            _switchCreate.BorderColor = Color.White;
            _switchCreate.BorderRadius = 20;
            _switchCreate.BorderSize = 0;
            _switchCreate.Dock = DockStyle.Top;
            _switchCreate.FlatAppearance.BorderSize = 0;
            _switchCreate.FlatStyle = FlatStyle.Flat;
            _switchCreate.ForeColor = Color.White;
            _switchCreate.Location = new Point(5, 85);
            _switchCreate.Name = "_switchCreate";
            _switchCreate.Size = new Size(147, 40);
            _switchCreate.TabIndex = 2;
            _switchCreate.Text = "Create";
            _switchCreate.TextColor = Color.White;
            _switchCreate.UseVisualStyleBackColor = false;
            _switchCreate.Click += _switchCreate_Click;
            // 
            // _switchSettings
            // 
            _switchSettings.BackColor = Color.FromArgb(50, 50, 50);
            _switchSettings.BackgroundColor = Color.FromArgb(50, 50, 50);
            _switchSettings.BorderColor = Color.White;
            _switchSettings.BorderRadius = 20;
            _switchSettings.BorderSize = 0;
            _switchSettings.Dock = DockStyle.Bottom;
            _switchSettings.FlatAppearance.BorderSize = 0;
            _switchSettings.FlatStyle = FlatStyle.Flat;
            _switchSettings.ForeColor = Color.White;
            _switchSettings.Location = new Point(5, 455);
            _switchSettings.Name = "_switchSettings";
            _switchSettings.Size = new Size(147, 40);
            _switchSettings.TabIndex = 2;
            _switchSettings.Text = "Settings";
            _switchSettings.TextColor = Color.White;
            _switchSettings.UseVisualStyleBackColor = false;
            _switchSettings.Click += _switchSettings_Click;
            // 
            // _switchSticker
            // 
            _switchSticker.BackColor = Color.FromArgb(50, 50, 50);
            _switchSticker.BackgroundColor = Color.FromArgb(50, 50, 50);
            _switchSticker.BorderColor = Color.White;
            _switchSticker.BorderRadius = 20;
            _switchSticker.BorderSize = 0;
            _switchSticker.Dock = DockStyle.Top;
            _switchSticker.FlatAppearance.BorderSize = 0;
            _switchSticker.FlatStyle = FlatStyle.Flat;
            _switchSticker.ForeColor = Color.White;
            _switchSticker.Location = new Point(5, 45);
            _switchSticker.Name = "_switchSticker";
            _switchSticker.Size = new Size(147, 40);
            _switchSticker.TabIndex = 1;
            _switchSticker.Text = "Sticker";
            _switchSticker.TextColor = Color.White;
            _switchSticker.UseVisualStyleBackColor = false;
            _switchSticker.Click += _switchSticker_Click;
            // 
            // _switchEmoji
            // 
            _switchEmoji.BackColor = Color.FromArgb(50, 50, 50);
            _switchEmoji.BackgroundColor = Color.FromArgb(50, 50, 50);
            _switchEmoji.BorderColor = Color.White;
            _switchEmoji.BorderRadius = 20;
            _switchEmoji.BorderSize = 0;
            _switchEmoji.Dock = DockStyle.Top;
            _switchEmoji.FlatAppearance.BorderSize = 0;
            _switchEmoji.FlatStyle = FlatStyle.Flat;
            _switchEmoji.ForeColor = Color.White;
            _switchEmoji.Location = new Point(5, 5);
            _switchEmoji.Name = "_switchEmoji";
            _switchEmoji.Size = new Size(147, 40);
            _switchEmoji.TabIndex = 0;
            _switchEmoji.Text = "Emoji";
            _switchEmoji.TextColor = Color.White;
            _switchEmoji.UseVisualStyleBackColor = false;
            _switchEmoji.Click += _switchEmoji_Click;
            // 
            // MainPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(50, 50, 50);
            ClientSize = new Size(1022, 582);
            Controls.Add(SwitchPanel);
            Controls.Add(FormsPanel);
            Controls.Add(Icon);
            MaximumSize = new Size(1200, 750);
            MinimumSize = new Size(600, 300);
            Name = "MainPanel";
            Text = "VRCGalleryManager";
            ((System.ComponentModel.ISupportInitialize)Icon).EndInit();
            SwitchPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PictureBox Icon;
        private RoundedPanel FormsPanel;
        private RoundedPanel SwitchPanel;
        private RoundedButton _switchEmoji;
        private RoundedButton _switchSticker;
        private RoundedButton _switchSettings;
        private RoundedButton _switchCreate;
    }
}
