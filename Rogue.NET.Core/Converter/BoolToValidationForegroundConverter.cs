using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.Core.Converter
{
    public class BoolToValidationForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            return (bool)value ? Binding.DoNothing : Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
