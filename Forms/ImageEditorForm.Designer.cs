using CustomControls;

namespace VRCGalleryManager.Forms
{
    partial class ImageEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageEditorForm));
            mainTableLayout = new TableLayoutPanel();
            previewPanel = new DoubleBufferedPanel();
            settingsPanel = new Design.RoundedPanel();
            lblTitle = new Label();
            lblAdaptation = new Label();
            comboAdaptation = new ComboBox();
            lblZoom = new Label();
            sliderZoom = new RoundedTrackBar();
            lblZoomVal = new Label();
            lblBgColor = new Label();
            comboBgColor = new ComboBox();
            chkRemoveBg = new Design.RoundedCheckBox();
            lblRemoveBgColor = new Label();
            comboRemoveBgColor = new ComboBox();
            btnPickColor = new Design.RoundedButton();
            lblTolerance = new Label();
            sliderTolerance = new RoundedTrackBar();
            lblToleranceVal = new Label();
            chkFeather = new Design.RoundedCheckBox();
            lblChoke = new Label();
            sliderChoke = new RoundedTrackBar();
            lblChokeVal = new Label();
            lblFeather = new Label();
            sliderFeather = new RoundedTrackBar();
            lblFeatherVal = new Label();
            btnRotate = new Design.RoundedButton();
            btnReset = new Design.RoundedButton();
            btnSave = new Design.RoundedButton();
            btnCancel = new Design.RoundedButton();
            mainTableLayout.SuspendLayout();
            settingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)sliderZoom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sliderTolerance).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sliderChoke).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sliderFeather).BeginInit();
            SuspendLayout();
            // 
            // mainTableLayout
            // 
            mainTableLayout.ColumnCount = 2;
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400F));
            mainTableLayout.Controls.Add(previewPanel, 0, 0);
            mainTableLayout.Controls.Add(settingsPanel, 1, 0);
            mainTableLayout.Dock = DockStyle.Fill;
            mainTableLayout.Location = new Point(0, 0);
            mainTableLayout.Margin = new Padding(4);
            mainTableLayout.Name = "mainTableLayout";
            mainTableLayout.RowCount = 1;
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTableLayout.Size = new Size(1049, 913);
            mainTableLayout.TabIndex = 0;
            // 
            // previewPanel
            // 
            previewPanel.BackColor = Color.FromArgb(5, 5, 5);
            previewPanel.Dock = DockStyle.Fill;
            previewPanel.Location = new Point(12, 12);
            previewPanel.Margin = new Padding(12);
            previewPanel.Name = "previewPanel";
            previewPanel.Size = new Size(625, 889);
            previewPanel.TabIndex = 0;
            previewPanel.Paint += previewPanel_Paint;
            previewPanel.MouseDown += previewPanel_MouseDown;
            previewPanel.MouseMove += previewPanel_MouseMove;
            previewPanel.MouseUp += previewPanel_MouseUp;
            // 
            // settingsPanel
            // 
            settingsPanel.BackColor = Color.FromArgb(10, 25, 30);
            settingsPanel.BackgroundColor = Color.FromArgb(10, 25, 30);
            settingsPanel.BorderColor = Color.FromArgb(5, 55, 66);
            settingsPanel.BorderRadius = 15;
            settingsPanel.BorderSize = 2;
            settingsPanel.Controls.Add(lblTitle);
            settingsPanel.Controls.Add(lblAdaptation);
            settingsPanel.Controls.Add(comboAdaptation);
            settingsPanel.Controls.Add(lblZoom);
            settingsPanel.Controls.Add(sliderZoom);
            settingsPanel.Controls.Add(lblZoomVal);
            settingsPanel.Controls.Add(lblBgColor);
            settingsPanel.Controls.Add(comboBgColor);
            settingsPanel.Controls.Add(chkRemoveBg);
            settingsPanel.Controls.Add(lblRemoveBgColor);
            settingsPanel.Controls.Add(comboRemoveBgColor);
            settingsPanel.Controls.Add(btnPickColor);
            settingsPanel.Controls.Add(lblTolerance);
            settingsPanel.Controls.Add(sliderTolerance);
            settingsPanel.Controls.Add(lblToleranceVal);
            settingsPanel.Controls.Add(chkFeather);
            settingsPanel.Controls.Add(lblChoke);
            settingsPanel.Controls.Add(sliderChoke);
            settingsPanel.Controls.Add(lblChokeVal);
            settingsPanel.Controls.Add(lblFeather);
            settingsPanel.Controls.Add(sliderFeather);
            settingsPanel.Controls.Add(lblFeatherVal);
            settingsPanel.Controls.Add(btnRotate);
            settingsPanel.Controls.Add(btnReset);
            settingsPanel.Controls.Add(btnSave);
            settingsPanel.Controls.Add(btnCancel);
            settingsPanel.Dock = DockStyle.Fill;
            settingsPanel.Location = new Point(655, 12);
            settingsPanel.Margin = new Padding(6, 12, 12, 12);
            settingsPanel.Name = "settingsPanel";
            settingsPanel.Size = new Size(382, 889);
            settingsPanel.TabIndex = 1;
            // 
            // lblTitle
            // 
            lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblTitle.Font = new Font("Segoe UI Black", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(106, 227, 249);
            lblTitle.Location = new Point(19, 19);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(345, 38);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "IMAGE EDITOR";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAdaptation
            // 
            lblAdaptation.AutoSize = true;
            lblAdaptation.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblAdaptation.ForeColor = Color.White;
            lblAdaptation.Location = new Point(25, 81);
            lblAdaptation.Margin = new Padding(4, 0, 4, 0);
            lblAdaptation.Name = "lblAdaptation";
            lblAdaptation.Size = new Size(96, 21);
            lblAdaptation.TabIndex = 4;
            lblAdaptation.Text = "Adaptation";
            // 
            // comboAdaptation
            // 
            comboAdaptation.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboAdaptation.BackColor = Color.FromArgb(7, 36, 43);
            comboAdaptation.DropDownStyle = ComboBoxStyle.DropDownList;
            comboAdaptation.FlatStyle = FlatStyle.Flat;
            comboAdaptation.Font = new Font("Segoe UI", 9.5F);
            comboAdaptation.ForeColor = Color.FromArgb(106, 227, 249);
            comboAdaptation.FormattingEnabled = true;
            comboAdaptation.Location = new Point(25, 112);
            comboAdaptation.Margin = new Padding(4);
            comboAdaptation.Name = "comboAdaptation";
            comboAdaptation.Size = new Size(331, 29);
            comboAdaptation.TabIndex = 5;
            comboAdaptation.SelectedIndexChanged += comboAdaptation_SelectedIndexChanged;
            // 
            // lblZoom
            // 
            lblZoom.AutoSize = true;
            lblZoom.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblZoom.ForeColor = Color.White;
            lblZoom.Location = new Point(25, 175);
            lblZoom.Margin = new Padding(4, 0, 4, 0);
            lblZoom.Name = "lblZoom";
            lblZoom.Size = new Size(55, 21);
            lblZoom.TabIndex = 6;
            lblZoom.Text = "Zoom";
            // 
            // sliderZoom
            // 
            sliderZoom.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            sliderZoom.BackColor = Color.FromArgb(10, 25, 30);
            sliderZoom.BorderColor = Color.FromArgb(10, 25, 30);
            sliderZoom.BorderRadius = 4;
            sliderZoom.LabelFont = new Font("Segoe UI", 9F);
            sliderZoom.LabelOffset = new Point(0, 0);
            sliderZoom.LabelText = "";
            sliderZoom.LabelTextColor = Color.Black;
            sliderZoom.Location = new Point(19, 206);
            sliderZoom.Margin = new Padding(4);
            sliderZoom.Maximum = 500;
            sliderZoom.Minimum = 100;
            sliderZoom.Name = "sliderZoom";
            sliderZoom.Size = new Size(276, 56);
            sliderZoom.TabIndex = 7;
            sliderZoom.ThumbColor = Color.FromArgb(106, 227, 249);
            sliderZoom.ThumbSize = 16;
            sliderZoom.TrackColor = Color.FromArgb(7, 36, 43);
            sliderZoom.Value = 100;
            sliderZoom.Scroll += sliderZoom_Scroll;
            // 
            // lblZoomVal
            // 
            lblZoomVal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblZoomVal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblZoomVal.ForeColor = Color.FromArgb(106, 227, 249);
            lblZoomVal.Location = new Point(301, 209);
            lblZoomVal.Margin = new Padding(4, 0, 4, 0);
            lblZoomVal.Name = "lblZoomVal";
            lblZoomVal.Size = new Size(56, 25);
            lblZoomVal.TabIndex = 8;
            lblZoomVal.Text = "100%";
            lblZoomVal.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblBgColor
            // 
            lblBgColor.AutoSize = true;
            lblBgColor.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblBgColor.ForeColor = Color.White;
            lblBgColor.Location = new Point(25, 269);
            lblBgColor.Margin = new Padding(4, 0, 4, 0);
            lblBgColor.Name = "lblBgColor";
            lblBgColor.Size = new Size(147, 21);
            lblBgColor.TabIndex = 9;
            lblBgColor.Text = "Background Color";
            // 
            // comboBgColor
            // 
            comboBgColor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBgColor.BackColor = Color.FromArgb(7, 36, 43);
            comboBgColor.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBgColor.FlatStyle = FlatStyle.Flat;
            comboBgColor.Font = new Font("Segoe UI", 9.5F);
            comboBgColor.ForeColor = Color.FromArgb(106, 227, 249);
            comboBgColor.FormattingEnabled = true;
            comboBgColor.Location = new Point(25, 300);
            comboBgColor.Margin = new Padding(4);
            comboBgColor.Name = "comboBgColor";
            comboBgColor.Size = new Size(331, 29);
            comboBgColor.TabIndex = 10;
            comboBgColor.SelectedIndexChanged += comboBgColor_SelectedIndexChanged;
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
            chkRemoveBg.Location = new Point(24, 355);
            chkRemoveBg.Margin = new Padding(4);
            chkRemoveBg.Name = "chkRemoveBg";
            chkRemoveBg.Size = new Size(190, 25);
            chkRemoveBg.TabIndex = 15;
            chkRemoveBg.Text = "Remove Background";
            chkRemoveBg.UseVisualStyleBackColor = false;
            chkRemoveBg.CheckedChanged += chkRemoveBg_CheckedChanged;
            // 
            // lblRemoveBgColor
            // 
            lblRemoveBgColor.AutoSize = true;
            lblRemoveBgColor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRemoveBgColor.ForeColor = Color.White;
            lblRemoveBgColor.Location = new Point(24, 390);
            lblRemoveBgColor.Margin = new Padding(4, 0, 4, 0);
            lblRemoveBgColor.Name = "lblRemoveBgColor";
            lblRemoveBgColor.Size = new Size(126, 20);
            lblRemoveBgColor.TabIndex = 16;
            lblRemoveBgColor.Text = "Color to Remove";
            // 
            // comboRemoveBgColor
            // 
            comboRemoveBgColor.BackColor = Color.FromArgb(7, 36, 43);
            comboRemoveBgColor.DropDownStyle = ComboBoxStyle.DropDownList;
            comboRemoveBgColor.FlatStyle = FlatStyle.Flat;
            comboRemoveBgColor.Font = new Font("Segoe UI", 9F);
            comboRemoveBgColor.ForeColor = Color.FromArgb(106, 227, 249);
            comboRemoveBgColor.FormattingEnabled = true;
            comboRemoveBgColor.Location = new Point(24, 415);
            comboRemoveBgColor.Margin = new Padding(4);
            comboRemoveBgColor.Name = "comboRemoveBgColor";
            comboRemoveBgColor.Size = new Size(150, 28);
            comboRemoveBgColor.TabIndex = 17;
            comboRemoveBgColor.SelectedIndexChanged += comboRemoveBgColor_SelectedIndexChanged;
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
            btnPickColor.Location = new Point(182, 412);
            btnPickColor.Margin = new Padding(4);
            btnPickColor.Name = "btnPickColor";
            btnPickColor.Size = new Size(32, 32);
            btnPickColor.SvgAlignment = ContentAlignment.MiddleCenter;
            btnPickColor.SvgColor = Color.FromArgb(106, 227, 249);
            btnPickColor.SvgContent = resources.GetString("btnPickColor.SvgContent");
            btnPickColor.SvgOffset = new Point(0, 0);
            btnPickColor.SvgPadding = new Padding(0);
            btnPickColor.SvgResource = "color_picker_svgrepo_com";
            btnPickColor.SvgSize = new Size(25, 25);
            btnPickColor.TabIndex = 18;
            btnPickColor.TextColor = Color.FromArgb(106, 227, 249);
            btnPickColor.UseVisualStyleBackColor = false;
            btnPickColor.Click += btnPickColor_Click;
            // 
            // lblTolerance
            // 
            lblTolerance.AutoSize = true;
            lblTolerance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTolerance.ForeColor = Color.White;
            lblTolerance.Location = new Point(222, 390);
            lblTolerance.Margin = new Padding(4, 0, 4, 0);
            lblTolerance.Name = "lblTolerance";
            lblTolerance.Size = new Size(76, 20);
            lblTolerance.TabIndex = 19;
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
            sliderTolerance.Location = new Point(222, 400);
            sliderTolerance.Margin = new Padding(4);
            sliderTolerance.Maximum = 100;
            sliderTolerance.Name = "sliderTolerance";
            sliderTolerance.Size = new Size(104, 56);
            sliderTolerance.TabIndex = 21;
            sliderTolerance.ThumbColor = Color.FromArgb(106, 227, 249);
            sliderTolerance.ThumbSize = 14;
            sliderTolerance.TrackColor = Color.FromArgb(7, 36, 43);
            sliderTolerance.Value = 15;
            sliderTolerance.Scroll += sliderTolerance_Scroll;
            // 
            // lblToleranceVal
            // 
            lblToleranceVal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblToleranceVal.ForeColor = Color.FromArgb(106, 227, 249);
            lblToleranceVal.Location = new Point(334, 415);
            lblToleranceVal.Margin = new Padding(4, 0, 4, 0);
            lblToleranceVal.Name = "lblToleranceVal";
            lblToleranceVal.Size = new Size(30, 25);
            lblToleranceVal.TabIndex = 22;
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
            chkFeather.Location = new Point(24, 480);
            chkFeather.Margin = new Padding(4);
            chkFeather.Name = "chkFeather";
            chkFeather.Size = new Size(123, 25);
            chkFeather.TabIndex = 23;
            chkFeather.Text = "Refine Edge";
            chkFeather.UseVisualStyleBackColor = false;
            chkFeather.CheckedChanged += chkFeather_CheckedChanged;
            // 
            // lblChoke
            // 
            lblChoke.AutoSize = true;
            lblChoke.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblChoke.ForeColor = Color.White;
            lblChoke.Location = new Point(19, 509);
            lblChoke.Margin = new Padding(4, 0, 4, 0);
            lblChoke.Name = "lblChoke";
            lblChoke.Size = new Size(52, 20);
            lblChoke.TabIndex = 24;
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
            sliderChoke.Location = new Point(19, 519);
            sliderChoke.Margin = new Padding(4);
            sliderChoke.Maximum = 100;
            sliderChoke.Name = "sliderChoke";
            sliderChoke.Size = new Size(110, 56);
            sliderChoke.TabIndex = 25;
            sliderChoke.ThumbColor = Color.FromArgb(106, 227, 249);
            sliderChoke.ThumbSize = 14;
            sliderChoke.TrackColor = Color.FromArgb(7, 36, 43);
            sliderChoke.Value = 1;
            sliderChoke.Scroll += sliderChoke_Scroll;
            // 
            // lblChokeVal
            // 
            lblChokeVal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblChokeVal.ForeColor = Color.FromArgb(106, 227, 249);
            lblChokeVal.Location = new Point(129, 534);
            lblChokeVal.Margin = new Padding(4, 0, 4, 0);
            lblChokeVal.Name = "lblChokeVal";
            lblChokeVal.Size = new Size(45, 25);
            lblChokeVal.TabIndex = 26;
            lblChokeVal.Text = "1px";
            lblChokeVal.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblFeather
            // 
            lblFeather.AutoSize = true;
            lblFeather.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblFeather.ForeColor = Color.White;
            lblFeather.Location = new Point(199, 508);
            lblFeather.Margin = new Padding(4, 0, 4, 0);
            lblFeather.Name = "lblFeather";
            lblFeather.Size = new Size(62, 20);
            lblFeather.TabIndex = 27;
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
            sliderFeather.Location = new Point(194, 519);
            sliderFeather.Margin = new Padding(4);
            sliderFeather.Maximum = 100;
            sliderFeather.Name = "sliderFeather";
            sliderFeather.Size = new Size(110, 56);
            sliderFeather.TabIndex = 28;
            sliderFeather.ThumbColor = Color.FromArgb(106, 227, 249);
            sliderFeather.ThumbSize = 14;
            sliderFeather.TrackColor = Color.FromArgb(7, 36, 43);
            sliderFeather.Value = 2;
            sliderFeather.Scroll += sliderFeather_Scroll;
            // 
            // lblFeatherVal
            // 
            lblFeatherVal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblFeatherVal.ForeColor = Color.FromArgb(106, 227, 249);
            lblFeatherVal.Location = new Point(311, 534);
            lblFeatherVal.Margin = new Padding(4, 0, 4, 0);
            lblFeatherVal.Name = "lblFeatherVal";
            lblFeatherVal.Size = new Size(45, 25);
            lblFeatherVal.TabIndex = 29;
            lblFeatherVal.Text = "2px";
            lblFeatherVal.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnRotate
            // 
            btnRotate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnRotate.BackColor = Color.FromArgb(7, 36, 43);
            btnRotate.BackgroundColor = Color.FromArgb(7, 36, 43);
            btnRotate.BorderColor = Color.FromArgb(5, 55, 66);
            btnRotate.BorderRadius = 10;
            btnRotate.BorderSize = 2;
            btnRotate.FlatAppearance.BorderSize = 0;
            btnRotate.FlatStyle = FlatStyle.Flat;
            btnRotate.Font = new Font("Segoe UI Black", 9.5F, FontStyle.Bold);
            btnRotate.ForeColor = Color.FromArgb(106, 227, 249);
            btnRotate.Location = new Point(25, 583);
            btnRotate.Margin = new Padding(4);
            btnRotate.Name = "btnRotate";
            btnRotate.Size = new Size(332, 44);
            btnRotate.SvgAlignment = ContentAlignment.MiddleCenter;
            btnRotate.SvgColor = Color.Black;
            btnRotate.SvgContent = null;
            btnRotate.SvgOffset = new Point(0, 0);
            btnRotate.SvgPadding = new Padding(0);
            btnRotate.SvgResource = null;
            btnRotate.SvgSize = new Size(50, 50);
            btnRotate.TabIndex = 11;
            btnRotate.Text = "Rotate 90° Clockwise";
            btnRotate.TextColor = Color.FromArgb(106, 227, 249);
            btnRotate.UseVisualStyleBackColor = false;
            btnRotate.Click += btnRotate_Click;
            // 
            // btnReset
            // 
            btnReset.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnReset.BackColor = Color.FromArgb(7, 36, 43);
            btnReset.BackgroundColor = Color.FromArgb(7, 36, 43);
            btnReset.BorderColor = Color.FromArgb(5, 55, 66);
            btnReset.BorderRadius = 10;
            btnReset.BorderSize = 2;
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Font = new Font("Segoe UI Black", 9.5F, FontStyle.Bold);
            btnReset.ForeColor = Color.FromArgb(106, 227, 249);
            btnReset.Location = new Point(25, 643);
            btnReset.Margin = new Padding(4);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(332, 44);
            btnReset.SvgAlignment = ContentAlignment.MiddleCenter;
            btnReset.SvgColor = Color.Black;
            btnReset.SvgContent = null;
            btnReset.SvgOffset = new Point(0, 0);
            btnReset.SvgPadding = new Padding(0);
            btnReset.SvgResource = null;
            btnReset.SvgSize = new Size(50, 50);
            btnReset.TabIndex = 12;
            btnReset.Text = "Reset Changes";
            btnReset.TextColor = Color.FromArgb(106, 227, 249);
            btnReset.UseVisualStyleBackColor = false;
            btnReset.Click += btnReset_Click;
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
            btnSave.Font = new Font("Segoe UI Black", 11F, FontStyle.Bold);
            btnSave.ForeColor = Color.FromArgb(106, 227, 249);
            btnSave.Location = new Point(25, 739);
            btnSave.Margin = new Padding(4);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(332, 56);
            btnSave.SvgAlignment = ContentAlignment.MiddleCenter;
            btnSave.SvgColor = Color.Black;
            btnSave.SvgContent = null;
            btnSave.SvgOffset = new Point(0, 0);
            btnSave.SvgPadding = new Padding(0);
            btnSave.SvgResource = null;
            btnSave.SvgSize = new Size(50, 50);
            btnSave.TabIndex = 13;
            btnSave.Text = "Apply & Save";
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
            btnCancel.Location = new Point(25, 808);
            btnCancel.Margin = new Padding(4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(332, 56);
            btnCancel.SvgAlignment = ContentAlignment.MiddleCenter;
            btnCancel.SvgColor = Color.Black;
            btnCancel.SvgContent = null;
            btnCancel.SvgOffset = new Point(0, 0);
            btnCancel.SvgPadding = new Padding(0);
            btnCancel.SvgResource = null;
            btnCancel.SvgSize = new Size(50, 50);
            btnCancel.TabIndex = 14;
            btnCancel.Text = "Cancel";
            btnCancel.TextColor = Color.FromArgb(255, 128, 128);
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // ImageEditorForm
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(15, 17, 19);
            ClientSize = new Size(1049, 913);
            Controls.Add(mainTableLayout);
            Margin = new Padding(4);
            MinimumSize = new Size(989, 960);
            Name = "ImageEditorForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Image Editor";
            FormClosing += ImageEditorForm_FormClosing;
            mainTableLayout.ResumeLayout(false);
            settingsPanel.ResumeLayout(false);
            settingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)sliderZoom).EndInit();
            ((System.ComponentModel.ISupportInitialize)sliderTolerance).EndInit();
            ((System.ComponentModel.ISupportInitialize)sliderChoke).EndInit();
            ((System.ComponentModel.ISupportInitialize)sliderFeather).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainTableLayout;
        private DoubleBufferedPanel previewPanel;
        private VRCGalleryManager.Design.RoundedPanel settingsPanel;
        private Label lblTitle;
        private Label lblAdaptation;
        private ComboBox comboAdaptation;
        private Label lblZoom;
        private RoundedTrackBar sliderZoom;
        private Label lblZoomVal;
        private Label lblBgColor;
        private ComboBox comboBgColor;
        private Design.RoundedCheckBox chkRemoveBg;
        private Label lblRemoveBgColor;
        private ComboBox comboRemoveBgColor;
        private Label lblTolerance;
        private RoundedTrackBar sliderTolerance;
        private Label lblToleranceVal;
        private Design.RoundedCheckBox chkFeather;
        private Label lblChoke;
        private RoundedTrackBar sliderChoke;
        private Label lblChokeVal;
        private Label lblFeather;
        private RoundedTrackBar sliderFeather;
        private Label lblFeatherVal;
        private VRCGalleryManager.Design.RoundedButton btnRotate;
        private VRCGalleryManager.Design.RoundedButton btnPickColor;
        private VRCGalleryManager.Design.RoundedButton btnReset;
        private VRCGalleryManager.Design.RoundedButton btnSave;
        private VRCGalleryManager.Design.RoundedButton btnCancel;
    }

    /// <summary>
    /// Double buffered panel to prevent preview flickering when panning/zooming.
    /// </summary>
    public class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }
    }
}
