using CustomControls;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Create));
            previewGif = new RoundedPictureBox();
            previewSS = new RoundedPictureBox();
            buttonSave = new RoundedButton();
            uploadButton = new RoundedButton();
            titleGifCreator = new Label();
            trackBarFPS = new RoundedTrackBar();
            labelFPS = new Label();
            createTypePanel = new RoundedPanel();
            createOpenTypePanel = new RoundedButton();
            pasteButton = new RoundedButton();
            panel1 = new Panel();
            label3 = new Label();
            label2 = new Label();
            previewVRChat = new RoundedPictureBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)previewGif).BeginInit();
            ((System.ComponentModel.ISupportInitialize)previewSS).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarFPS).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)previewVRChat).BeginInit();
            SuspendLayout();
            // 
            // previewGif
            // 
            previewGif.Anchor = AnchorStyles.Top;
            previewGif.BackColor = Color.FromArgb(7, 36, 43);
            previewGif.BackgroundColor = Color.FromArgb(7, 36, 43);
            previewGif.BorderColor = Color.PaleVioletRed;
            previewGif.BorderRadiusBottomLeft = 10;
            previewGif.BorderRadiusBottomRight = 10;
            previewGif.BorderRadiusTopLeft = 10;
            previewGif.BorderRadiusTopRight = 10;
            previewGif.BorderSize = 0;
            previewGif.ImageLocation = "";
            previewGif.Location = new Point(167, 60);
            previewGif.Margin = new Padding(4, 4, 4, 4);
            previewGif.Name = "previewGif";
            previewGif.Size = new Size(325, 325);
            previewGif.SizeMode = PictureBoxSizeMode.StretchImage;
            previewGif.TabIndex = 0;
            previewGif.TabStop = false;
            // 
            // previewSS
            // 
            previewSS.Anchor = AnchorStyles.Top;
            previewSS.BackColor = Color.FromArgb(7, 36, 43);
            previewSS.BackgroundColor = Color.FromArgb(7, 36, 43);
            previewSS.BorderColor = Color.PaleVioletRed;
            previewSS.BorderRadiusBottomLeft = 10;
            previewSS.BorderRadiusBottomRight = 10;
            previewSS.BorderRadiusTopLeft = 10;
            previewSS.BorderRadiusTopRight = 10;
            previewSS.BorderSize = 0;
            previewSS.Location = new Point(500, 60);
            previewSS.Margin = new Padding(4, 4, 4, 4);
            previewSS.Name = "previewSS";
            previewSS.Size = new Size(325, 325);
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
            buttonSave.Location = new Point(719, 634);
            buttonSave.Margin = new Padding(4, 4, 4, 4);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(145, 50);
            buttonSave.SvgAlignment = ContentAlignment.MiddleCenter;
            buttonSave.SvgColor = Color.Black;
            buttonSave.SvgContent = null;
            buttonSave.SvgOffset = new Point(0, 0);
            buttonSave.SvgPadding = new Padding(0);
            buttonSave.SvgResource = null;
            buttonSave.SvgSize = new Size(50, 50);
            buttonSave.TabIndex = 2;
            buttonSave.Text = "Local Save";
            buttonSave.TextColor = Color.FromArgb(106, 227, 249);
            buttonSave.UseVisualStyleBackColor = false;
            buttonSave.Click += buttonLocalSave_Click;
            // 
            // uploadButton
            // 
            uploadButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            uploadButton.BackColor = Color.FromArgb(7, 36, 43);
            uploadButton.BackgroundColor = Color.FromArgb(7, 36, 43);
            uploadButton.BorderColor = Color.FromArgb(5, 55, 66);
            uploadButton.BorderRadius = 10;
            uploadButton.BorderSize = 2;
            uploadButton.FlatAppearance.BorderSize = 0;
            uploadButton.FlatStyle = FlatStyle.Flat;
            uploadButton.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            uploadButton.ForeColor = Color.FromArgb(106, 227, 249);
            uploadButton.Location = new Point(15, 634);
            uploadButton.Margin = new Padding(4, 4, 4, 4);
            uploadButton.Name = "uploadButton";
            uploadButton.Size = new Size(639, 50);
            uploadButton.SvgAlignment = ContentAlignment.MiddleCenter;
            uploadButton.SvgColor = Color.Black;
            uploadButton.SvgContent = null;
            uploadButton.SvgOffset = new Point(0, 0);
            uploadButton.SvgPadding = new Padding(0);
            uploadButton.SvgResource = null;
            uploadButton.SvgSize = new Size(50, 50);
            uploadButton.TabIndex = 4;
            uploadButton.Text = "Upload";
            uploadButton.TextColor = Color.FromArgb(106, 227, 249);
            uploadButton.UseVisualStyleBackColor = false;
            uploadButton.Click += creatorUpload_Click;
            // 
            // titleGifCreator
            // 
            titleGifCreator.Dock = DockStyle.Top;
            titleGifCreator.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            titleGifCreator.ForeColor = Color.White;
            titleGifCreator.Location = new Point(0, 0);
            titleGifCreator.Margin = new Padding(4, 0, 4, 0);
            titleGifCreator.Name = "titleGifCreator";
            titleGifCreator.Size = new Size(1032, 46);
            titleGifCreator.TabIndex = 5;
            titleGifCreator.Text = "Gif Creator";
            titleGifCreator.TextAlign = ContentAlignment.TopCenter;
            // 
            // trackBarFPS
            // 
            trackBarFPS.Anchor = AnchorStyles.Top;
            trackBarFPS.BackColor = Color.FromArgb(5, 5, 5);
            trackBarFPS.BorderColor = Color.FromArgb(5, 5, 5);
            trackBarFPS.BorderRadius = 5;
            trackBarFPS.LabelFont = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            trackBarFPS.LabelOffset = new Point(0, 0);
            trackBarFPS.LabelText = "0";
            trackBarFPS.LabelTextColor = Color.FromArgb(106, 227, 249);
            trackBarFPS.Location = new Point(167, 415);
            trackBarFPS.Margin = new Padding(4, 4, 4, 4);
            trackBarFPS.Maximum = 64;
            trackBarFPS.Minimum = 1;
            trackBarFPS.Name = "trackBarFPS";
            trackBarFPS.Size = new Size(658, 56);
            trackBarFPS.TabIndex = 9;
            trackBarFPS.ThumbColor = Color.FromArgb(7, 36, 43);
            trackBarFPS.ThumbSize = 24;
            trackBarFPS.TrackColor = Color.FromArgb(7, 36, 43);
            trackBarFPS.Value = 15;
            trackBarFPS.ValueChanged += trackBarFPS_ValueChanged;
            // 
            // labelFPS
            // 
            labelFPS.Anchor = AnchorStyles.Top;
            labelFPS.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelFPS.ForeColor = Color.White;
            labelFPS.Location = new Point(167, 404);
            labelFPS.Margin = new Padding(4, 0, 4, 0);
            labelFPS.Name = "labelFPS";
            labelFPS.Size = new Size(658, 19);
            labelFPS.TabIndex = 10;
            labelFPS.Text = "FPS";
            labelFPS.TextAlign = ContentAlignment.TopCenter;
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
            createTypePanel.Location = new Point(694, 15);
            createTypePanel.Margin = new Padding(4, 4, 4, 4);
            createTypePanel.Name = "createTypePanel";
            createTypePanel.Padding = new Padding(6, 6, 6, 6);
            createTypePanel.Size = new Size(322, 611);
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
            createOpenTypePanel.Location = new Point(872, 634);
            createOpenTypePanel.Margin = new Padding(4, 4, 4, 4);
            createOpenTypePanel.Name = "createOpenTypePanel";
            createOpenTypePanel.Size = new Size(145, 50);
            createOpenTypePanel.SvgAlignment = ContentAlignment.MiddleCenter;
            createOpenTypePanel.SvgColor = Color.Black;
            createOpenTypePanel.SvgContent = null;
            createOpenTypePanel.SvgOffset = new Point(0, 0);
            createOpenTypePanel.SvgPadding = new Padding(0);
            createOpenTypePanel.SvgResource = null;
            createOpenTypePanel.SvgSize = new Size(50, 50);
            createOpenTypePanel.TabIndex = 11;
            createOpenTypePanel.Text = "Type";
            createOpenTypePanel.TextColor = Color.FromArgb(106, 227, 249);
            createOpenTypePanel.UseVisualStyleBackColor = false;
            createOpenTypePanel.Click += createOpenTypePanel_Click;
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
            pasteButton.Location = new Point(662, 634);
            pasteButton.Margin = new Padding(4, 4, 4, 4);
            pasteButton.Name = "pasteButton";
            pasteButton.Size = new Size(50, 50);
            pasteButton.SvgAlignment = ContentAlignment.MiddleCenter;
            pasteButton.SvgColor = Color.FromArgb(106, 227, 249);
            pasteButton.SvgContent = resources.GetString("pasteButton.SvgContent");
            pasteButton.SvgOffset = new Point(0, 0);
            pasteButton.SvgPadding = new Padding(0);
            pasteButton.SvgResource = "backward_svgrepo_com";
            pasteButton.SvgSize = new Size(25, 25);
            pasteButton.TabIndex = 13;
            pasteButton.TextColor = Color.FromArgb(106, 227, 249);
            pasteButton.UseVisualStyleBackColor = false;
            pasteButton.Click += pasteButton_Click;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.AutoScroll = true;
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(previewVRChat);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(labelFPS);
            panel1.Controls.Add(previewSS);
            panel1.Controls.Add(previewGif);
            panel1.Controls.Add(trackBarFPS);
            panel1.Location = new Point(15, 50);
            panel1.Margin = new Padding(4, 4, 4, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(1002, 576);
            panel1.TabIndex = 14;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top;
            label3.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.White;
            label3.Location = new Point(500, 38);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(325, 19);
            label3.TabIndex = 14;
            label3.Text = "SpriteSheet Preview";
            label3.TextAlign = ContentAlignment.TopCenter;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.White;
            label2.Location = new Point(167, 38);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(325, 19);
            label2.TabIndex = 13;
            label2.Text = "Gif Preview";
            label2.TextAlign = ContentAlignment.TopCenter;
            // 
            // previewVRChat
            // 
            previewVRChat.Anchor = AnchorStyles.Top;
            previewVRChat.BackColor = Color.FromArgb(7, 36, 43);
            previewVRChat.BackgroundColor = Color.FromArgb(7, 36, 43);
            previewVRChat.BorderColor = Color.PaleVioletRed;
            previewVRChat.BorderRadiusBottomLeft = 10;
            previewVRChat.BorderRadiusBottomRight = 10;
            previewVRChat.BorderRadiusTopLeft = 10;
            previewVRChat.BorderRadiusTopRight = 10;
            previewVRChat.BorderSize = 0;
            previewVRChat.Location = new Point(334, 501);
            previewVRChat.Margin = new Padding(4, 4, 4, 4);
            previewVRChat.Name = "previewVRChat";
            previewVRChat.Size = new Size(325, 325);
            previewVRChat.SizeMode = PictureBoxSizeMode.StretchImage;
            previewVRChat.TabIndex = 11;
            previewVRChat.TabStop = false;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(334, 479);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(325, 19);
            label1.TabIndex = 12;
            label1.Text = "VRChat Preview";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // Create
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(5, 5, 5);
            ClientSize = new Size(1032, 699);
            Controls.Add(pasteButton);
            Controls.Add(createTypePanel);
            Controls.Add(createOpenTypePanel);
            Controls.Add(titleGifCreator);
            Controls.Add(uploadButton);
            Controls.Add(buttonSave);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 4, 4, 4);
            Name = "Create";
            Text = "Create";
            DragDrop += File_DragDrop;
            DragEnter += File_DragEnter;
            ((System.ComponentModel.ISupportInitialize)previewGif).EndInit();
            ((System.ComponentModel.ISupportInitialize)previewSS).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarFPS).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)previewVRChat).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private RoundedPictureBox previewGif;
        private RoundedPictureBox previewSS;
        private Design.RoundedButton buttonSave;
        private RoundedButton uploadButton;
        private Label titleGifCreator;
        private RoundedTrackBar trackBarFPS;
        private Label labelFPS;
        private RoundedPanel createTypePanel;
        private RoundedButton createOpenTypePanel;
        private RoundedButton pasteButton;
        private Panel panel1;
        private RoundedPictureBox previewVRChat;
        private Label label1;
        private Label label3;
        private Label label2;
    }
}