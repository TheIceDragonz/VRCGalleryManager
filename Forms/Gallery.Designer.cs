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
            BackPanel = new Panel();
            galleryInfoPanel = new Panel();
            userInfoPanel = new Panel();
            worldImage = new RoundedPictureBox();
            worldNameLabel = new Label();
            changeFolder = new RoundedButton();
            BackPanel.SuspendLayout();
            galleryInfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)worldImage).BeginInit();
            SuspendLayout();
            // 
            // galleryPanel
            // 
            galleryPanel.AutoScroll = true;
            galleryPanel.BackColor = Color.FromArgb(5, 5, 5);
            galleryPanel.Dock = DockStyle.Fill;
            galleryPanel.Location = new Point(0, 0);
            galleryPanel.Margin = new Padding(4);
            galleryPanel.Name = "galleryPanel";
            galleryPanel.Size = new Size(896, 792);
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
            _refreshButton.Location = new Point(1088, 12);
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
            folderBack.SvgResource = "back_arrow_svgrepo_com";
            folderBack.SvgSize = new Size(15, 15);
            folderBack.TabIndex = 4;
            folderBack.TextColor = Color.FromArgb(106, 227, 249);
            folderBack.UseVisualStyleBackColor = false;
            folderBack.Click += folderBack_Click;
            // 
            // BackPanel
            // 
            BackPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BackPanel.Controls.Add(galleryPanel);
            BackPanel.Controls.Add(galleryInfoPanel);
            BackPanel.Location = new Point(12, 54);
            BackPanel.Name = "BackPanel";
            BackPanel.Size = new Size(1221, 792);
            BackPanel.TabIndex = 5;
            // 
            // galleryInfoPanel
            // 
            galleryInfoPanel.Controls.Add(userInfoPanel);
            galleryInfoPanel.Controls.Add(worldImage);
            galleryInfoPanel.Controls.Add(worldNameLabel);
            galleryInfoPanel.Dock = DockStyle.Right;
            galleryInfoPanel.Location = new Point(896, 0);
            galleryInfoPanel.Name = "galleryInfoPanel";
            galleryInfoPanel.Padding = new Padding(10);
            galleryInfoPanel.Size = new Size(325, 792);
            galleryInfoPanel.TabIndex = 0;
            galleryInfoPanel.Visible = false;
            // 
            // userInfoPanel
            // 
            userInfoPanel.AutoScroll = true;
            userInfoPanel.Dock = DockStyle.Fill;
            userInfoPanel.Location = new Point(10, 273);
            userInfoPanel.Name = "userInfoPanel";
            userInfoPanel.Padding = new Padding(10);
            userInfoPanel.Size = new Size(305, 509);
            userInfoPanel.TabIndex = 3;
            // 
            // worldImage
            // 
            worldImage.BackColor = Color.FromArgb(7, 36, 43);
            worldImage.BackgroundColor = Color.FromArgb(7, 36, 43);
            worldImage.BorderColor = Color.PaleVioletRed;
            worldImage.BorderRadiusBottomLeft = 20;
            worldImage.BorderRadiusBottomRight = 20;
            worldImage.BorderRadiusTopLeft = 20;
            worldImage.BorderRadiusTopRight = 20;
            worldImage.BorderSize = 0;
            worldImage.Dock = DockStyle.Top;
            worldImage.Location = new Point(10, 60);
            worldImage.Name = "worldImage";
            worldImage.Size = new Size(305, 213);
            worldImage.SizeMode = PictureBoxSizeMode.StretchImage;
            worldImage.TabIndex = 1;
            worldImage.TabStop = false;
            worldImage.Click += worldImage_Click;
            // 
            // worldNameLabel
            // 
            worldNameLabel.Dock = DockStyle.Top;
            worldNameLabel.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            worldNameLabel.ForeColor = Color.White;
            worldNameLabel.Location = new Point(10, 10);
            worldNameLabel.Name = "worldNameLabel";
            worldNameLabel.Size = new Size(305, 50);
            worldNameLabel.TabIndex = 2;
            worldNameLabel.Text = "world name";
            worldNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // changeFolder
            // 
            changeFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            changeFolder.BackColor = Color.FromArgb(7, 36, 43);
            changeFolder.BackgroundColor = Color.FromArgb(7, 36, 43);
            changeFolder.BorderColor = Color.FromArgb(5, 55, 66);
            changeFolder.BorderRadius = 10;
            changeFolder.BorderSize = 2;
            changeFolder.FlatAppearance.BorderSize = 0;
            changeFolder.FlatStyle = FlatStyle.Flat;
            changeFolder.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            changeFolder.ForeColor = Color.FromArgb(106, 227, 249);
            changeFolder.Location = new Point(1045, 13);
            changeFolder.Margin = new Padding(4);
            changeFolder.Name = "changeFolder";
            changeFolder.Size = new Size(35, 35);
            changeFolder.SvgAlignment = ContentAlignment.MiddleCenter;
            changeFolder.SvgColor = Color.FromArgb(106, 227, 249);
            changeFolder.SvgContent = resources.GetString("changeFolder.SvgContent");
            changeFolder.SvgOffset = new Point(0, 0);
            changeFolder.SvgPadding = new Padding(0);
            changeFolder.SvgResource = "folder_svgrepo_com";
            changeFolder.SvgSize = new Size(15, 15);
            changeFolder.TabIndex = 6;
            changeFolder.TextColor = Color.FromArgb(106, 227, 249);
            changeFolder.UseVisualStyleBackColor = false;
            changeFolder.Click += changeFolder_Click;
            // 
            // Gallery
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(5, 5, 5);
            ClientSize = new Size(1248, 858);
            Controls.Add(changeFolder);
            Controls.Add(BackPanel);
            Controls.Add(folderBack);
            Controls.Add(_refreshButton);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4);
            Name = "Gallery";
            Text = "Gallery";
            BackPanel.ResumeLayout(false);
            galleryInfoPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)worldImage).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private FlowLayoutPanel galleryPanel;
        private RoundedButton _refreshButton;
        private RoundedButton folderBack;
        private Panel BackPanel;
        private Panel galleryInfoPanel;
        private RoundedPictureBox worldImage;
        private Label worldNameLabel;
        private Panel userInfoPanel;
        private RoundedButton changeFolder;
    }
}