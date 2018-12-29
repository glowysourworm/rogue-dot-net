using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid
{
    /// <summary>
    /// Each mode relates the Item Grid to a different collection of items
    /// </summary>
    public enum ItemGridModes
    {
        Consumable,
        Equipment,
        Identify,
        Uncurse,
        EnchantArmor,
        EnchantWeapon,
        ImbueArmor,
        ImbueWeapon
    }

    /// <summary>
    /// Each Intended action relates to a back-end supported process
    /// </summary>
    public enum ItemGridActions
    {
        Consume,
        Drop,
        EnchantWeapon,
        EnchantArmor,
        Equip,
        Identify,
        ImbueArmor,
        ImbueWeapon,
        Throw,
        Uncurse
    }
}
