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
            roundedButton1 = new RoundedButton();
            limitPanelGallery = new RoundedPanel();
            limitLabel = new Label();
            limitGalleryLabel = new Label();
            pasteButton = new RoundedButton();
            limitPanelGallery.SuspendLayout();
            SuspendLayout();
            // 
            // galleryPanel
            // 
            galleryPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            galleryPanel.AutoScroll = true;
            galleryPanel.BackColor = Color.FromArgb(5, 5, 5);
            galleryPanel.Location = new Point(12, 45);
            galleryPanel.Name = "galleryPanel";
            galleryPanel.Size = new Size(778, 446);
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
            _refreshButton.Location = new Point(674, 10);
            _refreshButton.Name = "_refreshButton";
            _refreshButton.Size = new Size(116, 29);
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
            // roundedButton1
            // 
            roundedButton1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            roundedButton1.BackColor = Color.FromArgb(7, 36, 43);
            roundedButton1.BackgroundColor = Color.FromArgb(7, 36, 43);
            roundedButton1.BorderColor = Color.FromArgb(5, 55, 66);
            roundedButton1.BorderRadius = 10;
            roundedButton1.BorderSize = 2;
            roundedButton1.FlatAppearance.BorderSize = 0;
            roundedButton1.FlatStyle = FlatStyle.Flat;
            roundedButton1.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            roundedButton1.ForeColor = Color.FromArgb(106, 227, 249);
            roundedButton1.Location = new Point(12, 497);
            roundedButton1.Name = "roundedButton1";
            roundedButton1.Size = new Size(732, 40);
            roundedButton1.SvgAlignment = ContentAlignment.MiddleCenter;
            roundedButton1.SvgColor = Color.Black;
            roundedButton1.SvgContent = null;
            roundedButton1.SvgOffset = new Point(0, 0);
            roundedButton1.SvgPadding = new Padding(0);
            roundedButton1.SvgResource = null;
            roundedButton1.SvgSize = new Size(50, 50);
            roundedButton1.TabIndex = 4;
            roundedButton1.Text = "Upload";
            roundedButton1.TextColor = Color.FromArgb(106, 227, 249);
            roundedButton1.UseVisualStyleBackColor = false;
            roundedButton1.Click += uploadGallery_Click;
            // 
            // limitPanelGallery
            // 
            limitPanelGallery.BackColor = Color.FromArgb(7, 36, 43);
            limitPanelGallery.BackgroundColor = Color.FromArgb(7, 36, 43);
            limitPanelGallery.BorderColor = Color.FromArgb(255, 128, 128);
            limitPanelGallery.BorderRadius = 10;
            limitPanelGallery.BorderSize = 2;
            limitPanelGallery.Controls.Add(limitLabel);
            limitPanelGallery.Location = new Point(12, 10);
            limitPanelGallery.Name = "limitPanelGallery";
            limitPanelGallery.Size = new Size(284, 29);
            limitPanelGallery.TabIndex = 5;
            limitPanelGallery.Visible = false;
            // 
            // limitLabel
            // 
            limitLabel.BackColor = Color.Transparent;
            limitLabel.Dock = DockStyle.Fill;
            limitLabel.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            limitLabel.ForeColor = Color.FromArgb(255, 128, 128);
            limitLabel.Location = new Point(0, 0);
            limitLabel.Name = "limitLabel";
            limitLabel.Size = new Size(284, 29);
            limitLabel.TabIndex = 0;
            limitLabel.Text = "You have reached your photos limit!";
            limitLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // limitGalleryLabel
            // 
            limitGalleryLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            limitGalleryLabel.AutoSize = true;
            limitGalleryLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            limitGalleryLabel.ForeColor = Color.White;
            limitGalleryLabel.Location = new Point(594, 17);
            limitGalleryLabel.Name = "limitGalleryLabel";
            limitGalleryLabel.RightToLeft = RightToLeft.No;
            limitGalleryLabel.Size = new Size(74, 15);
            limitGalleryLabel.TabIndex = 6;
            limitGalleryLabel.Text = "0/64 Photos";
            limitGalleryLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // pasteButton
            // 
            pasteButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            pasteButton.BackColor = Color.FromArgb(7, 36, 43);
            pasteButton.BackgroundColor = Color.FromArgb(7, 36, 43);
            pasteButton.BorderColor = Color.FromArgb(5, 55, 66);
            pasteButton.BorderRadius = 10;
            pasteButton.BorderSize = 2;
            pasteButton.FlatAppearance.BorderSize = 0;
            pasteButton.FlatStyle = FlatStyle.Flat;
            pasteButton.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            pasteButton.ForeColor = Color.FromArgb(106, 227, 249);
            pasteButton.Location = new Point(750, 497);
            pasteButton.Name = "pasteButton";
            pasteButton.Size = new Size(40, 40);
            pasteButton.SvgAlignment = ContentAlignment.MiddleCenter;
            pasteButton.SvgColor = Color.FromArgb(106, 227, 249);
            pasteButton.SvgContent = resources.GetString("pasteButton.SvgContent");
            pasteButton.SvgOffset = new Point(0, 0);
            pasteButton.SvgPadding = new Padding(0);
            pasteButton.SvgResource = "backward_svgrepo_com";
            pasteButton.SvgSize = new Size(25, 25);
            pasteButton.TabIndex = 8;
            pasteButton.TextColor = Color.FromArgb(106, 227, 249);
            pasteButton.UseVisualStyleBackColor = false;
            pasteButton.Click += pasteButton_Click;
            // 
            // Gallery
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(5, 5, 5);
            ClientSize = new Size(802, 549);
            Controls.Add(pasteButton);
            Controls.Add(limitGalleryLabel);
            Controls.Add(limitPanelGallery);
            Controls.Add(roundedButton1);
            Controls.Add(_refreshButton);
            Controls.Add(galleryPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Gallery";
            Text = "Gallery";
            limitPanelGallery.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private FlowLayoutPanel galleryPanel;
        private RoundedButton _refreshButton;
        private RoundedButton roundedButton1;
        private RoundedPanel limitPanelGallery;
        private Label limitLabel;
        private Label limitGalleryLabel;
        private RoundedButton pasteButton;
    }
}