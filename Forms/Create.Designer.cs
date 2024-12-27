using VRCEmojiManager.Design;

namespace VRCEmojiManager.Forms
{
    partial class Create
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
            previewGif = new RoundedPictureBox();
            previewSS = new RoundedPictureBox();
            buttonSave = new RoundedButton();
            frameCounter = new Label();
            panel1 = new Panel();
            ((System.ComponentModel.ISupportInitialize)previewGif).BeginInit();
            ((System.ComponentModel.ISupportInitialize)previewSS).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // previewGif
            // 
            previewGif.BackColor = Color.FromArgb(50, 50, 50);
            previewGif.BackgroundColor = Color.FromArgb(50, 50, 50);
            previewGif.BorderColor = Color.PaleVioletRed;
            previewGif.BorderRadiusBottomLeft = 20;
            previewGif.BorderRadiusBottomRight = 20;
            previewGif.BorderRadiusTopLeft = 20;
            previewGif.BorderRadiusTopRight = 20;
            previewGif.BorderSize = 0;
            previewGif.Dock = DockStyle.Left;
            previewGif.ImageLocation = "";
            previewGif.Location = new Point(0, 0);
            previewGif.Name = "previewGif";
            previewGif.Size = new Size(260, 260);
            previewGif.SizeMode = PictureBoxSizeMode.StretchImage;
            previewGif.TabIndex = 0;
            previewGif.TabStop = false;
            previewGif.DragDrop += pictureSS_DragDrop;
            previewGif.DragEnter += pictureSS_DragEnter;
            // 
            // previewSS
            // 
            previewSS.BackColor = Color.FromArgb(50, 50, 50);
            previewSS.BackgroundColor = Color.FromArgb(50, 50, 50);
            previewSS.BorderColor = Color.PaleVioletRed;
            previewSS.BorderRadiusBottomLeft = 20;
            previewSS.BorderRadiusBottomRight = 20;
            previewSS.BorderRadiusTopLeft = 20;
            previewSS.BorderRadiusTopRight = 20;
            previewSS.BorderSize = 0;
            previewSS.Dock = DockStyle.Right;
            previewSS.Location = new Point(266, 0);
            previewSS.Name = "previewSS";
            previewSS.Size = new Size(260, 260);
            previewSS.SizeMode = PictureBoxSizeMode.StretchImage;
            previewSS.TabIndex = 1;
            previewSS.TabStop = false;
            // 
            // buttonSave
            // 
            buttonSave.BackColor = Color.FromArgb(50, 50, 50);
            buttonSave.BackgroundColor = Color.FromArgb(50, 50, 50);
            buttonSave.BorderColor = Color.PaleVioletRed;
            buttonSave.BorderRadius = 15;
            buttonSave.BorderSize = 0;
            buttonSave.Dock = DockStyle.Bottom;
            buttonSave.FlatAppearance.BorderSize = 0;
            buttonSave.FlatStyle = FlatStyle.Flat;
            buttonSave.ForeColor = Color.White;
            buttonSave.Location = new Point(0, 266);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(526, 35);
            buttonSave.TabIndex = 2;
            buttonSave.Text = "Save";
            buttonSave.TextColor = Color.White;
            buttonSave.UseVisualStyleBackColor = false;
            buttonSave.Click += buttonSave_Click;
            // 
            // frameCounter
            // 
            frameCounter.Anchor = AnchorStyles.None;
            frameCounter.AutoSize = true;
            frameCounter.ForeColor = Color.White;
            frameCounter.Location = new Point(65, 375);
            frameCounter.Name = "frameCounter";
            frameCounter.Size = new Size(52, 15);
            frameCounter.TabIndex = 3;
            frameCounter.Text = "Frame: 0";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.None;
            panel1.Controls.Add(previewGif);
            panel1.Controls.Add(previewSS);
            panel1.Controls.Add(buttonSave);
            panel1.Location = new Point(65, 71);
            panel1.Name = "panel1";
            panel1.Size = new Size(526, 301);
            panel1.TabIndex = 4;
            // 
            // Create
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(663, 506);
            Controls.Add(panel1);
            Controls.Add(frameCounter);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Create";
            Text = "Create";
            ((System.ComponentModel.ISupportInitialize)previewGif).EndInit();
            ((System.ComponentModel.ISupportInitialize)previewSS).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RoundedPictureBox previewGif;
        private RoundedPictureBox previewSS;
        private Design.RoundedButton buttonSave;
        private Label frameCounter;
        private Panel panel1;
    }
}