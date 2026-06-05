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

        // Events for inline use (replaces ShowDialog / DialogResult)
        public event Action<string> OnSave;
        public event Action OnCancel;

        public ImageEditorForm()
        {
            InitializeComponent();
            ApplyRecolorBar();
            previewPanel.MouseWheel += previewPanel_MouseWheel;
        }

        /// <summary>
        /// Carica una nuova immagine nell'editor e imposta il ratio di default.
        /// Chiama questo metodo prima di mostrare il pannello.
        /// </summary>
        public void LoadImage(string imagePath, string defaultRatioStr = "1:1")
        {
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

            // Trigger initial state layout
            UpdateEditorState();
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
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

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

            // 2. Draw canvas background color if it is not Transparent
            if (selectedBgColor.A > 0)
            {
                using (Brush bgB = new SolidBrush(selectedBgColor))
                {
                    g.FillRectangle(bgB, rectX, rectY, rectW, rectH);
                }
            }

            // 3. Render the image with GDI+ transform onto a temporary preview bitmap
            int bmpW = (int)Math.Max(1, Math.Round(rectW));
            int bmpH = (int)Math.Max(1, Math.Round(rectH));

            using (Bitmap previewBmp = new Bitmap(bmpW, bmpH, PixelFormat.Format32bppArgb))
            {
                using (Graphics pg = Graphics.FromImage(previewBmp))
                {
                    pg.SmoothingMode = SmoothingMode.AntiAlias;
                    pg.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    float pcx = bmpW / 2f;
                    float pcy = bmpH / 2f;

                    pg.TranslateTransform(pcx, pcy);
                    pg.ScaleTransform(previewScale, previewScale);
                    pg.TranslateTransform(panOffsetX, panOffsetY);

                    // Get image dimensions, swapping if rotated 90 or 270
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
                        baseScale = Math.Min((float)canvasWidth / imageW, (float)canvasHeight / imageH);
                    }
                    else if (adaptationMode == AdaptationMode.Fill)
                    {
                        baseScale = Math.Max((float)canvasWidth / imageW, (float)canvasHeight / imageH);
                    }
                    else if (adaptationMode == AdaptationMode.Center)
                    {
                        baseScale = 1f;
                    }

                    if (adaptationMode == AdaptationMode.Stretch)
                    {
                        float scaleX = ((float)canvasWidth / imageW) * zoomFactor;
                        float scaleY = ((float)canvasHeight / imageH) * zoomFactor;
                        pg.ScaleTransform(scaleX, scaleY);
                    }
                    else
                    {
                        float finalScale = baseScale * zoomFactor;
                        pg.ScaleTransform(finalScale, finalScale);
                    }

                    pg.RotateTransform(rotationAngle);
                    float drawW = originalImage.Width;
                    float drawH = originalImage.Height;
                    pg.TranslateTransform(-drawW / 2f, -drawH / 2f);

                    // A. Draw outline first if enabled
                    if (outlineEnabled && outlineThickness > 0)
                    {
                        using (ImageAttributes outlineAttr = new ImageAttributes())
                        {
                            if (removeBgEnabled)
                            {
                                Color targetColor = removeBgColor;
                                Color lowColor = Color.FromArgb(
                                    Math.Max(0, targetColor.R - removeBgTolerance),
                                    Math.Max(0, targetColor.G - removeBgTolerance),
                                    Math.Max(0, targetColor.B - removeBgTolerance)
                                );
                                Color highColor = Color.FromArgb(
                                    Math.Min(255, targetColor.R + removeBgTolerance),
                                    Math.Min(255, targetColor.G + removeBgTolerance),
                                    Math.Min(255, targetColor.B + removeBgTolerance)
                                );
                                outlineAttr.SetColorKey(lowColor, highColor);
                            }

                            Color oc = outlineColor;
                            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                            {
                                new float[] {0, 0, 0, 0, 0},
                                new float[] {0, 0, 0, 0, 0},
                                new float[] {0, 0, 0, 0, 0},
                                new float[] {0, 0, 0, 1, 0},
                                new float[] {oc.R/255f, oc.G/255f, oc.B/255f, 0, 1}
                            });
                            outlineAttr.SetColorMatrix(colorMatrix);

                            int steps = 16;
                            float thickness = outlineThickness;
                            for (int i = 0; i < steps; i++)
                            {
                                double angle = i * 2 * Math.PI / steps;
                                float ox = (float)(Math.Cos(angle) * thickness);
                                float oy = (float)(Math.Sin(angle) * thickness);

                                GraphicsState outlineState = pg.Save();
                                pg.TranslateTransform(ox, oy, MatrixOrder.Append);
                                pg.DrawImage(
                                    originalImage,
                                    new Rectangle(0, 0, (int)drawW, (int)drawH),
                                    0,
                                    0,
                                    originalImage.Width,
                                    originalImage.Height,
                                    GraphicsUnit.Pixel,
                                    outlineAttr
                                );
                                pg.Restore(outlineState);
                            }
                        }
                    }

                    // B. Draw main image
                    if (removeBgEnabled)
                    {
                        using (ImageAttributes attr = new ImageAttributes())
                        {
                            Color targetColor = removeBgColor;
                            Color lowColor = Color.FromArgb(
                                Math.Max(0, targetColor.R - removeBgTolerance),
                                Math.Max(0, targetColor.G - removeBgTolerance),
                                Math.Max(0, targetColor.B - removeBgTolerance)
                            );
                            Color highColor = Color.FromArgb(
                                Math.Min(255, targetColor.R + removeBgTolerance),
                                Math.Min(255, targetColor.G + removeBgTolerance),
                                Math.Min(255, targetColor.B + removeBgTolerance)
                            );
                            attr.SetColorKey(lowColor, highColor);
                            pg.DrawImage(
                                originalImage,
                                new Rectangle(0, 0, (int)drawW, (int)drawH),
                                0,
                                0,
                                originalImage.Width,
                                originalImage.Height,
                                GraphicsUnit.Pixel,
                                attr
                            );
                        }
                    }
                    else
                    {
                        pg.DrawImage(originalImage, 0, 0, drawW, drawH);
                    }
                }

                // C. Feather alpha channel if enabled
                if (featherEnabled && (featherRadius > 0 || chokeRadius > 0))
                {
                    int scaledChoke = chokeRadius > 0 ? (int)Math.Max(1, Math.Round(chokeRadius * previewScale)) : 0;
                    int scaledFeather = featherRadius > 0 ? (int)Math.Max(1, Math.Round(featherRadius * previewScale)) : 0;
                    ImageEditor.FeatherAlphaChannel(previewBmp, scaledChoke, scaledFeather);
                }

                // Draw the final preview bitmap to panel
                g.DrawImage(previewBmp, rectX, rectY);
            }

            // 4. Draw outer semi-transparent mask for preview
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
        }


        // Dropdown Events
        private void btnAdaptFit_Click(object sender, EventArgs e)
        {
            adaptationMode = AdaptationMode.Fit;
            UpdateAdaptationButtonsSelection(adaptationMode);
            UpdateEditorState();
        }

        private void btnAdaptFill_Click(object sender, EventArgs e)
        {
            adaptationMode = AdaptationMode.Fill;
            UpdateAdaptationButtonsSelection(adaptationMode);
            UpdateEditorState();
        }

        private void btnAdaptStretch_Click(object sender, EventArgs e)
        {
            adaptationMode = AdaptationMode.Stretch;
            UpdateAdaptationButtonsSelection(adaptationMode);
            UpdateEditorState();
        }

        private void btnAdaptCenter_Click(object sender, EventArgs e)
        {
            adaptationMode = AdaptationMode.Center;
            UpdateAdaptationButtonsSelection(adaptationMode);
            UpdateEditorState();
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
                }
            }
        }

        private void btnBgColorTransparent_Click(object sender, EventArgs e)
        {
            selectedBgColor = Color.Transparent;
            panelBgColorColor.BackgroundColor = Color.Transparent;
            previewPanel.Invalidate();
        }

        private void chkRemoveBg_CheckedChanged(object sender, EventArgs e)
        {
            removeBgEnabled = chkRemoveBg.Checked;
            UpdateRemoveBgControlsEnabled();
            previewPanel.Invalidate();
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
                }
            }
        }

        private void sliderTolerance_Scroll(object sender, EventArgs e)
        {
            removeBgTolerance = sliderTolerance.Value;
            lblToleranceVal.Text = removeBgTolerance.ToString();
            previewPanel.Invalidate();
        }

        private void chkFeather_CheckedChanged(object sender, EventArgs e)
        {
            featherEnabled = chkFeather.Checked;
            UpdateFeatherControlsEnabled();
            previewPanel.Invalidate();
        }

        private void sliderChoke_Scroll(object sender, EventArgs e)
        {
            chokeRadius = sliderChoke.Value;
            lblChokeVal.Text = $"{chokeRadius}px";
            previewPanel.Invalidate();
        }

        private void sliderFeather_Scroll(object sender, EventArgs e)
        {
            featherRadius = sliderFeather.Value;
            lblFeatherVal.Text = $"{featherRadius}px";
            previewPanel.Invalidate();
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

            ResetOffsets();
            ClampOffsets();
            previewPanel.Invalidate();
        }

        // OK / Save Click
        private void btnSave_Click(object sender, EventArgs e)
        {
            GetCurrentCanvasDimensions(out canvasWidth, out canvasHeight);
            adaptationMode = GetCurrentAdaptationMode();
            ClampOffsets();

            // Render output
            Bitmap rendered = null;
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

                resultPath = ImageEditor.SaveTempProcessedImage(rendered);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                rendered?.Dispose();
            }

            // Notify inline host that save is done
            OnSave?.Invoke(resultPath);
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
