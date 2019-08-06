using System;

namespace Rogue.NET.Core.Model.Enums
{
    public enum AlterationTargetType : int
    {
        Source = 1,
        Target = 2,
        AllInRange = 3,
        AllInRangeExceptSource = 4
    }
    public enum AlterationRandomPlacementType : int
    {
        InLevel = 1,
        InRangeOfCharacter = 2,
        InPlayerVisibleRange = 3
    }
    public enum AlterationOtherEffectType : int
    {
        Steal = 1,
        RunAway = 2,
        Identify = 3,
        Uncurse = 4
    }
    [Flags]
    public enum AlterationRevealType : int
    {
        Items = 1,
        Monsters = 2,
        SavePoint = 4,
        Food = 8,
        Layout = 16,
    }
    public enum AlterationModifyEquipmentType : int
    {
        ArmorClass = 1,
        ArmorImbue = 2,
        ArmorQuality = 3,
        WeaponClass = 4,
        WeaponImbue = 5,
        WeaponQuality = 6
    }
    public enum AlterationAttackAttributeApplicationType : int
    {
        Passive = 1,
        Temporary = 2,
        Melee = 3,
        Aura = 4
    }
    public enum AlterationAttackAttributeCombatType : int
    {
        Friendly = 1,
        Malign = 2
    }
}
