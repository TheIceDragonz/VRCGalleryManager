using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VRCGalleryManager.Core.Helpers;
using VRCGalleryManager.Design;

namespace VRCGalleryManager.Forms
{
    public partial class ImageEditorForm : Form
    {
        private Image originalImage;
        private string originalPath;

        // Configuration State
        private int canvasWidth = 2048;
        private int canvasHeight = 2048;
        private AdaptationMode adaptationMode = AdaptationMode.Fit;
        private float zoomFactor = 1.0f; // Slider maps 10% - 500% to 0.1f - 5.0f
        private float panOffsetX = 0f;
        private float panOffsetY = 0f;
        private int rotationAngle = 0; // 0, 90, 180, 270
        private Color selectedBgColor = Color.Transparent;

        // Background removal state
        private bool removeBgEnabled = false;
        private Color removeBgColor = Color.White;
        private int removeBgTolerance = 15;
        private bool isColorPicking = false;

        // Feathering state
        private bool featherEnabled = false;
        private int featherRadius = 2;
        private int chokeRadius = 1;

        // Outline state
        private bool outlineEnabled = false;
        private Color outlineColor = Color.White;
        private int outlineThickness = 5;

        // Fixed canvas dimensions — set by LoadImage() based on the ratio string
        private int _fixedCanvasWidth = 2048;
        private int _fixedCanvasHeight = 2048;

        // Drag state
        private bool isDragging = false;
        private Point startDragPoint;

        // Brush for transparent area check grid
        private TextureBrush checkerBrush;

        // DWM API for Dark Title bar
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        // Timer for debounced file size calculation
        private System.Windows.Forms.Timer sizeCalcTimer;
        private Bitmap _cachedRender = null;
        private string _lastRenderHash = "";

        // Events for inline use (replaces ShowDialog / DialogResult)
        public event Action<string, string> OnSave;
        public event Action OnCancel;

        public ImageEditorForm()
        {
            InitializeComponent();
            ApplyRecolorBar();
            previewPanel.MouseWheel += previewPanel_MouseWheel;
            ScrollBarHelper.Attach(settingsPanel);

            sizeCalcTimer = new System.Windows.Forms.Timer();
            sizeCalcTimer.Interval = 150;
            sizeCalcTimer.Tick += SizeCalcTimer_Tick;
        }

        /// <summary>
        /// Carica una nuova immagine nell'editor e imposta il ratio di default.
        /// Chiama questo metodo prima di mostrare il pannello.
        /// </summary>
        public void LoadImage(string imagePath, string defaultRatioStr = "1:1", bool showNote = false)
        {
            bool isGif = Path.GetExtension(imagePath).Equals(".gif", StringComparison.OrdinalIgnoreCase);
            if (isGif)
            {
                VRCGalleryManager.Core.NotificationManager.ShowNotification(
                    "Animated GIFs are only supported in the Emoji section.",
                    "Warning",
                    VRCGalleryManager.Core.NotificationType.Info
                );
            }

            // Dispose previous resources
            originalImage?.Dispose();
            checkerBrush?.Dispose();

            originalPath = imagePath;
            try
            {
                originalImage = Image.FromFile(imagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                OnCancel?.Invoke();
                return;
            }

            checkerBrush = CreateCheckerBrush();

            // Reset state
            rotationAngle = 0;
            selectedBgColor = Color.Transparent;

            // Set fixed canvas dimensions based on ratio
            if (defaultRatioStr == "16:9")
            {
                _fixedCanvasWidth = 2048;
                _fixedCanvasHeight = 1152;
            }
            else // 1:1 default
            {
                _fixedCanvasWidth = 2048;
                _fixedCanvasHeight = 2048;
            }

            // Set up adaptation mode selection buttons
            adaptationMode = AdaptationMode.Fit;
            UpdateAdaptationButtonsSelection(adaptationMode);

            // Set up background color panel
            panelBgColorColor.BackgroundColor = selectedBgColor;

            // Set up remove background color panel
            panelRemoveBgColorColor.BackgroundColor = removeBgColor;

            chkRemoveBg.Checked = false;
            removeBgEnabled = false;
            removeBgColor = Color.White;
            removeBgTolerance = 15;
            sliderTolerance.Value = 15;
            lblToleranceVal.Text = "15";
            UpdateRemoveBgControlsEnabled();

            // Set up feather controls
            chkFeather.Checked = false;
            featherEnabled = false;
            chokeRadius = 1;
            featherRadius = 2;
            sliderChoke.Value = 1;
            lblChokeVal.Text = "1px";
            sliderFeather.Value = 2;
            lblFeatherVal.Text = "2px";
            UpdateFeatherControlsEnabled();

            // Reset outline state (hidden from UI)
            outlineEnabled = false;
            outlineColor = Color.White;
            outlineThickness = 0;

            // Reset compression slider
            sliderCompression.Value = 100;
            lblCompression.Text = "Quality (Scale): 100%";

            // Set up note textbox
            textBoxNote.Text = "";
            UpdateNoteVisibility(showNote);

            // Trigger initial state layout
            UpdateEditorState();
            DebounceFileSizeCalculation();
        }

        private void UpdateNoteVisibility(bool showNote)
        {
            lblNote.Visible = showNote;
            panelNote.Visible = showNote;
        }

        private void ApplyRecolorBar()
        {
            try
            {
                Color c = Color.FromArgb(15, 17, 19);
                int color = ColorTranslator.ToWin32(c);
                DwmSetWindowAttribute(this.Handle, 35, ref color, sizeof(int));
            }
            catch { }
        }

        private static TextureBrush CreateCheckerBrush()
        {
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(20, 20, 20));
                using (Brush darkBrush = new SolidBrush(Color.FromArgb(32, 32, 32)))
                {
                    g.FillRectangle(darkBrush, 0, 0, 8, 8);
                    g.FillRectangle(darkBrush, 8, 8, 8, 8);
                }
            }
            return new TextureBrush(bmp, WrapMode.Tile);
        }

