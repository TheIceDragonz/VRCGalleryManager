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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Prints));
            printsPanel = new FlowLayoutPanel();
            _refreshButton = new RoundedButton();
            uploadButton = new RoundedButton();
            limitPanel = new RoundedPanel();
            limitLabel = new Label();
            limitCounterLabel = new Label();
            pasteButton = new RoundedButton();
            notePrintsLabel = new Label();
            textBoxNotePrint = new TextBox();
            limitPanel.SuspendLayout();
            SuspendLayout();
            // 
            // printsPanel
            // 
            printsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            printsPanel.AutoScroll = true;
            printsPanel.BackColor = Color.FromArgb(5, 5, 5);
            printsPanel.Location = new Point(12, 45);
            printsPanel.Name = "printsPanel";
            printsPanel.Size = new Size(772, 462);
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
            uploadButton.Location = new Point(12, 535);
            uploadButton.Name = "uploadButton";
            uploadButton.Size = new Size(726, 40);
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
            uploadButton.UseCompatibleTextRendering = true;
            uploadButton.UseVisualStyleBackColor = false;
            uploadButton.Click += uploadPrints_Click;
            // 
            // limitPanel
            // 
            limitPanel.BackColor = Color.FromArgb(7, 36, 43);
            limitPanel.BackgroundColor = Color.FromArgb(7, 36, 43);
            limitPanel.BorderColor = Color.FromArgb(255, 128, 128);
            limitPanel.BorderRadius = 10;
            limitPanel.BorderSize = 2;
            limitPanel.Controls.Add(limitLabel);
            limitPanel.Location = new Point(12, 10);
            limitPanel.Name = "limitPanel";
            limitPanel.Size = new Size(284, 29);
            limitPanel.TabIndex = 9;
            limitPanel.Visible = false;
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
            // limitCounterLabel
            // 
            limitCounterLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            limitCounterLabel.AutoSize = true;
            limitCounterLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            limitCounterLabel.ForeColor = Color.White;
            limitCounterLabel.Location = new Point(594, 17);
            limitCounterLabel.Name = "limitCounterLabel";
            limitCounterLabel.Size = new Size(68, 15);
            limitCounterLabel.TabIndex = 7;
            limitCounterLabel.Text = "0/64 Prints";
            limitCounterLabel.TextAlign = ContentAlignment.MiddleRight;
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
            pasteButton.Location = new Point(744, 535);
            pasteButton.Name = "pasteButton";
            pasteButton.Size = new Size(40, 40);
            pasteButton.SvgAlignment = ContentAlignment.MiddleCenter;
            pasteButton.SvgColor = Color.FromArgb(106, 227, 249);
            pasteButton.SvgContent = resources.GetString("pasteButton.SvgContent");
            pasteButton.SvgOffset = new Point(0, 0);
            pasteButton.SvgPadding = new Padding(0);
            pasteButton.SvgResource = "backward_svgrepo_com";
            pasteButton.SvgSize = new Size(25, 25);
            pasteButton.TabIndex = 10;
            pasteButton.TextColor = Color.FromArgb(106, 227, 249);
            pasteButton.UseVisualStyleBackColor = false;
            pasteButton.Click += pasteButton_Click;
            // 
            // notePrintsLabel
            // 
            notePrintsLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            notePrintsLabel.AutoSize = true;
            notePrintsLabel.BackColor = Color.FromArgb(5, 5, 5);
            notePrintsLabel.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            notePrintsLabel.ForeColor = Color.White;
            notePrintsLabel.Location = new Point(12, 513);
            notePrintsLabel.Name = "notePrintsLabel";
            notePrintsLabel.Size = new Size(41, 15);
            notePrintsLabel.TabIndex = 11;
            notePrintsLabel.Text = "Note:";
            // 
            // textBoxNotePrint
            // 
            textBoxNotePrint.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxNotePrint.BackColor = Color.FromArgb(7, 36, 43);
            textBoxNotePrint.BorderStyle = BorderStyle.None;
            textBoxNotePrint.ForeColor = Color.White;
            textBoxNotePrint.Location = new Point(59, 513);
            textBoxNotePrint.Name = "textBoxNotePrint";
            textBoxNotePrint.Size = new Size(725, 16);
            textBoxNotePrint.TabIndex = 12;
            // 
            // Prints
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(5, 5, 5);
            ClientSize = new Size(796, 587);
            Controls.Add(textBoxNotePrint);
            Controls.Add(notePrintsLabel);
            Controls.Add(pasteButton);
            Controls.Add(limitCounterLabel);
            Controls.Add(limitPanel);
            Controls.Add(uploadButton);
            Controls.Add(_refreshButton);
            Controls.Add(printsPanel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Prints";
            Text = "Prints";
            DragDrop += File_DragDrop;
            DragEnter += File_DragEnter;
            limitPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private FlowLayoutPanel printsPanel;
        private RoundedButton _refreshButton;
        private RoundedButton uploadButton;
        private RoundedPanel limitPanel;
        private Label limitLabel;
        private Label limitCounterLabel;
        private RoundedButton pasteButton;
        private Label notePrintsLabel;
        private TextBox textBoxNotePrint;
    }
}