using VRCEmojiManager.Design;

namespace VRCEmojiManager.Forms
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
            SuspendLayout();
            // 
            // emojiPanel
            // 
            emojiPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            emojiPanel.AutoScroll = true;
            emojiPanel.BackColor = Color.FromArgb(60, 60, 60);
            emojiPanel.Location = new Point(12, 45);
            emojiPanel.Name = "emojiPanel";
            emojiPanel.Size = new Size(595, 429);
            emojiPanel.TabIndex = 2;
            // 
            // _refreshButton
            // 
            _refreshButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _refreshButton.BackColor = Color.MediumSlateBlue;
            _refreshButton.BackgroundColor = Color.MediumSlateBlue;
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
            // Emoji
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(619, 486);
            Controls.Add(_refreshButton);
            Controls.Add(emojiPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Emoji";
            Text = "Emoji";
            ResumeLayout(false);
        }

        #endregion
        private FlowLayoutPanel emojiPanel;
        private RoundedButton _refreshButton;
    }
}