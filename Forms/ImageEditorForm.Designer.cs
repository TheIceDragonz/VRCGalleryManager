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
            mainTableLayout = new TableLayoutPanel();
            previewPanel = new DoubleBufferedPanel();
            settingsPanel = new Design.RoundedPanel();
            lblTitle = new Label();
            lblAdaptation = new Label();
            comboAdaptation = new ComboBox();
            lblZoom = new Label();
            sliderZoom = new TrackBar();
            lblZoomVal = new Label();
            lblBgColor = new Label();
            comboBgColor = new ComboBox();
            btnRotate = new Design.RoundedButton();
            btnReset = new Design.RoundedButton();
            btnSave = new Design.RoundedButton();
            btnCancel = new Design.RoundedButton();
            mainTableLayout.SuspendLayout();
            settingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)sliderZoom).BeginInit();
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
            mainTableLayout.Size = new Size(989, 653);
            mainTableLayout.TabIndex = 0;
            // 
            // previewPanel
            // 
            previewPanel.BackColor = Color.FromArgb(5, 5, 5);
            previewPanel.Dock = DockStyle.Fill;
            previewPanel.Location = new Point(12, 12);
            previewPanel.Margin = new Padding(12);
            previewPanel.Name = "previewPanel";
            previewPanel.Size = new Size(565, 629);
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
            settingsPanel.Controls.Add(btnRotate);
            settingsPanel.Controls.Add(btnReset);
            settingsPanel.Controls.Add(btnSave);
            settingsPanel.Controls.Add(btnCancel);
            settingsPanel.Dock = DockStyle.Fill;
            settingsPanel.Location = new Point(595, 12);
            settingsPanel.Margin = new Padding(6, 12, 12, 12);
            settingsPanel.Name = "settingsPanel";
            settingsPanel.Size = new Size(382, 629);
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
            lblAdaptation.Size = new Size(111, 21);
            lblAdaptation.TabIndex = 4;
            lblAdaptation.Text = "Adattamento";
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
            sliderZoom.Location = new Point(19, 206);
            sliderZoom.Margin = new Padding(4);
            sliderZoom.Maximum = 500;
            sliderZoom.Minimum = 100;
            sliderZoom.Name = "sliderZoom";
            sliderZoom.Size = new Size(276, 56);
            sliderZoom.TabIndex = 7;
            sliderZoom.TickStyle = TickStyle.None;
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
            lblBgColor.Size = new Size(138, 21);
            lblBgColor.TabIndex = 9;
            lblBgColor.Text = "Colore di Sfondo";
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
            btnRotate.Location = new Point(25, 362);
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
            btnRotate.Text = "Ruota 90° Orario";
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
            btnReset.Location = new Point(25, 419);
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
            btnReset.Text = "Reset Modifiche";
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
            btnSave.Location = new Point(25, 479);
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
            btnSave.Text = "Applica e Salva";
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
            btnCancel.Location = new Point(25, 548);
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
            btnCancel.Text = "Annulla";
            btnCancel.TextColor = Color.FromArgb(255, 128, 128);
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // ImageEditorForm
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(15, 17, 19);
            ClientSize = new Size(989, 653);
            Controls.Add(mainTableLayout);
            Margin = new Padding(4);
            MinimumSize = new Size(500, 700);
            Name = "ImageEditorForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Image Editor";
            FormClosing += ImageEditorForm_FormClosing;
            mainTableLayout.ResumeLayout(false);
            settingsPanel.ResumeLayout(false);
            settingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)sliderZoom).EndInit();
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
        private TrackBar sliderZoom;
        private Label lblZoomVal;
        private Label lblBgColor;
        private ComboBox comboBgColor;
        private VRCGalleryManager.Design.RoundedButton btnRotate;
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
