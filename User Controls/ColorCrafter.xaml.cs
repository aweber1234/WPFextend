using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFextend
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorCrafter : Window
    {
        private bool _hsvAdjusting;
        private bool _rgbAdjusting;

        private Color _oldColor;

        private Color _newColor;

        public bool ColorAdusting => _rgbAdjusting || _hsvAdjusting;

        public Color NewColor
        {
            get
            {
                return _newColor;
            }
            set
            {                
                _newColor = value;
                RedNumBox.CurrentValue = value.R;
                GreenNumBox.CurrentValue = value.G;
                BlueNumBox.CurrentValue = value.B;
            }

        }


        public Color OldColor
        {
            set
            {
                _oldColor = value;
                ColorPreviewOld.Fill = new SolidColorBrush(value);
                NewColor = value;
                
            }

        }


        public event EventHandler NewColorChosen;



        public ColorCrafter()
        {
            InitializeComponent();
            OldColor = Colors.Black;
        }


        private void OnHSVchange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!ColorAdusting)
            {
                _hsvAdjusting = true;

                UpdateRGBfromHSV();
                _newColor = Color.FromRgb((byte)RedNumBox.CurrentValue, (byte)GreenNumBox.CurrentValue, (byte)BlueNumBox.CurrentValue);
                ColorPreviewNew.Fill = new SolidColorBrush(_newColor);
                HexNotation.Text = _newColor.ToString();
                UpdateColorGradients();

                _hsvAdjusting = false;
            }

        }

        private void OnRGBchange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!ColorAdusting)
            {
                _rgbAdjusting = true;

                UpdateHSVfromRGB();
                _newColor = Color.FromRgb((byte)RedNumBox.CurrentValue, (byte)GreenNumBox.CurrentValue, (byte)BlueNumBox.CurrentValue);
                ColorPreviewNew.Fill = new SolidColorBrush(_newColor);
                HexNotation.Text = _newColor.ToString();
                UpdateColorGradients();

                _rgbAdjusting = false;
            }

        }

        public void UpdateHSVfromRGB()
        {
            double h;
            double s;
            double v;

            ExtensionMethods.RgbToHsv
                (Color.FromRgb((byte)RedNumBox.CurrentValue, (byte)GreenNumBox.CurrentValue, (byte)BlueNumBox.CurrentValue),
                out h, out s, out v);

            HueNumBox.CurrentValue = (int)h;
            SaturationNumBox.CurrentValue = (int)(s * 100);
            ValueNumBox.CurrentValue = (int)(v * 100);
        }

        public void UpdateRGBfromHSV()
        {
            Color color =
                ExtensionMethods.HsvToRgb(HueNumBox.CurrentValue, SaturationNumBox.CurrentValue / 100, ValueNumBox.CurrentValue / 100);

            RedNumBox.CurrentValue = color.R;
            GreenNumBox.CurrentValue = color.G;
            BlueNumBox.CurrentValue = color.B;
        }

        public void UpdateColorGradients()
        {
            RedSlider.Background = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0),
                GradientStops = new GradientStopCollection {
                new GradientStop(Color.FromRgb(byte.MinValue,_newColor.G, _newColor.B), 0.0),
                new GradientStop(Color.FromRgb(byte.MaxValue,_newColor.G, _newColor.B), 1.0) }
            };

            GreenSlider.Background = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0),
                GradientStops = new GradientStopCollection {
                new GradientStop(Color.FromRgb(_newColor.R,byte.MinValue, _newColor.B), 0.0),
                new GradientStop(Color.FromRgb(_newColor.R,byte.MaxValue, _newColor.B), 1.0) }
            };

            BlueSlider.Background = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0),
                GradientStops = new GradientStopCollection {
                new GradientStop(Color.FromRgb(_newColor.R,_newColor.G,byte.MinValue), 0.0),
                new GradientStop(Color.FromRgb(_newColor.R,_newColor.G, byte.MaxValue), 1.0) }

            };


            HueSlider.Background =
                new LinearGradientBrush(ExtensionMethods.CreateHueGradient(SaturationNumBox.CurrentValue, ValueNumBox.CurrentValue / 100));

            SaturationSlider.Background =
                new LinearGradientBrush(ExtensionMethods.CreateSaturationGradient(HueNumBox.CurrentValue, ValueNumBox.CurrentValue / 100));

            ValueSlider.Background =
                new LinearGradientBrush(ExtensionMethods.CreateValueGradient(HueNumBox.CurrentValue, SaturationNumBox.CurrentValue / 100));


        }

        

        private void UpdateColorFromText()
        {
            if (!ColorAdusting)
            {
                Color color = default;

                try
                {
                    color = (Color)ColorConverter.ConvertFromString(HexNotation.Text);
                }
                catch { }

                if (color != default)
                {
                    NewColor = color;
                }

            }

        }

        private void OnColorTextChange(object sender, TextChangedEventArgs e)
        {
            UpdateColorFromText();
        }

        private void Click_Reset(object sender, RoutedEventArgs e)
        {
            NewColor = _oldColor;
            NewColorChosen?.Invoke(this, EventArgs.Empty);
        }

        private void Click_Set(object sender, RoutedEventArgs e)
        {
            NewColorChosen?.Invoke(this, EventArgs.Empty);
        }

        private void Click_OK(object sender, RoutedEventArgs e)
        {
            NewColorChosen?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}