using VRCGalleryManager.Design;

namespace VRCGalleryManager.Forms
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
            roundedButton1 = new RoundedButton();
            titleGifCreator = new Label();
            ((System.ComponentModel.ISupportInitialize)previewGif).BeginInit();
            ((System.ComponentModel.ISupportInitialize)previewSS).BeginInit();
            SuspendLayout();
            // 
            // previewGif
            // 
            previewGif.Anchor = AnchorStyles.None;
            previewGif.BackColor = Color.FromArgb(50, 50, 50);
            previewGif.BackgroundColor = Color.FromArgb(50, 50, 50);
            previewGif.BorderColor = Color.PaleVioletRed;
            previewGif.BorderRadiusBottomLeft = 20;
            previewGif.BorderRadiusBottomRight = 20;
            previewGif.BorderRadiusTopLeft = 20;
            previewGif.BorderRadiusTopRight = 20;
            previewGif.BorderSize = 0;
            previewGif.ImageLocation = "";
            previewGif.Location = new Point(66, 84);
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
            previewSS.Anchor = AnchorStyles.None;
            previewSS.BackColor = Color.FromArgb(50, 50, 50);
            previewSS.BackgroundColor = Color.FromArgb(50, 50, 50);
            previewSS.BorderColor = Color.PaleVioletRed;
            previewSS.BorderRadiusBottomLeft = 20;
            previewSS.BorderRadiusBottomRight = 20;
            previewSS.BorderRadiusTopLeft = 20;
            previewSS.BorderRadiusTopRight = 20;
            previewSS.BorderSize = 0;
            previewSS.Location = new Point(332, 84);
            previewSS.Name = "previewSS";
            previewSS.Size = new Size(260, 260);
            previewSS.SizeMode = PictureBoxSizeMode.StretchImage;
            previewSS.TabIndex = 1;
            previewSS.TabStop = false;
            // 
            // buttonSave
            // 
            buttonSave.Anchor = AnchorStyles.None;
            buttonSave.BackColor = Color.FromArgb(50, 50, 50);
            buttonSave.BackgroundColor = Color.FromArgb(50, 50, 50);
            buttonSave.BorderColor = Color.PaleVioletRed;
            buttonSave.BorderRadius = 15;
            buttonSave.BorderSize = 0;
            buttonSave.FlatAppearance.BorderSize = 0;
            buttonSave.FlatStyle = FlatStyle.Flat;
            buttonSave.ForeColor = Color.White;
            buttonSave.Location = new Point(66, 391);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(526, 35);
            buttonSave.TabIndex = 2;
            buttonSave.Text = "LocalSave";
            buttonSave.TextColor = Color.White;
            buttonSave.UseVisualStyleBackColor = false;
            buttonSave.Click += buttonSave_Click;
            // 
            // frameCounter
            // 
            frameCounter.Anchor = AnchorStyles.None;
            frameCounter.AutoSize = true;
            frameCounter.ForeColor = Color.White;
            frameCounter.Location = new Point(66, 429);
            frameCounter.Name = "frameCounter";
            frameCounter.Size = new Size(52, 15);
            frameCounter.TabIndex = 3;
            frameCounter.Text = "Frame: 0";
            // 
            // roundedButton1
            // 
            roundedButton1.Anchor = AnchorStyles.None;
            roundedButton1.BackColor = Color.FromArgb(50, 50, 50);
            roundedButton1.BackgroundColor = Color.FromArgb(50, 50, 50);
            roundedButton1.BorderColor = Color.PaleVioletRed;
            roundedButton1.BorderRadius = 15;
            roundedButton1.BorderSize = 0;
            roundedButton1.FlatAppearance.BorderSize = 0;
            roundedButton1.FlatStyle = FlatStyle.Flat;
            roundedButton1.ForeColor = Color.White;
            roundedButton1.Location = new Point(66, 350);
            roundedButton1.Name = "roundedButton1";
            roundedButton1.Size = new Size(526, 35);
            roundedButton1.TabIndex = 4;
            roundedButton1.Text = "UpLoad";
            roundedButton1.TextColor = Color.White;
            roundedButton1.UseVisualStyleBackColor = false;
            // 
            // titleGifCreator
            // 
            titleGifCreator.Anchor = AnchorStyles.None;
            titleGifCreator.AutoSize = true;
            titleGifCreator.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            titleGifCreator.ForeColor = Color.White;
            titleGifCreator.Location = new Point(255, 44);
            titleGifCreator.Name = "titleGifCreator";
            titleGifCreator.Size = new Size(158, 37);
            titleGifCreator.TabIndex = 5;
            titleGifCreator.Text = "Gif Creator";
            // 
            // Create
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(663, 506);
            Controls.Add(titleGifCreator);
            Controls.Add(roundedButton1);
            Controls.Add(previewGif);
            Controls.Add(previewSS);
            Controls.Add(frameCounter);
            Controls.Add(buttonSave);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Create";
            Text = "Create";
            ((System.ComponentModel.ISupportInitialize)previewGif).EndInit();
            ((System.ComponentModel.ISupportInitialize)previewSS).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RoundedPictureBox previewGif;
        private RoundedPictureBox previewSS;
        private Design.RoundedButton buttonSave;
        private Label frameCounter;
        private RoundedButton roundedButton1;
        private Label titleGifCreator;
    }
}