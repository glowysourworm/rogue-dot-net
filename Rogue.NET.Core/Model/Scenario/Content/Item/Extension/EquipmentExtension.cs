using Rogue.NET.Core.Model.Enums;

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
    }
}
