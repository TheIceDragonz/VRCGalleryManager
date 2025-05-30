﻿using VRCGalleryManager.Design;

namespace VRCGalleryManager.Forms
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            _vrcLogin = new TableLayoutPanelSettings();
            _loginButton = new RoundedButton();
            _vrcLoginLabel = new Label();
            panel1 = new RoundedPanel();
            viewPassword = new RoundedButton();
            _password = new TextBox();
            label6 = new Label();
            panel5 = new RoundedPanel();
            _username = new TextBox();
            label7 = new Label();
            _openTempFile = new RoundedButton();
            _openVrchatLogs = new RoundedButton();
            _clearAllCacheFiles = new RoundedButton();
            infoCacheLabel = new Label();
            _openGitHubPage = new RoundedButton();
            _newUpdate = new RoundedButton();
            _clearAllVRChatLogs = new RoundedButton();
            panel2 = new Panel();
            _vrcLogin.SuspendLayout();
            panel1.SuspendLayout();
            panel5.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // _vrcLogin
            // 
            _vrcLogin.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _vrcLogin.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _vrcLogin.BackColor = Color.FromArgb(14, 16, 19);
            _vrcLogin.BackgroundColor = Color.FromArgb(14, 16, 19);
            _vrcLogin.BorderColor = Color.PaleVioletRed;
            _vrcLogin.BorderRadius = 15;
            _vrcLogin.BorderSize = 0;
            _vrcLogin.ColumnCount = 1;
            _vrcLogin.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _vrcLogin.Controls.Add(_loginButton, 0, 3);
            _vrcLogin.Controls.Add(_vrcLoginLabel, 0, 0);
            _vrcLogin.Controls.Add(panel1, 0, 2);
            _vrcLogin.Controls.Add(panel5, 0, 1);
            _vrcLogin.Location = new Point(11, 11);
            _vrcLogin.Margin = new Padding(0);
            _vrcLogin.Name = "_vrcLogin";
            _vrcLogin.Padding = new Padding(5);
            _vrcLogin.RowCount = 4;
            _vrcLogin.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            _vrcLogin.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            _vrcLogin.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            _vrcLogin.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            _vrcLogin.Size = new Size(985, 170);
            _vrcLogin.TabIndex = 7;
            // 
            // _loginButton
            // 
            _loginButton.BackColor = Color.FromArgb(7, 36, 43);
            _loginButton.BackgroundColor = Color.FromArgb(7, 36, 43);
            _loginButton.BorderColor = Color.FromArgb(5, 55, 66);
            _loginButton.BorderRadius = 10;
            _loginButton.BorderSize = 2;
            _loginButton.Cursor = Cursors.Hand;
            _loginButton.Dock = DockStyle.Fill;
            _loginButton.FlatAppearance.BorderSize = 0;
            _loginButton.FlatStyle = FlatStyle.Flat;
            _loginButton.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _loginButton.ForeColor = Color.FromArgb(106, 227, 249);
            _loginButton.Location = new Point(9, 126);
            _loginButton.Margin = new Padding(4);
            _loginButton.Name = "_loginButton";
            _loginButton.Size = new Size(967, 36);
            _loginButton.SvgAlignment = ContentAlignment.MiddleCenter;
            _loginButton.SvgColor = Color.Black;
            _loginButton.SvgContent = null;
            _loginButton.SvgOffset = new Point(0, 0);
            _loginButton.SvgPadding = new Padding(0);
            _loginButton.SvgResource = null;
            _loginButton.SvgSize = new Size(50, 50);
            _loginButton.TabIndex = 9;
            _loginButton.Text = "Login";
            _loginButton.TextColor = Color.FromArgb(106, 227, 249);
            _loginButton.UseVisualStyleBackColor = false;
            _loginButton.Click += LoginButton_Click;
            // 
            // _vrcLoginLabel
            // 
            _vrcLoginLabel.BackColor = Color.FromArgb(14, 16, 19);
            _vrcLoginLabel.Dock = DockStyle.Fill;
            _vrcLoginLabel.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            _vrcLoginLabel.ForeColor = Color.Azure;
            _vrcLoginLabel.ImageAlign = ContentAlignment.TopLeft;
            _vrcLoginLabel.Location = new Point(10, 5);
            _vrcLoginLabel.Margin = new Padding(5, 0, 5, 0);
            _vrcLoginLabel.Name = "_vrcLoginLabel";
            _vrcLoginLabel.Size = new Size(965, 29);
            _vrcLoginLabel.TabIndex = 1;
            _vrcLoginLabel.Text = "VRChat Login";
            _vrcLoginLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel1.BackColor = Color.FromArgb(7, 36, 43);
            panel1.BackgroundColor = Color.FromArgb(7, 36, 43);
            panel1.BorderColor = Color.PaleVioletRed;
            panel1.BorderRadius = 10;
            panel1.BorderSize = 0;
            panel1.Controls.Add(viewPassword);
            panel1.Controls.Add(_password);
            panel1.Controls.Add(label6);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(5, 78);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new Size(975, 44);
            panel1.TabIndex = 5;
            // 
            // viewPassword
            // 
            viewPassword.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            viewPassword.BackColor = Color.FromArgb(7, 36, 43);
            viewPassword.BackgroundColor = Color.FromArgb(7, 36, 43);
            viewPassword.BorderColor = Color.PaleVioletRed;
            viewPassword.BorderRadius = 10;
            viewPassword.BorderSize = 0;
            viewPassword.FlatAppearance.BorderSize = 0;
            viewPassword.FlatStyle = FlatStyle.Flat;
            viewPassword.ForeColor = Color.White;
            viewPassword.Location = new Point(941, 6);
            viewPassword.Margin = new Padding(4);
            viewPassword.Name = "viewPassword";
            viewPassword.Size = new Size(31, 31);
            viewPassword.SvgAlignment = ContentAlignment.MiddleCenter;
            viewPassword.SvgColor = Color.FromArgb(106, 227, 249);
            viewPassword.SvgContent = resources.GetString("viewPassword.SvgContent");
            viewPassword.SvgOffset = new Point(0, 0);
            viewPassword.SvgPadding = new Padding(0);
            viewPassword.SvgResource = "eye_svgrepo_com";
            viewPassword.SvgSize = new Size(15, 15);
            viewPassword.TabIndex = 15;
            viewPassword.TextColor = Color.White;
            viewPassword.UseVisualStyleBackColor = false;
            viewPassword.MouseDown += viewPassword_MouseDown;
            viewPassword.MouseUp += viewPassword_MouseUp;
            // 
            // _password
            // 
            _password.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _password.BackColor = Color.FromArgb(5, 55, 66);
            _password.BorderStyle = BorderStyle.None;
            _password.Cursor = Cursors.IBeam;
            _password.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _password.ForeColor = Color.FromArgb(106, 227, 249);
            _password.Location = new Point(139, 12);
            _password.Margin = new Padding(5, 4, 5, 4);
            _password.Name = "_password";
            _password.Size = new Size(801, 20);
            _password.TabIndex = 5;
            _password.UseSystemPasswordChar = true;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Left;
            label6.AutoSize = true;
            label6.BackColor = Color.FromArgb(7, 36, 43);
            label6.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            label6.ForeColor = Color.FromArgb(106, 227, 249);
            label6.ImageAlign = ContentAlignment.TopLeft;
            label6.Location = new Point(24, 12);
            label6.Margin = new Padding(5, 0, 5, 0);
            label6.Name = "label6";
            label6.Size = new Size(86, 20);
            label6.TabIndex = 14;
            label6.Text = "Password:";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel5
            // 
            panel5.BackColor = Color.FromArgb(7, 36, 43);
            panel5.BackgroundColor = Color.FromArgb(7, 36, 43);
            panel5.BorderColor = Color.PaleVioletRed;
            panel5.BorderRadius = 10;
            panel5.BorderSize = 0;
            panel5.Controls.Add(_username);
            panel5.Controls.Add(label7);
            panel5.Dock = DockStyle.Fill;
            panel5.Location = new Point(5, 34);
            panel5.Margin = new Padding(0);
            panel5.Name = "panel5";
            panel5.Size = new Size(975, 44);
            panel5.TabIndex = 6;
            // 
            // _username
            // 
            _username.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _username.BackColor = Color.FromArgb(5, 55, 66);
            _username.BorderStyle = BorderStyle.None;
            _username.Cursor = Cursors.IBeam;
            _username.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _username.ForeColor = Color.FromArgb(106, 227, 249);
            _username.Location = new Point(139, 12);
            _username.Margin = new Padding(5, 4, 5, 4);
            _username.Name = "_username";
            _username.Size = new Size(801, 20);
            _username.TabIndex = 5;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label7.AutoSize = true;
            label7.BackColor = Color.FromArgb(7, 36, 43);
            label7.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            label7.ForeColor = Color.FromArgb(106, 227, 249);
            label7.Location = new Point(21, 12);
            label7.Margin = new Padding(5, 0, 5, 0);
            label7.Name = "label7";
            label7.Size = new Size(86, 20);
            label7.TabIndex = 14;
            label7.Text = "Username:";
            label7.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _openTempFile
            // 
            _openTempFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _openTempFile.BackColor = Color.FromArgb(7, 36, 43);
            _openTempFile.BackgroundColor = Color.FromArgb(7, 36, 43);
            _openTempFile.BorderColor = Color.FromArgb(5, 55, 66);
            _openTempFile.BorderRadius = 10;
            _openTempFile.BorderSize = 2;
            _openTempFile.FlatAppearance.BorderSize = 0;
            _openTempFile.FlatStyle = FlatStyle.Flat;
            _openTempFile.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _openTempFile.ForeColor = Color.FromArgb(106, 227, 249);
            _openTempFile.Location = new Point(90, 343);
            _openTempFile.Margin = new Padding(4);
            _openTempFile.Name = "_openTempFile";
            _openTempFile.Size = new Size(843, 50);
            _openTempFile.SvgAlignment = ContentAlignment.MiddleCenter;
            _openTempFile.SvgColor = Color.Black;
            _openTempFile.SvgContent = null;
            _openTempFile.SvgOffset = new Point(0, 0);
            _openTempFile.SvgPadding = new Padding(0);
            _openTempFile.SvgResource = null;
            _openTempFile.SvgSize = new Size(50, 50);
            _openTempFile.TabIndex = 8;
            _openTempFile.Text = "Open Cache Folder";
            _openTempFile.TextColor = Color.FromArgb(106, 227, 249);
            _openTempFile.UseVisualStyleBackColor = false;
            _openTempFile.Click += _openCacheFolder_Click;
            // 
            // _openVrchatLogs
            // 
            _openVrchatLogs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _openVrchatLogs.BackColor = Color.FromArgb(7, 36, 43);
            _openVrchatLogs.BackgroundColor = Color.FromArgb(7, 36, 43);
            _openVrchatLogs.BorderColor = Color.FromArgb(5, 55, 66);
            _openVrchatLogs.BorderRadius = 10;
            _openVrchatLogs.BorderSize = 2;
            _openVrchatLogs.FlatAppearance.BorderSize = 0;
            _openVrchatLogs.FlatStyle = FlatStyle.Flat;
            _openVrchatLogs.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _openVrchatLogs.ForeColor = Color.FromArgb(106, 227, 249);
            _openVrchatLogs.Location = new Point(90, 286);
            _openVrchatLogs.Margin = new Padding(4);
            _openVrchatLogs.Name = "_openVrchatLogs";
            _openVrchatLogs.Size = new Size(843, 50);
            _openVrchatLogs.SvgAlignment = ContentAlignment.MiddleCenter;
            _openVrchatLogs.SvgColor = Color.Black;
            _openVrchatLogs.SvgContent = null;
            _openVrchatLogs.SvgOffset = new Point(0, 0);
            _openVrchatLogs.SvgPadding = new Padding(0);
            _openVrchatLogs.SvgResource = null;
            _openVrchatLogs.SvgSize = new Size(50, 50);
            _openVrchatLogs.TabIndex = 9;
            _openVrchatLogs.Text = "Open VRChat Logs";
            _openVrchatLogs.TextColor = Color.FromArgb(106, 227, 249);
            _openVrchatLogs.UseVisualStyleBackColor = false;
            _openVrchatLogs.Click += _openVrchatLogs_Click;
            // 
            // _clearAllCacheFiles
            // 
            _clearAllCacheFiles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _clearAllCacheFiles.BackColor = Color.FromArgb(7, 36, 43);
            _clearAllCacheFiles.BackgroundColor = Color.FromArgb(7, 36, 43);
            _clearAllCacheFiles.BorderColor = Color.FromArgb(5, 55, 66);
            _clearAllCacheFiles.BorderRadius = 10;
            _clearAllCacheFiles.BorderSize = 2;
            _clearAllCacheFiles.FlatAppearance.BorderSize = 0;
            _clearAllCacheFiles.FlatStyle = FlatStyle.Flat;
            _clearAllCacheFiles.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _clearAllCacheFiles.ForeColor = Color.FromArgb(255, 128, 128);
            _clearAllCacheFiles.Location = new Point(90, 459);
            _clearAllCacheFiles.Margin = new Padding(4);
            _clearAllCacheFiles.Name = "_clearAllCacheFiles";
            _clearAllCacheFiles.Size = new Size(843, 50);
            _clearAllCacheFiles.SvgAlignment = ContentAlignment.MiddleCenter;
            _clearAllCacheFiles.SvgColor = Color.Black;
            _clearAllCacheFiles.SvgContent = null;
            _clearAllCacheFiles.SvgOffset = new Point(0, 0);
            _clearAllCacheFiles.SvgPadding = new Padding(0);
            _clearAllCacheFiles.SvgResource = null;
            _clearAllCacheFiles.SvgSize = new Size(50, 50);
            _clearAllCacheFiles.TabIndex = 10;
            _clearAllCacheFiles.Text = "Clear All Cache Files";
            _clearAllCacheFiles.TextColor = Color.FromArgb(255, 128, 128);
            _clearAllCacheFiles.UseVisualStyleBackColor = false;
            _clearAllCacheFiles.Click += _clearAllCacheFiles_Click;
            // 
            // infoCacheLabel
            // 
            infoCacheLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            infoCacheLabel.BackColor = Color.FromArgb(14, 16, 19);
            infoCacheLabel.ForeColor = Color.FromArgb(106, 227, 249);
            infoCacheLabel.Location = new Point(90, 525);
            infoCacheLabel.Margin = new Padding(4, 0, 4, 0);
            infoCacheLabel.Name = "infoCacheLabel";
            infoCacheLabel.Size = new Size(843, 115);
            infoCacheLabel.TabIndex = 11;
            infoCacheLabel.Text = "Info\r";
            // 
            // _openGitHubPage
            // 
            _openGitHubPage.BackColor = Color.FromArgb(7, 36, 43);
            _openGitHubPage.BackgroundColor = Color.FromArgb(7, 36, 43);
            _openGitHubPage.BorderColor = Color.FromArgb(5, 55, 66);
            _openGitHubPage.BorderRadius = 10;
            _openGitHubPage.BorderSize = 2;
            _openGitHubPage.Dock = DockStyle.Fill;
            _openGitHubPage.FlatAppearance.BorderSize = 0;
            _openGitHubPage.FlatStyle = FlatStyle.Flat;
            _openGitHubPage.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _openGitHubPage.ForeColor = Color.FromArgb(106, 227, 249);
            _openGitHubPage.Location = new Point(0, 0);
            _openGitHubPage.Margin = new Padding(4);
            _openGitHubPage.Name = "_openGitHubPage";
            _openGitHubPage.Size = new Size(654, 45);
            _openGitHubPage.SvgAlignment = ContentAlignment.MiddleCenter;
            _openGitHubPage.SvgColor = Color.Black;
            _openGitHubPage.SvgContent = null;
            _openGitHubPage.SvgOffset = new Point(0, 0);
            _openGitHubPage.SvgPadding = new Padding(0);
            _openGitHubPage.SvgResource = null;
            _openGitHubPage.SvgSize = new Size(50, 50);
            _openGitHubPage.TabIndex = 12;
            _openGitHubPage.Text = "GitHub Page";
            _openGitHubPage.TextColor = Color.FromArgb(106, 227, 249);
            _openGitHubPage.UseVisualStyleBackColor = false;
            _openGitHubPage.Click += _openGitHubPage_Click;
            // 
            // _newUpdate
            // 
            _newUpdate.BackColor = Color.FromArgb(7, 36, 43);
            _newUpdate.BackgroundColor = Color.FromArgb(7, 36, 43);
            _newUpdate.BorderColor = Color.FromArgb(5, 55, 66);
            _newUpdate.BorderRadius = 10;
            _newUpdate.BorderSize = 2;
            _newUpdate.Dock = DockStyle.Right;
            _newUpdate.FlatAppearance.BorderSize = 0;
            _newUpdate.FlatStyle = FlatStyle.Flat;
            _newUpdate.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _newUpdate.ForeColor = Color.FromArgb(255, 255, 128);
            _newUpdate.Location = new Point(654, 0);
            _newUpdate.Margin = new Padding(4);
            _newUpdate.Name = "_newUpdate";
            _newUpdate.Size = new Size(189, 45);
            _newUpdate.SvgAlignment = ContentAlignment.MiddleCenter;
            _newUpdate.SvgColor = Color.Black;
            _newUpdate.SvgContent = null;
            _newUpdate.SvgOffset = new Point(0, 0);
            _newUpdate.SvgPadding = new Padding(0);
            _newUpdate.SvgResource = null;
            _newUpdate.SvgSize = new Size(50, 50);
            _newUpdate.TabIndex = 13;
            _newUpdate.Text = "New Update";
            _newUpdate.TextColor = Color.FromArgb(255, 255, 128);
            _newUpdate.UseVisualStyleBackColor = false;
            _newUpdate.Visible = false;
            _newUpdate.Click += _newUpdate_Click;
            // 
            // _clearAllVRChatLogs
            // 
            _clearAllVRChatLogs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _clearAllVRChatLogs.BackColor = Color.FromArgb(7, 36, 43);
            _clearAllVRChatLogs.BackgroundColor = Color.FromArgb(7, 36, 43);
            _clearAllVRChatLogs.BorderColor = Color.FromArgb(5, 55, 66);
            _clearAllVRChatLogs.BorderRadius = 10;
            _clearAllVRChatLogs.BorderSize = 2;
            _clearAllVRChatLogs.FlatAppearance.BorderSize = 0;
            _clearAllVRChatLogs.FlatStyle = FlatStyle.Flat;
            _clearAllVRChatLogs.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _clearAllVRChatLogs.ForeColor = Color.FromArgb(255, 128, 128);
            _clearAllVRChatLogs.Location = new Point(90, 401);
            _clearAllVRChatLogs.Margin = new Padding(4);
            _clearAllVRChatLogs.Name = "_clearAllVRChatLogs";
            _clearAllVRChatLogs.Size = new Size(843, 50);
            _clearAllVRChatLogs.SvgAlignment = ContentAlignment.MiddleCenter;
            _clearAllVRChatLogs.SvgColor = Color.Black;
            _clearAllVRChatLogs.SvgContent = null;
            _clearAllVRChatLogs.SvgOffset = new Point(0, 0);
            _clearAllVRChatLogs.SvgPadding = new Padding(0);
            _clearAllVRChatLogs.SvgResource = null;
            _clearAllVRChatLogs.SvgSize = new Size(50, 50);
            _clearAllVRChatLogs.TabIndex = 14;
            _clearAllVRChatLogs.Text = "Clear All VRChat Logs";
            _clearAllVRChatLogs.TextColor = Color.FromArgb(255, 128, 128);
            _clearAllVRChatLogs.UseVisualStyleBackColor = false;
            _clearAllVRChatLogs.Click += _clearAllVRChatLogs_Click;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel2.Controls.Add(_openGitHubPage);
            panel2.Controls.Add(_newUpdate);
            panel2.Location = new Point(90, 208);
            panel2.Name = "panel2";
            panel2.Size = new Size(843, 45);
            panel2.TabIndex = 15;
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            BackColor = Color.FromArgb(5, 5, 5);
            ClientSize = new Size(1028, 823);
            Controls.Add(panel2);
            Controls.Add(_clearAllVRChatLogs);
            Controls.Add(infoCacheLabel);
            Controls.Add(_clearAllCacheFiles);
            Controls.Add(_openVrchatLogs);
            Controls.Add(_openTempFile);
            Controls.Add(_vrcLogin);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(5, 4, 5, 4);
            Name = "Settings";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AssetBundle";
            _vrcLogin.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private VRCGalleryManager.Design.TableLayoutPanelSettings _vrcLogin;
        private VRCGalleryManager.Design.RoundedPanel panel1;
        private System.Windows.Forms.Label label6;
        private VRCGalleryManager.Design.RoundedPanel panel5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label _vrcLoginLabel;
        public System.Windows.Forms.TextBox _password;
        public System.Windows.Forms.TextBox _username;
        public VRCGalleryManager.Design.RoundedButton _loginButton;
        private RoundedButton _openTempFile;
        private RoundedButton _openVrchatLogs;
        private RoundedButton _clearAllCacheFiles;
        private Label infoCacheLabel;
        private RoundedButton _openGitHubPage;
        private RoundedButton _newUpdate;
        private RoundedButton _clearAllVRChatLogs;
        private RoundedButton viewPassword;
        private Panel panel2;
    }
}