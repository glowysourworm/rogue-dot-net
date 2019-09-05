using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter
{
    public class SumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null ||
                values.Any(x => x == DependencyProperty.UnsetValue))
                return Binding.DoNothing;

            // Using System.Convert - TypeConverter (?)
            return values.Sum(x => System.Convert.ToDouble(x));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
