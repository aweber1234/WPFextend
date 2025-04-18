
using System;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Data.Common;
using System.Diagnostics;
using System.Windows.Media.Media3D;


namespace WPFextend
{

    /// <summary>
    /// Various helper methods for WPF with a focus on image manipulation.
    /// </summary>
    public static class ExtensionMethods
    {

        /// <summary>
        /// Returns a brightness adjusted color. 
        /// </summary>
        /// <param name="color">Original color.</param>
        /// <param name="lightValue">Brightness multiplier. 1 will return the same color, 2 will return twice as bight, etc.</param>
        /// <returns></returns>
        public static Color AdjustColorBrightness(this Color color, float lightValue)
        {
            Color adjusted = Color.FromArgb(
                color.A,
                (byte)Math.Min(color.R * lightValue, 255),
                (byte)Math.Min(color.G * lightValue, 255),
                (byte)Math.Min(color.B * lightValue, 255));
            return adjusted;
        }

        /// <summary>
        /// Convert Color to BGRA byte array
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static byte[] ColorToBgraBytes(this Color color)
        {
            return new byte[]
            {
                color.B, // Blue
                color.G, // Green
                color.R, // Red
                color.A  // Alpha
            };
        }

        /// <summary>
        /// Converts color to 0xAARRGGBB int
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int ColorToInt(this Color color)
        {
            return (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
        }

        public static unsafe void DrawPixelDataToBitmap(this WriteableBitmap bitmap, PixelData pixelData, int xPos, int yPos, bool blendAlpha = false)
        {
            // Validate inputs
            if (bitmap == null || pixelData.array == null)
                throw new ArgumentNullException();

            if (xPos < 0 || yPos < 0 || xPos + pixelData.width > bitmap.PixelWidth || yPos + pixelData.height > bitmap.PixelHeight)
                throw new ArgumentOutOfRangeException("Destination rectangle out of bounds.");

            bitmap.Lock();
            try
            {
                int* pBackBuffer = (int*)bitmap.BackBuffer;
                int wbStride = bitmap.BackBufferStride / 4; // Stride in ints (4 bytes per pixel)

                for (int y = 0; y < pixelData.height; y++)
                {
                    for (int x = 0; x < pixelData.width; x++)
                    {
                        // Calculate source/dest indices
                        int srcX = x; // Modify if source offset is needed
                        int srcY = y;
                        int srcIndex = srcY * pixelData.width + srcX;
                        int destIndex = (yPos + y) * wbStride + (xPos + x);

                        // Skip if source pixel is out of bounds
                        if (srcIndex >= pixelData.array.Length || srcIndex < 0) continue;

                        int srcPixel = pixelData.array[srcIndex];
                        byte alpha = (byte)((srcPixel >> 24) & 0xFF);

                        // Skip fully transparent pixels (if not blending)
                        if (!blendAlpha && alpha == 0) continue;

                        // Blend with destination pixel (if enabled)
                        if (blendAlpha && alpha < 255)
                        {
                            int destPixel = pBackBuffer[destIndex];
                            pBackBuffer[destIndex] = BlendPixels(srcPixel, destPixel);
                        }
                        else
                        {
                            pBackBuffer[destIndex] = srcPixel;
                        }
                    }
                }

                bitmap.AddDirtyRect(new Int32Rect(xPos, yPos, pixelData.width, pixelData.height));
            }
            finally
            {
                bitmap.Unlock();
            }
        }

        /// <summary>
        /// Helper: Alpha blending (premultiplied)
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static int BlendPixels(int src, int dest)
        {
            float srcA = ((src >> 24) & 0xFF) / 255f;
            float destA = ((dest >> 24) & 0xFF) / 255f;
            float outA = srcA + destA * (1 - srcA);

            if (outA == 0) return 0;

            int BlendChannel(int s, int d) =>
                (int)((s * srcA + d * destA * (1 - srcA)) / outA);

            return
                ((int)(outA * 255) << 24) |
                (BlendChannel((src >> 16) & 0xFF, (dest >> 16) & 0xFF) << 16) |
                (BlendChannel((src >> 8) & 0xFF, (dest >> 8) & 0xFF) << 8) |
                BlendChannel(src & 0xFF, dest & 0xFF);
        }


        /// <summary>
        /// Converts value to the lowest whole dividend. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="divdend"></param>
        /// <returns></returns>
        public static double ToDividendFloor(double value, double dividend)
        {
            return dividend * Math.Floor(value / dividend);
        }

        /// <summary>
        /// Converts value to the largest whole dividend. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="divdend"></param>
        /// <returns></returns>
        public static double ToDividendCeiling(double value, double dividend)
        {
            return dividend * Math.Ceiling(value / dividend);
        }

        /// <summary>
        /// Converts value to the nearest whole dividend. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="divdend"></param>
        /// <returns></returns>
        public static double ToDividendRound(double value, double dividend)
        {
            return dividend * Math.Round(value / dividend);
        }


        public static Color HsvToRgb(double h, double s, double v)
        {
            h = h % 360;
            if (h < 0) h += 360;



            s = Math.Clamp(s, 0, 1);
            v = Math.Clamp(v, 0, 1);

            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = v - c;

            double r, g, b;

            if (h < 60) { r = c; g = x; b = 0; }
            else if (h < 120) { r = x; g = c; b = 0; }
            else if (h < 180) { r = 0; g = c; b = x; }
            else if (h < 240) { r = 0; g = x; b = c; }
            else if (h < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            return Color.FromRgb(
                (byte)((r + m) * 255),
                (byte)((g + m) * 255),
                (byte)((b + m) * 255)
            );
        }

        public static void RgbToHsv(Color color, out double h, out double s, out double v)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            // Calculate Value
            v = max;

            // Calculate Saturation
            if (max == 0)
            {
                s = 0;
            }
            else
            {
                s = (delta / max);
            }

            // Calculate Hue
            if (delta == 0)
            {
                h = 0;
            }
            else
            {
                if (max == r)
                {
                    h = 60 * (((g - b) / delta) % 6);
                }
                else if (max == g)
                {
                    h = 60 * (((b - r) / delta) + 2);
                }
                else // max == b
                {
                    h = 60 * (((r - g) / delta) + 4);
                }

                if (h < 0) h += 360;
            }
        }



        public static GradientStopCollection CreateHueGradient(double s, double v, int steps = 256)
        {
            var gradient = new GradientStopCollection();

            for (int i = 0; i < steps; i++)
            {
                double h = 360 * i / (steps - 1.0);
                Color color = HsvToRgb(h, s, v);
                gradient.Add(new GradientStop(color, (double)i / (steps - 1)));
            }

            return gradient;
        }



        public static GradientStopCollection CreateSaturationGradient(double h, double v, int steps = 256)
        {
            var gradient = new GradientStopCollection();

            for (int i = 0; i < steps; i++)
            {
                double s = i / (steps - 1.0);
                Color color = HsvToRgb(h, s, v);
                gradient.Add(new GradientStop(color, (double)i / (steps - 1)));
            }

            return gradient;
        }



        public static GradientStopCollection CreateValueGradient(double h, double s, int steps = 256)
        {
            var gradient = new GradientStopCollection();

            for (int i = 0; i < steps; i++)
            {
                double v = i / (steps - 1.0);
                Color color = HsvToRgb(h, s, v);
                gradient.Add(new GradientStop(color, (double)i / (steps - 1)));
            }

            return gradient;
        }



    }
}