        private void UpdateEditorState()
        {
            // Reset position when changing canvas dimensions
            ResetOffsets();

            // Re-render
            previewPanel.Invalidate();
        }

        private void ResetOffsets()
        {
            panOffsetX = 0f;
            panOffsetY = 0f;
            zoomFactor = 1.0f;
            sliderZoom.Value = 100;
            lblZoomVal.Text = "100%";
        }

        private void UpdateRemoveBgControlsEnabled()
        {
            bool enabled = chkRemoveBg.Checked;
            lblRemoveBgColor.Enabled = enabled;
            panelRemoveBgColorColor.Enabled = enabled;
            btnPickColor.Enabled = enabled;
            lblTolerance.Enabled = enabled;
            sliderTolerance.Enabled = enabled;
            lblToleranceVal.Enabled = enabled;
        }

        private void UpdateFeatherControlsEnabled()
        {
            bool enabled = chkFeather.Checked;
            lblChoke.Enabled = enabled;
            sliderChoke.Enabled = enabled;
            lblChokeVal.Enabled = enabled;
            lblFeather.Enabled = enabled;
            sliderFeather.Enabled = enabled;
            lblFeatherVal.Enabled = enabled;
        }


        private void GetCurrentCanvasDimensions(out int width, out int height)
        {
            width = _fixedCanvasWidth;
            height = _fixedCanvasHeight;
        }

        private AdaptationMode GetCurrentAdaptationMode()
        {
            return adaptationMode;
        }

        private void ClampOffsets()
        {
            GetCurrentCanvasDimensions(out int cw, out int ch);
            adaptationMode = GetCurrentAdaptationMode();

            float imageW = originalImage.Width;
            float imageH = originalImage.Height;
            if (rotationAngle == 90 || rotationAngle == 270)
            {
                imageW = originalImage.Height;
                imageH = originalImage.Width;
            }

            float baseScale = 1f;
            if (adaptationMode == AdaptationMode.Fit)
            {
                baseScale = Math.Min((float)cw / imageW, (float)ch / imageH);
            }
            else if (adaptationMode == AdaptationMode.Fill)
            {
                baseScale = Math.Max((float)cw / imageW, (float)ch / imageH);
            }
            else if (adaptationMode == AdaptationMode.Center)
            {
                baseScale = 1f;
            }

            float drawnW, drawnH;
            if (adaptationMode == AdaptationMode.Stretch)
            {
                drawnW = cw * zoomFactor;
                drawnH = ch * zoomFactor;
            }
            else
            {
                drawnW = imageW * baseScale * zoomFactor;
                drawnH = imageH * baseScale * zoomFactor;
            }

            // Clamp X
            if (drawnW >= cw)
            {
                float maxOffset = (drawnW - cw) / 2f;
                panOffsetX = Math.Max(-maxOffset, Math.Min(maxOffset, panOffsetX));
            }
            else
            {
                float maxOffset = (cw - drawnW) / 2f;
                panOffsetX = Math.Max(-maxOffset, Math.Min(maxOffset, panOffsetX));
            }

            // Clamp Y
            if (drawnH >= ch)
            {
                float maxOffset = (drawnH - ch) / 2f;
                panOffsetY = Math.Max(-maxOffset, Math.Min(maxOffset, panOffsetY));
            }
            else
            {
                float maxOffset = (ch - drawnH) / 2f;
                panOffsetY = Math.Max(-maxOffset, Math.Min(maxOffset, panOffsetY));
            }
        }

