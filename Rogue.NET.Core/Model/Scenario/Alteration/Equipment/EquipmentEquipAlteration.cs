using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Equipment
{
    [Serializable]
    public class EquipmentEquipAlteration : AlterationBase
    {
        public AuraSourceParameters AuraParameters { get; set; }

        public EquipmentEquipAlteration()
        {
            this.AuraParameters = new AuraSourceParameters();
        }
        public EquipmentEquipAlteration(string guid) : base(guid)
        {
            this.AuraParameters = new AuraSourceParameters();
        }

        protected override bool ValidateEffectInterfaceType()
        {
            return this.Effect is IEquipmentEquipAlterationEffect;
        }
    }
}
