
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

        // Convert Color to BGRA byte array
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

        public static void DrawPixelDataToBitmap(this WriteableBitmap bitmap, PixelData pixelData, int x, int y)
        {
            if (x >= 0 && y >= 0 && x + pixelData.width < bitmap.Width && y + pixelData.height < bitmap.Height)
            {
                Int32Rect drawArea = new Int32Rect(x, y, pixelData.width, pixelData.height);

                // Write the pixel data to the WriteableBitmap
                bitmap.WritePixels(
                    drawArea,
                    pixelData.array,                          // The byte array containing pixel data
                    pixelData.width * 4,                          // Stride (number of bytes per row)
                    0                                    // Offset (not needed here, so set to 0)
                );
            }
            else { Debug.WriteLine("DrawPixelDataToBitmap draw area is outside of bounds for WriteableBitmap"); }
                      
        }

 

    }
}



