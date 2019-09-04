using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.Core.Converter.ItemGrid
{
    public class ItemGridFontStyleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 4 || values.Any(x => x == DependencyProperty.UnsetValue))
                return Brushes.White;

            bool equipped = (bool)values[0];
            bool cursed = (bool)values[1];
            bool objective = (bool)values[2];
            bool unique = (bool)values[3];

            if (objective || unique)
                return FontStyles.Italic;

            return FontStyles.Normal;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
