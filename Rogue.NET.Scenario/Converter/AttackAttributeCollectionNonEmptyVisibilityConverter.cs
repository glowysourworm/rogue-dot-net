using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Scenario.Converter
{
    public class AttackAttributeCollectionNonEmptyVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as ICollection<AttackAttributeViewModel>;

            if (collection == null)
                return Binding.DoNothing;

            if (collection.Any(x => x == DependencyProperty.UnsetValue))
                return Binding.DoNothing;

            return collection.Any(x => x.IsSet()) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
