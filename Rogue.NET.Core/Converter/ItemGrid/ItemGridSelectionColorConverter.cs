using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.Core.Converter.ItemGrid
{
    public class ItemGridSelectionColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return Colors.YellowGreen;

            return Colors.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
