using System.ComponentModel.DataAnnotations;

namespace Rogue.NET.Core.Model.Enums
{
    public enum EquipmentType 
    {
        None = 0,
        [Display(Name = "Armor",
                 Description = "The item is worn on your character's body and contriubtes to your defense")]
        Armor,
        [Display(Name = "Shoulder",
                 Description = "The item is worn on your character's shoulders and contriubtes to your defense")]
        Shoulder,
        [Display(Name = "Belt",
                 Description = "The item is worn on your character's waist and contriubtes to your defense")]
        Belt,
        [Display(Name = "Helmet",
                 Description = "The item is worn on your character's head and contriubtes to your defense")]
        Helmet,
        [Display(Name = "Ring",
                 Description = "The item is worn on your character's fingers and may have some effect on your character...")]
        Ring,
        [Display(Name = "Amulet",
                 Description = "The item is worn on your character's neck and may have some effect on your character...")]
        Amulet,
        [Display(Name = "Boots",
                 Description = "The item is worn on your character's feet and contributes to your defense")]
        Boots,
        [Display(Name = "Gauntlets",
                 Description = "The item is worn on your character's hands and contributes to your defense")]
        Gauntlets,
        [Display(Name = "One Handed Weapon",
                 Description = "The item is wielded by your character with one hand for attack")]
        OneHandedMeleeWeapon,
        [Display(Name = "Two Handed Weapon",
                 Description = "The item is wielded by your character with both hands for attack")]
        TwoHandedMeleeWeapon,
        [Display(Name = "Range Weapon",
                 Description = "The item is wielded by your character with both hands for ranged attack; and requires ammunition")]
        RangeWeapon,
        [Display(Name = "Shield",
                 Description = "The item is wielded by your character and contributes to your defense")]
        Shield,
        [Display(Name = "Orb",
                 Description = "The item mysteriously orbits your character's body and may have some effect on your character...")]
        Orb
    }
    public enum ConsumableType
    {
        [Display(Name = "One Use",
                 Description = "The item has one use")]
        OneUse,
        [Display(Name = "Multiple Uses",
                 Description = "The item has many uses")]
        MultipleUses,
        [Display(Name = "Unlimited Uses",
                 Description = "The consumable has unlimited uses")]
        UnlimitedUses
    }
    public enum ConsumableSubType
    {
        [Display(Name = "Scroll",
                 Description = "An item that must be read")]
        Scroll,
        [Display(Name = "Potion",
                 Description = "An item that must be drunk")]
        Potion,
        [Display(Name = "Wand",
                 Description = "An item that may have multiple uses")]
        Wand,
        [Display(Name = "Manual",
                 Description = "An item that teaches a skill to your character")]
        Manual,
        [Display(Name = "Food",
                 Description = "An item that to prevent starvation")]
        Food,
        [Display(Name = "Ammo",
                 Description = "An item that is consumed by a range weapon")]
        Ammo,
        [Display(Name = "Misc",
                 Description = "Items that have varied uses")]
        Misc,
        [Display(Name = "Note",
                 Description = "Special items that are read to provide part of the narrative")]
        Note
    }
}
