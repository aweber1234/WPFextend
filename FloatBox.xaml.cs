using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static System.Net.Mime.MediaTypeNames;

namespace WPFextend
{
    /// <summary>
    /// Interaction logic for FloatBox.xaml
    /// </summary>
    public partial class FloatBox : UserControl
    {
        private double _currentValue;

        private double _intervalSize;



        bool OnlyPositive { get; set; }

        public double IntervalSize
        {
            get
            {
                return _intervalSize;
            }
            set
            {
                _intervalSize = Math.Abs(value);
            }
        }


        public float CurrentValue
        {
            get
            {
                return (float)_currentValue;
            }
            set
            {
                _currentValue = value;
                textBox.Text = CurrentValue.ToString();
            }
        }


        public FloatBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Determines if entered text is valid and discards the input if it is not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (IsTextAllowed(textBox.Text + e.Text))
            {

                //textBox.Text = CurrentValue.ToString();
            }
            else { e.Handled = true; }


        }


        private bool IsTextAllowed(string text)
        {
            float val;
            bool IsFloat = float.TryParse(text, out val);

            if (OnlyPositive)
            {
                return IsFloat && val >= 0;
            }

            return IsFloat;
        }


        private void UpButton(object sender, RoutedEventArgs e)
        {
            CurrentValue += (float)_intervalSize;
        }

        private void DownButton(object sender, RoutedEventArgs e)
        {
            CurrentValue -= (float)_intervalSize;
        }

        private void OnTextChange(object sender, TextChangedEventArgs e)
        {
            if (textBox.IsKeyboardFocused) { double.TryParse(textBox.Text, out _currentValue); }
        }

        /// <summary>
        /// Discards pasted content if it is not valid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}

