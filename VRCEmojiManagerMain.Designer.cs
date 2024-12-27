using VRCEmojiManager.Design;

namespace VRCEmojiManager
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
            _switchSettings = new RoundedButton();
            _switchCreate = new RoundedButton();
            _switchEmoji = new RoundedButton();
            ((System.ComponentModel.ISupportInitialize)Icon).BeginInit();
            SwitchPanel.SuspendLayout();
            SuspendLayout();
            // 
            // Icon
            // 
            Icon.ImageLocation = "E:\\- ProgramLabs\\VRCEmojiManager\\256x256.ico";
            Icon.Location = new Point(12, 12);
            Icon.Name = "Icon";
            Icon.Size = new Size(278, 66);
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
            FormsPanel.Location = new Point(296, 12);
            FormsPanel.Name = "FormsPanel";
            FormsPanel.Size = new Size(647, 422);
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
            SwitchPanel.Controls.Add(_switchSettings);
            SwitchPanel.Controls.Add(_switchCreate);
            SwitchPanel.Controls.Add(_switchEmoji);
            SwitchPanel.Location = new Point(12, 84);
            SwitchPanel.Name = "SwitchPanel";
            SwitchPanel.Padding = new Padding(5);
            SwitchPanel.Size = new Size(278, 350);
            SwitchPanel.TabIndex = 6;
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
            _switchSettings.Location = new Point(5, 305);
            _switchSettings.Name = "_switchSettings";
            _switchSettings.Size = new Size(268, 40);
            _switchSettings.TabIndex = 2;
            _switchSettings.Text = "Settings";
            _switchSettings.TextColor = Color.White;
            _switchSettings.UseVisualStyleBackColor = false;
            _switchSettings.Click += _switchSettings_Click;
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
            _switchCreate.Location = new Point(5, 45);
            _switchCreate.Name = "_switchCreate";
            _switchCreate.Size = new Size(268, 40);
            _switchCreate.TabIndex = 1;
            _switchCreate.Text = "Create";
            _switchCreate.TextColor = Color.White;
            _switchCreate.UseVisualStyleBackColor = false;
            _switchCreate.Click += _switchCreate_Click;
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
            _switchEmoji.Size = new Size(268, 40);
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
            ClientSize = new Size(955, 446);
            Controls.Add(SwitchPanel);
            Controls.Add(FormsPanel);
            Controls.Add(Icon);
            MaximumSize = new Size(1200, 750);
            MinimumSize = new Size(600, 300);
            Name = "MainPanel";
            Text = "VRCEmojiManager";
            ((System.ComponentModel.ISupportInitialize)Icon).EndInit();
            SwitchPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PictureBox Icon;
        private RoundedPanel FormsPanel;
        private RoundedPanel SwitchPanel;
        private RoundedButton _switchEmoji;
        private RoundedButton _switchCreate;
        private RoundedButton _switchSettings;
    }
}
