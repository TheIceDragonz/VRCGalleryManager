using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using Color = SixLabors.ImageSharp.Color;

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
        /// Renders the image on a canvas Image using the provided parameters.
        /// </summary>
        public static Image<Rgba32> RenderImage(
            Image<Rgba32> originalImage,
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
            bool addOutline = false, // Ignored in this port as it's not used currently
            Color outlineColor = default,
            int outlineThickness = 0,
            bool featherEdges = false,
            int featherRadius = 0,
            int chokeRadius = 0,
            System.Threading.CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            // Create target bitmap
            Image<Rgba32> result = new Image<Rgba32>(canvasW, canvasH);
            result.Mutate(x => x.BackgroundColor(backgroundColor));

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

            float finalScale = mode == AdaptationMode.Stretch ? 1f : baseScale * zoomFactor;

            Image<Rgba32> transformed = originalImage.Clone(x => {
                token.ThrowIfCancellationRequested();
                if (mode == AdaptationMode.Stretch)
                {
                    float scaleX = (canvasW / imageW) * zoomFactor;
                    float scaleY = (canvasH / imageH) * zoomFactor;
                    x.Resize((int)(originalImage.Width * scaleX), (int)(originalImage.Height * scaleY), KnownResamplers.Bicubic);
                }
                else if (finalScale != 1f)
                {
                    x.Resize((int)Math.Max(1, originalImage.Width * finalScale), (int)Math.Max(1, originalImage.Height * finalScale), KnownResamplers.Bicubic);
                }

                if (rotationAngle != 0)
                {
                    x.Rotate(rotationAngle);
                }
            });

            if (removeBg)
            {
                var rColor = removeBgColor.ToPixel<Rgba32>();
                transformed.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        if (y % 16 == 0) token.ThrowIfCancellationRequested();
                        Span<Rgba32> row = accessor.GetRowSpan(y);
                        for (int x = 0; x < row.Length; x++)
                        {
                            ref Rgba32 pixel = ref row[x];
                            int rDiff = Math.Abs(pixel.R - rColor.R);
                            int gDiff = Math.Abs(pixel.G - rColor.G);
                            int bDiff = Math.Abs(pixel.B - rColor.B);
                            if (rDiff <= removeBgTolerance && gDiff <= removeBgTolerance && bDiff <= removeBgTolerance)
                            {
                                pixel.A = 0;
                            }
                        }
                    }
                });
            }

            float drawOffsetX = canvasW / 2f + panOffsetX - transformed.Width / 2f;
            float drawOffsetY = canvasH / 2f + panOffsetY - transformed.Height / 2f;

            result.Mutate(x => x.DrawImage(transformed, new SixLabors.ImageSharp.Point((int)Math.Round(drawOffsetX), (int)Math.Round(drawOffsetY)), 1f));

            // 3. Feather alpha channel if enabled
            if (featherEdges && (featherRadius > 0 || chokeRadius > 0))
            {
                FeatherAlphaChannel(result, chokeRadius, featherRadius, token);
            }

            transformed.Dispose();

            return result;
        }

        /// <summary>
        /// Real feathering and erosion (choking) filter applied strictly to the alpha channel of a 32bpp ARGB image.
        /// Uses a high-precision 2-pass float Chamfer distance transform to calculate rounded (Euclidean-like) distances,
        /// and applies a smoothstep interpolation curve for perfect, banding-free blending (even at high radius).
        /// </summary>
        public static void FeatherAlphaChannel(Image<Rgba32> bmp, int choke, int feather, System.Threading.CancellationToken token = default)
        {
            if (choke < 0) choke = 0;
            if (feather < 0) feather = 0;
            if (choke == 0 && feather == 0) return;

            token.ThrowIfCancellationRequested();

            int width = bmp.Width;
            int height = bmp.Height;

            // Extract original alpha for masking/clamping
            byte[] origAlpha = System.Buffers.ArrayPool<byte>.Shared.Rent(width * height);
            bmp.ProcessPixelRows(accessor => {
                for (int y = 0; y < height; y++) {
                    var row = accessor.GetRowSpan(y);
                    int rowOffset = y * width;
                    for (int x = 0; x < width; x++) {
                        origAlpha[rowOffset + x] = row[x].A;
                    }
                }
            });

            // Allocate distance array
            float[] dist = System.Buffers.ArrayPool<float>.Shared.Rent(width * height);
            float maxDist = 999999f;
            byte[] alpha = System.Buffers.ArrayPool<byte>.Shared.Rent(width * height);
            try
            {

            // Pass 1: Forward Pass (top-to-bottom, left-to-right)
            int idx = 0;
            for (int y = 0; y < height; y++)
            {
                if (y % 16 == 0) token.ThrowIfCancellationRequested();
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
            for (int y = height - 1; y >= 0; y--)
            {
                if (y % 16 == 0) token.ThrowIfCancellationRequested();
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
                BlurAlpha(alpha, width, height, blurRadius, token);

                // Clamp to the original alpha shape to prevent any outward expansion
                for (int i = 0; i < alpha.Length; i++)
                {
                    if (i % 1024 == 0) token.ThrowIfCancellationRequested();
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
            bmp.ProcessPixelRows(accessor => {
                for (int y = 0; y < height; y++) {
                    var row = accessor.GetRowSpan(y);
                    int rowOffset = y * width;
                    for (int x = 0; x < width; x++) {
                        row[x].A = alpha[rowOffset + x];
                    }
                }
            });
            }
            finally
            {
                System.Buffers.ArrayPool<byte>.Shared.Return(origAlpha);
                System.Buffers.ArrayPool<float>.Shared.Return(dist);
                System.Buffers.ArrayPool<byte>.Shared.Return(alpha);
            }
        }

        private static void BlurAlpha(byte[] alpha, int width, int height, int radius, System.Threading.CancellationToken token = default)
        {
            if (radius <= 0) return;

            token.ThrowIfCancellationRequested();

            byte[] temp = System.Buffers.ArrayPool<byte>.Shared.Rent(width * height);
            try
            {
                int windowSize = 2 * radius + 1;

                // Horizontal Pass
                for (int y = 0; y < height; y++)
                {
                    if (y % 16 == 0) token.ThrowIfCancellationRequested();
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
                    if (x % 16 == 0) token.ThrowIfCancellationRequested();
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
            finally
            {
                System.Buffers.ArrayPool<byte>.Shared.Return(temp);
            }
        }

        /// <summary>
        /// Saves the edited image to a temporary file.
        /// </summary>
        public static string SaveTempProcessedImage(Image<Rgba32> bmp)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "VRCGalleryManager");
            Directory.CreateDirectory(tempDir);
            string outPath = Path.Combine(tempDir, $"edited_{Guid.NewGuid():N}.png");
            bmp.SaveAsPng(outPath);
            return outPath;
        }
    }
}
