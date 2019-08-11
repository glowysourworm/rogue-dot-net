using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Equipment
{
    [Serializable]
    public class EquipmentAttackAlteration : AlterationContainer
    {
        public EquipmentAttackAlteration()
        {
        }

        protected override bool ValidateEffectType()
        {
            return this.Effect is IEquipmentAttackAlterationEffect;
        }
    }
}
