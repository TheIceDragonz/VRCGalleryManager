using VRCGalleryManager.Design;

namespace VRCGalleryManager.Forms
{
    partial class Gallery
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Gallery));
            galleryPanel = new FlowLayoutPanel();
            _refreshButton = new RoundedButton();
            folderBack = new RoundedButton();
            SuspendLayout();
            // 
            // galleryPanel
            // 
            galleryPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            galleryPanel.AutoScroll = true;
            galleryPanel.BackColor = Color.FromArgb(5, 5, 5);
            galleryPanel.Location = new Point(15, 56);
            galleryPanel.Margin = new Padding(4);
            galleryPanel.Name = "galleryPanel";
            galleryPanel.Size = new Size(972, 617);
            galleryPanel.TabIndex = 2;
            // 
            // _refreshButton
            // 
            _refreshButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _refreshButton.BackColor = Color.FromArgb(7, 36, 43);
            _refreshButton.BackgroundColor = Color.FromArgb(7, 36, 43);
            _refreshButton.BorderColor = Color.FromArgb(5, 55, 66);
            _refreshButton.BorderRadius = 10;
            _refreshButton.BorderSize = 2;
            _refreshButton.FlatAppearance.BorderSize = 0;
            _refreshButton.FlatStyle = FlatStyle.Flat;
            _refreshButton.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _refreshButton.ForeColor = Color.FromArgb(106, 227, 249);
            _refreshButton.Location = new Point(842, 12);
            _refreshButton.Margin = new Padding(4);
            _refreshButton.Name = "_refreshButton";
            _refreshButton.Size = new Size(145, 36);
            _refreshButton.SvgAlignment = ContentAlignment.MiddleCenter;
            _refreshButton.SvgColor = Color.Black;
            _refreshButton.SvgContent = null;
            _refreshButton.SvgOffset = new Point(0, 0);
            _refreshButton.SvgPadding = new Padding(0);
            _refreshButton.SvgResource = null;
            _refreshButton.SvgSize = new Size(50, 50);
            _refreshButton.TabIndex = 3;
            _refreshButton.Text = "Refresh";
            _refreshButton.TextColor = Color.FromArgb(106, 227, 249);
            _refreshButton.UseVisualStyleBackColor = false;
            _refreshButton.Click += _refreshButton_Click;
            // 
            // folderBack
            // 
            folderBack.BackColor = Color.FromArgb(7, 36, 43);
            folderBack.BackgroundColor = Color.FromArgb(7, 36, 43);
            folderBack.BorderColor = Color.FromArgb(5, 55, 66);
            folderBack.BorderRadius = 10;
            folderBack.BorderSize = 2;
            folderBack.FlatAppearance.BorderSize = 0;
            folderBack.FlatStyle = FlatStyle.Flat;
            folderBack.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            folderBack.ForeColor = Color.FromArgb(106, 227, 249);
            folderBack.Location = new Point(15, 12);
            folderBack.Margin = new Padding(4);
            folderBack.Name = "folderBack";
            folderBack.Size = new Size(52, 35);
            folderBack.SvgAlignment = ContentAlignment.MiddleCenter;
            folderBack.SvgColor = Color.FromArgb(106, 227, 249);
            folderBack.SvgContent = resources.GetString("folderBack.SvgContent");
            folderBack.SvgOffset = new Point(0, 0);
            folderBack.SvgPadding = new Padding(0);
            folderBack.SvgResource = "open_in_browser_svgrepo_com";
            folderBack.SvgSize = new Size(20, 20);
            folderBack.TabIndex = 4;
            folderBack.TextColor = Color.FromArgb(106, 227, 249);
            folderBack.UseVisualStyleBackColor = false;
            folderBack.Click += folderBack_Click;
            // 
            // Gallery
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(5, 5, 5);
            ClientSize = new Size(1002, 686);
            Controls.Add(folderBack);
            Controls.Add(_refreshButton);
            Controls.Add(galleryPanel);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4);
            Name = "Gallery";
            Text = "Gallery";
            ResumeLayout(false);
        }

        #endregion
        private FlowLayoutPanel galleryPanel;
        private RoundedButton _refreshButton;
        private RoundedButton folderBack;
    }
}