using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;
using Rogue.NET.Scenario.ViewModel.ItemGrid;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Scenario.Converter
{
    public class ItemGridButtonVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Visibility.Collapsed;

            else if (values.Any(x => x == DependencyProperty.UnsetValue))
                return Visibility.Collapsed;

            else if (values.Length != 2)
                return Visibility.Collapsed;

            var items = values[0] as IEnumerable<ItemGridRowViewModel>;
            var intendedAction = (ItemGridActions)values[1];

            if (items != null)
            {
                return items.Any(x => x.ConsumeEnable && intendedAction == ItemGridActions.Consume ||
                                      x.EquipEnable && intendedAction == ItemGridActions.Equip ||
                                      x.IdentifyEnable && intendedAction == ItemGridActions.Identify ||
                                      x.UncurseEnable && intendedAction == ItemGridActions.Uncurse ||
                                      x.EnchantArmorEnable && intendedAction == ItemGridActions.EnchantArmor ||
                                      x.EnchantWeaponEnable && intendedAction == ItemGridActions.EnchantWeapon ||
                                      x.ImbueArmorEnable && intendedAction == ItemGridActions.ImbueArmor ||
                                      x.ImbueWeaponEnable && intendedAction == ItemGridActions.ImbueWeapon ||
                                      x.ThrowEnable && intendedAction == ItemGridActions.Throw ||
                                      x.DropEnable && intendedAction == ItemGridActions.Drop) ?

                                      Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
