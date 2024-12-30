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
            _vrcLogin = new TableLayoutPanelSettings();
            _loginButton = new RoundedButton();
            _vrcLoginLabel = new Label();
            panel1 = new RoundedPanel();
            _password = new TextBox();
            label6 = new Label();
            panel5 = new RoundedPanel();
            _username = new TextBox();
            label7 = new Label();
            _vrcLogin.SuspendLayout();
            panel1.SuspendLayout();
            panel5.SuspendLayout();
            SuspendLayout();
            // 
            // _vrcLogin
            // 
            _vrcLogin.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _vrcLogin.AutoSize = true;
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
            _vrcLogin.Location = new Point(14, 16);
            _vrcLogin.Margin = new Padding(0);
            _vrcLogin.Name = "_vrcLogin";
            _vrcLogin.Padding = new Padding(4);
            _vrcLogin.RowCount = 4;
            _vrcLogin.RowStyles.Add(new RowStyle(SizeType.Absolute, 23F));
            _vrcLogin.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            _vrcLogin.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            _vrcLogin.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            _vrcLogin.Size = new Size(487, 136);
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
            _loginButton.Size = new Size(473, 29);
            _loginButton.TabIndex = 9;
            _loginButton.Text = "Login";
            _loginButton.TextColor = Color.FromArgb(106, 227, 249);
            _loginButton.UseVisualStyleBackColor = false;
            _loginButton.Click += LoginButton;
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
            _vrcLoginLabel.Size = new Size(471, 23);
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
            panel1.Controls.Add(_password);
            panel1.Controls.Add(label6);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(4, 62);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new Size(479, 35);
            panel1.TabIndex = 5;
            // 
            // _password
            // 
            _password.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _password.BackColor = Color.FromArgb(5, 55, 66);
            _password.BorderStyle = BorderStyle.None;
            _password.Cursor = Cursors.IBeam;
            _password.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _password.ForeColor = Color.FromArgb(106, 227, 249);
            _password.Location = new Point(111, 11);
            _password.Margin = new Padding(4, 3, 4, 3);
            _password.Name = "_password";
            _password.PasswordChar = '*';
            _password.Size = new Size(357, 13);
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
            label6.Location = new Point(4, 10);
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
            panel5.Size = new Size(479, 35);
            panel5.TabIndex = 6;
            // 
            // _username
            // 
            _username.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _username.BackColor = Color.FromArgb(5, 55, 66);
            _username.BorderStyle = BorderStyle.None;
            _username.Cursor = Cursors.IBeam;
            _username.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _username.ForeColor = Color.FromArgb(106, 227, 249);
            _username.Location = new Point(111, 11);
            _username.Margin = new Padding(4, 3, 4, 3);
            _username.Name = "_username";
            _username.Size = new Size(357, 13);
            _username.TabIndex = 5;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label7.AutoSize = true;
            label7.BackColor = Color.FromArgb(7, 36, 43);
            label7.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            label7.ForeColor = Color.FromArgb(106, 227, 249);
            label7.Location = new Point(4, 10);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(71, 15);
            label7.TabIndex = 14;
            label7.Text = "Username:";
            label7.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            BackColor = Color.FromArgb(5, 5, 5);
            ClientSize = new Size(515, 387);
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
            PerformLayout();
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
    }
}