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
        DependencyProperty.Register
            ("CurrentValue",            
            typeof(double),            
            typeof(NumberBox),            
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnValueChanged,
                 new CoerceValueCallback(CoerceValue)));

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(
            "ValueChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<double>),
            typeof(NumberBox));

        public event RoutedPropertyChangedEventHandler<double> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }



        public double IntervalSize
        {
            get => (double)GetValue(IntervalSizeProperty);
            set => SetValue(IntervalSizeProperty, value);
        }
        public static readonly DependencyProperty IntervalSizeProperty =
       DependencyProperty.Register("IntervalSize", typeof(double), typeof(NumberBox), new PropertyMetadata(default(double)));

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }  
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof(double),
                typeof(NumberBox),
                new FrameworkPropertyMetadata(double.MaxValue,
                    new PropertyChangedCallback(OnConstraintChanged)));

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }
        // MinimumValue property
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof(double),
                typeof(NumberBox),
                new FrameworkPropertyMetadata(double.MinValue,
                    new PropertyChangedCallback(OnConstraintChanged)));



        public NumberBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            var control = (NumberBox)d;
            double value = (double)baseValue;

            // Ensure value doesn't go below minimum
            if (value < control.Minimum)
                return control.Minimum;

            if (value > control.Maximum)
                return control.Maximum;

            return value;
        }

        private static void OnConstraintChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumberBox)d;
            control.CoerceValue(CurrentValueProperty);
        }



        // Property changed callback
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumberBox)d;
            control.OnValueChanged(e);
        }

        // Method to raise the event
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            RoutedPropertyChangedEventArgs<double> args = new RoutedPropertyChangedEventArgs<double>(
                (double)e.OldValue,
                (double)e.NewValue,
                ValueChangedEvent);
            RaiseEvent(args);
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

    public static class TextBoxBehavior
    {
        public static readonly DependencyProperty EnableEnterSubmitProperty =
            DependencyProperty.RegisterAttached(
                "EnableEnterSubmit",
                typeof(bool),
                typeof(TextBoxBehavior),
                new PropertyMetadata(false, OnEnableEnterSubmitChanged));

        public static bool GetEnableEnterSubmit(DependencyObject obj)
            => (bool)obj.GetValue(EnableEnterSubmitProperty);

        public static void SetEnableEnterSubmit(DependencyObject obj, bool value)
            => obj.SetValue(EnableEnterSubmitProperty, value);

        private static void OnEnableEnterSubmitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                {
                    textBox.KeyDown += TextBox_KeyDown;
                }
                else
                {
                    textBox.KeyDown -= TextBox_KeyDown;
                }
            }
        }

        private static void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = (TextBox)sender;
                // Update the binding source immediately
                var binding = textBox.GetBindingExpression(TextBox.TextProperty);
                binding?.UpdateSource();

                // Optional: Move focus to next control
                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}

