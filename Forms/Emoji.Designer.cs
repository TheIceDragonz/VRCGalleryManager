using VRCGalleryManager.Design;

namespace VRCGalleryManager.Forms
{
    partial class Emoji
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
            emojiPanel = new FlowLayoutPanel();
            _refreshButton = new RoundedButton();
            roundedButton1 = new RoundedButton();
            emojiOpenTypePanel = new RoundedButton();
            emojiTypePanel = new RoundedPanel();
            limitPanelEmoji = new RoundedPanel();
            limitLabel = new Label();
            limitStickerLabel = new Label();
            limitPanelEmoji.SuspendLayout();
            SuspendLayout();
            // 
            // emojiPanel
            // 
            emojiPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            emojiPanel.AutoScroll = true;
            emojiPanel.BackColor = Color.FromArgb(5, 5, 5);
            emojiPanel.Location = new Point(12, 45);
            emojiPanel.Name = "emojiPanel";
            emojiPanel.Size = new Size(761, 484);
            emojiPanel.TabIndex = 2;
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
            _refreshButton.Location = new Point(657, 10);
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
            roundedButton1.Location = new Point(12, 535);
            roundedButton1.Name = "roundedButton1";
            roundedButton1.Size = new Size(639, 40);
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
            roundedButton1.Click += uploadEmoji_Click;
            // 
            // emojiOpenTypePanel
            // 
            emojiOpenTypePanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            emojiOpenTypePanel.BackColor = Color.FromArgb(7, 36, 43);
            emojiOpenTypePanel.BackgroundColor = Color.FromArgb(7, 36, 43);
            emojiOpenTypePanel.BorderColor = Color.FromArgb(5, 55, 66);
            emojiOpenTypePanel.BorderRadius = 10;
            emojiOpenTypePanel.BorderSize = 2;
            emojiOpenTypePanel.FlatAppearance.BorderSize = 0;
            emojiOpenTypePanel.FlatStyle = FlatStyle.Flat;
            emojiOpenTypePanel.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            emojiOpenTypePanel.ForeColor = Color.FromArgb(106, 227, 249);
            emojiOpenTypePanel.Location = new Point(657, 535);
            emojiOpenTypePanel.Name = "emojiOpenTypePanel";
            emojiOpenTypePanel.Size = new Size(116, 40);
            emojiOpenTypePanel.SvgAlignment = ContentAlignment.MiddleCenter;
            emojiOpenTypePanel.SvgColor = Color.Black;
            emojiOpenTypePanel.SvgContent = null;
            emojiOpenTypePanel.SvgOffset = new Point(0, 0);
            emojiOpenTypePanel.SvgPadding = new Padding(0);
            emojiOpenTypePanel.SvgResource = null;
            emojiOpenTypePanel.SvgSize = new Size(50, 50);
            emojiOpenTypePanel.TabIndex = 7;
            emojiOpenTypePanel.Text = "Type";
            emojiOpenTypePanel.TextColor = Color.FromArgb(106, 227, 249);
            emojiOpenTypePanel.UseVisualStyleBackColor = false;
            emojiOpenTypePanel.Click += emojiOpenTypePanel_Click;
            // 
            // emojiTypePanel
            // 
            emojiTypePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            emojiTypePanel.AutoScroll = true;
            emojiTypePanel.BackColor = Color.FromArgb(7, 36, 43);
            emojiTypePanel.BackgroundColor = Color.FromArgb(7, 36, 43);
            emojiTypePanel.BorderColor = Color.PaleVioletRed;
            emojiTypePanel.BorderRadius = 9;
            emojiTypePanel.BorderSize = 0;
            emojiTypePanel.Location = new Point(515, 45);
            emojiTypePanel.Name = "emojiTypePanel";
            emojiTypePanel.Padding = new Padding(5);
            emojiTypePanel.Size = new Size(258, 484);
            emojiTypePanel.TabIndex = 8;
            emojiTypePanel.Visible = false;
            // 
            // limitPanelEmoji
            // 
            limitPanelEmoji.BackColor = Color.FromArgb(7, 36, 43);
            limitPanelEmoji.BackgroundColor = Color.FromArgb(7, 36, 43);
            limitPanelEmoji.BorderColor = Color.FromArgb(255, 128, 128);
            limitPanelEmoji.BorderRadius = 10;
            limitPanelEmoji.BorderSize = 2;
            limitPanelEmoji.Controls.Add(limitLabel);
            limitPanelEmoji.Location = new Point(12, 10);
            limitPanelEmoji.Name = "limitPanelEmoji";
            limitPanelEmoji.Size = new Size(284, 29);
            limitPanelEmoji.TabIndex = 9;
            limitPanelEmoji.Visible = false;
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
            limitLabel.Text = "You have reached your emoji limit!";
            limitLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // limitStickerLabel
            // 
            limitStickerLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            limitStickerLabel.AutoSize = true;
            limitStickerLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            limitStickerLabel.ForeColor = Color.White;
            limitStickerLabel.Location = new Point(592, 17);
            limitStickerLabel.Name = "limitStickerLabel";
            limitStickerLabel.Size = new Size(59, 15);
            limitStickerLabel.TabIndex = 7;
            limitStickerLabel.Text = "0/9 Emoji";
            limitStickerLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Emoji
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(5, 5, 5);
            ClientSize = new Size(785, 587);
            Controls.Add(limitStickerLabel);
            Controls.Add(limitPanelEmoji);
            Controls.Add(emojiTypePanel);
            Controls.Add(emojiOpenTypePanel);
            Controls.Add(roundedButton1);
            Controls.Add(_refreshButton);
            Controls.Add(emojiPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Emoji";
            Text = "Emoji";
            limitPanelEmoji.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private RoundedButton _refreshButton;
        private RoundedButton roundedButton1;
        private RoundedButton emojiOpenTypePanel;
        private RoundedPanel emojiTypePanel;
        private RoundedPanel limitPanelEmoji;
        private Label limitLabel;
        private Label limitStickerLabel;
        public FlowLayoutPanel emojiPanel;
    }
}