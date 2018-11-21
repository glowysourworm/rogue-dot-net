using Rogue.NET.Core.Model.Enums;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.Converter
{
    public class SymbolDetailsTemplateAuraToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return Colors.Transparent;

            if (values.Length != 2)
                return Colors.Transparent;

            if (values.Any(x => x == DependencyProperty.UnsetValue))
                return Colors.Transparent;

            var color = (string)values[0];
            var type = (SymbolTypes)values[1];

            return type != SymbolTypes.Smiley ? Colors.Transparent : ColorConverter.ConvertFromString(color);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
