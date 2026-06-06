using CustomControls;
using VRCGalleryManager.Design;

namespace VRCGalleryManager.Forms
{
    partial class EmojiEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmojiEditorForm));
            mainTableLayout = new TableLayoutPanel();
            workspacePanel = new Panel();
            previewPanel = new DoubleBufferedPanel();
            emojiTypePanel = new Panel();
            previewVRChat = new RoundedPictureBox();
            flowPanelFrames = new ModernFlowPanel();
            settingsPanel = new RoundedPanel();
            containerImageAndGif = new Panel();
            panelStaticControls = new Panel();
            lblAdaptation = new Label();
            btnAdaptFit = new RoundedButton();
            btnAdaptFill = new RoundedButton();
            btnAdaptStretch = new RoundedButton();
            btnAdaptCenter = new RoundedButton();
            lblZoom = new Label();
            sliderZoom = new RoundedTrackBar();
            lblZoomVal = new Label();
            lblBgColor = new Label();
            panelBgColorColor = new RoundedPanel();
            btnBgColorTransparent = new RoundedButton();
            chkRemoveBg = new RoundedCheckBox();
            lblRemoveBgColor = new Label();
            panelRemoveBgColorColor = new RoundedPanel();
            btnPickColor = new RoundedButton();
            lblTolerance = new Label();
            sliderTolerance = new RoundedTrackBar();
            lblToleranceVal = new Label();
            chkFeather = new RoundedCheckBox();
            lblChoke = new Label();
            sliderChoke = new RoundedTrackBar();
            lblChokeVal = new Label();
            lblFeather = new Label();
            sliderFeather = new RoundedTrackBar();
            lblFeatherVal = new Label();
            btnReset = new RoundedButton();
            btnRotate = new RoundedButton();
            panelGifControls = new Panel();
            lblStartFrame = new Label();
            trackBarStartFrame = new RoundedTrackBar();
            lblEndFrame = new Label();
            trackBarEndFrame = new RoundedTrackBar();
            labelFPS = new Label();
            trackBarFPS = new RoundedTrackBar();
            lblTitle = new Label();
            btnEmojiStyle = new RoundedButton();
            btnSave = new RoundedButton();
            btnCancel = new RoundedButton();
            mainTableLayout.SuspendLayout();
            workspacePanel.SuspendLayout();
            previewPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)previewVRChat).BeginInit();
            settingsPanel.SuspendLayout();
            containerImageAndGif.SuspendLayout();
            panelStaticControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)sliderZoom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sliderTolerance).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sliderChoke).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sliderFeather).BeginInit();
            panelGifControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarStartFrame).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarEndFrame).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarFPS).BeginInit();
            SuspendLayout();
            // 
            // mainTableLayout
            // 
            mainTableLayout.ColumnCount = 2;
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400F));
            mainTableLayout.Controls.Add(workspacePanel, 0, 0);
            mainTableLayout.Controls.Add(settingsPanel, 1, 0);
            mainTableLayout.Dock = DockStyle.Fill;
            mainTableLayout.Location = new Point(0, 0);
            mainTableLayout.Margin = new Padding(4);
            mainTableLayout.Name = "mainTableLayout";
            mainTableLayout.RowCount = 1;
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTableLayout.Size = new Size(1566, 893);
            mainTableLayout.TabIndex = 0;
            // 
            // workspacePanel
            // 
            workspacePanel.BackColor = Color.FromArgb(5, 5, 5);
            workspacePanel.Controls.Add(previewPanel);
            workspacePanel.Controls.Add(flowPanelFrames);
            workspacePanel.Dock = DockStyle.Fill;
            workspacePanel.Location = new Point(12, 12);
            workspacePanel.Margin = new Padding(12);
            workspacePanel.Name = "workspacePanel";
            workspacePanel.Size = new Size(1142, 869);
            workspacePanel.TabIndex = 0;
            // 
            // previewPanel
            // 
            previewPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            previewPanel.BackColor = Color.FromArgb(5, 5, 5);
            previewPanel.Controls.Add(emojiTypePanel);
            previewPanel.Controls.Add(previewVRChat);
            previewPanel.Location = new Point(0, 0);
            previewPanel.Name = "previewPanel";
            previewPanel.Size = new Size(1142, 762);
            previewPanel.TabIndex = 0;
            previewPanel.Paint += previewPanel_Paint;
            previewPanel.MouseDown += previewPanel_MouseDown;
            previewPanel.MouseMove += previewPanel_MouseMove;
            previewPanel.MouseUp += previewPanel_MouseUp;
            // 
            // emojiTypePanel
            // 
            emojiTypePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            emojiTypePanel.AutoScroll = true;
            emojiTypePanel.BackColor = Color.FromArgb(7, 36, 43);
            emojiTypePanel.Location = new Point(849, 3);
            emojiTypePanel.Name = "emojiTypePanel";
            emojiTypePanel.Padding = new Padding(6, 6, 0, 0);
            emojiTypePanel.Size = new Size(290, 756);
            emojiTypePanel.TabIndex = 2;
            emojiTypePanel.Visible = false;
            // 
            // previewVRChat
            // 
            previewVRChat.Anchor = AnchorStyles.None;
            previewVRChat.BackColor = Color.FromArgb(7, 36, 43);
            previewVRChat.BackgroundColor = Color.FromArgb(7, 36, 43);
            previewVRChat.BorderColor = Color.FromArgb(5, 55, 66);
            previewVRChat.BorderRadiusBottomLeft = 0;
            previewVRChat.BorderRadiusBottomRight = 0;
            previewVRChat.BorderRadiusTopLeft = 0;
            previewVRChat.BorderRadiusTopRight = 0;
            previewVRChat.BorderSize = 2;
            previewVRChat.Location = new Point(3, 3);
            previewVRChat.Name = "previewVRChat";
            previewVRChat.Size = new Size(350, 350);
            previewVRChat.SizeMode = PictureBoxSizeMode.StretchImage;
            previewVRChat.TabIndex = 0;
            previewVRChat.TabStop = false;
            // 
            // flowPanelFrames
            // 
            flowPanelFrames.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowPanelFrames.AutoScroll = true;
            flowPanelFrames.BackColor = Color.FromArgb(10, 25, 30);
            flowPanelFrames.Location = new Point(3, 768);
            flowPanelFrames.Name = "flowPanelFrames";
            flowPanelFrames.Size = new Size(1136, 98);
            flowPanelFrames.TabIndex = 8;
            flowPanelFrames.UseCustomScrollBar = true;
            flowPanelFrames.WrapContents = false;
            // 
            // settingsPanel
            // 
            settingsPanel.BackColor = Color.FromArgb(10, 25, 30);
            settingsPanel.BackgroundColor = Color.FromArgb(10, 25, 30);
            settingsPanel.BorderColor = Color.FromArgb(5, 55, 66);
            settingsPanel.BorderRadius = 15;
            settingsPanel.BorderSize = 2;
            settingsPanel.Controls.Add(containerImageAndGif);
            settingsPanel.Controls.Add(lblTitle);
            settingsPanel.Controls.Add(btnEmojiStyle);
            settingsPanel.Controls.Add(btnSave);
            settingsPanel.Controls.Add(btnCancel);
            settingsPanel.Dock = DockStyle.Fill;
            settingsPanel.Location = new Point(1178, 12);
            settingsPanel.Margin = new Padding(12);
            settingsPanel.Name = "settingsPanel";
            settingsPanel.Size = new Size(376, 869);
            settingsPanel.TabIndex = 1;
            // 
            // containerImageAndGif
            // 
            containerImageAndGif.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            containerImageAndGif.Controls.Add(panelStaticControls);
            containerImageAndGif.Controls.Add(panelGifControls);
            containerImageAndGif.Location = new Point(3, 113);
            containerImageAndGif.Name = "containerImageAndGif";
            containerImageAndGif.Size = new Size(370, 600);
            containerImageAndGif.TabIndex = 27;
            // 
            // panelStaticControls
            // 
            panelStaticControls.Controls.Add(lblAdaptation);
            panelStaticControls.Controls.Add(btnAdaptFit);
            panelStaticControls.Controls.Add(btnAdaptFill);
            panelStaticControls.Controls.Add(btnAdaptStretch);
            panelStaticControls.Controls.Add(btnAdaptCenter);
            panelStaticControls.Controls.Add(lblZoom);
            panelStaticControls.Controls.Add(sliderZoom);
            panelStaticControls.Controls.Add(lblZoomVal);
            panelStaticControls.Controls.Add(lblBgColor);
            panelStaticControls.Controls.Add(panelBgColorColor);
            panelStaticControls.Controls.Add(btnBgColorTransparent);
            panelStaticControls.Controls.Add(chkRemoveBg);
            panelStaticControls.Controls.Add(lblRemoveBgColor);
            panelStaticControls.Controls.Add(panelRemoveBgColorColor);
            panelStaticControls.Controls.Add(btnPickColor);
            panelStaticControls.Controls.Add(lblTolerance);
            panelStaticControls.Controls.Add(sliderTolerance);
            panelStaticControls.Controls.Add(lblToleranceVal);
            panelStaticControls.Controls.Add(chkFeather);
            panelStaticControls.Controls.Add(lblChoke);
            panelStaticControls.Controls.Add(sliderChoke);
            panelStaticControls.Controls.Add(lblChokeVal);
            panelStaticControls.Controls.Add(lblFeather);
            panelStaticControls.Controls.Add(sliderFeather);
            panelStaticControls.Controls.Add(lblFeatherVal);
            panelStaticControls.Controls.Add(btnReset);
            panelStaticControls.Controls.Add(btnRotate);
            panelStaticControls.Location = new Point(0, 0);
            panelStaticControls.Name = "panelStaticControls";
            panelStaticControls.Size = new Size(370, 290);
            panelStaticControls.TabIndex = 3;
            // 
            // lblAdaptation
            // 
            lblAdaptation.AutoSize = true;
            lblAdaptation.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblAdaptation.ForeColor = Color.White;
            lblAdaptation.Location = new Point(16, 0);
            lblAdaptation.Name = "lblAdaptation";
            lblAdaptation.Size = new Size(96, 21);
            lblAdaptation.TabIndex = 0;
            lblAdaptation.Text = "Adaptation";
            // 
            // btnAdaptFit
            // 
            btnAdaptFit.BackColor = Color.FromArgb(10, 25, 30);
            btnAdaptFit.BackgroundColor = Color.FromArgb(10, 25, 30);
            btnAdaptFit.BorderColor = Color.FromArgb(20, 40, 45);
            btnAdaptFit.BorderRadius = 8;
            btnAdaptFit.BorderSize = 1;
            btnAdaptFit.FlatStyle = FlatStyle.Flat;
            btnAdaptFit.Font = new Font("Segoe UI", 8.5F);
            btnAdaptFit.ForeColor = Color.Gray;
            btnAdaptFit.Location = new Point(16, 25);
            btnAdaptFit.Name = "btnAdaptFit";
            btnAdaptFit.Size = new Size(79, 32);
            btnAdaptFit.SvgAlignment = ContentAlignment.MiddleCenter;
            btnAdaptFit.SvgColor = Color.Black;
            btnAdaptFit.SvgContent = null;
            btnAdaptFit.SvgOffset = new Point(0, 0);
            btnAdaptFit.SvgPadding = new Padding(0);
            btnAdaptFit.SvgResource = null;
            btnAdaptFit.SvgSize = new Size(50, 50);
            btnAdaptFit.TabIndex = 1;
            btnAdaptFit.Text = "Fit";
            btnAdaptFit.TextColor = Color.Gray;
            btnAdaptFit.UseVisualStyleBackColor = false;
            btnAdaptFit.Click += btnAdaptFit_Click;
            // 
            // btnAdaptFill
            // 
            btnAdaptFill.BackColor = Color.FromArgb(10, 25, 30);
            btnAdaptFill.BackgroundColor = Color.FromArgb(10, 25, 30);
            btnAdaptFill.BorderColor = Color.FromArgb(20, 40, 45);
            btnAdaptFill.BorderRadius = 8;
            btnAdaptFill.BorderSize = 1;
            btnAdaptFill.FlatStyle = FlatStyle.Flat;
            btnAdaptFill.Font = new Font("Segoe UI", 8.5F);
            btnAdaptFill.ForeColor = Color.Gray;
            btnAdaptFill.Location = new Point(100, 25);
            btnAdaptFill.Name = "btnAdaptFill";
            btnAdaptFill.Size = new Size(79, 32);
            btnAdaptFill.SvgAlignment = ContentAlignment.MiddleCenter;
            btnAdaptFill.SvgColor = Color.Black;
            btnAdaptFill.SvgContent = null;
            btnAdaptFill.SvgOffset = new Point(0, 0);
            btnAdaptFill.SvgPadding = new Padding(0);
            btnAdaptFill.SvgResource = null;
            btnAdaptFill.SvgSize = new Size(50, 50);
            btnAdaptFill.TabIndex = 2;
            btnAdaptFill.Text = "Fill";
            btnAdaptFill.TextColor = Color.Gray;
            btnAdaptFill.UseVisualStyleBackColor = false;
            btnAdaptFill.Click += btnAdaptFill_Click;
            // 
            // btnAdaptStretch
            // 
            btnAdaptStretch.BackColor = Color.FromArgb(10, 25, 30);
            btnAdaptStretch.BackgroundColor = Color.FromArgb(10, 25, 30);
            btnAdaptStretch.BorderColor = Color.FromArgb(20, 40, 45);
            btnAdaptStretch.BorderRadius = 8;
            btnAdaptStretch.BorderSize = 1;
            btnAdaptStretch.FlatStyle = FlatStyle.Flat;
            btnAdaptStretch.Font = new Font("Segoe UI", 8.5F);
            btnAdaptStretch.ForeColor = Color.Gray;
            btnAdaptStretch.Location = new Point(184, 25);
            btnAdaptStretch.Name = "btnAdaptStretch";
            btnAdaptStretch.Size = new Size(79, 32);
            btnAdaptStretch.SvgAlignment = ContentAlignment.MiddleCenter;
            btnAdaptStretch.SvgColor = Color.Black;
            btnAdaptStretch.SvgContent = null;
            btnAdaptStretch.SvgOffset = new Point(0, 0);
            btnAdaptStretch.SvgPadding = new Padding(0);
            btnAdaptStretch.SvgResource = null;
            btnAdaptStretch.SvgSize = new Size(50, 50);
            btnAdaptStretch.TabIndex = 3;
            btnAdaptStretch.Text = "Stretch";
            btnAdaptStretch.TextColor = Color.Gray;
            btnAdaptStretch.UseVisualStyleBackColor = false;
            btnAdaptStretch.Click += btnAdaptStretch_Click;
            // 
            // btnAdaptCenter
            // 
            btnAdaptCenter.BackColor = Color.FromArgb(10, 25, 30);
            btnAdaptCenter.BackgroundColor = Color.FromArgb(10, 25, 30);
            btnAdaptCenter.BorderColor = Color.FromArgb(20, 40, 45);
            btnAdaptCenter.BorderRadius = 8;
            btnAdaptCenter.BorderSize = 1;
            btnAdaptCenter.FlatStyle = FlatStyle.Flat;
            btnAdaptCenter.Font = new Font("Segoe UI", 8.5F);
            btnAdaptCenter.ForeColor = Color.Gray;
            btnAdaptCenter.Location = new Point(268, 25);
            btnAdaptCenter.Name = "btnAdaptCenter";
            btnAdaptCenter.Size = new Size(79, 32);
            btnAdaptCenter.SvgAlignment = ContentAlignment.MiddleCenter;
            btnAdaptCenter.SvgColor = Color.Black;
            btnAdaptCenter.SvgContent = null;
            btnAdaptCenter.SvgOffset = new Point(0, 0);
            btnAdaptCenter.SvgPadding = new Padding(0);
            btnAdaptCenter.SvgResource = null;
            btnAdaptCenter.SvgSize = new Size(50, 50);
            btnAdaptCenter.TabIndex = 4;
            btnAdaptCenter.Text = "Center";
            btnAdaptCenter.TextColor = Color.Gray;
            btnAdaptCenter.UseVisualStyleBackColor = false;
            btnAdaptCenter.Click += btnAdaptCenter_Click;
            // 
            // lblZoom
            // 
            lblZoom.AutoSize = true;
            lblZoom.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblZoom.ForeColor = Color.White;
            lblZoom.Location = new Point(17, 68);
            lblZoom.Name = "lblZoom";
            lblZoom.Size = new Size(55, 21);
            lblZoom.TabIndex = 5;
            lblZoom.Text = "Zoom";
            // 
            // sliderZoom
            // 
            sliderZoom.BackColor = Color.FromArgb(10, 25, 30);
            sliderZoom.BorderColor = Color.FromArgb(10, 25, 30);
            sliderZoom.BorderRadius = 4;
            sliderZoom.LabelFont = new Font("Segoe UI", 9F);
            sliderZoom.LabelOffset = new Point(0, 0);
            sliderZoom.LabelText = "";
            sliderZoom.LabelTextColor = Color.Black;
            sliderZoom.Location = new Point(11, 78);
            sliderZoom.Maximum = 500;
            sliderZoom.Minimum = 100;
            sliderZoom.Name = "sliderZoom";
            sliderZoom.Size = new Size(290, 56);
            sliderZoom.TabIndex = 6;
            sliderZoom.ThumbColor = Color.FromArgb(106, 227, 249);
            sliderZoom.ThumbSize = 16;
            sliderZoom.TrackColor = Color.FromArgb(7, 36, 43);
            sliderZoom.Value = 100;
            sliderZoom.Scroll += sliderZoom_Scroll;
            // 
            // lblZoomVal
            // 
            lblZoomVal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblZoomVal.ForeColor = Color.FromArgb(106, 227, 249);
            lblZoomVal.Location = new Point(297, 95);
            lblZoomVal.Name = "lblZoomVal";
            lblZoomVal.Size = new Size(56, 25);
            lblZoomVal.TabIndex = 7;
            lblZoomVal.Text = "100%";
            lblZoomVal.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblBgColor
            // 
            lblBgColor.AutoSize = true;
            lblBgColor.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblBgColor.ForeColor = Color.White;
            lblBgColor.Location = new Point(16, 130);
            lblBgColor.Name = "lblBgColor";
            lblBgColor.Size = new Size(147, 21);
            lblBgColor.TabIndex = 8;
            lblBgColor.Text = "Background Color";
            // 
            // panelBgColorColor
            // 
            panelBgColorColor.BackColor = Color.Transparent;
            panelBgColorColor.BackgroundColor = Color.Transparent;
            panelBgColorColor.BorderColor = Color.FromArgb(5, 55, 66);
            panelBgColorColor.BorderRadius = 8;
            panelBgColorColor.BorderSize = 2;
            panelBgColorColor.Cursor = Cursors.Hand;
            panelBgColorColor.Location = new Point(19, 160);
            panelBgColorColor.Name = "panelBgColorColor";
            panelBgColorColor.Size = new Size(123, 32);
            panelBgColorColor.TabIndex = 9;
            panelBgColorColor.Click += panelBgColorColor_Click;
            // 
            // btnBgColorTransparent
            // 
            btnBgColorTransparent.BackColor = Color.FromArgb(7, 36, 43);
            btnBgColorTransparent.BackgroundColor = Color.FromArgb(7, 36, 43);
            btnBgColorTransparent.BorderColor = Color.FromArgb(5, 55, 66);
            btnBgColorTransparent.BorderRadius = 8;
            btnBgColorTransparent.BorderSize = 2;
            btnBgColorTransparent.FlatAppearance.BorderSize = 0;
            btnBgColorTransparent.FlatStyle = FlatStyle.Flat;
            btnBgColorTransparent.Font = new Font("Segoe UI Black", 8.5F, FontStyle.Bold);
            btnBgColorTransparent.ForeColor = Color.FromArgb(106, 227, 249);
            btnBgColorTransparent.Location = new Point(148, 160);
            btnBgColorTransparent.Name = "btnBgColorTransparent";
            btnBgColorTransparent.Size = new Size(201, 32);
            btnBgColorTransparent.SvgAlignment = ContentAlignment.MiddleCenter;
            btnBgColorTransparent.SvgColor = Color.Black;
            btnBgColorTransparent.SvgContent = null;
            btnBgColorTransparent.SvgOffset = new Point(0, 0);
            btnBgColorTransparent.SvgPadding = new Padding(0);
            btnBgColorTransparent.SvgResource = null;
            btnBgColorTransparent.SvgSize = new Size(50, 50);
            btnBgColorTransparent.TabIndex = 10;
            btnBgColorTransparent.Text = "Transparent";
            btnBgColorTransparent.TextColor = Color.FromArgb(106, 227, 249);
            btnBgColorTransparent.UseVisualStyleBackColor = false;
            btnBgColorTransparent.Click += btnBgColorTransparent_Click;
            // 
            // chkRemoveBg
            // 
            chkRemoveBg.AutoCheck = false;
            chkRemoveBg.AutoSize = true;
            chkRemoveBg.BackColor = Color.Transparent;
            chkRemoveBg.BorderColor = Color.FromArgb(5, 55, 66);
            chkRemoveBg.BorderRadius = 4;
            chkRemoveBg.BorderSize = 2;
            chkRemoveBg.BoxFillColor = Color.FromArgb(7, 36, 43);
            chkRemoveBg.CheckColor = Color.FromArgb(106, 227, 249);
            chkRemoveBg.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            chkRemoveBg.ForeColor = Color.White;
            chkRemoveBg.Location = new Point(16, 208);
            chkRemoveBg.Name = "chkRemoveBg";
            chkRemoveBg.Size = new Size(190, 25);
            chkRemoveBg.TabIndex = 11;
            chkRemoveBg.Text = "Remove Background";
            chkRemoveBg.UseVisualStyleBackColor = false;
            chkRemoveBg.CheckedChanged += chkRemoveBg_CheckedChanged;
            // 
            // lblRemoveBgColor
            // 
            lblRemoveBgColor.AutoSize = true;
            lblRemoveBgColor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRemoveBgColor.ForeColor = Color.White;
            lblRemoveBgColor.Location = new Point(16, 243);
            lblRemoveBgColor.Name = "lblRemoveBgColor";
            lblRemoveBgColor.Size = new Size(126, 20);
            lblRemoveBgColor.TabIndex = 12;
            lblRemoveBgColor.Text = "Color to Remove";
            // 
            // panelRemoveBgColorColor
            // 
            panelRemoveBgColorColor.BackColor = Color.White;
            panelRemoveBgColorColor.BackgroundColor = Color.White;
            panelRemoveBgColorColor.BorderColor = Color.FromArgb(5, 55, 66);
            panelRemoveBgColorColor.BorderRadius = 8;
            panelRemoveBgColorColor.BorderSize = 2;
            panelRemoveBgColorColor.Cursor = Cursors.Hand;
            panelRemoveBgColorColor.Location = new Point(16, 265);
            panelRemoveBgColorColor.Name = "panelRemoveBgColorColor";
            panelRemoveBgColorColor.Size = new Size(126, 32);
            panelRemoveBgColorColor.TabIndex = 13;
            panelRemoveBgColorColor.Click += panelRemoveBgColorColor_Click;
            // 
            // btnPickColor
            // 
            btnPickColor.BackColor = Color.FromArgb(7, 36, 43);
            btnPickColor.BackgroundColor = Color.FromArgb(7, 36, 43);
            btnPickColor.BorderColor = Color.FromArgb(5, 55, 66);
            btnPickColor.BorderRadius = 8;
            btnPickColor.BorderSize = 2;
            btnPickColor.FlatAppearance.BorderSize = 0;
            btnPickColor.FlatStyle = FlatStyle.Flat;
            btnPickColor.Font = new Font("Segoe UI Black", 8.5F, FontStyle.Bold);
            btnPickColor.ForeColor = Color.FromArgb(106, 227, 249);
            btnPickColor.Location = new Point(147, 265);
            btnPickColor.Name = "btnPickColor";
            btnPickColor.Size = new Size(32, 32);
            btnPickColor.SvgAlignment = ContentAlignment.MiddleCenter;
            btnPickColor.SvgColor = Color.FromArgb(106, 227, 249);
            btnPickColor.SvgContent = resources.GetString("btnPickColor.SvgContent");
            btnPickColor.SvgOffset = new Point(0, 0);
            btnPickColor.SvgPadding = new Padding(0);
            btnPickColor.SvgResource = "color_picker_svgrepo_com";
            btnPickColor.SvgSize = new Size(25, 25);
            btnPickColor.TabIndex = 14;
            btnPickColor.TextColor = Color.FromArgb(106, 227, 249);
            btnPickColor.UseVisualStyleBackColor = false;
            btnPickColor.Click += btnPickColor_Click;
            // 
            // lblTolerance
            // 
            lblTolerance.AutoSize = true;
            lblTolerance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTolerance.ForeColor = Color.White;
            lblTolerance.Location = new Point(193, 243);
            lblTolerance.Name = "lblTolerance";
            lblTolerance.Size = new Size(76, 20);
            lblTolerance.TabIndex = 15;
            lblTolerance.Text = "Tolerance";
            // 
            // sliderTolerance
            // 
            sliderTolerance.BackColor = Color.FromArgb(10, 25, 30);
            sliderTolerance.BorderColor = Color.FromArgb(10, 25, 30);
            sliderTolerance.BorderRadius = 4;
            sliderTolerance.LabelFont = new Font("Segoe UI", 9F);
            sliderTolerance.LabelOffset = new Point(0, 0);
            sliderTolerance.LabelText = "";
            sliderTolerance.LabelTextColor = Color.Black;
            sliderTolerance.Location = new Point(188, 253);
            sliderTolerance.Maximum = 100;
            sliderTolerance.Name = "sliderTolerance";
            sliderTolerance.Size = new Size(128, 56);
            sliderTolerance.TabIndex = 16;
            sliderTolerance.ThumbColor = Color.FromArgb(106, 227, 249);
            sliderTolerance.ThumbSize = 14;
            sliderTolerance.TrackColor = Color.FromArgb(7, 36, 43);
            sliderTolerance.Value = 15;
            sliderTolerance.Scroll += sliderTolerance_Scroll;
            // 
            // lblToleranceVal
            // 
            lblToleranceVal.AutoSize = true;
            lblToleranceVal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblToleranceVal.ForeColor = Color.FromArgb(106, 227, 249);
            lblToleranceVal.Location = new Point(326, 268);
            lblToleranceVal.Name = "lblToleranceVal";
            lblToleranceVal.Size = new Size(27, 20);
            lblToleranceVal.TabIndex = 17;
            lblToleranceVal.Text = "15";
            lblToleranceVal.TextAlign = ContentAlignment.MiddleRight;
            // 
            // chkFeather
            // 
            chkFeather.AutoCheck = false;
            chkFeather.AutoSize = true;
            chkFeather.BackColor = Color.Transparent;
            chkFeather.BorderColor = Color.FromArgb(5, 55, 66);
            chkFeather.BorderRadius = 4;
            chkFeather.BorderSize = 2;
            chkFeather.BoxFillColor = Color.FromArgb(7, 36, 43);
            chkFeather.CheckColor = Color.FromArgb(106, 227, 249);
            chkFeather.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            chkFeather.ForeColor = Color.White;
            chkFeather.Location = new Point(17, 304);
            chkFeather.Name = "chkFeather";
            chkFeather.Size = new Size(123, 25);
            chkFeather.TabIndex = 18;
            chkFeather.Text = "Refine Edge";
            chkFeather.UseVisualStyleBackColor = false;
            chkFeather.CheckedChanged += chkFeather_CheckedChanged;
            // 
            // lblChoke
            // 
            lblChoke.AutoSize = true;
            lblChoke.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblChoke.ForeColor = Color.White;
            lblChoke.Location = new Point(13, 338);
            lblChoke.Name = "lblChoke";
            lblChoke.Size = new Size(52, 20);
            lblChoke.TabIndex = 19;
            lblChoke.Text = "Choke";
            // 
            // sliderChoke
            // 
            sliderChoke.BackColor = Color.FromArgb(10, 25, 30);
            sliderChoke.BorderColor = Color.FromArgb(10, 25, 30);
            sliderChoke.BorderRadius = 4;
            sliderChoke.LabelFont = new Font("Segoe UI", 9F);
            sliderChoke.LabelOffset = new Point(0, 0);
            sliderChoke.LabelText = "";
            sliderChoke.LabelTextColor = Color.Black;
            sliderChoke.Location = new Point(13, 347);
            sliderChoke.Maximum = 100;
            sliderChoke.Name = "sliderChoke";
            sliderChoke.Size = new Size(128, 56);
            sliderChoke.TabIndex = 20;
            sliderChoke.ThumbColor = Color.FromArgb(106, 227, 249);
            sliderChoke.ThumbSize = 14;
            sliderChoke.TrackColor = Color.FromArgb(7, 36, 43);
            sliderChoke.Value = 1;
            sliderChoke.Scroll += sliderChoke_Scroll;
            // 
            // lblChokeVal
            // 
            lblChokeVal.AutoSize = true;
            lblChokeVal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblChokeVal.ForeColor = Color.FromArgb(106, 227, 249);
            lblChokeVal.Location = new Point(144, 363);
            lblChokeVal.Name = "lblChokeVal";
            lblChokeVal.Size = new Size(35, 20);
            lblChokeVal.TabIndex = 21;
            lblChokeVal.Text = "1px";
            lblChokeVal.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblFeather
            // 
            lblFeather.AutoSize = true;
            lblFeather.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblFeather.ForeColor = Color.White;
            lblFeather.Location = new Point(193, 337);
            lblFeather.Name = "lblFeather";
            lblFeather.Size = new Size(62, 20);
            lblFeather.TabIndex = 22;
            lblFeather.Text = "Feather";
            // 
            // sliderFeather
            // 
            sliderFeather.BackColor = Color.FromArgb(10, 25, 30);
            sliderFeather.BorderColor = Color.FromArgb(10, 25, 30);
            sliderFeather.BorderRadius = 4;
            sliderFeather.LabelFont = new Font("Segoe UI", 9F);
            sliderFeather.LabelOffset = new Point(0, 0);
            sliderFeather.LabelText = "";
            sliderFeather.LabelTextColor = Color.Black;
            sliderFeather.Location = new Point(188, 347);
            sliderFeather.Maximum = 100;
            sliderFeather.Name = "sliderFeather";
            sliderFeather.Size = new Size(128, 56);
            sliderFeather.TabIndex = 23;
            sliderFeather.ThumbColor = Color.FromArgb(106, 227, 249);
            sliderFeather.ThumbSize = 14;
            sliderFeather.TrackColor = Color.FromArgb(7, 36, 43);
            sliderFeather.Value = 2;
            sliderFeather.Scroll += sliderFeather_Scroll;
            // 
            // lblFeatherVal
            // 
            lblFeatherVal.AutoSize = true;
            lblFeatherVal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblFeatherVal.ForeColor = Color.FromArgb(106, 227, 249);
            lblFeatherVal.Location = new Point(321, 363);
            lblFeatherVal.Name = "lblFeatherVal";
            lblFeatherVal.Size = new Size(35, 20);
            lblFeatherVal.TabIndex = 24;
            lblFeatherVal.Text = "2px";
            lblFeatherVal.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnReset
            // 
            btnReset.BackColor = Color.FromArgb(7, 36, 43);
            btnReset.BackgroundColor = Color.FromArgb(7, 36, 43);
            btnReset.BorderColor = Color.FromArgb(5, 55, 66);
            btnReset.BorderRadius = 10;
            btnReset.BorderSize = 2;
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Font = new Font("Segoe UI Black", 9.5F, FontStyle.Bold);
            btnReset.ForeColor = Color.FromArgb(106, 227, 249);
            btnReset.Location = new Point(17, 410);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(163, 44);
            btnReset.SvgAlignment = ContentAlignment.MiddleCenter;
            btnReset.SvgColor = Color.Black;
            btnReset.SvgContent = null;
            btnReset.SvgOffset = new Point(0, 0);
            btnReset.SvgPadding = new Padding(0);
            btnReset.SvgResource = null;
            btnReset.SvgSize = new Size(50, 50);
            btnReset.TabIndex = 25;
            btnReset.Text = "Reset";
            btnReset.TextColor = Color.FromArgb(106, 227, 249);
            btnReset.UseVisualStyleBackColor = false;
            btnReset.Click += btnReset_Click;
            // 
            // btnRotate
            // 
            btnRotate.BackColor = Color.FromArgb(7, 36, 43);
            btnRotate.BackgroundColor = Color.FromArgb(7, 36, 43);
            btnRotate.BorderColor = Color.FromArgb(5, 55, 66);
            btnRotate.BorderRadius = 10;
            btnRotate.BorderSize = 2;
            btnRotate.FlatAppearance.BorderSize = 0;
            btnRotate.FlatStyle = FlatStyle.Flat;
            btnRotate.Font = new Font("Segoe UI Black", 9.5F, FontStyle.Bold);
            btnRotate.ForeColor = Color.FromArgb(106, 227, 249);
            btnRotate.Location = new Point(188, 410);
            btnRotate.Name = "btnRotate";
            btnRotate.Size = new Size(161, 44);
            btnRotate.SvgAlignment = ContentAlignment.MiddleRight;
            btnRotate.SvgColor = Color.FromArgb(106, 227, 249);
            btnRotate.SvgContent = resources.GetString("btnRotate.SvgContent");
            btnRotate.SvgOffset = new Point(-10, 1);
            btnRotate.SvgPadding = new Padding(0);
            btnRotate.SvgResource = "rotate_right_svgrepo_com";
            btnRotate.SvgSize = new Size(25, 25);
            btnRotate.TabIndex = 26;
            btnRotate.Text = "Rotate 90°";
            btnRotate.TextColor = Color.FromArgb(106, 227, 249);
            btnRotate.UseVisualStyleBackColor = false;
            btnRotate.Click += btnRotate_Click;
            // 
            // panelGifControls
            // 
            panelGifControls.Controls.Add(lblStartFrame);
            panelGifControls.Controls.Add(trackBarStartFrame);
            panelGifControls.Controls.Add(lblEndFrame);
            panelGifControls.Controls.Add(trackBarEndFrame);
            panelGifControls.Controls.Add(labelFPS);
            panelGifControls.Controls.Add(trackBarFPS);
            panelGifControls.Location = new Point(0, 321);
            panelGifControls.Name = "panelGifControls";
            panelGifControls.Size = new Size(370, 279);
            panelGifControls.TabIndex = 4;
            panelGifControls.Visible = false;
            // 
            // lblStartFrame
            // 
            lblStartFrame.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblStartFrame.Font = new Font("Segoe UI Black", 9.5F, FontStyle.Bold);
            lblStartFrame.ForeColor = Color.White;
            lblStartFrame.Location = new Point(25, 20);
            lblStartFrame.Name = "lblStartFrame";
            lblStartFrame.Size = new Size(320, 20);
            lblStartFrame.TabIndex = 4;
            lblStartFrame.Text = "Start Frame";
            lblStartFrame.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // trackBarStartFrame
            // 
            trackBarStartFrame.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarStartFrame.BackColor = Color.FromArgb(10, 25, 30);
            trackBarStartFrame.BorderColor = Color.FromArgb(10, 25, 30);
            trackBarStartFrame.BorderRadius = 5;
            trackBarStartFrame.LabelFont = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            trackBarStartFrame.LabelOffset = new Point(0, 0);
            trackBarStartFrame.LabelText = "0";
            trackBarStartFrame.LabelTextColor = Color.Black;
            trackBarStartFrame.Location = new Point(25, 45);
            trackBarStartFrame.Maximum = 63;
            trackBarStartFrame.Name = "trackBarStartFrame";
            trackBarStartFrame.Size = new Size(320, 56);
            trackBarStartFrame.TabIndex = 5;
            trackBarStartFrame.ThumbColor = Color.FromArgb(106, 227, 249);
            trackBarStartFrame.ThumbSize = 24;
            trackBarStartFrame.TrackColor = Color.FromArgb(7, 36, 43);
            trackBarStartFrame.Scroll += trackBarStartFrame_Scroll;
            // 
            // lblEndFrame
            // 
            lblEndFrame.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblEndFrame.Font = new Font("Segoe UI Black", 9.5F, FontStyle.Bold);
            lblEndFrame.ForeColor = Color.White;
            lblEndFrame.Location = new Point(25, 115);
            lblEndFrame.Name = "lblEndFrame";
            lblEndFrame.Size = new Size(320, 20);
            lblEndFrame.TabIndex = 6;
            lblEndFrame.Text = "End Frame";
            lblEndFrame.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // trackBarEndFrame
            // 
            trackBarEndFrame.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarEndFrame.BackColor = Color.FromArgb(10, 25, 30);
            trackBarEndFrame.BorderColor = Color.FromArgb(10, 25, 30);
            trackBarEndFrame.BorderRadius = 5;
            trackBarEndFrame.LabelFont = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            trackBarEndFrame.LabelOffset = new Point(0, 0);
            trackBarEndFrame.LabelText = "0";
            trackBarEndFrame.LabelTextColor = Color.Black;
            trackBarEndFrame.Location = new Point(25, 140);
            trackBarEndFrame.Maximum = 63;
            trackBarEndFrame.Name = "trackBarEndFrame";
            trackBarEndFrame.Size = new Size(320, 56);
            trackBarEndFrame.TabIndex = 7;
            trackBarEndFrame.ThumbColor = Color.FromArgb(106, 227, 249);
            trackBarEndFrame.ThumbSize = 24;
            trackBarEndFrame.TrackColor = Color.FromArgb(7, 36, 43);
            trackBarEndFrame.Scroll += trackBarEndFrame_Scroll;
            // 
            // labelFPS
            // 
            labelFPS.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelFPS.Font = new Font("Segoe UI Black", 9.5F, FontStyle.Bold);
            labelFPS.ForeColor = Color.White;
            labelFPS.Location = new Point(25, 210);
            labelFPS.Name = "labelFPS";
            labelFPS.Size = new Size(320, 20);
            labelFPS.TabIndex = 2;
            labelFPS.Text = "FPS";
            labelFPS.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // trackBarFPS
            // 
            trackBarFPS.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarFPS.BackColor = Color.FromArgb(10, 25, 30);
            trackBarFPS.BorderColor = Color.FromArgb(10, 25, 30);
            trackBarFPS.BorderRadius = 5;
            trackBarFPS.LabelFont = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            trackBarFPS.LabelOffset = new Point(0, 0);
            trackBarFPS.LabelText = "15";
            trackBarFPS.LabelTextColor = Color.Black;
            trackBarFPS.Location = new Point(25, 235);
            trackBarFPS.Maximum = 64;
            trackBarFPS.Minimum = 1;
            trackBarFPS.Name = "trackBarFPS";
            trackBarFPS.Size = new Size(320, 56);
            trackBarFPS.TabIndex = 3;
            trackBarFPS.ThumbColor = Color.FromArgb(106, 227, 249);
            trackBarFPS.ThumbSize = 24;
            trackBarFPS.TrackColor = Color.FromArgb(7, 36, 43);
            trackBarFPS.Value = 15;
            trackBarFPS.Scroll += trackBarFPS_Scroll;
            // 
            // lblTitle
            // 
            lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblTitle.Font = new Font("Segoe UI Black", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(106, 227, 249);
            lblTitle.Location = new Point(19, 19);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(339, 38);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "EMOJI EDITOR";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnEmojiStyle
            // 
            btnEmojiStyle.BackColor = Color.FromArgb(7, 36, 43);
            btnEmojiStyle.BackgroundColor = Color.FromArgb(7, 36, 43);
            btnEmojiStyle.BorderColor = Color.FromArgb(5, 55, 66);
            btnEmojiStyle.BorderRadius = 10;
            btnEmojiStyle.BorderSize = 2;
            btnEmojiStyle.FlatAppearance.BorderSize = 0;
            btnEmojiStyle.FlatStyle = FlatStyle.Flat;
            btnEmojiStyle.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            btnEmojiStyle.ForeColor = Color.FromArgb(106, 227, 249);
            btnEmojiStyle.Location = new Point(25, 67);
            btnEmojiStyle.Name = "btnEmojiStyle";
            btnEmojiStyle.Size = new Size(332, 40);
            btnEmojiStyle.SvgAlignment = ContentAlignment.MiddleCenter;
            btnEmojiStyle.SvgColor = Color.Black;
            btnEmojiStyle.SvgContent = null;
            btnEmojiStyle.SvgOffset = new Point(0, 0);
            btnEmojiStyle.SvgPadding = new Padding(0);
            btnEmojiStyle.SvgResource = null;
            btnEmojiStyle.SvgSize = new Size(50, 50);
            btnEmojiStyle.TabIndex = 1;
            btnEmojiStyle.Text = "Style: Select...";
            btnEmojiStyle.TextColor = Color.FromArgb(106, 227, 249);
            btnEmojiStyle.UseVisualStyleBackColor = false;
            btnEmojiStyle.Click += btnEmojiStyle_Click;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSave.BackColor = Color.FromArgb(7, 36, 43);
            btnSave.BackgroundColor = Color.FromArgb(7, 36, 43);
            btnSave.BorderColor = Color.FromArgb(106, 227, 249);
            btnSave.BorderRadius = 12;
            btnSave.BorderSize = 2;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            btnSave.ForeColor = Color.FromArgb(106, 227, 249);
            btnSave.Location = new Point(25, 719);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(326, 56);
            btnSave.SvgAlignment = ContentAlignment.MiddleCenter;
            btnSave.SvgColor = Color.Black;
            btnSave.SvgContent = null;
            btnSave.SvgOffset = new Point(0, 0);
            btnSave.SvgPadding = new Padding(0);
            btnSave.SvgResource = null;
            btnSave.SvgSize = new Size(50, 50);
            btnSave.TabIndex = 5;
            btnSave.Text = "Apply And Upload";
            btnSave.TextColor = Color.FromArgb(106, 227, 249);
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnCancel.BackColor = Color.FromArgb(20, 15, 15);
            btnCancel.BackgroundColor = Color.FromArgb(20, 15, 15);
            btnCancel.BorderColor = Color.FromArgb(80, 20, 20);
            btnCancel.BorderRadius = 12;
            btnCancel.BorderSize = 2;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI Black", 11F, FontStyle.Bold);
            btnCancel.ForeColor = Color.FromArgb(255, 128, 128);
            btnCancel.Location = new Point(25, 788);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(326, 56);
            btnCancel.SvgAlignment = ContentAlignment.MiddleCenter;
            btnCancel.SvgColor = Color.Black;
            btnCancel.SvgContent = null;
            btnCancel.SvgOffset = new Point(0, 0);
            btnCancel.SvgPadding = new Padding(0);
            btnCancel.SvgResource = null;
            btnCancel.SvgSize = new Size(50, 50);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "Cancel";
            btnCancel.TextColor = Color.FromArgb(255, 128, 128);
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // EmojiEditorForm
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(15, 17, 19);
            ClientSize = new Size(1566, 893);
            Controls.Add(mainTableLayout);
            Name = "EmojiEditorForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Emoji Editor";
            mainTableLayout.ResumeLayout(false);
            workspacePanel.ResumeLayout(false);
            previewPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)previewVRChat).EndInit();
            settingsPanel.ResumeLayout(false);
            containerImageAndGif.ResumeLayout(false);
            panelStaticControls.ResumeLayout(false);
            panelStaticControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)sliderZoom).EndInit();
            ((System.ComponentModel.ISupportInitialize)sliderTolerance).EndInit();
            ((System.ComponentModel.ISupportInitialize)sliderChoke).EndInit();
            ((System.ComponentModel.ISupportInitialize)sliderFeather).EndInit();
            panelGifControls.ResumeLayout(false);
            panelGifControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarStartFrame).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarEndFrame).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarFPS).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainTableLayout;
        private Panel workspacePanel;
        private DoubleBufferedPanel previewPanel;
        private RoundedPictureBox previewVRChat;
        private Label labelFPS;
        private RoundedTrackBar trackBarFPS;
        private ModernFlowPanel flowPanelFrames;
        private Label lblStartFrame;
        private RoundedTrackBar trackBarStartFrame;
        private Label lblEndFrame;
        private RoundedTrackBar trackBarEndFrame;

        private RoundedPanel settingsPanel;
        private Label lblTitle;
        private RoundedButton btnEmojiStyle;
        private Panel emojiTypePanel;
        private Panel panelStaticControls;
        private Label lblAdaptation;
        private RoundedButton btnAdaptFit;
        private RoundedButton btnAdaptFill;
        private RoundedButton btnAdaptStretch;
        private RoundedButton btnAdaptCenter;
        private Label lblZoom;
        private RoundedTrackBar sliderZoom;
        private Label lblZoomVal;
        private Label lblBgColor;
        private RoundedPanel panelBgColorColor;
        private RoundedButton btnBgColorTransparent;
        private RoundedCheckBox chkRemoveBg;
        private Label lblRemoveBgColor;
        private RoundedPanel panelRemoveBgColorColor;
        private RoundedButton btnPickColor;
        private Label lblTolerance;
        private RoundedTrackBar sliderTolerance;
        private Label lblToleranceVal;
        private RoundedCheckBox chkFeather;
        private Label lblChoke;
        private RoundedTrackBar sliderChoke;
        private Label lblChokeVal;
        private Label lblFeather;
        private RoundedTrackBar sliderFeather;
        private Label lblFeatherVal;
        private RoundedButton btnReset;
        private RoundedButton btnRotate;
        private RoundedButton btnSave;
        private RoundedButton btnCancel;
        private Panel containerImageAndGif;
        private Panel panelGifControls;
    }
}
