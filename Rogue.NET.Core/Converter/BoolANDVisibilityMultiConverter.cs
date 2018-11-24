using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter
{
    public class BoolANDVisibilityMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Binding.DoNothing;

            else if (values.Any(x => x == DependencyProperty.UnsetValue))
                return Binding.DoNothing;

            else if (values.Any(x => x.GetType() != typeof(bool)))
                return Binding.DoNothing;

            return values.Cast<bool>().All(x => x) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes.Select(x => Binding.DoNothing).ToArray();
        }
    }
}
