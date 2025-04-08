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
    /// BGRA pixel data stored as byte array. 
    /// </summary>
    public class PixelData
    {
        public int width;
        public int height;

        /// <summary>
        /// byte array which stores bgra pixel data.
        /// </summary>
        public byte[] array;

        public PixelData(int width, int height)
        {
            //4 bytes per pixel, bgra
            array = new byte[width * height * 4];
        }

        public void SetPixelColor(Color color, int x, int y)
        {
            // Calculate the index of the pixel
            int index = (y * width + x) * 4;

            if (index >= 0 && index + 3 < array.Length)
            {
                byte[] bgra = ExtensionMethods.ColorToBgraBytes(color);

                // Set the pixel in the array
                array[index] = bgra[0];     // Blue
                array[index + 1] = bgra[1]; // Green
                array[index + 2] = bgra[2]; // Red
                array[index + 3] = bgra[3]; // Alpha
            }
            else
            {
                Debug.WriteLine("SetPixelColor out of bounds pixel index...");
            }

        }

        public Color GetPixelColor(int x, int y)
        {
            // Calculate the index of the pixel
            int index = (y * width + x) * 4;

            if (index >= 0 && index + 3 < array.Length)
            {
                byte b = array[index];
                byte g = array[index + 1];
                byte r = array[index + 2];
                byte a = array[index + 3];

                return Color.FromArgb(a, r, g, b);
            }
            Debug.WriteLine("GetPixelColor out of bounds pixel index...");
            return default;
        }
    }
}

