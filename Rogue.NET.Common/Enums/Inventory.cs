using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rogue.NET.Common
{
    public enum EquipmentType 
    {
        None,
        Armor,
        Shoulder,
        Belt,
        Helmet,
        Ring,
        Amulet,
        Boots,
        Gauntlets,
        OneHandedMeleeWeapon,
        TwoHandedMeleeWeapon,
        RangeWeapon,
        Shield,
        Orb,
        CompassGadget,
        EnemyScopeGadet,
        EquipmentGadget
    }
    public enum ConsumableType
    {
        OneUse,
        MultipleUses,
        UnlimitedUses
    }
    public enum ConsumableSubType
    {
        Scroll,
        Potion,
        Wand,
        Manual,
        Food,
        Ammo,
        Misc
    }
}
