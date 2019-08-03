using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Equipment
{
    [Serializable]
    public class EquipmentEquipAlteration : RogueBase
    {
        public IEquipmentEquipAlterationEffect Effect { get; set; }

        public EquipmentEquipAlteration()
        {
        }
    }
}
