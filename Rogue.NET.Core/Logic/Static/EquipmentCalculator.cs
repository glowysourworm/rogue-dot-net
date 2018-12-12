using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Static
{
    /// <summary>
    /// Common methods used for calculating equipment values
    /// </summary>
    public static class EquipmentCalculator
    {
        public static bool IsArmorType(EquipmentType type)
        {
            return type == EquipmentType.Armor ||
                   type == EquipmentType.Belt ||
                   type == EquipmentType.Boots ||
                   type == EquipmentType.Gauntlets ||
                   type == EquipmentType.Helmet ||
                   type == EquipmentType.Shield ||
                   type == EquipmentType.Shoulder;
        }

        public static bool IsWeaponType(EquipmentType type)
        {
            return type == EquipmentType.OneHandedMeleeWeapon ||
                    type == EquipmentType.RangeWeapon ||
                    type == EquipmentType.TwoHandedMeleeWeapon;
        }

        public static double GetAttackValue(EquipmentType equipmentType, int classs, double quality)
        {
            switch (equipmentType)
            {
                case EquipmentType.OneHandedMeleeWeapon:
                    return ((classs + 1) * quality) * ModelConstants.EquipmentMultipliers.OneHandedMeleeWeapon;
                case EquipmentType.TwoHandedMeleeWeapon:
                    return ((classs + 1) * quality) * ModelConstants.EquipmentMultipliers.TwoHandedMeleeWeapon;
                case EquipmentType.RangeWeapon:
                    return ((classs + 1) * quality) * ModelConstants.EquipmentMultipliers.RangeWeapon;
                default:
                    throw new Exception("Trying to calculate Attack Value for " + equipmentType.ToString());
            }
        }

        public static double GetDefenseValue(EquipmentType equipmentType, int classs, double quality)
        {
            switch (equipmentType)
            {
                case EquipmentType.Armor:
                    return ((classs + 1) * quality) * ModelConstants.EquipmentMultipliers.Armor;
                case EquipmentType.Shoulder:
                    return (((classs + 1) * quality) * ModelConstants.EquipmentMultipliers.Shoulder);
                case EquipmentType.Boots:
                    return (((classs + 1) * quality) * ModelConstants.EquipmentMultipliers.Boots);
                case EquipmentType.Gauntlets:
                    return (((classs + 1) * quality) * ModelConstants.EquipmentMultipliers.Gauntlets);
                case EquipmentType.Belt:
                    return (((classs + 1) * quality) * ModelConstants.EquipmentMultipliers.Belt);
                case EquipmentType.Shield:
                    return (((classs + 1) * quality) * ModelConstants.EquipmentMultipliers.Shield);
                case EquipmentType.Helmet:
                    return (((classs + 1) * quality) * ModelConstants.EquipmentMultipliers.Helmet);
                default:
                    throw new Exception("Trying to calculate defense value for " + equipmentType.ToString());
            }
        }
    }
}
