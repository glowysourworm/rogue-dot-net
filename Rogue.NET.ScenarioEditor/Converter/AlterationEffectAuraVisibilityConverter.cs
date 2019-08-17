using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.ScenarioEditor.Converter
{
    public class AlterationEffectAuraVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            else if (value == DependencyProperty.UnsetValue)
                return Binding.DoNothing;

            else if (value is AttackAttributeAuraAlterationEffectTemplateViewModel)
                return Visibility.Visible;

            else if (value is AuraAlterationEffectTemplateViewModel)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
