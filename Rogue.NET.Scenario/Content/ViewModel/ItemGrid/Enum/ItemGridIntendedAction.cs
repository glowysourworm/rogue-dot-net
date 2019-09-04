using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid.Enum
{
    /// <summary>
    /// Each Intended action relates to a back-end supported process
    /// </summary>
    public enum ItemGridIntendedAction
    {
        Consume,
        Drop,
        EnchantWeapon,
        EnchantArmor,
        ImbueArmor,
        ImbueWeapon,
        EnhanceWeapon,
        EnhanceArmor,
        Equip,
        Identify,
        Throw,
        Uncurse
    }
}
