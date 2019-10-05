using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter
{
    public class GreaterThanOrEqualParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                value == DependencyProperty.UnsetValue ||
                parameter == null)
                return Binding.DoNothing;

            var threshold = (double)parameter;

            return (double)value > threshold;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
