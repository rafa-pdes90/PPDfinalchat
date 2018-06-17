using System;
using System.Globalization;
using System.Windows.Data;

namespace Chat.Converters
{
    [ValueConversion(typeof(object), typeof(double))]
    public class HalfishWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return (double?) value * 0.42;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
