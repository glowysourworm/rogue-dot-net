using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Converter
{
    public class CharacterStatsValueForegroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null ||
                values.Length != 2 ||
                values.Any(x => x == DependencyProperty.UnsetValue) ||
                parameter == null)
                return Binding.DoNothing;

            var baseValue = (double)values[0];
            var value = (double)values[1];
            var invert = (bool)parameter;

            if ((value - baseValue) > 0)
                return invert ? Brushes.Red : Brushes.GreenYellow;

            else if ((value - baseValue) < 0)
                return invert ? Brushes.GreenYellow : Brushes.Red;

            else
                return Brushes.White;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
