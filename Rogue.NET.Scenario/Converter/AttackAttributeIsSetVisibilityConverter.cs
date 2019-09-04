using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
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
    public class AttackAttributeIsSetVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var attackAttribute = value as AttackAttributeViewModel;

            if (attackAttribute == null)
                return Binding.DoNothing;

            if (attackAttribute == DependencyProperty.UnsetValue)
                return Binding.DoNothing;

            return attackAttribute.IsSet() ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
