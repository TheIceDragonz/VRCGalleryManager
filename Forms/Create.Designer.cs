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
            roundedButton1 = new RoundedButton();
            titleGifCreator = new Label();
            urlToSpriteSheetText = new TextBox();
            label1 = new Label();
            trackBarFPS = new TrackBar();
            labelFPS = new Label();
            createTypePanel = new RoundedPanel();
            createOpenTypePanel = new RoundedButton();
            ((System.ComponentModel.ISupportInitialize)previewGif).BeginInit();
            ((System.ComponentModel.ISupportInitialize)previewSS).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarFPS).BeginInit();
            SuspendLayout();
            // 
            // previewGif
            // 
            previewGif.Anchor = AnchorStyles.None;
            previewGif.BackColor = Color.FromArgb(7, 36, 43);
            previewGif.BackgroundColor = Color.FromArgb(7, 36, 43);
            previewGif.BorderColor = Color.PaleVioletRed;
            previewGif.BorderRadiusBottomLeft = 10;
            previewGif.BorderRadiusBottomRight = 10;
            previewGif.BorderRadiusTopLeft = 10;
            previewGif.BorderRadiusTopRight = 10;
            previewGif.BorderSize = 0;
            previewGif.ImageLocation = "";
            previewGif.Location = new Point(104, 63);
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
            previewSS.BackColor = Color.FromArgb(7, 36, 43);
            previewSS.BackgroundColor = Color.FromArgb(7, 36, 43);
            previewSS.BorderColor = Color.PaleVioletRed;
            previewSS.BorderRadiusBottomLeft = 10;
            previewSS.BorderRadiusBottomRight = 10;
            previewSS.BorderRadiusTopLeft = 10;
            previewSS.BorderRadiusTopRight = 10;
            previewSS.BorderSize = 0;
            previewSS.Location = new Point(370, 63);
            previewSS.Name = "previewSS";
            previewSS.Size = new Size(260, 260);
            previewSS.SizeMode = PictureBoxSizeMode.StretchImage;
            previewSS.TabIndex = 1;
            previewSS.TabStop = false;
            // 
            // buttonSave
            // 
            buttonSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonSave.BackColor = Color.FromArgb(7, 36, 43);
            buttonSave.BackgroundColor = Color.FromArgb(7, 36, 43);
            buttonSave.BorderColor = Color.FromArgb(5, 55, 66);
            buttonSave.BorderRadius = 10;
            buttonSave.BorderSize = 2;
            buttonSave.FlatAppearance.BorderSize = 0;
            buttonSave.FlatStyle = FlatStyle.Flat;
            buttonSave.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            buttonSave.ForeColor = Color.FromArgb(106, 227, 249);
            buttonSave.Location = new Point(485, 509);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(116, 40);
            buttonSave.TabIndex = 2;
            buttonSave.Text = "Local Save";
            buttonSave.TextColor = Color.FromArgb(106, 227, 249);
            buttonSave.UseVisualStyleBackColor = false;
            buttonSave.Click += buttonLocalSave_Click;
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
            roundedButton1.Location = new Point(12, 509);
            roundedButton1.Name = "roundedButton1";
            roundedButton1.Size = new Size(467, 40);
            roundedButton1.TabIndex = 4;
            roundedButton1.Text = "Upload";
            roundedButton1.TextColor = Color.FromArgb(106, 227, 249);
            roundedButton1.UseVisualStyleBackColor = false;
            roundedButton1.Click += creatorUpload_Click;
            // 
            // titleGifCreator
            // 
            titleGifCreator.Dock = DockStyle.Top;
            titleGifCreator.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            titleGifCreator.ForeColor = Color.White;
            titleGifCreator.Location = new Point(0, 0);
            titleGifCreator.Name = "titleGifCreator";
            titleGifCreator.Size = new Size(735, 37);
            titleGifCreator.TabIndex = 5;
            titleGifCreator.Text = "Gif Creator";
            titleGifCreator.TextAlign = ContentAlignment.TopCenter;
            // 
            // urlToSpriteSheetText
            // 
            urlToSpriteSheetText.Anchor = AnchorStyles.None;
            urlToSpriteSheetText.BackColor = Color.FromArgb(7, 36, 43);
            urlToSpriteSheetText.BorderStyle = BorderStyle.None;
            urlToSpriteSheetText.ForeColor = Color.White;
            urlToSpriteSheetText.Location = new Point(104, 364);
            urlToSpriteSheetText.Name = "urlToSpriteSheetText";
            urlToSpriteSheetText.Size = new Size(526, 16);
            urlToSpriteSheetText.TabIndex = 6;
            urlToSpriteSheetText.TextChanged += urlToSpriteSheet;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.None;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(104, 346);
            label1.Name = "label1";
            label1.Size = new Size(124, 15);
            label1.TabIndex = 7;
            label1.Text = "Import Link (\".gif\")";
            // 
            // trackBarFPS
            // 
            trackBarFPS.Anchor = AnchorStyles.None;
            trackBarFPS.BackColor = Color.FromArgb(7, 36, 43);
            trackBarFPS.Location = new Point(104, 420);
            trackBarFPS.Maximum = 64;
            trackBarFPS.Minimum = 1;
            trackBarFPS.Name = "trackBarFPS";
            trackBarFPS.Size = new Size(526, 45);
            trackBarFPS.TabIndex = 9;
            trackBarFPS.Value = 15;
            trackBarFPS.ValueChanged += trackBarFPS_ValueChanged;
            // 
            // labelFPS
            // 
            labelFPS.Anchor = AnchorStyles.None;
            labelFPS.AutoSize = true;
            labelFPS.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelFPS.ForeColor = Color.White;
            labelFPS.Location = new Point(104, 402);
            labelFPS.Name = "labelFPS";
            labelFPS.Size = new Size(28, 15);
            labelFPS.TabIndex = 10;
            labelFPS.Text = "FPS";
            // 
            // createTypePanel
            // 
            createTypePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            createTypePanel.AutoScroll = true;
            createTypePanel.BackColor = Color.FromArgb(7, 36, 43);
            createTypePanel.BackgroundColor = Color.FromArgb(7, 36, 43);
            createTypePanel.BorderColor = Color.PaleVioletRed;
            createTypePanel.BorderRadius = 9;
            createTypePanel.BorderSize = 0;
            createTypePanel.Location = new Point(465, 12);
            createTypePanel.Name = "createTypePanel";
            createTypePanel.Padding = new Padding(5);
            createTypePanel.Size = new Size(258, 491);
            createTypePanel.TabIndex = 12;
            createTypePanel.Visible = false;
            // 
            // createOpenTypePanel
            // 
            createOpenTypePanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            createOpenTypePanel.BackColor = Color.FromArgb(7, 36, 43);
            createOpenTypePanel.BackgroundColor = Color.FromArgb(7, 36, 43);
            createOpenTypePanel.BorderColor = Color.FromArgb(5, 55, 66);
            createOpenTypePanel.BorderRadius = 10;
            createOpenTypePanel.BorderSize = 2;
            createOpenTypePanel.FlatAppearance.BorderSize = 0;
            createOpenTypePanel.FlatStyle = FlatStyle.Flat;
            createOpenTypePanel.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            createOpenTypePanel.ForeColor = Color.FromArgb(106, 227, 249);
            createOpenTypePanel.Location = new Point(607, 509);
            createOpenTypePanel.Name = "createOpenTypePanel";
            createOpenTypePanel.Size = new Size(116, 40);
            createOpenTypePanel.TabIndex = 11;
            createOpenTypePanel.Text = "Type";
            createOpenTypePanel.TextColor = Color.FromArgb(106, 227, 249);
            createOpenTypePanel.UseVisualStyleBackColor = false;
            createOpenTypePanel.Click += createOpenTypePanel_Click;
            // 
            // Create
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(5, 5, 5);
            ClientSize = new Size(735, 561);
            Controls.Add(createTypePanel);
            Controls.Add(createOpenTypePanel);
            Controls.Add(labelFPS);
            Controls.Add(trackBarFPS);
            Controls.Add(label1);
            Controls.Add(urlToSpriteSheetText);
            Controls.Add(titleGifCreator);
            Controls.Add(roundedButton1);
            Controls.Add(previewGif);
            Controls.Add(previewSS);
            Controls.Add(buttonSave);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Create";
            Text = "Create";
            ((System.ComponentModel.ISupportInitialize)previewGif).EndInit();
            ((System.ComponentModel.ISupportInitialize)previewSS).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarFPS).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RoundedPictureBox previewGif;
        private RoundedPictureBox previewSS;
        private Design.RoundedButton buttonSave;
        private RoundedButton roundedButton1;
        private Label titleGifCreator;
        private TextBox urlToSpriteSheetText;
        private Label label1;
        private TrackBar trackBarFPS;
        private Label labelFPS;
        private RoundedPanel createTypePanel;
        private RoundedButton createOpenTypePanel;
    }
}