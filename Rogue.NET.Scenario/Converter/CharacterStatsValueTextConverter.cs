using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Scenario.Converter
{
    public class CharacterStatsValueTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null ||
                values.Length != 2 ||
                values.Any(x => x == DependencyProperty.UnsetValue))
                return Binding.DoNothing;

            var baseValue = (double)values[0];
            var value = (double)values[1];

            return value > 0 ? " + " + value.ToString("F2") :
                   value < 0 ? " - " + value.ToString("F2") :
                   "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
