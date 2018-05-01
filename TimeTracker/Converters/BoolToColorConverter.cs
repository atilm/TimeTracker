using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TimeTracker.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
               object parameter, CultureInfo culture)
        {
            if (!(value is bool isActive))
                return inactiveColor;

            return isActive ?
                activeColor :
                inactiveColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        private readonly Color inactiveColor = Color.FromRgb(211, 211, 211);
        private readonly Color activeColor = Color.FromRgb(220, 20, 60);
    }
}
