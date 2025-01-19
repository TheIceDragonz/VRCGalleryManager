using VRCGalleryManager.Design;

namespace VRCGalleryManager.Forms
{
    partial class Prints
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
            printsPanel = new FlowLayoutPanel();
            _refreshButton = new RoundedButton();
            roundedButton1 = new RoundedButton();
            limitPanelPrints = new RoundedPanel();
            limitLabel = new Label();
            limitPrintsLabel = new Label();
            limitPanelPrints.SuspendLayout();
            SuspendLayout();
            // 
            // printsPanel
            // 
            printsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            printsPanel.AutoScroll = true;
            printsPanel.BackColor = Color.FromArgb(5, 5, 5);
            printsPanel.Location = new Point(12, 45);
            printsPanel.Name = "printsPanel";
            printsPanel.Size = new Size(772, 484);
            printsPanel.TabIndex = 2;
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
            _refreshButton.Location = new Point(668, 10);
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
            // roundedButton1
            // 
            roundedButton1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            roundedButton1.BackColor = Color.FromArgb(7, 36, 43);
            roundedButton1.BackgroundColor = Color.FromArgb(7, 36, 43);
            roundedButton1.BorderColor = Color.FromArgb(5, 55, 66);
            roundedButton1.BorderRadius = 10;
            roundedButton1.BorderSize = 2;
            roundedButton1.Enabled = false;
            roundedButton1.FlatAppearance.BorderSize = 0;
            roundedButton1.FlatStyle = FlatStyle.Flat;
            roundedButton1.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold);
            roundedButton1.ForeColor = Color.FromArgb(106, 227, 249);
            roundedButton1.Location = new Point(12, 535);
            roundedButton1.Name = "roundedButton1";
            roundedButton1.Size = new Size(772, 40);
            roundedButton1.SvgAlignment = ContentAlignment.MiddleCenter;
            roundedButton1.SvgColor = Color.Black;
            roundedButton1.SvgContent = null;
            roundedButton1.SvgOffset = new Point(0, 0);
            roundedButton1.SvgPadding = new Padding(0);
            roundedButton1.SvgResource = null;
            roundedButton1.SvgSize = new Size(50, 50);
            roundedButton1.TabIndex = 4;
            roundedButton1.Text = "Upload";
            roundedButton1.TextColor = Color.FromArgb(106, 227, 249);
            roundedButton1.UseCompatibleTextRendering = true;
            roundedButton1.UseVisualStyleBackColor = false;
            roundedButton1.Click += uploadPrints_Click;
            // 
            // limitPanelPrints
            // 
            limitPanelPrints.BackColor = Color.FromArgb(7, 36, 43);
            limitPanelPrints.BackgroundColor = Color.FromArgb(7, 36, 43);
            limitPanelPrints.BorderColor = Color.FromArgb(255, 128, 128);
            limitPanelPrints.BorderRadius = 10;
            limitPanelPrints.BorderSize = 2;
            limitPanelPrints.Controls.Add(limitLabel);
            limitPanelPrints.Location = new Point(12, 10);
            limitPanelPrints.Name = "limitPanelPrints";
            limitPanelPrints.Size = new Size(284, 29);
            limitPanelPrints.TabIndex = 9;
            limitPanelPrints.Visible = false;
            // 
            // limitLabel
            // 
            limitLabel.BackColor = Color.Transparent;
            limitLabel.Dock = DockStyle.Fill;
            limitLabel.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            limitLabel.ForeColor = Color.FromArgb(255, 128, 128);
            limitLabel.Location = new Point(0, 0);
            limitLabel.Name = "limitLabel";
            limitLabel.Size = new Size(284, 29);
            limitLabel.TabIndex = 0;
            limitLabel.Text = "You have reached your prints limit!";
            limitLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // limitPrintsLabel
            // 
            limitPrintsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            limitPrintsLabel.AutoSize = true;
            limitPrintsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            limitPrintsLabel.ForeColor = Color.White;
            limitPrintsLabel.Location = new Point(594, 17);
            limitPrintsLabel.Name = "limitPrintsLabel";
            limitPrintsLabel.Size = new Size(68, 15);
            limitPrintsLabel.TabIndex = 7;
            limitPrintsLabel.Text = "0/64 Prints";
            limitPrintsLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Prints
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(5, 5, 5);
            ClientSize = new Size(796, 587);
            Controls.Add(limitPrintsLabel);
            Controls.Add(limitPanelPrints);
            Controls.Add(roundedButton1);
            Controls.Add(_refreshButton);
            Controls.Add(printsPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Prints";
            Text = "Prints";
            limitPanelPrints.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private FlowLayoutPanel printsPanel;
        private RoundedButton _refreshButton;
        private RoundedButton roundedButton1;
        private RoundedPanel limitPanelPrints;
        private Label limitLabel;
        private Label limitPrintsLabel;
    }
}