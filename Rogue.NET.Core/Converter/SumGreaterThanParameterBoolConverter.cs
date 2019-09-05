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
    public class SumGreaterThanParameterBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null ||
                values.Any(x => x == DependencyProperty.UnsetValue) ||
                parameter == null)
                return Binding.DoNothing;

            // Using System.Convert - TypeConverter (?)
            var sum = values.Sum(x => System.Convert.ToDouble(x));
            var limit = System.Convert.ToDouble(parameter);

            return sum > limit;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
