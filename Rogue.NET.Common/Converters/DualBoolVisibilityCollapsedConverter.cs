using System;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Common.Converters
{
    public class DualBoolVisibilityCollapsedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return false;

            if (values.Length < 2)
                return false;

            if (values[0] is bool && values[1] is bool)
            {
                bool val1 = (bool)values[0];
                bool val2 = (bool)values[1];
                return (val1 && val2) ? Visibility.Visible : Visibility.Collapsed;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
