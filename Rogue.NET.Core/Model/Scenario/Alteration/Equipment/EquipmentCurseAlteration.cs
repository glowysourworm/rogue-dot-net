using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Equipment
{
    [Serializable]
    public class EquipmentCurseAlteration : RogueBase
    {
        public IEquipmentCurseAlterationEffect Effect { get; set; }

        public EquipmentCurseAlteration()
        {
        }
    }
}
