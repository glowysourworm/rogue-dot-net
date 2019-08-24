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
        Source = 0,

        [Display(Name = "Target",
                 Description = "Effect applied to target character only")]
        Target = 1,

        [Display(Name = "All In Range",
                 Description = "Effect applied to all characters in numeric range of source (including source character)")]
        AllInRange = 2,

        [Display(Name = "All In Range (Except Source)",
                 Description = "Effect applied to all characters in numeric range of source")]
        AllInRangeExceptSource = 3
    }
    public enum AlterationRandomPlacementType : int
    {
        [Display(Name = "In Level",
                 Description = "Character placed on random open tile in level")]
        InLevel = 0,

        [Display(Name = "In Range Of Character",
                 Description = "Character placed on random open tile in numeric range of source character")]
        InRangeOfCharacter = 1,

        [Display(Name = "In Player Visible Range",
                 Description = "Character placed on random open tile in visible range of player")]
        InPlayerVisibleRange = 2
    }
    public enum AlterationOtherEffectType : int
    {
        [Display(Name = "Identify",
                 Description = "Identifies a player item")]
        Identify = 0,

        [Display(Name = "Uncurse",
                 Description = "Removes curse from player item")]
        Uncurse = 1
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

        [Display(Name = "Reveal Stairs",
                 Description = "Reveals stairs up and stairs down (if applicable)")]
        Stairs = 32,

        [Display(Name = "Reveal Scenario Objects",
                 Description = "Reveals scenario objects - including hidden ones")]
        ScenarioObjects = 64
    }
    public enum AlterationModifyEquipmentType : int
    {
        [Display(Name = "Armor Class",
                 Description = "Modifies Armor Class Attribute")]
        ArmorClass = 0,

        [Display(Name = "Armor Imbue",
                 Description = "Modifies Armor Attack Attributes")]
        ArmorImbue = 1,

        [Display(Name = "Armor Quality",
                 Description = "Modifies Armor Quality Attribute")]
        ArmorQuality = 2,

        [Display(Name = "Weapon Class",
                 Description = "Modifies Weapon Class Attribute")]
        WeaponClass = 3,

        [Display(Name = "Weapon Imbue",
                 Description = "Modifies Weapon Imbue Attribute")]
        WeaponImbue = 4,

        [Display(Name = "Weapon Quality",
                 Description = "Modifies Weapon Quality Attribute")]
        WeaponQuality = 5
    }
    public enum AlterationAttackAttributeCombatType : int
    {
        /// <summary>
        /// For this type the attack attributes are aggregated to be applied during combat
        /// </summary>
        [Display(Name = "Friendly (Aggregate)",
                 Description = "Attack Attributes are aggregated along with the affected characters' other attributes and applied to their combat turns")]
        FriendlyAggregate = 0,

        /// <summary>
        /// For this type the attack attributes are used to fight the character (source, target,
        /// targets in range, in aura range, etc...) at the end of each turn. All attack attributes
        /// with this combat type are aggregated to be pitted against the character's other 
        /// attributes and any alteration that has FriendlyAggregate specified.
        /// </summary>
        [Display(Name = "Malign (Per Step)",
                 Description = "Attack Attributes are applied as an attack each turn against the affected characters")]
        MalignPerStep = 1
    }
    public enum AlterationEquipmentModifyType
    {
        [Display(Name = "Class (Enchant)",
                 Description = "Modifies class parameter of an item")]
        Class,

        [Display(Name = "Quality (Enhance)",
                 Description = "Modifies quality parameter of an item")]
        Quality,

        [Display(Name = "Attack Attribute (Imbue)",
                 Description = "Modifies attack attributes of an item")]
        AttackAttribute
    }
}
