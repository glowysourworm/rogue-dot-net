using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Item.Extension
{
    public static class EquipmentExtension
    {
        public static bool ClassApplies(this Equipment equipment)
        {
            return equipment.Type == EquipmentType.Armor ||
                   equipment.Type == EquipmentType.Boots ||
                   equipment.Type == EquipmentType.Gauntlets ||
                   equipment.Type == EquipmentType.Helmet ||
                   equipment.Type == EquipmentType.OneHandedMeleeWeapon ||
                   equipment.Type == EquipmentType.Shield ||
                   equipment.Type == EquipmentType.RangeWeapon ||
                   equipment.Type == EquipmentType.Shoulder ||
                   equipment.Type == EquipmentType.TwoHandedMeleeWeapon ||
                   equipment.Type == EquipmentType.Belt;
        }

        public static bool CanImbue(this Equipment equipment)
        {
            return equipment.Type == EquipmentType.Armor ||
                   equipment.Type == EquipmentType.Helmet ||
                   equipment.Type == EquipmentType.Shield ||
                   equipment.Type == EquipmentType.Shoulder ||
                   equipment.Type == EquipmentType.OneHandedMeleeWeapon ||
                   equipment.Type == EquipmentType.TwoHandedMeleeWeapon;
        }

        public static bool IsArmorType(this Equipment equipment)
        {
            return equipment.Type == EquipmentType.Armor ||
                   equipment.Type == EquipmentType.Belt ||
                   equipment.Type == EquipmentType.Boots ||
                   equipment.Type == EquipmentType.Gauntlets ||
                   equipment.Type == EquipmentType.Helmet ||
                   equipment.Type == EquipmentType.Shield ||
                   equipment.Type == EquipmentType.Shoulder;
        }

        public static bool IsWeaponType(this Equipment equipment)
        {
            return  equipment.Type == EquipmentType.OneHandedMeleeWeapon ||
                    equipment.Type == EquipmentType.RangeWeapon ||
                    equipment.Type == EquipmentType.TwoHandedMeleeWeapon;
        }

        public static double GetAttackValue(this Equipment equipment)
        {
            if (equipment.IsArmorType())
                return 0;

            // TODO: Create static constants for the divisors
            switch (equipment.Type)
            {
                case EquipmentType.OneHandedMeleeWeapon:
                    return ((equipment.Class + 1) * equipment.Quality);
                case EquipmentType.TwoHandedMeleeWeapon:
                    return ((equipment.Class + 1) * equipment.Quality) * 2;
                case EquipmentType.RangeWeapon:
                    return ((equipment.Class + 1) * equipment.Quality) / 2;
                default:
                    throw new Exception("Trying to calculate Attack Value for " + equipment.Type.ToString());
            }
        }

        public static double GetDefenseValue(this Equipment equipment)
        {
            if (equipment.IsWeaponType())
                return 0;

            // TODO: Create static constants for the divisors
            switch (equipment.Type)
            {
                case EquipmentType.Armor:
                    return ((equipment.Class + 1) * equipment.Quality);
                case EquipmentType.Shoulder:
                    return (((equipment.Class + 1) * equipment.Quality) / 5);
                case EquipmentType.Boots:
                    return (((equipment.Class + 1) * equipment.Quality) / 10);
                case EquipmentType.Gauntlets:
                    return (((equipment.Class + 1) * equipment.Quality) / 10);
                case EquipmentType.Belt:
                    return (((equipment.Class + 1) * equipment.Quality) / 8);
                case EquipmentType.Shield:
                    return (((equipment.Class + 1) * equipment.Quality) / 3);
                case EquipmentType.Helmet:
                    return (((equipment.Class + 1) * equipment.Quality) / 5);
                default:
                    throw new Exception("Trying to calculate defense value for " + equipment.Type.ToString());
            }
        }
    }
}
