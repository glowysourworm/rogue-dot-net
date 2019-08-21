using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;
using Rogue.NET.Scenario.ViewModel.ItemGrid;
using System;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Scenario.Converter
{
    public class ItemGridRowButtonVisibilitiyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Visibility.Collapsed;

            else if (values.Any(x => x == DependencyProperty.UnsetValue))
                return Visibility.Collapsed;

            else if (values.Length != 13)
                return Visibility.Collapsed;

            var intendedAction = (ItemGridActions)values[0];

            return ((bool)values[1] && intendedAction == ItemGridActions.Consume ||
                    (bool)values[2] && intendedAction == ItemGridActions.Equip ||
                    (bool)values[3] && intendedAction == ItemGridActions.Identify ||
                    (bool)values[4] && intendedAction == ItemGridActions.Uncurse ||
                    (bool)values[5] && intendedAction == ItemGridActions.EnchantArmor ||
                    (bool)values[6] && intendedAction == ItemGridActions.EnchantWeapon ||
                    (bool)values[7] && intendedAction == ItemGridActions.ImbueArmor ||
                    (bool)values[8] && intendedAction == ItemGridActions.ImbueWeapon ||
                    (bool)values[9] && intendedAction == ItemGridActions.EnhanceArmor ||
                    (bool)values[10] && intendedAction == ItemGridActions.EnhanceWeapon ||
                    (bool)values[11] && intendedAction == ItemGridActions.Throw ||
                    (bool)values[12] && intendedAction == ItemGridActions.Drop) ? Visibility.Visible : Visibility.Collapsed;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
