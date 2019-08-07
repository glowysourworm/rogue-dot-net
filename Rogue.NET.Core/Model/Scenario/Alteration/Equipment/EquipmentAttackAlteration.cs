using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Equipment
{
    [Serializable]
    public class EquipmentAttackAlteration : AlterationBase
    {
        public EquipmentAttackAlteration()
        {
        }
        public EquipmentAttackAlteration(string guid) : base(guid)
        {
        }
        protected override bool ValidateEffectInterfaceType()
        {
            return this.Effect is IEquipmentAttackAlterationEffect;
        }
    }
}
