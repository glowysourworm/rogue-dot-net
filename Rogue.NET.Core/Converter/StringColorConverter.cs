using Rogue.NET.Core.Media.SymbolEffect.Utility;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.Core.Converter
{
    public class StringColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Colors.Transparent;

            return ColorFilter.Convert((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "#00000000";

            return ColorFilter.ConvertBack((Color)value);
        }
    }
}
