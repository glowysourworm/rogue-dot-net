using System;
using System.Globalization;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter
{
    public class ParameterEqualityInverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Binding.DoNothing;

            return !value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO: Might need to return a default value for the parameter type for two-way binding
            return parameter;
        }
    }
}
