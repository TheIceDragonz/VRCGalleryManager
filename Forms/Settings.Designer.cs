using VRCGalleryManager.Design;

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
            _checkUpdate = new RoundedButton();
            _clearAllVRChatLogs = new RoundedButton();
            _vrcLogin.SuspendLayout();
            panel1.SuspendLayout();
            panel5.SuspendLayout();
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
            _vrcLogin.Location = new Point(9, 9);
            _vrcLogin.Margin = new Padding(0);
            _vrcLogin.Name = "_vrcLogin";
            _vrcLogin.Padding = new Padding(4);
            _vrcLogin.RowCount = 4;
            _vrcLogin.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
            _vrcLogin.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            _vrcLogin.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            _vrcLogin.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            _vrcLogin.Size = new Size(855, 136);
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
            _loginButton.Location = new Point(7, 100);
            _loginButton.Name = "_loginButton";
            _loginButton.Size = new Size(841, 29);
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
            _vrcLoginLabel.Location = new Point(8, 4);
            _vrcLoginLabel.Margin = new Padding(4, 0, 4, 0);
            _vrcLoginLabel.Name = "_vrcLoginLabel";
            _vrcLoginLabel.Size = new Size(839, 23);
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
            panel1.Location = new Point(4, 62);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new Size(847, 35);
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
            viewPassword.Location = new Point(816, 5);
            viewPassword.Name = "viewPassword";
            viewPassword.Size = new Size(25, 25);
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
            _password.Location = new Point(111, 10);
            _password.Margin = new Padding(4, 3, 4, 3);
            _password.Name = "_password";
            _password.Size = new Size(698, 16);
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
            label6.Location = new Point(19, 10);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(69, 15);
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
            panel5.Location = new Point(4, 27);
            panel5.Margin = new Padding(0);
            panel5.Name = "panel5";
            panel5.Size = new Size(847, 35);
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
            _username.Location = new Point(111, 10);
            _username.Margin = new Padding(4, 3, 4, 3);
            _username.Name = "_username";
            _username.Size = new Size(725, 16);
            _username.TabIndex = 5;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label7.AutoSize = true;
            label7.BackColor = Color.FromArgb(7, 36, 43);
            label7.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            label7.ForeColor = Color.FromArgb(106, 227, 249);
            label7.Location = new Point(17, 10);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(71, 15);
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
            _openTempFile.Location = new Point(72, 330);
            _openTempFile.Name = "_openTempFile";
            _openTempFile.Size = new Size(730, 40);
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
            _openVrchatLogs.Location = new Point(72, 284);
            _openVrchatLogs.Name = "_openVrchatLogs";
            _openVrchatLogs.Size = new Size(730, 40);
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
            _clearAllCacheFiles.Location = new Point(72, 422);
            _clearAllCacheFiles.Name = "_clearAllCacheFiles";
            _clearAllCacheFiles.Size = new Size(730, 40);
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
            infoCacheLabel.Location = new Point(72, 475);
            infoCacheLabel.Name = "infoCacheLabel";
            infoCacheLabel.Size = new Size(730, 81);
            infoCacheLabel.TabIndex = 11;
            infoCacheLabel.Text = "Info\r";
            // 
            // _openGitHubPage
            // 
            _openGitHubPage.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _openGitHubPage.BackColor = Color.FromArgb(7, 36, 43);
            _openGitHubPage.BackgroundColor = Color.FromArgb(7, 36, 43);
            _openGitHubPage.BorderColor = Color.FromArgb(5, 55, 66);
            _openGitHubPage.BorderRadius = 10;
            _openGitHubPage.BorderSize = 2;
            _openGitHubPage.FlatAppearance.BorderSize = 0;
            _openGitHubPage.FlatStyle = FlatStyle.Flat;
            _openGitHubPage.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _openGitHubPage.ForeColor = Color.FromArgb(106, 227, 249);
            _openGitHubPage.Location = new Point(72, 170);
            _openGitHubPage.Name = "_openGitHubPage";
            _openGitHubPage.Size = new Size(730, 40);
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
            // _checkUpdate
            // 
            _checkUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _checkUpdate.BackColor = Color.FromArgb(7, 36, 43);
            _checkUpdate.BackgroundColor = Color.FromArgb(7, 36, 43);
            _checkUpdate.BorderColor = Color.FromArgb(5, 55, 66);
            _checkUpdate.BorderRadius = 10;
            _checkUpdate.BorderSize = 2;
            _checkUpdate.FlatAppearance.BorderSize = 0;
            _checkUpdate.FlatStyle = FlatStyle.Flat;
            _checkUpdate.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _checkUpdate.ForeColor = Color.FromArgb(255, 255, 128);
            _checkUpdate.Location = new Point(72, 216);
            _checkUpdate.Name = "_checkUpdate";
            _checkUpdate.Size = new Size(730, 40);
            _checkUpdate.SvgAlignment = ContentAlignment.MiddleCenter;
            _checkUpdate.SvgColor = Color.Black;
            _checkUpdate.SvgContent = null;
            _checkUpdate.SvgOffset = new Point(0, 0);
            _checkUpdate.SvgPadding = new Padding(0);
            _checkUpdate.SvgResource = null;
            _checkUpdate.SvgSize = new Size(50, 50);
            _checkUpdate.TabIndex = 13;
            _checkUpdate.Text = "Check Update";
            _checkUpdate.TextColor = Color.FromArgb(255, 255, 128);
            _checkUpdate.UseVisualStyleBackColor = false;
            _checkUpdate.Click += _checkUpdate_Click;
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
            _clearAllVRChatLogs.Location = new Point(72, 376);
            _clearAllVRChatLogs.Name = "_clearAllVRChatLogs";
            _clearAllVRChatLogs.Size = new Size(730, 40);
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
            // Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = Color.FromArgb(5, 5, 5);
            ClientSize = new Size(873, 621);
            Controls.Add(_clearAllVRChatLogs);
            Controls.Add(_checkUpdate);
            Controls.Add(_openGitHubPage);
            Controls.Add(infoCacheLabel);
            Controls.Add(_clearAllCacheFiles);
            Controls.Add(_openVrchatLogs);
            Controls.Add(_openTempFile);
            Controls.Add(_vrcLogin);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            Name = "Settings";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AssetBundle";
            _vrcLogin.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
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
        private RoundedButton _checkUpdate;
        private RoundedButton _clearAllVRChatLogs;
        private RoundedButton viewPassword;
    }
}