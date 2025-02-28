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
            streamPicFlow = new RoundedCheckBox();
            _clearButton = new RoundedButton();
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
            limitCounterLabel.Location = new Point(442, 17);
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
            logLabel.Location = new Point(145, 14);
            logLabel.Name = "logLabel";
            logLabel.Size = new Size(192, 15);
            logLabel.TabIndex = 7;
            logLabel.Text = "$\"Found {logFiles.Length} log files\"";
            // 
            // streamPicFlow
            // 
            streamPicFlow.AutoCheck = false;
            streamPicFlow.AutoSize = true;
            streamPicFlow.BackColor = Color.Black;
            streamPicFlow.BorderColor = Color.FromArgb(5, 55, 66);
            streamPicFlow.BorderRadius = 5;
            streamPicFlow.BorderSize = 2;
            streamPicFlow.BoxFillColor = Color.FromArgb(7, 36, 43);
            streamPicFlow.CheckColor = Color.FromArgb(106, 227, 249);
            streamPicFlow.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            streamPicFlow.ForeColor = Color.White;
            streamPicFlow.Location = new Point(12, 13);
            streamPicFlow.Name = "streamPicFlow";
            streamPicFlow.Size = new Size(117, 19);
            streamPicFlow.TabIndex = 8;
            streamPicFlow.Text = "Stream PicFlow";
            streamPicFlow.UseVisualStyleBackColor = false;
            streamPicFlow.CheckedChanged += streamPicFlow_CheckedChanged;
            // 
            // _clearButton
            // 
            _clearButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _clearButton.BackColor = Color.FromArgb(7, 36, 43);
            _clearButton.BackgroundColor = Color.FromArgb(7, 36, 43);
            _clearButton.BorderColor = Color.FromArgb(5, 55, 66);
            _clearButton.BorderRadius = 10;
            _clearButton.BorderSize = 2;
            _clearButton.FlatAppearance.BorderSize = 0;
            _clearButton.FlatStyle = FlatStyle.Flat;
            _clearButton.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            _clearButton.ForeColor = Color.FromArgb(106, 227, 249);
            _clearButton.Location = new Point(587, 10);
            _clearButton.Name = "_clearButton";
            _clearButton.Size = new Size(81, 29);
            _clearButton.SvgAlignment = ContentAlignment.MiddleCenter;
            _clearButton.SvgColor = Color.Black;
            _clearButton.SvgContent = null;
            _clearButton.SvgOffset = new Point(0, 0);
            _clearButton.SvgPadding = new Padding(0);
            _clearButton.SvgResource = null;
            _clearButton.SvgSize = new Size(50, 50);
            _clearButton.TabIndex = 9;
            _clearButton.Text = "Clear";
            _clearButton.TextColor = Color.FromArgb(106, 227, 249);
            _clearButton.UseVisualStyleBackColor = false;
            _clearButton.Click += _clearButton_Click;
            // 
            // Picflow
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(5, 5, 5);
            ClientSize = new Size(802, 549);
            Controls.Add(_clearButton);
            Controls.Add(streamPicFlow);
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
        private RoundedCheckBox streamPicFlow;
        private RoundedButton _clearButton;
    }
}