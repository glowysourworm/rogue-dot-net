using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter.ItemGrid
{
    public class ItemGridZeroValueParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            else if (value == DependencyProperty.UnsetValue)
                return Binding.DoNothing;

            else if (parameter == null)
                return Binding.DoNothing;

            else if (!(parameter is string))
                return Binding.DoNothing;

            var doubleValue = (double)value;
            var zeroValue = (string)parameter;

            return doubleValue == 0D ? zeroValue : doubleValue.ToString("F1");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
