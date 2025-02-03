using VRCGalleryManager.Design;

namespace VRCGalleryManager.Forms
{
    partial class Picflow
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
            picflowPanel = new FlowLayoutPanel();
            _refreshButton = new RoundedButton();
            limitCounterLabel = new Label();
            logLabel = new Label();
            SuspendLayout();
            // 
            // picflowPanel
            // 
            picflowPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picflowPanel.AutoScroll = true;
            picflowPanel.BackColor = Color.FromArgb(5, 5, 5);
            picflowPanel.Location = new Point(12, 45);
            picflowPanel.Name = "picflowPanel";
            picflowPanel.Size = new Size(778, 492);
            picflowPanel.TabIndex = 2;
            // 
            // _refreshButton
            // 
            _refreshButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _refreshButton.BackColor = Color.FromArgb(7, 36, 43);
            _refreshButton.BackgroundColor = Color.FromArgb(7, 36, 43);
            _refreshButton.BorderColor = Color.FromArgb(5, 55, 66);
            _refreshButton.BorderRadius = 10;
            _refreshButton.BorderSize = 2;
            _refreshButton.FlatAppearance.BorderSize = 0;
            _refreshButton.FlatStyle = FlatStyle.Flat;
            _refreshButton.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _refreshButton.ForeColor = Color.FromArgb(106, 227, 249);
            _refreshButton.Location = new Point(674, 10);
            _refreshButton.Name = "_refreshButton";
            _refreshButton.Size = new Size(116, 29);
            _refreshButton.SvgAlignment = ContentAlignment.MiddleCenter;
            _refreshButton.SvgColor = Color.Black;
            _refreshButton.SvgContent = null;
            _refreshButton.SvgOffset = new Point(0, 0);
            _refreshButton.SvgPadding = new Padding(0);
            _refreshButton.SvgResource = null;
            _refreshButton.SvgSize = new Size(50, 50);
            _refreshButton.TabIndex = 3;
            _refreshButton.Text = "Refresh";
            _refreshButton.TextColor = Color.FromArgb(106, 227, 249);
            _refreshButton.UseVisualStyleBackColor = false;
            _refreshButton.Click += _refreshButton_Click;
            // 
            // limitCounterLabel
            // 
            limitCounterLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            limitCounterLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            limitCounterLabel.ForeColor = Color.White;
            limitCounterLabel.Location = new Point(529, 18);
            limitCounterLabel.Name = "limitCounterLabel";
            limitCounterLabel.Size = new Size(139, 15);
            limitCounterLabel.TabIndex = 6;
            limitCounterLabel.Text = "0";
            limitCounterLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // logLabel
            // 
            logLabel.AutoSize = true;
            logLabel.ForeColor = Color.FromArgb(106, 227, 249);
            logLabel.Location = new Point(12, 17);
            logLabel.Name = "logLabel";
            logLabel.Size = new Size(192, 15);
            logLabel.TabIndex = 7;
            logLabel.Text = "$\"Found {logFiles.Length} log files\"";
            // 
            // Picflow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(5, 5, 5);
            ClientSize = new Size(802, 549);
            Controls.Add(logLabel);
            Controls.Add(limitCounterLabel);
            Controls.Add(_refreshButton);
            Controls.Add(picflowPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Picflow";
            Text = "picflow";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private FlowLayoutPanel picflowPanel;
        private RoundedButton _refreshButton;
        private Label limitCounterLabel;
        private Label logLabel;
    }
}