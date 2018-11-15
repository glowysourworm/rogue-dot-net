using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid
{
    public enum ItemGridModes
    {
        Inventory,
        Consumable,
        Equipment,
        Identify,
        Uncurse,
        Enchant,
        Imbue
    }
    public enum ItemGridActions
    {
        Consume,
        Drop,
        Enchant,
        Equip,
        Identify,
        Imbue,
        Throw,
        Uncurse
    }
}
