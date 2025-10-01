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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Picflow));
            picflowPanel = new FlowLayoutPanel();
            infoPicflowButton = new CircularButton();
            _refreshButton = new RoundedButton();
            limitCounterLabel = new Label();
            logLabel = new Label();
            streamPicFlow = new RoundedCheckBox();
            _clearButton = new RoundedButton();
            infoPicFlowLabel = new Label();
            SuspendLayout();
            // 
            // picflowPanel
            // 
            picflowPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picflowPanel.AutoScroll = true;
            picflowPanel.BackColor = Color.FromArgb(5, 5, 5);
            picflowPanel.Location = new Point(15, 56);
            picflowPanel.Margin = new Padding(4);
            picflowPanel.Name = "picflowPanel";
            picflowPanel.Size = new Size(972, 615);
            picflowPanel.TabIndex = 2;
            // 
            // infoPicflowButton
            // 
            infoPicflowButton.BackColor = Color.FromArgb(5, 5, 5);
            infoPicflowButton.BackgroundColor = Color.FromArgb(5, 5, 5);
            infoPicflowButton.BorderColor = Color.PaleVioletRed;
            infoPicflowButton.BorderSize = 0;
            infoPicflowButton.FlatAppearance.BorderSize = 0;
            infoPicflowButton.FlatStyle = FlatStyle.Flat;
            infoPicflowButton.ForeColor = Color.White;
            infoPicflowButton.Location = new Point(15, 11);
            infoPicflowButton.Name = "infoPicflowButton";
            infoPicflowButton.Size = new Size(36, 36);
            infoPicflowButton.SvgAlignment = ContentAlignment.MiddleCenter;
            infoPicflowButton.SvgColor = Color.FromArgb(106, 227, 249);
            infoPicflowButton.SvgContent = resources.GetString("infoPicflowButton.SvgContent");
            infoPicflowButton.SvgOffset = new Point(0, 0);
            infoPicflowButton.SvgPadding = new Padding(0);
            infoPicflowButton.SvgResource = "info_svgrepo_com";
            infoPicflowButton.SvgSize = new Size(28, 28);
            infoPicflowButton.TabIndex = 0;
            infoPicflowButton.TextColor = Color.White;
            infoPicflowButton.UseVisualStyleBackColor = false;
            infoPicflowButton.Click += infoPicflowButton_Click;
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
            _refreshButton.Location = new Point(842, 12);
            _refreshButton.Margin = new Padding(4);
            _refreshButton.Name = "_refreshButton";
            _refreshButton.Size = new Size(145, 36);
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
            limitCounterLabel.Location = new Point(552, 21);
            limitCounterLabel.Margin = new Padding(4, 0, 4, 0);
            limitCounterLabel.Name = "limitCounterLabel";
            limitCounterLabel.Size = new Size(174, 19);
            limitCounterLabel.TabIndex = 6;
            limitCounterLabel.Text = "0";
            limitCounterLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // logLabel
            // 
            logLabel.AutoSize = true;
            logLabel.ForeColor = Color.FromArgb(106, 227, 249);
            logLabel.Location = new Point(224, 18);
            logLabel.Margin = new Padding(4, 0, 4, 0);
            logLabel.Name = "logLabel";
            logLabel.Size = new Size(240, 20);
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
            streamPicFlow.Location = new Point(58, 16);
            streamPicFlow.Margin = new Padding(4);
            streamPicFlow.Name = "streamPicFlow";
            streamPicFlow.Size = new Size(145, 24);
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
            _clearButton.Location = new Point(734, 12);
            _clearButton.Margin = new Padding(4);
            _clearButton.Name = "_clearButton";
            _clearButton.Size = new Size(101, 36);
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
            // infoPicFlowLabel
            // 
            infoPicFlowLabel.Anchor = AnchorStyles.None;
            infoPicFlowLabel.AutoSize = true;
            infoPicFlowLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            infoPicFlowLabel.ForeColor = Color.FromArgb(106, 227, 249);
            infoPicFlowLabel.Location = new Point(280, 329);
            infoPicFlowLabel.Name = "infoPicFlowLabel";
            infoPicFlowLabel.Size = new Size(438, 46);
            infoPicFlowLabel.TabIndex = 10;
            infoPicFlowLabel.Text = "Requires \"--enable-sdk-log-levels\" VRC launch option\r\n~ Click to copy ~";
            infoPicFlowLabel.TextAlign = ContentAlignment.MiddleCenter;
            infoPicFlowLabel.Visible = false;
            infoPicFlowLabel.Click += infoPicFlowLabel_Click;
            // 
            // Picflow
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(5, 5, 5);
            ClientSize = new Size(1002, 686);
            Controls.Add(infoPicFlowLabel);
            Controls.Add(infoPicflowButton);
            Controls.Add(_clearButton);
            Controls.Add(streamPicFlow);
            Controls.Add(logLabel);
            Controls.Add(limitCounterLabel);
            Controls.Add(_refreshButton);
            Controls.Add(picflowPanel);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4);
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
        private CircularButton infoPicflowButton;
        private Label infoPicFlowLabel;
    }
}