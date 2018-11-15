using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Content
{
    [Export(typeof(IItemProcessor))]
    public class ItemProcessor : IItemProcessor
    {
        public bool ClassApplies(EquipmentType type)
        {
            return type == EquipmentType.Armor ||
                   type == EquipmentType.Boots ||
                   type == EquipmentType.Gauntlets ||
                   type == EquipmentType.Helmet ||
                   type == EquipmentType.OneHandedMeleeWeapon ||
                   type == EquipmentType.Shield ||
                   type == EquipmentType.RangeWeapon ||
                   type == EquipmentType.Shoulder ||
                   type == EquipmentType.TwoHandedMeleeWeapon ||
                   type == EquipmentType.Belt;
        }
    }
}
