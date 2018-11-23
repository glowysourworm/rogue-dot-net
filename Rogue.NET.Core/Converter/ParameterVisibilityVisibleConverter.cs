using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter
{
    /// <summary>
    /// Returns visible if the value is equal to the parameter value
    /// </summary>
    public class ParameterVisibilityVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Visibility.Collapsed;

            return value.Equals(parameter) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
