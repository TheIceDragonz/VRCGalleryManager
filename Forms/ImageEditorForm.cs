using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VRCGalleryManager.Core.Helpers;

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

        // Fixed canvas dimensions — set by LoadImage() based on the ratio string
        private int _fixedCanvasWidth  = 2048;
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
                MessageBox.Show($"Errore nell'aprire l'immagine: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                _fixedCanvasWidth  = 2048;
                _fixedCanvasHeight = 1152;
            }
            else // 1:1 default
            {
                _fixedCanvasWidth  = 2048;
                _fixedCanvasHeight = 2048;
            }

            // Set up adaptation combo
            comboAdaptation.Items.Clear();
            comboAdaptation.Items.Add("Adatta (Fit)");
            comboAdaptation.Items.Add("Riempi (Fill)");
            comboAdaptation.Items.Add("Stira (Stretch)");
            comboAdaptation.Items.Add("Centra (Center)");
            comboAdaptation.SelectedIndex = 0; // Fit

            // Set up background color combo
            comboBgColor.Items.Clear();
            comboBgColor.Items.Add("Trasparente");
            comboBgColor.Items.Add("Nero");
            comboBgColor.Items.Add("Bianco");
            comboBgColor.Items.Add("Personalizzato...");
            comboBgColor.SelectedIndex = 0; // Transparent

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

        private void GetCurrentCanvasDimensions(out int width, out int height)
        {
            width  = _fixedCanvasWidth;
            height = _fixedCanvasHeight;
        }

        private AdaptationMode GetCurrentAdaptationMode()
        {
            switch (comboAdaptation.SelectedIndex)
            {
                case 1: return AdaptationMode.Fill;
                case 2: return AdaptationMode.Stretch;
                case 3: return AdaptationMode.Center;
                case 0:
                default:
                    return AdaptationMode.Fit;
            }
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

            // 3. Render the image with GDI+ transform using clipping
            GraphicsState state = g.Save();
            g.SetClip(new RectangleF(rectX, rectY, rectW, rectH));

            // Set graphics transformation matrix for preview
            g.TranslateTransform(cx, cy);
            g.ScaleTransform(previewScale, previewScale);
            
            // Translate to canvas center + pan offsets
            g.TranslateTransform(panOffsetX, panOffsetY);

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
                g.ScaleTransform(scaleX, scaleY);
            }
            else
            {
                float finalScale = baseScale * zoomFactor;
                g.ScaleTransform(finalScale, finalScale);
            }

            // Rotate around center
            g.RotateTransform(rotationAngle);
            
            // Translate back to draw centered
            float drawW = originalImage.Width;
            float drawH = originalImage.Height;
            g.TranslateTransform(-drawW / 2f, -drawH / 2f);

            // Draw image high quality
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(originalImage, 0, 0, drawW, drawH);

            g.Restore(state);

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
        private void comboAdaptation_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEditorState();
        }

        private void comboBgColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBgColor.SelectedIndex)
            {
                case 1:
                    selectedBgColor = Color.Black;
                    break;
                case 2:
                    selectedBgColor = Color.White;
                    break;
                case 3:
                    // Open color dialog
                    using (ColorDialog cd = new ColorDialog())
                    {
                        if (cd.ShowDialog() == DialogResult.OK)
                        {
                            selectedBgColor = cd.Color;
                        }
                        else
                        {
                            // fallback to Transparent if cancelled
                            comboBgColor.SelectedIndex = 0;
                        }
                    }
                    break;
                case 0:
                default:
                    selectedBgColor = Color.Transparent;
                    break;
            }
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
            comboAdaptation.SelectedIndex = 0; // Fit
            comboBgColor.SelectedIndex = 0; // Transparent
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
                    selectedBgColor
                );

                resultPath = ImageEditor.SaveTempProcessedImage(rendered);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nel salvataggio dell'immagine: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void ImageEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            checkerBrush?.Dispose();
            originalImage?.Dispose();
        }
    }
}
