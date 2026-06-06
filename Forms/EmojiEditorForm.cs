using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VRCGalleryManager.Core;
using VRCGalleryManager.Core.DTO;
using VRCGalleryManager.Core.Helpers;
using VRCGalleryManager.Design;
using VRCGalleryManager.Forms.Panels;

namespace VRCGalleryManager.Forms
{
    public partial class EmojiEditorForm : Form
    {
        // Callbacks
        public event Action<string, bool, string, int, int> OnSave;
        public event Action OnCancel;

        private string originalPath;
        private Image originalImage;
        private bool isAnimatedMode = false;

        // GIF Mode State
        private readonly GifToSpriteSheetConverter gifConverter;
        private SpriteSheetViewer spriteSheetViewer;
        private System.Windows.Forms.Timer gifPreviewDebounceTimer;
        private int gifFramesCount = 0;
        private Bitmap spriteSheetBitmap;
        private List<Bitmap> gifFrames = new List<Bitmap>();
        private List<RoundedPictureBox> frameThumbnails = new List<RoundedPictureBox>();

        // Static Mode State
        private int canvasWidth = 2048;
        private int canvasHeight = 2048;
        private AdaptationMode adaptationMode = AdaptationMode.Fit;
        private float zoomFactor = 1.0f;
        private float panOffsetX = 0f;
        private float panOffsetY = 0f;
        private bool isDragging = false;
        private Point startDragPoint;
        private int rotationAngle = 0;
        private Color selectedBgColor = Color.Transparent;

        private bool removeBgEnabled = false;
        private Color removeBgColor = Color.White;
        private int removeBgTolerance = 15;
        private bool isColorPicking = false;

        private bool featherEnabled = false;
        private int featherRadius = 2;
        private int chokeRadius = 1;

        private TextureBrush checkerBrush;

