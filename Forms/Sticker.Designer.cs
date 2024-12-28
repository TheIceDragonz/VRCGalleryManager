using VRCGalleryManager.Design;

namespace VRCGalleryManager.Forms
{
    partial class Sticker
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
            stickerPanel = new FlowLayoutPanel();
            _refreshButton = new RoundedButton();
            roundedButton1 = new RoundedButton();
            SuspendLayout();
            // 
            // stickerPanel
            // 
            stickerPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            stickerPanel.AutoScroll = true;
            stickerPanel.BackColor = Color.FromArgb(64, 64, 64);
            stickerPanel.Location = new Point(12, 45);
            stickerPanel.Name = "stickerPanel";
            stickerPanel.Size = new Size(595, 383);
            stickerPanel.TabIndex = 2;
            // 
            // _refreshButton
            // 
            _refreshButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _refreshButton.BackColor = Color.FromArgb(50, 50, 50);
            _refreshButton.BackgroundColor = Color.FromArgb(50, 50, 50);
            _refreshButton.BorderColor = Color.PaleVioletRed;
            _refreshButton.BorderRadius = 15;
            _refreshButton.BorderSize = 0;
            _refreshButton.FlatAppearance.BorderSize = 0;
            _refreshButton.FlatStyle = FlatStyle.Flat;
            _refreshButton.ForeColor = Color.White;
            _refreshButton.Location = new Point(491, 12);
            _refreshButton.Name = "_refreshButton";
            _refreshButton.Size = new Size(116, 27);
            _refreshButton.TabIndex = 3;
            _refreshButton.Text = "Refresh";
            _refreshButton.TextColor = Color.White;
            _refreshButton.UseVisualStyleBackColor = false;
            _refreshButton.Click += _refreshButton_Click;
            // 
            // roundedButton1
            // 
            roundedButton1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            roundedButton1.BackColor = Color.FromArgb(50, 50, 50);
            roundedButton1.BackgroundColor = Color.FromArgb(50, 50, 50);
            roundedButton1.BorderColor = Color.PaleVioletRed;
            roundedButton1.BorderRadius = 20;
            roundedButton1.BorderSize = 0;
            roundedButton1.FlatAppearance.BorderSize = 0;
            roundedButton1.FlatStyle = FlatStyle.Flat;
            roundedButton1.ForeColor = Color.White;
            roundedButton1.Location = new Point(12, 434);
            roundedButton1.Name = "roundedButton1";
            roundedButton1.Size = new Size(595, 40);
            roundedButton1.TabIndex = 4;
            roundedButton1.Text = "Upload";
            roundedButton1.TextColor = Color.White;
            roundedButton1.UseVisualStyleBackColor = false;
            roundedButton1.Click += uploadSticker_Click;
            // 
            // Sticker
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(619, 486);
            Controls.Add(roundedButton1);
            Controls.Add(_refreshButton);
            Controls.Add(stickerPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Sticker";
            Text = "Sticker";
            ResumeLayout(false);
        }

        #endregion
        private FlowLayoutPanel stickerPanel;
        private RoundedButton _refreshButton;
        private RoundedButton roundedButton1;
    }
}