using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Equipment
{
    [Serializable]
    public class EquipmentCurseAlteration : AlterationContainer
    {
        public AuraSourceParameters AuraParameters { get; set; }

        public EquipmentCurseAlteration()
        {
            this.AuraParameters = new AuraSourceParameters();
        }

        protected override bool ValidateEffectType()
        {
            return this.Effect is IEquipmentCurseAlterationEffect;
        }
    }
}
