using ImageFormat = System.Drawing.Imaging.ImageFormat;
using System.Drawing;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;
using Bitmap = System.Drawing.Bitmap;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace VRCGalleryManager.Core.Helpers
{
    public enum AdaptationMode
    {
        Fit,
        Fill,
        Scale, // Unused but kept for enum alignment
        Stretch,
        Center
    }

    public static class ImageEditor
    {
        /// <summary>
        /// Calculates the drawing bounds of the image on the canvas based on the adaptation mode.
        /// </summary>
        public static void CalculateBaseBounds(
            AdaptationMode mode,
            float imageW,
            float imageH,
            float canvasW,
            float canvasH,
            out float baseScale,
            out float baseOffsetX,
            out float baseOffsetY)
        {
            switch (mode)
            {
                case AdaptationMode.Stretch:
                    baseScale = 1f;
                    baseOffsetX = 0f;
                    baseOffsetY = 0f;
                    break;

                case AdaptationMode.Fit:
                    baseScale = Math.Min(canvasW / imageW, canvasH / imageH);
                    baseOffsetX = (canvasW - (imageW * baseScale)) / 2f;
                    baseOffsetY = (canvasH - (imageH * baseScale)) / 2f;
                    break;

                case AdaptationMode.Fill:
                    baseScale = Math.Max(canvasW / imageW, canvasH / imageH);
                    baseOffsetX = (canvasW - (imageW * baseScale)) / 2f;
                    baseOffsetY = (canvasH - (imageH * baseScale)) / 2f;
                    break;

                case AdaptationMode.Center:
                default:
                    baseScale = 1f;
                    baseOffsetX = (canvasW - imageW) / 2f;
                    baseOffsetY = (canvasH - imageH) / 2f;
                    break;
            }
        }

        /// <summary>
        /// Renders the image on a canvas Bitmap using the provided parameters.
        /// </summary>
        public static Bitmap RenderImage(
            Image originalImage,
            int canvasW,
            int canvasH,
            AdaptationMode mode,
            float zoomFactor,
            float panOffsetX,
            float panOffsetY,
            int rotationAngle,
            Color backgroundColor,
            bool removeBg = false,
            Color removeBgColor = default,
            int removeBgTolerance = 0,
            bool addOutline = false,
            Color outlineColor = default,
            int outlineThickness = 0,
            bool featherEdges = false,
            int featherRadius = 0,
            int chokeRadius = 0)
        {
            // Create target bitmap
            Bitmap result = new Bitmap(canvasW, canvasH, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(result))
            {
                // Set high quality options
                g.Clear(backgroundColor);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                float imageW = originalImage.Width;
                float imageH = originalImage.Height;
                if (rotationAngle == 90 || rotationAngle == 270)
                {
                    imageW = originalImage.Height;
                    imageH = originalImage.Width;
                }

                float baseScale = 1f;
                if (mode == AdaptationMode.Fit)
                {
                    baseScale = Math.Min(canvasW / imageW, canvasH / imageH);
                }
                else if (mode == AdaptationMode.Fill)
                {
                    baseScale = Math.Max(canvasW / imageW, canvasH / imageH);
                }
                else if (mode == AdaptationMode.Center)
                {
                    baseScale = 1f;
                }

                // Translate to canvas center + offsets (this makes zoom and pan centered!)
                g.TranslateTransform(canvasW / 2f + panOffsetX, canvasH / 2f + panOffsetY);

                if (mode == AdaptationMode.Stretch)
                {
                    float scaleX = (canvasW / imageW) * zoomFactor;
                    float scaleY = (canvasH / imageH) * zoomFactor;
                    g.ScaleTransform(scaleX, scaleY);
                }
                else
                {
                    float finalScale = baseScale * zoomFactor;
                    g.ScaleTransform(finalScale, finalScale);
                }

                g.RotateTransform(rotationAngle);

                // Draw centered
                float drawW = originalImage.Width;
                float drawH = originalImage.Height;
                g.TranslateTransform(-drawW / 2f, -drawH / 2f);

                Image imageToDraw = originalImage;
                bool disposeImageToDraw = false;

                if (removeBg)
                {
                    imageToDraw = new Bitmap(originalImage.Width, originalImage.Height, PixelFormat.Format32bppArgb);
                    disposeImageToDraw = true;
                    using (Graphics gTemp = Graphics.FromImage(imageToDraw))
                    {
                        gTemp.InterpolationMode = InterpolationMode.NearestNeighbor;
                        gTemp.SmoothingMode = SmoothingMode.None;
                        
                        using (ImageAttributes attr = new ImageAttributes())
                        {
                            Color targetColor = removeBgColor == default ? Color.White : removeBgColor;
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
                            
                            gTemp.DrawImage(originalImage, new Rectangle(0, 0, originalImage.Width, originalImage.Height), 0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel, attr);
                        }
                    }
                }

                try
                {
                    // 1. Draw outline if enabled
                    if (addOutline && outlineThickness > 0)
                    {
                        using (ImageAttributes outlineAttr = new ImageAttributes())
                        {
                            Color oc = outlineColor == default ? Color.White : outlineColor;
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
                            for (int i = 0; i < steps; i++)
                            {
                                double angle = i * 2 * Math.PI / steps;
                                float ox = (float)(Math.Cos(angle) * outlineThickness);
                                float oy = (float)(Math.Sin(angle) * outlineThickness);

                                GraphicsState outlineState = g.Save();
                                g.TranslateTransform(ox, oy, MatrixOrder.Append);
                                g.DrawImage(
                                    imageToDraw,
                                    new Rectangle(0, 0, (int)drawW, (int)drawH),
                                    0,
                                    0,
                                    imageToDraw.Width,
                                    imageToDraw.Height,
                                    GraphicsUnit.Pixel,
                                    outlineAttr
                                );
                                g.Restore(outlineState);
                            }
                        }
                    }

                    // 2. Draw main image
                    g.DrawImage(imageToDraw, 0, 0, drawW, drawH);
                }
                finally
                {
                    if (disposeImageToDraw)
                    {
                        imageToDraw.Dispose();
                    }
                }
            }

            // 3. Feather alpha channel if enabled
            if (featherEdges && (featherRadius > 0 || chokeRadius > 0))
            {
                FeatherAlphaChannel(result, chokeRadius, featherRadius);
            }

            return result;
        }

        /// <summary>
        /// Real feathering and erosion (choking) filter applied strictly to the alpha channel of a 32bpp ARGB image.
        /// Uses a high-precision 2-pass float Chamfer distance transform to calculate rounded (Euclidean-like) distances,
        /// and applies a smoothstep interpolation curve for perfect, banding-free blending (even at high radius).
        /// </summary>
        public static void FeatherAlphaChannel(Bitmap bmp, int choke, int feather)
        {
            if (choke < 0) choke = 0;
            if (feather < 0) feather = 0;
            if (choke == 0 && feather == 0) return;

            int width = bmp.Width;
            int height = bmp.Height;
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            int bytes = bmpData.Stride * height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(bmpData.Scan0, rgbValues, 0, bytes);

            // Allocate distance array
            float[] dist = new float[width * height];
            float maxDist = 999999f;

            // Extract original alpha for masking/clamping
            byte[] origAlpha = new byte[width * height];
            for (int y = 0; y < height; y++)
            {
                int rowOffset = y * bmpData.Stride;
                int alphaRowOffset = y * width;
                for (int x = 0; x < width; x++)
                {
                    origAlpha[alphaRowOffset + x] = rgbValues[rowOffset + x * 4 + 3];
                }
            }

            // Pass 1: Forward Pass (top-to-bottom, left-to-right)
            int idx = 0;
            for (int y = 0; y < height; y++)
            {
                int rowOffset = y * bmpData.Stride;
                int prevRowOffset = (y - 1) * width;
                for (int x = 0; x < width; x++, idx++)
                {
                    if (origAlpha[idx] == 0)
                    {
                        dist[idx] = 0f;
                    }
                    else
                    {
                        float min = maxDist;
                        // Check left
                        if (x > 0)
                        {
                            float leftVal = dist[idx - 1] + 1.0f;
                            if (leftVal < min) min = leftVal;
                        }
                        // Check top-left, top, top-right
                        if (y > 0)
                        {
                            float topVal = dist[prevRowOffset + x] + 1.0f;
                            if (topVal < min) min = topVal;

                            if (x > 0)
                            {
                                float topLeftVal = dist[prevRowOffset + x - 1] + 1.41421356f;
                                if (topLeftVal < min) min = topLeftVal;
                            }
                            if (x < width - 1)
                            {
                                float topRightVal = dist[prevRowOffset + x + 1] + 1.41421356f;
                                if (topRightVal < min) min = topRightVal;
                            }
                        }
                        dist[idx] = min;
                    }
                }
            }

            // Pass 2: Backward Pass (bottom-to-top, right-to-left)
            byte[] alpha = new byte[width * height];
            for (int y = height - 1; y >= 0; y--)
            {
                int rowOffset = y * bmpData.Stride;
                int nextRowOffset = (y + 1) * width;
                int distRowOffset = y * width;
                for (int x = width - 1; x >= 0; x--)
                {
                    int currentIdx = distRowOffset + x;
                    float currentDist = dist[currentIdx];
                    if (currentDist > 0f)
                    {
                        float min = currentDist;
                        // Check right
                        if (x < width - 1)
                        {
                            float rightVal = dist[currentIdx + 1] + 1.0f;
                            if (rightVal < min) min = rightVal;
                        }
                        // Check bottom-left, bottom, bottom-right
                        if (y < height - 1)
                        {
                            float bottomVal = dist[nextRowOffset + x] + 1.0f;
                            if (bottomVal < min) min = bottomVal;

                            if (x > 0)
                            {
                                float bottomLeftVal = dist[nextRowOffset + x - 1] + 1.41421356f;
                                if (bottomLeftVal < min) min = bottomLeftVal;
                            }
                            if (x < width - 1)
                            {
                                float bottomRightVal = dist[nextRowOffset + x + 1] + 1.41421356f;
                                if (bottomRightVal < min) min = bottomRightVal;
                            }
                        }
                        dist[currentIdx] = min;

                        // Apply erosion (choke) and feather
                        if (min <= choke)
                        {
                            alpha[currentIdx] = 0;
                        }
                        else if (feather > 0 && min < choke + feather)
                        {
                            byte a = origAlpha[currentIdx];
                            float t = (min - choke) / feather;
                            float smoothT = t * t * (3.0f - 2.0f * t);
                            alpha[currentIdx] = (byte)Math.Round(a * smoothT);
                        }
                        else
                        {
                            alpha[currentIdx] = origAlpha[currentIdx];
                        }
                    }
                    else
                    {
                        alpha[currentIdx] = 0;
                    }
                }
            }

            // 3. Apply a small smoothing box blur to the feathered alpha channel
            // to eliminate any residual Mach bands/contour lines.
            if (feather > 0)
            {
                int blurRadius = Math.Clamp(feather / 12, 1, 4);
                BlurAlpha(alpha, width, height, blurRadius);

                // Clamp to the original alpha shape to prevent any outward expansion
                for (int i = 0; i < alpha.Length; i++)
                {
                    if (origAlpha[i] == 0)
                    {
                        alpha[i] = 0;
                    }
                    else if (alpha[i] > origAlpha[i])
                    {
                        alpha[i] = origAlpha[i];
                    }
                }
            }

            // Write alpha channel back
            for (int y = 0; y < height; y++)
            {
                int rowOffset = y * bmpData.Stride;
                int alphaRowOffset = y * width;
                for (int x = 0; x < width; x++)
                {
                    rgbValues[rowOffset + x * 4 + 3] = alpha[alphaRowOffset + x];
                }
            }

            Marshal.Copy(rgbValues, 0, bmpData.Scan0, bytes);
            bmp.UnlockBits(bmpData);
        }

        private static void BlurAlpha(byte[] alpha, int width, int height, int radius)
        {
            if (radius <= 0) return;

            byte[] temp = new byte[width * height];
            int windowSize = 2 * radius + 1;

            // Horizontal Pass
            for (int y = 0; y < height; y++)
            {
                int rowOffset = y * width;
                int sum = 0;

                // Initialize sum for first window
                for (int x = -radius; x <= radius; x++)
                {
                    sum += alpha[rowOffset + Math.Clamp(x, 0, width - 1)];
                }
                temp[rowOffset] = (byte)((sum + windowSize / 2) / windowSize);

                for (int x = 1; x < width; x++)
                {
                    int leftIdx = rowOffset + Math.Clamp(x - radius - 1, 0, width - 1);
                    int rightIdx = rowOffset + Math.Clamp(x + radius, 0, width - 1);
                    sum += alpha[rightIdx] - alpha[leftIdx];
                    temp[rowOffset + x] = (byte)((sum + windowSize / 2) / windowSize);
                }
            }

            // Vertical Pass
            for (int x = 0; x < width; x++)
            {
                int sum = 0;

                // Initialize sum for first window
                for (int y = -radius; y <= radius; y++)
                {
                    sum += temp[Math.Clamp(y, 0, height - 1) * width + x];
                }
                alpha[x] = (byte)((sum + windowSize / 2) / windowSize);

                for (int y = 1; y < height; y++)
                {
                    int topIdx = Math.Clamp(y - radius - 1, 0, height - 1) * width + x;
                    int bottomIdx = Math.Clamp(y + radius, 0, height - 1) * width + x;
                    sum += temp[bottomIdx] - temp[topIdx];
                    alpha[y * width + x] = (byte)((sum + windowSize / 2) / windowSize);
                }
            }
        }

        /// <summary>
        /// Saves the edited image to a temporary file.
        /// </summary>
        public static string SaveTempProcessedImage(Bitmap bmp)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(tempDir);
            string outPath = Path.Combine(tempDir, $"edited_{Guid.NewGuid():N}.png");
            bmp.Save(outPath, ImageFormat.Png);
            return outPath;
        }
    }
}
