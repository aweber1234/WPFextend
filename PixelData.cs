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
        public int[] array;

        public PixelData(int width, int height)
        {
            this.width = width;
            this.height = height;
            array = new int[width * height];
        }

        public void SetPixelColor(Color color, int x, int y)
        {
            // Calculate the index of the pixel
            int index = y * width + x;

            if (index >= 0 && index < array.Length)
            {
                array[index] = ExtensionMethods.ColorToInt(color);
            }
            else
            {
                Debug.WriteLine("SetPixelColor out of bounds pixel index...");
            }

        }

        public Color GetPixelColor(int x, int y)
        {
            // Calculate the index of the pixel
            int index = (y * width + x);

            if (index >= 0 && index < array.Length)
            {
                int val = array[index];

                byte a = (byte)((val >> 24) & 0xFF);
                byte r = (byte)((val >> 16) & 0xFF);
                byte g = (byte)((val >> 8) & 0xFF);
                byte b = (byte)(val & 0xFF);

                return Color.FromArgb(a, r, g, b);
            }
            Debug.WriteLine("GetPixelColor out of bounds pixel index...");
            return default;
        }
    }
}

