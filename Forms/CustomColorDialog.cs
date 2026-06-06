using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CustomControls; // For RoundedTrackBar
using VRCGalleryManager.Design; // For RoundedButton

namespace VRCGalleryManager.Forms
{
    public class CustomColorDialog : Form
    {
        public Color SelectedColor { get; set; } = Color.White;

        private DoubleBufferedPanel panelSV;
        private DoubleBufferedPanel panelHue;
        private Panel previewPanel;
        private TextBox txtHex;
        private RoundedTrackBar sliderR;
        private RoundedTrackBar sliderG;
        private RoundedTrackBar sliderB;
        private Label lblRVal;
        private Label lblGVal;
        private Label lblBVal;

        private double currentHue = 0;   // 0 - 360
        private double currentSat = 0;   // 0 - 1
        private double currentVal = 1;   // 0 - 1
        private Label lblTitle;
        private Label lblHex;
        private Label lblR;
        private Label lblG;
        private Label lblB;
        private RoundedButton btnOK;
        private RoundedButton btnCancel;
        private bool isUpdating = false;

        public CustomColorDialog(Color initialColor)
        {
            SelectedColor = initialColor;
            InitializeComponent();
            UpdateColorUI(updateSV: true, updateHue: true);
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();
            panelSV = new DoubleBufferedPanel();
            panelHue = new DoubleBufferedPanel();
            previewPanel = new Panel();
            lblHex = new Label();
            txtHex = new TextBox();
            lblR = new Label();
            sliderR = new RoundedTrackBar();
            lblRVal = new Label();
            lblG = new Label();
            sliderG = new RoundedTrackBar();
            lblGVal = new Label();
            lblB = new Label();
            sliderB = new RoundedTrackBar();
            lblBVal = new Label();
            btnOK = new RoundedButton();
            btnCancel = new RoundedButton();
            ((System.ComponentModel.ISupportInitialize)sliderR).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sliderG).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sliderB).BeginInit();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(106, 227, 249);
            lblTitle.Location = new Point(20, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(420, 25);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "SELECT CUSTOM COLOR";
            // 
            // panelSV
            // 
            panelSV.Cursor = Cursors.Cross;
            panelSV.Location = new Point(20, 50);
            panelSV.Name = "panelSV";
            panelSV.Size = new Size(180, 180);
            panelSV.TabIndex = 1;
            panelSV.Paint += PanelSV_Paint;
            panelSV.MouseDown += PanelSV_MouseDown;
            panelSV.MouseMove += PanelSV_MouseMove;
            // 
            // panelHue
            // 
            panelHue.Cursor = Cursors.Hand;
            panelHue.Location = new Point(215, 50);
            panelHue.Name = "panelHue";
            panelHue.Size = new Size(25, 180);
            panelHue.TabIndex = 2;
            panelHue.Paint += PanelHue_Paint;
            panelHue.MouseDown += PanelHue_MouseDown;
            panelHue.MouseMove += PanelHue_MouseMove;
            // 
            // previewPanel
            // 
            previewPanel.BackColor = Color.White;
            previewPanel.Location = new Point(260, 50);
            previewPanel.Name = "previewPanel";
            previewPanel.Size = new Size(120, 120);
            previewPanel.TabIndex = 3;
            previewPanel.Paint += PreviewPanel_Paint;
            // 
            // lblHex
            // 
            lblHex.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblHex.ForeColor = Color.FromArgb(106, 227, 249);
            lblHex.Location = new Point(260, 181);
            lblHex.Name = "lblHex";
            lblHex.Size = new Size(80, 15);
            lblHex.TabIndex = 4;
            lblHex.Text = "HEX VALUE";
            // 
            // txtHex
            // 
            txtHex.BackColor = Color.FromArgb(7, 36, 43);
            txtHex.BorderStyle = BorderStyle.FixedSingle;
            txtHex.Font = new Font("Segoe UI", 9.5F);
            txtHex.ForeColor = Color.White;
            txtHex.Location = new Point(260, 201);
            txtHex.Name = "txtHex";
            txtHex.Size = new Size(120, 29);
            txtHex.TabIndex = 5;
            txtHex.TextAlign = HorizontalAlignment.Center;
            txtHex.TextChanged += TxtHex_TextChanged;
            // 
            // lblR
            // 
            lblR.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblR.ForeColor = Color.FromArgb(106, 227, 249);
            lblR.Location = new Point(20, 252);
            lblR.Name = "lblR";
            lblR.Size = new Size(20, 25);
            lblR.TabIndex = 6;
            lblR.Text = "R";
            lblR.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // sliderR
            // 
            sliderR.BackColor = Color.FromArgb(10, 25, 30);
            sliderR.BorderColor = Color.FromArgb(10, 25, 30);
            sliderR.BorderRadius = 4;
            sliderR.LabelFont = new Font("Segoe UI", 9F);
            sliderR.LabelOffset = new Point(0, 0);
            sliderR.LabelText = "";
            sliderR.LabelTextColor = Color.Black;
            sliderR.Location = new Point(45, 236);
            sliderR.Maximum = 255;
            sliderR.Name = "sliderR";
            sliderR.Size = new Size(284, 56);
            sliderR.TabIndex = 7;
            sliderR.ThumbColor = Color.FromArgb(106, 227, 249);
            sliderR.ThumbSize = 14;
            sliderR.TrackColor = Color.FromArgb(7, 36, 43);
            sliderR.Value = 255;
            sliderR.Scroll += Slider_Scroll;
            // 
            // lblRVal
            // 
            lblRVal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRVal.ForeColor = Color.White;
            lblRVal.Location = new Point(335, 252);
            lblRVal.Name = "lblRVal";
            lblRVal.Size = new Size(45, 25);
            lblRVal.TabIndex = 8;
            lblRVal.Text = "255";
            lblRVal.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblG
            // 
            lblG.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblG.ForeColor = Color.FromArgb(106, 227, 249);
            lblG.Location = new Point(20, 287);
            lblG.Name = "lblG";
            lblG.Size = new Size(20, 25);
            lblG.TabIndex = 9;
            lblG.Text = "G";
            lblG.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // sliderG
            // 
            sliderG.BackColor = Color.FromArgb(10, 25, 30);
            sliderG.BorderColor = Color.FromArgb(10, 25, 30);
            sliderG.BorderRadius = 4;
            sliderG.LabelFont = new Font("Segoe UI", 9F);
            sliderG.LabelOffset = new Point(0, 0);
            sliderG.LabelText = "";
            sliderG.LabelTextColor = Color.Black;
            sliderG.Location = new Point(45, 271);
            sliderG.Maximum = 255;
            sliderG.Name = "sliderG";
            sliderG.Size = new Size(284, 56);
            sliderG.TabIndex = 10;
            sliderG.ThumbColor = Color.FromArgb(106, 227, 249);
            sliderG.ThumbSize = 14;
            sliderG.TrackColor = Color.FromArgb(7, 36, 43);
            sliderG.Value = 255;
            sliderG.Scroll += Slider_Scroll;
            // 
            // lblGVal
            // 
            lblGVal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblGVal.ForeColor = Color.White;
            lblGVal.Location = new Point(335, 287);
            lblGVal.Name = "lblGVal";
            lblGVal.Size = new Size(45, 25);
            lblGVal.TabIndex = 11;
            lblGVal.Text = "255";
            lblGVal.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblB
            // 
            lblB.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblB.ForeColor = Color.FromArgb(106, 227, 249);
            lblB.Location = new Point(20, 322);
            lblB.Name = "lblB";
            lblB.Size = new Size(20, 25);
            lblB.TabIndex = 12;
            lblB.Text = "B";
            lblB.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // sliderB
            // 
            sliderB.BackColor = Color.FromArgb(10, 25, 30);
            sliderB.BorderColor = Color.FromArgb(10, 25, 30);
            sliderB.BorderRadius = 4;
            sliderB.LabelFont = new Font("Segoe UI", 9F);
            sliderB.LabelOffset = new Point(0, 0);
            sliderB.LabelText = "";
            sliderB.LabelTextColor = Color.Black;
            sliderB.Location = new Point(45, 306);
            sliderB.Maximum = 255;
            sliderB.Name = "sliderB";
            sliderB.Size = new Size(284, 56);
            sliderB.TabIndex = 13;
            sliderB.ThumbColor = Color.FromArgb(106, 227, 249);
            sliderB.ThumbSize = 14;
            sliderB.TrackColor = Color.FromArgb(7, 36, 43);
            sliderB.Value = 255;
            sliderB.Scroll += Slider_Scroll;
            // 
            // lblBVal
            // 
            lblBVal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblBVal.ForeColor = Color.White;
            lblBVal.Location = new Point(335, 322);
            lblBVal.Name = "lblBVal";
            lblBVal.Size = new Size(45, 25);
            lblBVal.TabIndex = 14;
            lblBVal.Text = "255";
            lblBVal.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnOK
            // 
            btnOK.BackColor = Color.FromArgb(7, 36, 43);
            btnOK.BackgroundColor = Color.FromArgb(7, 36, 43);
            btnOK.BorderColor = Color.FromArgb(106, 227, 249);
            btnOK.BorderRadius = 12;
            btnOK.BorderSize = 2;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Font = new Font("Segoe UI Black", 9.5F, FontStyle.Bold);
            btnOK.ForeColor = Color.FromArgb(106, 227, 249);
            btnOK.Location = new Point(20, 350);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(100, 35);
            btnOK.SvgAlignment = ContentAlignment.MiddleCenter;
            btnOK.SvgColor = Color.Black;
            btnOK.SvgContent = null;
            btnOK.SvgOffset = new Point(0, 0);
            btnOK.SvgPadding = new Padding(0);
            btnOK.SvgResource = null;
            btnOK.SvgSize = new Size(50, 50);
            btnOK.TabIndex = 15;
            btnOK.Text = "OK";
            btnOK.TextColor = Color.FromArgb(106, 227, 249);
            btnOK.UseVisualStyleBackColor = false;
            btnOK.Click += BtnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(20, 15, 15);
            btnCancel.BackgroundColor = Color.FromArgb(20, 15, 15);
            btnCancel.BorderColor = Color.FromArgb(80, 20, 20);
            btnCancel.BorderRadius = 12;
            btnCancel.BorderSize = 2;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI Black", 9.5F, FontStyle.Bold);
            btnCancel.ForeColor = Color.FromArgb(255, 128, 128);
            btnCancel.Location = new Point(280, 350);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 35);
            btnCancel.SvgAlignment = ContentAlignment.MiddleCenter;
            btnCancel.SvgColor = Color.Black;
            btnCancel.SvgContent = null;
            btnCancel.SvgOffset = new Point(0, 0);
            btnCancel.SvgPadding = new Padding(0);
            btnCancel.SvgResource = null;
            btnCancel.SvgSize = new Size(50, 50);
            btnCancel.TabIndex = 16;
            btnCancel.Text = "Cancel";
            btnCancel.TextColor = Color.FromArgb(255, 128, 128);
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += BtnCancel_Click;
            // 
            // CustomColorDialog
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(10, 25, 30);
            ClientSize = new Size(403, 406);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Controls.Add(lblTitle);
            Controls.Add(panelSV);
            Controls.Add(panelHue);
            Controls.Add(previewPanel);
            Controls.Add(lblHex);
            Controls.Add(txtHex);
            Controls.Add(lblR);
            Controls.Add(sliderR);
            Controls.Add(lblRVal);
            Controls.Add(lblG);
            Controls.Add(sliderG);
            Controls.Add(lblGVal);
            Controls.Add(lblB);
            Controls.Add(sliderB);
            Controls.Add(lblBVal);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CustomColorDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select Custom Color";
            ((System.ComponentModel.ISupportInitialize)sliderR).EndInit();
            ((System.ComponentModel.ISupportInitialize)sliderG).EndInit();
            ((System.ComponentModel.ISupportInitialize)sliderB).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void PreviewPanel_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(106, 227, 249), 2))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, previewPanel.Width - 1, previewPanel.Height - 1);
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // SV Square painting & mouse logic
        private void PanelSV_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = panelSV.ClientRectangle;

            // 1. Draw horizontal gradient (White to Hue Color)
            Color hueColor = ColorFromHSV(currentHue, 1.0, 1.0);
            using (LinearGradientBrush brushH = new LinearGradientBrush(rect, Color.White, hueColor, 0f))
            {
                g.FillRectangle(brushH, rect);
            }

            // 2. Draw vertical gradient (Transparent to Black)
            using (LinearGradientBrush brushV = new LinearGradientBrush(rect, Color.Transparent, Color.Black, 90f))
            {
                g.FillRectangle(brushV, rect);
            }

            // 3. Draw crosshair circle at current Saturation and Value
            int x = (int)(currentSat * rect.Width);
            int y = (int)((1.0 - currentVal) * rect.Height);

            // Clamp drawing position to keep the indicator circle fully inside the panel boundaries
            x = Math.Clamp(x, 5, rect.Width - 5);
            y = Math.Clamp(y, 5, rect.Height - 5);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen penBlack = new Pen(Color.Black, 2))
            {
                g.DrawEllipse(penBlack, x - 5, y - 5, 10, 10);
            }
            using (Pen penWhite = new Pen(Color.White, 1))
            {
                g.DrawEllipse(penWhite, x - 4, y - 4, 8, 8);
            }
        }

        private void PanelSV_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                UpdateSVFromMouse(e.X, e.Y);
            }
        }

        private void PanelSV_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                UpdateSVFromMouse(e.X, e.Y);
            }
        }

        private void UpdateSVFromMouse(int mouseX, int mouseY)
        {
            double s = (double)mouseX / panelSV.Width;
            double v = 1.0 - ((double)mouseY / panelSV.Height);

            currentSat = Math.Clamp(s, 0.0, 1.0);
            currentVal = Math.Clamp(v, 0.0, 1.0);

            SelectedColor = ColorFromHSV(currentHue, currentSat, currentVal);
            UpdateColorUI(updateSV: false, updateHue: false);
            panelSV.Invalidate();
        }

        // Hue Strip painting & mouse logic
        private void PanelHue_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = panelHue.ClientRectangle;

            using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.Red, Color.Red, 90f))
            {
                ColorBlend blend = new ColorBlend();
                blend.Colors = new Color[] {
                    Color.Red,
                    Color.FromArgb(255, 0, 255), // Magenta
                    Color.Blue,
                    Color.FromArgb(0, 255, 255), // Cyan
                    Color.Green,
                    Color.FromArgb(255, 255, 0), // Yellow
                    Color.Red
                };
                blend.Positions = new float[] { 0.0f, 0.16f, 0.33f, 0.5f, 0.66f, 0.83f, 1.0f };
                brush.InterpolationColors = blend;
                g.FillRectangle(brush, rect);
            }

            int y = (int)((currentHue / 360.0) * rect.Height);
            y = Math.Clamp(y, 1, rect.Height - 2);

            using (Pen pen = new Pen(Color.White, 2))
            {
                g.DrawLine(pen, 0, y, rect.Width, y);
            }
            using (Pen penBorder = new Pen(Color.Black, 1))
            {
                g.DrawLine(penBorder, 0, y - 1, rect.Width, y - 1);
                g.DrawLine(penBorder, 0, y + 1, rect.Width, y + 1);
            }
        }

        private void PanelHue_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                UpdateHueFromMouse(e.Y);
            }
        }

        private void PanelHue_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                UpdateHueFromMouse(e.Y);
            }
        }

        private void UpdateHueFromMouse(int mouseY)
        {
            double h = ((double)mouseY / panelHue.Height) * 360.0;
            currentHue = Math.Clamp(h, 0.0, 360.0);

            SelectedColor = ColorFromHSV(currentHue, currentSat, currentVal);
            UpdateColorUI(updateSV: false, updateHue: false);
            panelHue.Invalidate();
            panelSV.Invalidate();
        }

        // RGB Sliders & HEX text interaction
        private void Slider_Scroll(object sender, EventArgs e)
        {
            if (isUpdating) return;
            isUpdating = true;
            try
            {
                SelectedColor = Color.FromArgb(sliderR.Value, sliderG.Value, sliderB.Value);
                lblRVal.Text = sliderR.Value.ToString();
                lblGVal.Text = sliderG.Value.ToString();
                lblBVal.Text = sliderB.Value.ToString();
                txtHex.Text = $"#{SelectedColor.R:X2}{SelectedColor.G:X2}{SelectedColor.B:X2}";
                previewPanel.BackColor = SelectedColor;

                ColorToHSV(SelectedColor, out currentHue, out currentSat, out currentVal);
                panelHue.Invalidate();
                panelSV.Invalidate();
            }
            finally
            {
                isUpdating = false;
            }
        }

        private void TxtHex_TextChanged(object sender, EventArgs e)
        {
            if (isUpdating) return;
            string hex = txtHex.Text.TrimStart('#');
            if (hex.Length == 6)
            {
                try
                {
                    int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                    int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                    int b = Convert.ToInt32(hex.Substring(4, 2), 16);

                    isUpdating = true;
                    try
                    {
                        SelectedColor = Color.FromArgb(r, g, b);
                        sliderR.Value = r;
                        sliderG.Value = g;
                        sliderB.Value = b;
                        lblRVal.Text = r.ToString();
                        lblGVal.Text = g.ToString();
                        lblBVal.Text = b.ToString();
                        previewPanel.BackColor = SelectedColor;

                        ColorToHSV(SelectedColor, out currentHue, out currentSat, out currentVal);
                        panelHue.Invalidate();
                        panelSV.Invalidate();
                    }
                    finally
                    {
                        isUpdating = false;
                    }
                }
                catch { }
            }
        }

        private void UpdateColorUI(bool updateSV = true, bool updateHue = true)
        {
            isUpdating = true;
            try
            {
                sliderR.Value = SelectedColor.R;
                sliderG.Value = SelectedColor.G;
                sliderB.Value = SelectedColor.B;
                lblRVal.Text = SelectedColor.R.ToString();
                lblGVal.Text = SelectedColor.G.ToString();
                lblBVal.Text = SelectedColor.B.ToString();
                txtHex.Text = $"#{SelectedColor.R:X2}{SelectedColor.G:X2}{SelectedColor.B:X2}";
                previewPanel.BackColor = SelectedColor;

                if (updateSV || updateHue)
                {
                    ColorToHSV(SelectedColor, out double h, out double s, out double v);
                    if (updateHue) currentHue = h;
                    if (updateSV)
                    {
                        currentSat = s;
                        currentVal = v;
                    }
                    panelHue.Invalidate();
                    panelSV.Invalidate();
                }
            }
            finally
            {
                isUpdating = false;
            }
        }

        // HSV <=> RGB Math
        private static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        private static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }
    }
}