        // DWM API for Dark Title bar
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        public EmojiEditorForm()
        {
            InitializeComponent();
            gifConverter = new GifToSpriteSheetConverter();
            ApplyRecolorBar();
            previewPanel.MouseWheel += previewPanel_MouseWheel;
            previewVRChat.Click += previewVRChat_Click;
            previewPanel.Resize += (s, e) => UpdateGifPreviewBounds();
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

        public void LoadImage(string imagePath)
        {
            // Dispose old resources
            originalImage?.Dispose();
            originalImage = null;
            checkerBrush?.Dispose();
            checkerBrush = null;
            if (spriteSheetViewer != null)
            {
                spriteSheetViewer.StopAnimation();
                spriteSheetViewer = null;
            }
            spriteSheetBitmap?.Dispose();
            spriteSheetBitmap = null;
            if (previewVRChat.Image != null)
            {
                previewVRChat.Image.Dispose();
                previewVRChat.Image = null;
            }
            ClearGifFrames();

            originalPath = imagePath;
            bool isGif = Path.GetExtension(imagePath).Equals(".gif", StringComparison.OrdinalIgnoreCase);

            // Hide the style dropdown panel initially
            emojiTypePanel.Visible = false;
            btnEmojiStyle.Text = "Style: Select...";

            if (isGif)
            {
                isAnimatedMode = true;
                previewPanel.Dock = DockStyle.None;
                previewPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                previewPanel.Size = new Size(workspacePanel.Width, workspacePanel.Height - flowPanelFrames.Height - 9);
                previewPanel.Show();
                previewPanel.BringToFront();
                previewVRChat.Show();
                flowPanelFrames.Show();
                panelStaticControls.Dock = DockStyle.None;
                panelStaticControls.Hide();
                panelGifControls.Dock = DockStyle.Fill;
                panelGifControls.Show();
                panelGifControls.BringToFront();

                try
                {
                    ExtractGifFrames(imagePath);

                    int maxFrames = gifFrames.Count;

                    trackBarStartFrame.Minimum = 0;
                    trackBarStartFrame.Maximum = Math.Max(0, maxFrames - 1);
                    trackBarStartFrame.Value = 0;
                    trackBarStartFrame.LabelText = "1";
                    lblStartFrame.Text = "Start Frame";

                    trackBarEndFrame.Minimum = 0;
                    trackBarEndFrame.Maximum = Math.Max(0, maxFrames - 1);
                    int defaultEnd = Math.Min(maxFrames - 1, 63);
                    trackBarEndFrame.Value = defaultEnd;
                    trackBarEndFrame.LabelText = (defaultEnd + 1).ToString();
                    lblEndFrame.Text = "End Frame";

                    PopulateFrameStrip();
                    UpdateFrameStripHighlight();

                    var (ssBmp, frameCount) = gifConverter.ConvertGifToSpriteSheet(imagePath, 0, defaultEnd);
                    spriteSheetBitmap = ssBmp;
                    gifFramesCount = frameCount;

                    trackBarFPS.Value = 15;
                    trackBarFPS.LabelText = "15";

                    VRChatPreview();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error converting GIF: {ex.Message}", "Conversion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    OnCancel?.Invoke();
                }
            }
            else
            {
                isAnimatedMode = false;
                previewPanel.Dock = DockStyle.Fill;
                previewPanel.Show();
                previewPanel.BringToFront();
                previewVRChat.Hide();
                flowPanelFrames.Hide();
                panelGifControls.Dock = DockStyle.None;
                panelGifControls.Hide();
                panelStaticControls.Dock = DockStyle.Fill;
                panelStaticControls.Show();
                panelStaticControls.BringToFront();

                try
                {
                    originalImage = Image.FromFile(imagePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}", "Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    OnCancel?.Invoke();
                    return;
                }

                // Reset Static States
                rotationAngle = 0;
                zoomFactor = 1.0f;
                panOffsetX = 0f;
                panOffsetY = 0f;
                selectedBgColor = Color.Transparent;
                panelBgColorColor.BackgroundColor = Color.Transparent;

                removeBgEnabled = false;
                chkRemoveBg.Checked = false;
                removeBgColor = Color.White;
                panelRemoveBgColorColor.BackgroundColor = Color.White;
                removeBgTolerance = 15;
                sliderTolerance.Value = 15;
                lblToleranceVal.Text = "15";

                featherEnabled = false;
                chkFeather.Checked = false;
                chokeRadius = 1;
                featherRadius = 2;
                sliderChoke.Value = 1;
                lblChokeVal.Text = "1px";
                sliderFeather.Value = 2;
                lblFeatherVal.Text = "2px";

                adaptationMode = AdaptationMode.Fit;
                UpdateAdaptationButtonsSelection(adaptationMode);
                UpdateStaticControlsEnabled();

                checkerBrush = CreateCheckerBrush();
                previewPanel.Invalidate();
            }
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

        private void UpdateStaticControlsEnabled()
        {
            bool enabled = chkRemoveBg.Checked;
            lblRemoveBgColor.Enabled = enabled;
            panelRemoveBgColorColor.Enabled = enabled;
            btnPickColor.Enabled = enabled;
            lblTolerance.Enabled = enabled;
            sliderTolerance.Enabled = enabled;
            lblToleranceVal.Enabled = enabled;

            bool fEnabled = chkFeather.Checked;
            lblChoke.Enabled = fEnabled;
            sliderChoke.Enabled = fEnabled;
            lblChokeVal.Enabled = fEnabled;
            lblFeather.Enabled = fEnabled;
            sliderFeather.Enabled = fEnabled;
            lblFeatherVal.Enabled = fEnabled;
        }

        private void ClampOffsets()
        {
            if (originalImage == null) return;

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

            float drawnW, drawnH;
            if (adaptationMode == AdaptationMode.Stretch)
            {
                drawnW = canvasWidth * zoomFactor;
                drawnH = canvasHeight * zoomFactor;
            }
            else
            {
                drawnW = imageW * baseScale * zoomFactor;
                drawnH = imageH * baseScale * zoomFactor;
            }

            if (drawnW >= canvasWidth)
            {
                float maxOffset = (drawnW - canvasWidth) / 2f;
                panOffsetX = Math.Max(-maxOffset, Math.Min(maxOffset, panOffsetX));
            }
            else
            {
                float maxOffset = (canvasWidth - drawnW) / 2f;
                panOffsetX = Math.Max(-maxOffset, Math.Min(maxOffset, panOffsetX));
            }

            if (drawnH >= canvasHeight)
            {
                float maxOffset = (drawnH - canvasHeight) / 2f;
                panOffsetY = Math.Max(-maxOffset, Math.Min(maxOffset, panOffsetY));
            }
            else
            {
                float maxOffset = (canvasHeight - drawnH) / 2f;
                panOffsetY = Math.Max(-maxOffset, Math.Min(maxOffset, panOffsetY));
            }
        }

        // GIF mode preview
        private async void VRChatPreview()
        {
            try
            {
                if (spriteSheetBitmap != null)
                {
                    if (spriteSheetViewer != null)
                    {
                        spriteSheetViewer.StopAnimation();
                        spriteSheetViewer = null;
                    }

                    if (previewVRChat.Image != null)
                    {
                        previewVRChat.Image.Dispose();
                        previewVRChat.Image = null;
                    }

                    UpdateGifPreviewBounds();

                    spriteSheetViewer = new SpriteSheetViewer(previewVRChat);
                    await spriteSheetViewer.LoadSpriteSheetAsync(spriteSheetBitmap, gifFramesCount, trackBarFPS.Value);
                    spriteSheetViewer.StartAnimation();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during preview: {ex.Message}", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Posiziona e dimensiona previewVRChat in modo che occupi
        /// la stessa area quadrata 1:1 calcolata dal Paint handler,
        /// sempre centrata nel previewPanel.
        /// </summary>
        private void UpdateGifPreviewBounds()
        {
            int pBoxW = previewPanel.Width;
            int pBoxH = previewPanel.Height;
            if (pBoxW <= 0 || pBoxH <= 0) return;

            // Stessa formula usata in previewPanel_Paint
            float previewScale = Math.Min((float)pBoxW / canvasWidth, (float)pBoxH / canvasHeight) * 0.9f;
            int side = (int)Math.Max(1, Math.Round(canvasWidth * previewScale));

            int x = (pBoxW - side) / 2;
            int y = (pBoxH - side) / 2;

            previewVRChat.SetBounds(x, y, side, side);
        }

        private void trackBarFPS_Scroll(object sender, EventArgs e)
        {
            trackBarFPS.LabelText = trackBarFPS.Value.ToString();
            if (spriteSheetViewer != null)
                spriteSheetViewer.UpdateFPS(trackBarFPS.Value);
        }

        // Static mode paint/interactivity
        private void previewPanel_Paint(object sender, PaintEventArgs e)
        {
            if (originalImage == null || checkerBrush == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int pBoxW = previewPanel.Width;
            int pBoxH = previewPanel.Height;

            float previewScale = Math.Min((float)pBoxW / canvasWidth, (float)pBoxH / canvasHeight) * 0.9f;
            float cx = pBoxW / 2f;
            float cy = pBoxH / 2f;

            float rectW = canvasWidth * previewScale;
            float rectH = canvasHeight * previewScale;
            float rectX = cx - rectW / 2f;
            float rectY = cy - rectH / 2f;

            // 1. Draw checkerboard background
            g.FillRectangle(checkerBrush, rectX, rectY, rectW, rectH);

            // 2. Draw canvas color
            if (selectedBgColor.A > 0)
            {
                using (Brush bgB = new SolidBrush(selectedBgColor))
                {
                    g.FillRectangle(bgB, rectX, rectY, rectW, rectH);
                }
            }

            // 3. Render image onto temp preview bitmap
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

                if (featherEnabled && (featherRadius > 0 || chokeRadius > 0))
                {
                    int scaledChoke = chokeRadius > 0 ? (int)Math.Max(1, Math.Round(chokeRadius * previewScale)) : 0;
                    int scaledFeather = featherRadius > 0 ? (int)Math.Max(1, Math.Round(featherRadius * previewScale)) : 0;
                    ImageEditor.FeatherAlphaChannel(previewBmp, scaledChoke, scaledFeather);
                }

                g.DrawImage(previewBmp, rectX, rectY);
            }

            // 4. Draw semi-transparent mask outer border
            using (GraphicsPath outerPath = new GraphicsPath())
            {
                outerPath.AddRectangle(new Rectangle(0, 0, pBoxW, pBoxH));
                outerPath.AddRectangle(new Rectangle((int)rectX, (int)rectY, (int)rectW, (int)rectH));
                using (Brush maskBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
                {
                    g.FillPath(maskBrush, outerPath);
                }
            }

            // 5. Draw neon cyan border
            using (Pen borderPen = new Pen(Color.FromArgb(106, 227, 249), 2f))
            {
                g.DrawRectangle(borderPen, rectX, rectY, rectW, rectH);
            }
        }

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
            if (isDragging && e.Button == MouseButtons.Left)
            {
                int pBoxW = previewPanel.Width;
                int pBoxH = previewPanel.Height;

                float previewScale = Math.Min((float)pBoxW / canvasWidth, (float)pBoxH / canvasHeight) * 0.9f;

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

        private void btnAdaptFit_Click(object sender, EventArgs e)
        {
            adaptationMode = AdaptationMode.Fit;
            UpdateAdaptationButtonsSelection(adaptationMode);
            previewPanel.Invalidate();
        }

        private void btnAdaptFill_Click(object sender, EventArgs e)
        {
            adaptationMode = AdaptationMode.Fill;
            UpdateAdaptationButtonsSelection(adaptationMode);
            previewPanel.Invalidate();
        }

        private void btnAdaptStretch_Click(object sender, EventArgs e)
        {
            adaptationMode = AdaptationMode.Stretch;
            UpdateAdaptationButtonsSelection(adaptationMode);
            previewPanel.Invalidate();
        }

        private void btnAdaptCenter_Click(object sender, EventArgs e)
        {
            adaptationMode = AdaptationMode.Center;
            UpdateAdaptationButtonsSelection(adaptationMode);
            previewPanel.Invalidate();
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
            UpdateStaticControlsEnabled();
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

        private void btnPickColor_Click(object sender, EventArgs e)
        {
            isColorPicking = !isColorPicking;
            previewPanel.Cursor = isColorPicking ? Cursors.Cross : Cursors.Default;
        }

        private void SampleColorFromMouse(int mouseX, int mouseY)
        {
            if (originalImage == null) return;

            int pBoxW = previewPanel.Width;
            int pBoxH = previewPanel.Height;

            float previewScale = Math.Min((float)pBoxW / canvasWidth, (float)pBoxH / canvasHeight) * 0.9f;
            float cx = pBoxW / 2f;
            float cy = pBoxH / 2f;

            float rectW = canvasWidth * previewScale;
            float rectH = canvasHeight * previewScale;
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
                    return;
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

        private void sliderTolerance_Scroll(object sender, EventArgs e)
        {
            removeBgTolerance = sliderTolerance.Value;
            lblToleranceVal.Text = removeBgTolerance.ToString();
            previewPanel.Invalidate();
        }

        private void chkFeather_CheckedChanged(object sender, EventArgs e)
        {
            featherEnabled = chkFeather.Checked;
            UpdateStaticControlsEnabled();
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

        private void btnRotate_Click(object sender, EventArgs e)
        {
            rotationAngle = (rotationAngle + 90) % 360;
            ClampOffsets();
            previewPanel.Invalidate();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            rotationAngle = 0;
            zoomFactor = 1.0f;
            sliderZoom.Value = 100;
            lblZoomVal.Text = "100%";
            panOffsetX = 0f;
            panOffsetY = 0f;

            selectedBgColor = Color.Transparent;
            panelBgColorColor.BackgroundColor = Color.Transparent;

            chkRemoveBg.Checked = false;
            removeBgEnabled = false;
            removeBgColor = Color.White;
            panelRemoveBgColorColor.BackgroundColor = Color.White;
            sliderTolerance.Value = 15;
            lblToleranceVal.Text = "15";

            chkFeather.Checked = false;
            featherEnabled = false;
            chokeRadius = 1;
            featherRadius = 2;
            sliderChoke.Value = 1;
            lblChokeVal.Text = "1px";
            sliderFeather.Value = 2;
            lblFeatherVal.Text = "2px";

            adaptationMode = AdaptationMode.Fit;
            UpdateAdaptationButtonsSelection(adaptationMode);
            UpdateStaticControlsEnabled();

            previewPanel.Invalidate();
        }

        // Action controls
        private void btnEmojiStyle_Click(object sender, EventArgs e)
        {
            if (!emojiTypePanel.Visible)
            {
                if (emojiTypePanel.Controls.Count == 0)
                {
                    TypePanel.LoadEmojiType(btnEmojiStyle, emojiTypePanel);
                }
                emojiTypePanel.Visible = true;
                emojiTypePanel.BringToFront();
            }
            else
            {
                emojiTypePanel.Visible = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string style = btnEmojiStyle.Text;
            if (style.Contains("Select..."))
            {
                VRCGalleryManager.Core.NotificationManager.ShowNotification("Please choose an Emoji style/type first!", "Style Required", VRCGalleryManager.Core.NotificationType.Error);
                return;
            }

            if (isAnimatedMode)
            {
                if (spriteSheetBitmap != null)
                {
                    try
                    {
                        string tempPath = gifConverter.SaveTempSpriteSheet();
                        OnSave?.Invoke(tempPath, true, style, gifFramesCount, trackBarFPS.Value);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving sprite sheet: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                if (originalImage != null)
                {
                    Bitmap rendered = null;
                    string tempPath = string.Empty;
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
                            false,
                            Color.White,
                            0,
                            featherEnabled,
                            featherRadius,
                            chokeRadius
                        );

                        string tempDir = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
                        Directory.CreateDirectory(tempDir);
                        tempPath = Path.Combine(tempDir, $"emoji_{Guid.NewGuid()}.png");
                        rendered.Save(tempPath, ImageFormat.Png);

                        OnSave?.Invoke(tempPath, false, style, 0, 0);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving rendered image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        rendered?.Dispose();
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            OnCancel?.Invoke();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            gifPreviewDebounceTimer?.Stop();
            gifPreviewDebounceTimer?.Dispose();
            gifPreviewDebounceTimer = null;
            originalImage?.Dispose();
            checkerBrush?.Dispose();
            spriteSheetBitmap?.Dispose();
            ClearGifFrames();
            TypePanel.ClearEmojiType(emojiTypePanel);
            gifConverter.Dispose();
        }

        private void trackBarStartFrame_Scroll(object sender, EventArgs e)
        {
            if (trackBarStartFrame.Value > trackBarEndFrame.Value)
            {
                trackBarEndFrame.Value = trackBarStartFrame.Value;
                trackBarEndFrame.LabelText = (trackBarEndFrame.Value + 1).ToString();
            }
            else if (trackBarEndFrame.Value - trackBarStartFrame.Value + 1 > 64)
            {
                trackBarEndFrame.Value = trackBarStartFrame.Value + 63;
                trackBarEndFrame.LabelText = (trackBarEndFrame.Value + 1).ToString();
            }
            trackBarStartFrame.LabelText = (trackBarStartFrame.Value + 1).ToString();

            UpdateFrameStripHighlight();
            UpdateGifPreview();
        }

        private void trackBarEndFrame_Scroll(object sender, EventArgs e)
        {
            if (trackBarEndFrame.Value < trackBarStartFrame.Value)
            {
                trackBarStartFrame.Value = trackBarEndFrame.Value;
                trackBarStartFrame.LabelText = (trackBarStartFrame.Value + 1).ToString();
            }
            else if (trackBarEndFrame.Value - trackBarStartFrame.Value + 1 > 64)
            {
                trackBarStartFrame.Value = trackBarEndFrame.Value - 63;
                trackBarStartFrame.LabelText = (trackBarStartFrame.Value + 1).ToString();
            }
            trackBarEndFrame.LabelText = (trackBarEndFrame.Value + 1).ToString();

            UpdateFrameStripHighlight();
            UpdateGifPreview();
        }

        private void InitializeGifDebounceTimer()
        {
            gifPreviewDebounceTimer = new System.Windows.Forms.Timer();
            gifPreviewDebounceTimer.Interval = 150; // 150ms delay
            gifPreviewDebounceTimer.Tick += (s, e) =>
            {
                gifPreviewDebounceTimer.Stop();
                UpdateGifPreviewInternal();
            };
        }

        private void UpdateGifPreview()
        {
            if (gifPreviewDebounceTimer == null)
            {
                InitializeGifDebounceTimer();
            }
            gifPreviewDebounceTimer.Stop();
            gifPreviewDebounceTimer.Start();
        }

        private void UpdateGifPreviewInternal()
        {
            if (string.IsNullOrEmpty(originalPath)) return;

            try
            {
                if (spriteSheetViewer != null)
                {
                    spriteSheetViewer.StopAnimation();
                    spriteSheetViewer = null;
                }

                spriteSheetBitmap?.Dispose();
                spriteSheetBitmap = null;

                int start = trackBarStartFrame.Value;
                int end = trackBarEndFrame.Value;

                var (ssBmp, frameCount) = gifConverter.ConvertGifToSpriteSheet(originalPath, start, end);
                spriteSheetBitmap = ssBmp;
                gifFramesCount = frameCount;

                VRChatPreview();
            }
            catch (Exception)
            {
            }
        }

        private void ExtractGifFrames(string path)
        {
            ClearGifFrames();

            try
            {
                using (Image img = Image.FromFile(path))
                {
                    FrameDimension dimension = new FrameDimension(img.FrameDimensionsList[0]);
                    int count = img.GetFrameCount(dimension);
                    int maxFrames = Math.Min(count, 256); // Limit to 256 frames to prevent memory issues and UI lag

                    for (int i = 0; i < maxFrames; i++)
                    {
                        img.SelectActiveFrame(dimension, i);
                        Bitmap frameBmp = new Bitmap(img.Width, img.Height);
                        using (Graphics g = Graphics.FromImage(frameBmp))
                        {
                            g.Clear(Color.Transparent);
                            g.DrawImage(img, 0, 0, img.Width, img.Height);
                        }
                        gifFrames.Add(frameBmp);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error extracting GIF frames: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearGifFrames()
        {
            foreach (var frame in gifFrames)
            {
                frame?.Dispose();
            }
            gifFrames.Clear();

            foreach (var pb in frameThumbnails)
            {
                pb.Image?.Dispose();
                pb.Dispose();
            }
            frameThumbnails.Clear();
            if (flowPanelFrames != null)
            {
                flowPanelFrames.Controls.Clear();
            }
        }

        private void PopulateFrameStrip()
        {
            if (flowPanelFrames == null) return;
            flowPanelFrames.Controls.Clear();
            frameThumbnails.Clear();

            for (int i = 0; i < gifFrames.Count; i++)
            {
                int frameIndex = i;
                Bitmap frame = gifFrames[i];

                Bitmap thumb = new Bitmap(20, 60);
                using (Graphics g = Graphics.FromImage(thumb))
                {
                    g.Clear(Color.Transparent);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    int minDim = Math.Min(frame.Width, frame.Height);
                    int srcX = (frame.Width - minDim) / 2;
                    int srcY = (frame.Height - minDim) / 2;
                    g.DrawImage(frame, new Rectangle(0, 0, 20, 60), new Rectangle(srcX, srcY, minDim, minDim), GraphicsUnit.Pixel);
                }

                RoundedPictureBox pb = new RoundedPictureBox();
                pb.Width = 20;
                pb.Height = 60;
                pb.BorderSize = 5;
                pb.BorderRadiusTopLeft = 6;
                pb.BorderRadiusTopRight = 6;
                pb.BorderRadiusBottomLeft = 6;
                pb.BorderRadiusBottomRight = 6;
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Image = thumb;
                pb.Cursor = Cursors.Hand;
                pb.Margin = new Padding(3, 3, 3, 3);

                pb.Click += (s, e) => {
                    if (spriteSheetViewer != null)
                    {
                        spriteSheetViewer.StopAnimation();
                    }
                    previewVRChat.Image?.Dispose();
                    previewVRChat.Image = CropToSquare(frame, 350);
                };

                frameThumbnails.Add(pb);
                flowPanelFrames.Controls.Add(pb);
            }
        }

        private void UpdateFrameStripHighlight()
        {
            int start = trackBarStartFrame.Value;
            int end = trackBarEndFrame.Value;

            for (int i = 0; i < frameThumbnails.Count; i++)
            {
                var pb = frameThumbnails[i];
                if (i >= start && i <= end)
                {
                    pb.BorderColor = Color.FromArgb(106, 227, 249);
                    pb.BackColor = Color.FromArgb(7, 36, 43);
                    pb.BackgroundColor = Color.FromArgb(7, 36, 43);
                }
                else
                {
                    pb.BorderColor = Color.FromArgb(40, 45, 50);
                    pb.BackColor = Color.FromArgb(15, 17, 19);
                    pb.BackgroundColor = Color.FromArgb(15, 17, 19);
                }
                pb.Invalidate();
            }
        }

        private Bitmap CropToSquare(Image img, int size)
        {
            Bitmap squareImage = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(squareImage))
            {
                g.Clear(Color.Transparent);
                int minDim = Math.Min(img.Width, img.Height);
                int srcX = (img.Width - minDim) / 2;
                int srcY = (img.Height - minDim) / 2;
                g.DrawImage(img, new Rectangle(0, 0, size, size), new Rectangle(srcX, srcY, minDim, minDim), GraphicsUnit.Pixel);
            }
            return squareImage;
        }

        private void previewVRChat_Click(object sender, EventArgs e)
        {
            if (isAnimatedMode)
            {
                UpdateGifPreview();
            }
        }
    }
}
