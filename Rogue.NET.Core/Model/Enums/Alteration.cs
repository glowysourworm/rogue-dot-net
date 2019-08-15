using System;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Non Blockable",
                 Description = "Alteration can't be blocked")]
        NonBlockable = 0,

        [Display(Name = "Mental",
                 Description = "Alteration must be blocked using the Intelligence attribute")]
        Mental = 1,

        [Display(Name = "Physical",
                 Description = "Alteration must be blocked using the Agility attribute")]
        Physical = 2
    }
    public enum AlterationTargetType : int
    {
        [Display(Name = "Source",
                 Description = "Effect applied to source character only")]
        Source = 1,

        [Display(Name = "Target",
                 Description = "Effect applied to target character only")]
        Target = 2,

        [Display(Name = "All In Range",
                 Description = "Effect applied to all characters in numeric range of source (including source character)")]
        AllInRange = 3,

        [Display(Name = "All In Range (Except Source)",
                 Description = "Effect applied to all characters in numeric range of source")]
        AllInRangeExceptSource = 4
    }
    public enum AlterationRandomPlacementType : int
    {
        [Display(Name = "In Level",
                 Description = "Character placed on random open tile in level")]
        InLevel = 1,

        [Display(Name = "In Range Of Character",
                 Description = "Character placed on random open tile in numeric range of source character")]
        InRangeOfCharacter = 2,

        [Display(Name = "In Player Visible Range",
                 Description = "Character placed on random open tile in visible range of player")]
        InPlayerVisibleRange = 3
    }
    public enum AlterationOtherEffectType : int
    {
        [Display(Name = "Identify",
                 Description = "Identifies a player item")]
        Identify = 1,

        [Display(Name = "Identify",
                 Description = "Removes curse from player item")]
        Uncurse = 2
    }
    [Flags]
    public enum AlterationRevealType : int
    {
        [Display(Name = "Reveal Items",
                 Description = "Reveals all items in the level")]
        Items = 1,

        [Display(Name = "Reveal Enemies",
                 Description = "Reveals all enemies in the level")]
        Monsters = 2,

        [Display(Name = "Reveal Save Point",
                 Description = "Reveals save point (non-survivor mode only)")]
        SavePoint = 4,

        [Display(Name = "Reveal Food",
                 Description = "Reveals food items")]
        Food = 8,

        [Display(Name = "Reveal Layout",
                 Description = "Reveals level layout")]
        Layout = 16,
    }
    public enum AlterationModifyEquipmentType : int
    {
        [Display(Name = "Armor Class",
                 Description = "Modifies Armor Class Attribute")]
        ArmorClass = 1,

        [Display(Name = "Armor Imbue",
                 Description = "Modifies Armor Attack Attributes")]
        ArmorImbue = 2,

        [Display(Name = "Armor Quality",
                 Description = "Modifies Armor Quality Attribute")]
        ArmorQuality = 3,

        [Display(Name = "Weapon Class",
                 Description = "Modifies Weapon Class Attribute")]
        WeaponClass = 4,

        [Display(Name = "Weapon Imbue",
                 Description = "Modifies Weapon Imbue Attribute")]
        WeaponImbue = 5,

        [Display(Name = "Weapon Quality",
                 Description = "Modifies Weapon Quality Attribute")]
        WeaponQuality = 6
    }
    public enum AlterationAttackAttributeCombatType : int
    {
        /// <summary>
        /// For this type the attack attributes are aggregated to be applied during combat
        /// </summary>
        [Display(Name = "Friendly (Aggregate)",
                 Description = "Attack Attributes are aggregated along with the affected characters' other attributes and applied to their combat turns")]
        FriendlyAggregate = 1,

        /// <summary>
        /// For this type the attack attributes are used to fight the character (source, target,
        /// targets in range, in aura range, etc...) at the end of each turn. All attack attributes
        /// with this combat type are aggregated to be pitted against the character's other 
        /// attributes and any alteration that has FriendlyAggregate specified.
        /// </summary>
        [Display(Name = "Malign (Per Step)",
                 Description = "Attack Attributes are applied as an attack each turn against the affected characters")]
        MalignPerStep = 2
    }
}
