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

        public override Type EffectInterfaceType
        {
            get { return typeof(IEquipmentEquipAlterationEffect); }
        }
    }
}
