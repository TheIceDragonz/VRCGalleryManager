using VRCGalleryManager.Design;

namespace VRCGalleryManager.Forms
{
    partial class Icons
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
            iconsPanel = new FlowLayoutPanel();
            _refreshButton = new RoundedButton();
            roundedButton1 = new RoundedButton();
            limitPanelIcons = new RoundedPanel();
            limitLabel = new Label();
            limitIconsLabel = new Label();
            limitPanelIcons.SuspendLayout();
            SuspendLayout();
            // 
            // iconsPanel
            // 
            iconsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            iconsPanel.AutoScroll = true;
            iconsPanel.BackColor = Color.FromArgb(5, 5, 5);
            iconsPanel.Location = new Point(12, 45);
            iconsPanel.Name = "iconsPanel";
            iconsPanel.Size = new Size(778, 446);
            iconsPanel.TabIndex = 2;
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
            _refreshButton.Location = new Point(674, 12);
            _refreshButton.Name = "_refreshButton";
            _refreshButton.Size = new Size(116, 27);
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
            roundedButton1.Size = new Size(778, 40);
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
            roundedButton1.Click += uploadIcons_Click;
            // 
            // limitPanelIcons
            // 
            limitPanelIcons.BackColor = Color.FromArgb(7, 36, 43);
            limitPanelIcons.BackgroundColor = Color.FromArgb(7, 36, 43);
            limitPanelIcons.BorderColor = Color.FromArgb(255, 128, 128);
            limitPanelIcons.BorderRadius = 10;
            limitPanelIcons.BorderSize = 2;
            limitPanelIcons.Controls.Add(limitLabel);
            limitPanelIcons.Location = new Point(12, 10);
            limitPanelIcons.Name = "limitPanelIcons";
            limitPanelIcons.Size = new Size(284, 29);
            limitPanelIcons.TabIndex = 5;
            limitPanelIcons.Visible = false;
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
            limitLabel.Text = "You have reached your icons limit!";
            limitLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // limitIconsLabel
            // 
            limitIconsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            limitIconsLabel.AutoSize = true;
            limitIconsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            limitIconsLabel.ForeColor = Color.White;
            limitIconsLabel.Location = new Point(603, 18);
            limitIconsLabel.Name = "limitIconsLabel";
            limitIconsLabel.Size = new Size(65, 15);
            limitIconsLabel.TabIndex = 6;
            limitIconsLabel.Text = "0/64 Icons";
            limitIconsLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Icons
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(5, 5, 5);
            ClientSize = new Size(802, 549);
            Controls.Add(limitIconsLabel);
            Controls.Add(limitPanelIcons);
            Controls.Add(roundedButton1);
            Controls.Add(_refreshButton);
            Controls.Add(iconsPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Icons";
            Text = "Icons";
            limitPanelIcons.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private FlowLayoutPanel iconsPanel;
        private RoundedButton _refreshButton;
        private RoundedButton roundedButton1;
        private RoundedPanel limitPanelIcons;
        private Label limitLabel;
        private Label limitIconsLabel;
    }
}