        // Preview rendering
        private void previewPanel_Paint(object sender, PaintEventArgs e)
        {
            if (originalImage == null) return;

            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Get target dimensions
            GetCurrentCanvasDimensions(out canvasWidth, out canvasHeight);
            adaptationMode = GetCurrentAdaptationMode();

            // Get preview coordinates
            int pBoxW = previewPanel.Width;
            int pBoxH = previewPanel.Height;

            // Fit the canvas within the preview control
            float previewScale = Math.Min((float)pBoxW / canvasWidth, (float)pBoxH / canvasHeight) * 0.9f;
            float cx = pBoxW / 2f;
            float cy = pBoxH / 2f;

            float rectW = canvasWidth * previewScale;
            float rectH = canvasHeight * previewScale;
            float rectX = cx - rectW / 2f;
            float rectY = cy - rectH / 2f;

            // 1. Draw Checker background inside the canvas bounds
            g.FillRectangle(checkerBrush, rectX, rectY, rectW, rectH);

            float outScale = sliderCompression.Value / 100f;

            // 2. Render image onto temp preview bitmap using the EXACT final logic
            int bmpW = (int)Math.Max(1, Math.Round(rectW * outScale));
            int bmpH = (int)Math.Max(1, Math.Round(rectH * outScale));

            float scaledPanX = panOffsetX * previewScale * outScale;
            float scaledPanY = panOffsetY * previewScale * outScale;
            
            int scaledOutline = outlineEnabled && outlineThickness > 0 ? (int)Math.Max(1, Math.Round(outlineThickness * previewScale * outScale)) : 0;
            int scaledFeather = featherEnabled && featherRadius > 0 ? (int)Math.Max(1, Math.Round(featherRadius * previewScale * outScale)) : 0;
            int scaledChoke = featherEnabled && chokeRadius > 0 ? (int)Math.Max(1, Math.Round(chokeRadius * previewScale * outScale)) : 0;

            using (Bitmap previewBmp = ImageEditor.RenderImage(
                originalImage,
                bmpW,
                bmpH,
                adaptationMode,
                zoomFactor,
                scaledPanX,
                scaledPanY,
                rotationAngle,
                selectedBgColor,
                removeBgEnabled,
                removeBgColor,
                removeBgTolerance,
                outlineEnabled,
                outlineColor,
                scaledOutline,
                featherEnabled,
                scaledFeather,
                scaledChoke))
            {
                g.DrawImage(previewBmp, rectX, rectY, rectW, rectH);
            }

            // 3. Draw outer semi-transparent mask for preview
            using (GraphicsPath outerPath = new GraphicsPath())
            {
                outerPath.AddRectangle(new Rectangle(0, 0, pBoxW, pBoxH));
                outerPath.AddRectangle(new Rectangle((int)rectX, (int)rectY, (int)rectW, (int)rectH));
                using (Brush maskBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
                {
                    g.FillPath(maskBrush, outerPath);
                }
            }

            // 5. Draw canvas boundary border
            using (Pen borderPen = new Pen(Color.FromArgb(106, 227, 249), 2f))
            {
                g.DrawRectangle(borderPen, rectX, rectY, rectW, rectH);
            }
        }

        // Panning interaction
        private void previewPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (isColorPicking)
            {
                SampleColorFromMouse(e.X, e.Y);
                isColorPicking = false;
                previewPanel.Cursor = Cursors.Default;
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                startDragPoint = e.Location;
                previewPanel.Cursor = Cursors.NoMove2D;
            }
        }

