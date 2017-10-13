using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace TimeTracker.Controls
{
    public class SpinTextBox : ClickSelectTextBox
    {
        public SpinTextBox()
        {
            HorizontalContentAlignment = HorizontalAlignment.Right;
            AddHandler(PreviewKeyDownEvent, new KeyEventHandler(HandleUpDownButtons), true);

            Binding binding = new Binding("Value")
            {
                Source = this,
                Mode = BindingMode.TwoWay
            };

            SetBinding(TextProperty, binding);
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        private void HandleUpDownButtons(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Up)
            {
                Value++;
                SelectAll();
                e.Handled = true;
            }
            if(e.Key == Key.Down)
            {
                Value--;
                SelectAll();
                e.Handled = true;
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(double),
                typeof(SpinTextBox),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnValueChanged, null, false, UpdateSourceTrigger.PropertyChanged));

        private static void OnValueChanged(
            DependencyObject d, 
            DependencyPropertyChangedEventArgs e)
        {
            var spinTextBox = (SpinTextBox)d;
            spinTextBox.ApplyMinMax();
        }

        private void ApplyMinMax()
        {
            if (Value > Max)
                Value = Min;

            if (Value < Min)
                Value = Max;
        }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register(
                "Max",
                typeof(double),
                typeof(SpinTextBox),
                new PropertyMetadata(double.MaxValue));

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register(
                "Min",
                typeof(double),
                typeof(SpinTextBox),
                new PropertyMetadata(double.MinValue));
    }
}
