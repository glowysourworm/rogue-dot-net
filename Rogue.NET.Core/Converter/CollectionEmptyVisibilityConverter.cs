using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter
{
    public class CollectionEmptyVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as ICollection;
            if (collection == null)
                return Visibility.Collapsed;

            return collection.Count <= 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
