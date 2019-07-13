using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter
{
    /// <summary>
    /// Converter that returns the specified parameter if all the boolean values evaluate to true. Else
    /// returns Binding.DoNothing.
    /// </summary>
    public class BoolANDParameterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null ||
                values.Any(x => x == DependencyProperty.UnsetValue))
                return Binding.DoNothing;

            return values.All(x => (bool)x) ? parameter : Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
