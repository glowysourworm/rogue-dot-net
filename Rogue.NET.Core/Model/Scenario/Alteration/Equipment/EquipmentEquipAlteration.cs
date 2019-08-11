using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Equipment
{
    [Serializable]
    public class EquipmentEquipAlteration : AlterationContainer
    {
        public AuraSourceParameters AuraParameters { get; set; }

        public EquipmentEquipAlteration()
        {
            this.AuraParameters = new AuraSourceParameters();
        }

        protected override bool ValidateEffectType()
        {
            return this.Effect is IEquipmentEquipAlterationEffect;
        }
    }
}
