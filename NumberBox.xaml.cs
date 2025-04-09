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
    public partial class NumberBox : UserControl
    {
        bool OnlyPositive { get; set; }

        public double CurrentValue
        {
            get => (double)GetValue(CurrentValueProperty);
            set => SetValue(CurrentValueProperty, value);
        }
        public static readonly DependencyProperty CurrentValueProperty =
       DependencyProperty.Register("CurrentValue", typeof(double), typeof(NumberBox), new PropertyMetadata(default(double)));

        public double IntervalSize
        {
            get => (double)GetValue(IntervalSizeProperty);
            set => SetValue(IntervalSizeProperty, value);
        }
        public static readonly DependencyProperty IntervalSizeProperty =
       DependencyProperty.Register("IntervalSize", typeof(double), typeof(NumberBox), new PropertyMetadata(default(double)));


        public NumberBox()
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
            double val;
            bool isNum = double.TryParse(textBox.Text + text, out val);
            
            if (OnlyPositive)
            {
                return isNum && val >= 0;
            }
            return isNum;

        }


        private void UpButton(object sender, RoutedEventArgs e)
        {
            CurrentValue += IntervalSize;
        }

        private void DownButton(object sender, RoutedEventArgs e)
        {
            CurrentValue -= IntervalSize;
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