        private void previewPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int pBoxW = previewPanel.Width;
                int pBoxH = previewPanel.Height;
                GetCurrentCanvasDimensions(out int cw, out int ch);

                float previewScale = Math.Min((float)pBoxW / cw, (float)pBoxH / ch) * 0.9f;

                float dx = e.X - startDragPoint.X;
                float dy = e.Y - startDragPoint.Y;

                // Update offset scaled correctly to canvas coordinates
                panOffsetX += dx / previewScale;
                panOffsetY += dy / previewScale;

                ClampOffsets();

                startDragPoint = e.Location;
                previewPanel.Invalidate();
            }
        }

        private void previewPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                previewPanel.Cursor = Cursors.Default;
                DebounceFileSizeCalculation();
            }
        }

        // Zooming interaction
        private void previewPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            int delta = e.Delta;
            int step = (delta > 0) ? 5 : -5;
            int newVal = sliderZoom.Value + step;
            if (newVal >= sliderZoom.Minimum && newVal <= sliderZoom.Maximum)
            {
                sliderZoom.Value = newVal;
                UpdateZoomFromSlider();
            }
        }

        private void sliderZoom_Scroll(object sender, EventArgs e)
        {
            UpdateZoomFromSlider();
        }

        private void UpdateZoomFromSlider()
        {
            zoomFactor = sliderZoom.Value / 100f;
            lblZoomVal.Text = $"{sliderZoom.Value}%";
            ClampOffsets();
            previewPanel.Invalidate();
            DebounceFileSizeCalculation();
        }


        // Dropdown Events
        private void btnAdaptFit_Click(object sender, EventArgs e)
        {
            adaptationMode = AdaptationMode.Fit;
            UpdateAdaptationButtonsSelection(adaptationMode);
            UpdateEditorState();
            DebounceFileSizeCalculation();
        }

        private void btnAdaptFill_Click(object sender, EventArgs e)
        {
            adaptationMode = AdaptationMode.Fill;
            UpdateAdaptationButtonsSelection(adaptationMode);
            UpdateEditorState();
            DebounceFileSizeCalculation();
        }

        private void btnAdaptStretch_Click(object sender, EventArgs e)
        {
            adaptationMode = AdaptationMode.Stretch;
            UpdateAdaptationButtonsSelection(adaptationMode);
            UpdateEditorState();
            DebounceFileSizeCalculation();
        }

        private void btnAdaptCenter_Click(object sender, EventArgs e)
        {
            adaptationMode = AdaptationMode.Center;
            UpdateAdaptationButtonsSelection(adaptationMode);
            UpdateEditorState();
            DebounceFileSizeCalculation();
        }

        private void UpdateAdaptationButtonsSelection(AdaptationMode mode)
        {
            SetButtonSelectedState(btnAdaptFit, mode == AdaptationMode.Fit);
            SetButtonSelectedState(btnAdaptFill, mode == AdaptationMode.Fill);
            SetButtonSelectedState(btnAdaptStretch, mode == AdaptationMode.Stretch);
            SetButtonSelectedState(btnAdaptCenter, mode == AdaptationMode.Center);
        }

        private void SetButtonSelectedState(RoundedButton btn, bool selected)
        {
            if (selected)
            {
                btn.BackColor = Color.FromArgb(7, 36, 43);
                btn.BackgroundColor = Color.FromArgb(7, 36, 43);
                btn.BorderColor = Color.FromArgb(106, 227, 249);
                btn.BorderSize = 2;
                btn.ForeColor = Color.FromArgb(106, 227, 249);
                btn.TextColor = Color.FromArgb(106, 227, 249);
                btn.Font = new Font("Segoe UI Black", 8.5F, FontStyle.Bold);
            }
            else
            {
                btn.BackColor = Color.FromArgb(10, 25, 30);
                btn.BackgroundColor = Color.FromArgb(10, 25, 30);
                btn.BorderColor = Color.FromArgb(20, 40, 45);
                btn.BorderSize = 1;
                btn.ForeColor = Color.Gray;
                btn.TextColor = Color.Gray;
                btn.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
            }
        }

        private void panelBgColorColor_Click(object sender, EventArgs e)
        {
            using (CustomColorDialog cd = new CustomColorDialog(selectedBgColor))
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    selectedBgColor = cd.SelectedColor;
                    panelBgColorColor.BackgroundColor = selectedBgColor;
                    previewPanel.Invalidate();
                    DebounceFileSizeCalculation();
                }
            }
        }

        private void btnBgColorTransparent_Click(object sender, EventArgs e)
        {
            selectedBgColor = Color.Transparent;
            panelBgColorColor.BackgroundColor = Color.Transparent;
            previewPanel.Invalidate();
            DebounceFileSizeCalculation();
        }

        private void chkRemoveBg_CheckedChanged(object sender, EventArgs e)
        {
            removeBgEnabled = chkRemoveBg.Checked;
            UpdateRemoveBgControlsEnabled();
            previewPanel.Invalidate();
            DebounceFileSizeCalculation();
        }

        private void panelRemoveBgColorColor_Click(object sender, EventArgs e)
        {
            if (!removeBgEnabled) return;

            using (CustomColorDialog cd = new CustomColorDialog(removeBgColor))
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    removeBgColor = cd.SelectedColor;
                    panelRemoveBgColorColor.BackgroundColor = removeBgColor;
                    previewPanel.Invalidate();
                    DebounceFileSizeCalculation();
                }
            }
        }

        private void sliderTolerance_Scroll(object sender, EventArgs e)
        {
            removeBgTolerance = sliderTolerance.Value;
            lblToleranceVal.Text = removeBgTolerance.ToString();
            previewPanel.Invalidate();
            DebounceFileSizeCalculation();
        }

        private void chkFeather_CheckedChanged(object sender, EventArgs e)
        {
            featherEnabled = chkFeather.Checked;
            UpdateFeatherControlsEnabled();
            previewPanel.Invalidate();
            DebounceFileSizeCalculation();
        }

        private void sliderChoke_Scroll(object sender, EventArgs e)
        {
            chokeRadius = sliderChoke.Value;
            lblChokeVal.Text = $"{chokeRadius}px";
            previewPanel.Invalidate();
            DebounceFileSizeCalculation();
        }

        private void sliderFeather_Scroll(object sender, EventArgs e)
        {
            featherRadius = sliderFeather.Value;
            lblFeatherVal.Text = $"{featherRadius}px";
            previewPanel.Invalidate();
            DebounceFileSizeCalculation();
        }

        private void sliderCompression_Scroll(object sender, EventArgs e)
        {
            lblCompression.Text = $"Quality (Scale): {sliderCompression.Value}%";
            previewPanel.Invalidate();
            DebounceFileSizeCalculation();
        }

        private void DebounceFileSizeCalculation()
        {
            if (sizeCalcTimer == null) return;
            sizeCalcTimer.Stop();
            lblFileSize.Text = "Size: Calculating...";
            lblFileSize.ForeColor = Color.Gray;
            btnSave.Enabled = false;
            sizeCalcTimer.Start();
        }

        private async void SizeCalcTimer_Tick(object sender, EventArgs e)
        {
            sizeCalcTimer.Stop();
            if (originalImage == null) return;

            GetCurrentCanvasDimensions(out int cW, out int cH);
            AdaptationMode mode = GetCurrentAdaptationMode();
            float outScale = sliderCompression.Value / 100f;

            // Snapshot current settings to avoid cross-thread UI access issues
            float z = zoomFactor;
            float px = panOffsetX;
            float py = panOffsetY;
            int rAngle = rotationAngle;
            Color bgCol = selectedBgColor;
            bool rBg = removeBgEnabled;
            Color rBgCol = removeBgColor;
            int rBgTol = removeBgTolerance;
            bool outEn = outlineEnabled;
            Color outCol = outlineColor;
            int outThick = outlineThickness;
            bool featEn = featherEnabled;
            int featRad = featherRadius;
            int chRad = chokeRadius;
            Image sourceImg = originalImage;

            long sizeBytes = 0;

            string currentHash = $"{cW}_{cH}_{mode}_{z}_{px}_{py}_{rAngle}_{bgCol.ToArgb()}_{rBg}_{rBgCol.ToArgb()}_{rBgTol}_{outEn}_{outCol.ToArgb()}_{outThick}_{featEn}_{featRad}_{chRad}";

            await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    Bitmap renderedToUse = null;
                    bool newRender = false;

                    if (_lastRenderHash == currentHash && _cachedRender != null)
                    {
                        renderedToUse = _cachedRender;
                    }
                    else
                    {
                        renderedToUse = ImageEditor.RenderImage(
                            sourceImg, cW, cH, mode, z, px, py, rAngle,
                            bgCol, rBg, rBgCol, rBgTol,
                            outEn, outCol, outThick, featEn, featRad, chRad);
                        
                        if (renderedToUse != null)
                        {
                            _cachedRender?.Dispose();
                            _cachedRender = (Bitmap)renderedToUse.Clone(); // Clone to prevent cross-thread issues
                            _lastRenderHash = currentHash;
                        }
                        newRender = true;
                    }

                    if (renderedToUse == null) return;

                    Bitmap finalRendered = renderedToUse;
                    bool disposeFinal = false;

                    if (outScale < 1.0f && outScale > 0.0f)
                    {
                        int newW = Math.Max(1, (int)(cW * outScale));
                        int newH = Math.Max(1, (int)(cH * outScale));
                        finalRendered = new Bitmap(newW, newH, PixelFormat.Format32bppArgb);
                        using (Graphics g = Graphics.FromImage(finalRendered))
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.DrawImage(renderedToUse, 0, 0, newW, newH);
                        }
                        disposeFinal = true;
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        finalRendered.Save(ms, ImageFormat.Png);
                        sizeBytes = ms.Length;
                    }

                    if (disposeFinal) finalRendered.Dispose();
                    if (newRender) renderedToUse.Dispose();
                }
                catch { }
            });

            if (this.IsDisposed) return;

            double mb = sizeBytes / (1024.0 * 1024.0);
            lblFileSize.Text = $"Size: {mb:0.00} MB";

            if (mb > 10.0)
            {
                lblFileSize.ForeColor = Color.FromArgb(255, 128, 128); // Light red
                btnSave.Enabled = false;
            }
            else
            {
                lblFileSize.ForeColor = Color.FromArgb(106, 227, 249); // Cyan
                btnSave.Enabled = true;
            }
        }

        // Rotate clockwise
        private void btnRotate_Click(object sender, EventArgs e)
        {
            rotationAngle = (rotationAngle + 90) % 360;
            ClampOffsets();
            previewPanel.Invalidate();
        }

        // Reset settings
        private void btnReset_Click(object sender, EventArgs e)
        {
            rotationAngle = 0;
            adaptationMode = AdaptationMode.Fit;
            UpdateAdaptationButtonsSelection(adaptationMode);
            selectedBgColor = Color.Transparent;
            panelBgColorColor.BackgroundColor = Color.Transparent;

            chkRemoveBg.Checked = false;
            panelRemoveBgColorColor.BackgroundColor = Color.White;
            sliderTolerance.Value = 15;
            lblToleranceVal.Text = "15";
            removeBgEnabled = false;
            removeBgColor = Color.White;
            removeBgTolerance = 15;
            UpdateRemoveBgControlsEnabled();

            chkFeather.Checked = false;
            sliderChoke.Value = 1;
            lblChokeVal.Text = "1px";
            sliderFeather.Value = 2;
            lblFeatherVal.Text = "2px";
            featherEnabled = false;
            chokeRadius = 1;
            featherRadius = 2;
            UpdateFeatherControlsEnabled();

            outlineEnabled = false;
            outlineColor = Color.White;
            outlineThickness = 0;
            
            sliderCompression.Value = 100;
            lblCompression.Text = "Quality (Scale): 100%";

            ResetOffsets();
            ClampOffsets();
            previewPanel.Invalidate();
            DebounceFileSizeCalculation();
        }

        // OK / Save Click
        private void btnSave_Click(object sender, EventArgs e)
        {
            GetCurrentCanvasDimensions(out canvasWidth, out canvasHeight);
            adaptationMode = GetCurrentAdaptationMode();
            ClampOffsets();
            float outScale = sliderCompression.Value / 100f;

            // Render output
            Bitmap rendered = null;
            Bitmap finalRendered = null;
            string resultPath = string.Empty;
            try
            {
                rendered = ImageEditor.RenderImage(
                    originalImage,
                    canvasWidth,
                    canvasHeight,
                    adaptationMode,
                    zoomFactor,
                    panOffsetX,
                    panOffsetY,
                    rotationAngle,
                    selectedBgColor,
                    removeBgEnabled,
                    removeBgColor,
                    removeBgTolerance,
                    outlineEnabled,
                    outlineColor,
                    outlineThickness,
                    featherEnabled,
                    featherRadius,
                    chokeRadius
                );

                finalRendered = rendered;
                if (outScale < 1.0f && outScale > 0.0f)
                {
                    int newW = Math.Max(1, (int)(canvasWidth * outScale));
                    int newH = Math.Max(1, (int)(canvasHeight * outScale));
                    finalRendered = new Bitmap(newW, newH, PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(finalRendered))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.DrawImage(rendered, 0, 0, newW, newH);
                    }
                }

                resultPath = ImageEditor.SaveTempProcessedImage(finalRendered);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                if (finalRendered != null && finalRendered != rendered)
                    finalRendered.Dispose();
                rendered?.Dispose();
            }

            // Notify inline host that save is done
            OnSave?.Invoke(resultPath, textBoxNote.Text);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            OnCancel?.Invoke();
        }

        private void btnPickColor_Click(object sender, EventArgs e)
        {
            isColorPicking = !isColorPicking;
            if (isColorPicking)
            {
                previewPanel.Cursor = Cursors.Cross;
            }
            else
            {
                previewPanel.Cursor = Cursors.Default;
            }
        }

        private void SampleColorFromMouse(int mouseX, int mouseY)
        {
            if (originalImage == null) return;

            GetCurrentCanvasDimensions(out int cw, out int ch);
            int pBoxW = previewPanel.Width;
            int pBoxH = previewPanel.Height;

            float previewScale = Math.Min((float)pBoxW / cw, (float)pBoxH / ch) * 0.9f;
            float cx = pBoxW / 2f;
            float cy = pBoxH / 2f;

            float rectW = cw * previewScale;
            float rectH = ch * previewScale;
            float rectX = cx - rectW / 2f;
            float rectY = cy - rectH / 2f;

            float bmpW = (int)Math.Max(1, Math.Round(rectW));
            float bmpH = (int)Math.Max(1, Math.Round(rectH));

            using (Matrix mat = new Matrix())
            {
                mat.Translate(rectX, rectY);
                mat.Translate(bmpW / 2f, bmpH / 2f);
                mat.Scale(previewScale, previewScale);
                mat.Translate(panOffsetX, panOffsetY);

                // Get image dimensions, swapping if rotated 90 or 270
                float imageW = originalImage.Width;
                float imageH = originalImage.Height;
                if (rotationAngle == 90 || rotationAngle == 270)
                {
                    imageW = originalImage.Height;
                    imageH = originalImage.Width;
                }

                float baseScale = 1f;
                adaptationMode = GetCurrentAdaptationMode();
                if (adaptationMode == AdaptationMode.Fit)
                {
                    baseScale = Math.Min((float)cw / imageW, (float)ch / imageH);
                }
                else if (adaptationMode == AdaptationMode.Fill)
                {
                    baseScale = Math.Max((float)cw / imageW, (float)ch / imageH);
                }
                else if (adaptationMode == AdaptationMode.Center)
                {
                    baseScale = 1f;
                }

                if (adaptationMode == AdaptationMode.Stretch)
                {
                    float scaleX = ((float)cw / imageW) * zoomFactor;
                    float scaleY = ((float)ch / imageH) * zoomFactor;
                    mat.Scale(scaleX, scaleY);
                }
                else
                {
                    float finalScale = baseScale * zoomFactor;
                    mat.Scale(finalScale, finalScale);
                }

                mat.Rotate(rotationAngle);
                mat.Translate(-originalImage.Width / 2f, -originalImage.Height / 2f);

                try
                {
                    mat.Invert();
                }
                catch
                {
                    return; // Matrix not invertible
                }

                PointF[] pts = new PointF[] { new PointF(mouseX, mouseY) };
                mat.TransformPoints(pts);

                int imgX = (int)Math.Round(pts[0].X);
                int imgY = (int)Math.Round(pts[0].Y);

                if (imgX >= 0 && imgX < originalImage.Width && imgY >= 0 && imgY < originalImage.Height)
                {
                    using (Bitmap bmp = new Bitmap(originalImage))
                    {
                        Color pickedColor = bmp.GetPixel(imgX, imgY);
                        removeBgColor = pickedColor;
                        panelRemoveBgColorColor.BackgroundColor = pickedColor;
                        previewPanel.Invalidate();
                    }
                }
            }
        }

        private void ImageEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            checkerBrush?.Dispose();
            originalImage?.Dispose();
        }
    }
}
