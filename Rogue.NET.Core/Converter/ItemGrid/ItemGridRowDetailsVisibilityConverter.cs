using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter.ItemGrid
{
    public class ItemGridRowDetailsVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
            else
                return DataGridRowDetailsVisibilityMode.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
