using Rogue.NET.Core.Model.Enums;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter.ItemGrid
{
    public class ItemGridCombatTypeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null ||
                values.Any(x => x == DependencyProperty.UnsetValue) ||
                values.Length != 2)
                return Binding.DoNothing;

            var type = (EquipmentType)values[0];
            var combatType = (CharacterBaseAttribute)values[1];

            // TODO: Consider moving this calculation to an enum extension
            var isCombatType = (type == EquipmentType.Armor ||
                                type == EquipmentType.Belt ||
                                type == EquipmentType.Boots ||
                                type == EquipmentType.Gauntlets ||
                                type == EquipmentType.Helmet ||
                                type == EquipmentType.OneHandedMeleeWeapon ||
                                type == EquipmentType.RangeWeapon ||
                                type == EquipmentType.Shield ||
                                type == EquipmentType.Shoulder ||
                                type == EquipmentType.TwoHandedMeleeWeapon);

            return isCombatType ? combatType.ToString().Substring(0, 1) : "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
