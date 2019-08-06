using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Equipment
{
    [Serializable]
    public class EquipmentCurseAlteration : RogueBase
    {
        public IEquipmentCurseAlterationEffect Effect { get; set; }

        // TODO:ALTERATION
        public AuraSourceParameters AuraParameters { get; set; }

        public EquipmentCurseAlteration()
        {
            this.AuraParameters = new AuraSourceParameters();
        }
    }
}
