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
    /// Interaction logic for IntegerBox.xaml
    /// </summary>
    public partial class IntegerBox : UserControl
    {
        public double _currentValue;
        bool OnlyPositive { get; set; }

        public double CurrentValue
        {
            get
            {
                return _currentValue;
            }
            set
            {
                _currentValue = value;
                //textBox.Text = CurrentValue.ToString();
            }
        }

        public static readonly DependencyProperty CurrentValueProperty =
        DependencyProperty.Register("CurrentValue", typeof(double), typeof(IntegerBox), new PropertyMetadata(default(double)));


        public IntegerBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (IsTextAllowed(e.Text))
            {

                //textBox.Text = CurrentValue.ToString();
            }
            else { e.Handled = true; }


        }


        private bool IsTextAllowed(string text)
        {
            int val;
            bool isInt = Int32.TryParse(text, out val);

            if (OnlyPositive)
            {
                return isInt && val >= 0;
            }

            return isInt;
        }


        private void UpButton(object sender, RoutedEventArgs e)
        {
            _currentValue++;
        }

        private void DownButton(object sender, RoutedEventArgs e)
        {
            _currentValue--;
        }

        private void OnTextChange(object sender, TextChangedEventArgs e)
        {
            Int32.TryParse(textBox.Text, out _);
        }

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

