using System;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter.AttackAttribute
{
    public class AttackAttributeVibilityCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            if (value is int)
                return (int)value <= 0 ? Visibility.Collapsed : Visibility.Visible;

            var truncatedValue = Math.Floor((double)value);

            if (truncatedValue <= 0)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
