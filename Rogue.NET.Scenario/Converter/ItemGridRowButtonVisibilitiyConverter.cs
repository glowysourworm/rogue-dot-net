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

            else if (values.Length != 2)
                return Visibility.Collapsed;

            var item = values[0] as ItemGridRowViewModel;
            var intendedAction = (ItemGridActions)values[1];

            if (item == null)
                return Visibility.Collapsed;

            return (item.ConsumeEnable && intendedAction == ItemGridActions.Consume ||
                    item.EquipEnable && intendedAction == ItemGridActions.Equip ||
                    item.IdentifyEnable && intendedAction == ItemGridActions.Identify ||
                    item.UncurseEnable && intendedAction == ItemGridActions.Uncurse ||
                    item.EnchantEnable && intendedAction == ItemGridActions.Enchant ||
                    item.ThrowEnable && intendedAction == ItemGridActions.Throw ||
                    item.DropEnable && intendedAction == ItemGridActions.Drop) ? Visibility.Visible : Visibility.Collapsed;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
