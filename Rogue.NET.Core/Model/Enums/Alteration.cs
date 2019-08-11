using System;

namespace Rogue.NET.Core.Model.Enums
{
    public enum AlterationCostType : int
    {
        None = 0,
        OneTime = 1,
        PerStep = 2
    }
    /// <summary>
    /// Defines a way to block (completely negate Effects) Alterations using different stats
    /// </summary>
    public enum AlterationBlockType
    {
        /// <summary>
        /// Alteration can't be blocked
        /// </summary>
        NonBlockable = 0,

        /// <summary>
        /// Blocks Alteration using Base Intelligence ONLY
        /// </summary>
        Mental = 1,

        /// <summary>
        /// Blocks Alteration using Base Agility ONLY
        /// </summary>
        Physical = 2
    }
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
        Identify = 1,
        Uncurse = 2
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
    public enum AlterationAttackAttributeCombatType : int
    {
        /// <summary>
        /// For this type the attack attributes are aggregated to be applied during combat
        /// </summary>
        FriendlyAggregate = 1,

        /// <summary>
        /// For this type the attack attributes are used to fight the character (source, target,
        /// targets in range, in aura range, etc...) at the end of each turn. All attack attributes
        /// with this combat type are aggregated to be pitted against the character's other 
        /// attributes and any alteration that has FriendlyAggregate specified.
        /// </summary>
        MalignPerStep = 2
    }
}
