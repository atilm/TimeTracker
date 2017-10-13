using System;
using System.Windows.Data;
using System.Windows.Media;

namespace TimeTracker.Converters
{
    public class ReferenceEqualityToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
               object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == values[1])
                return Color.FromRgb(220, 20, 60);
            else
                return Color.FromRgb(211, 211, 211);
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
               object parameter, System.Globalization.CultureInfo culture)
        {
            return new object[] { Binding.DoNothing, Binding.DoNothing };
        }
    }
}
