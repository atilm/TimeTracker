using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TimeTracker.Controls
{
    public partial class TimePicker : UserControl
    {
        public TimePicker()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        public int Hours
        {
            get { return (int)GetValue(HoursProperty); }
            set { SetValue(HoursProperty, value); }
        }

        public int Minutes
        {
            get { return (int)GetValue(MinutesProperty); }
            set { SetValue(MinutesProperty, value); }
        }

        public DateTime DateTime
        {
            get { return (DateTime)GetValue(DateTimeProperty); }
            set { SetValue(DateTimeProperty, value); }
        }

        public static readonly DependencyProperty HoursProperty =
            DependencyProperty.Register(
                "Hours", 
                typeof(int), 
                typeof(TimePicker), 
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnHoursChanged, null, false, UpdateSourceTrigger.PropertyChanged));

        public static readonly DependencyProperty MinutesProperty =
            DependencyProperty.Register("Minutes",
                typeof(int),
                typeof(TimePicker),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnMinutesChanged, null, false, UpdateSourceTrigger.PropertyChanged));

        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register(
                "DateTime",
                typeof(DateTime),
                typeof(TimePicker),
                new FrameworkPropertyMetadata(new DateTime(), 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
                    OnDateTimeChanged, null, false, UpdateSourceTrigger.PropertyChanged));

        private static void OnHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timePicker = d as TimePicker;
            var dt = timePicker.DateTime;
            timePicker.SetCurrentValue(DateTimeProperty, dt.Date.AddHours(timePicker.Hours).AddMinutes(dt.Minute));
        }

        private static void OnMinutesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timePicker = d as TimePicker;
            var dt = timePicker.DateTime;
            timePicker.SetCurrentValue(DateTimeProperty, dt.Date.AddHours(dt.Hour).AddMinutes(timePicker.Minutes));
        }

        private static void OnDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timePicker = d as TimePicker;
            timePicker.SetCurrentValue(HoursProperty, timePicker.DateTime.Hour);
            timePicker.SetCurrentValue(MinutesProperty, timePicker.DateTime.Minute);
        }
    }
}
