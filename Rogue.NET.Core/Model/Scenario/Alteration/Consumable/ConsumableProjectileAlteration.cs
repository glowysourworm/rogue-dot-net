using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Consumable
{
    [Serializable]
    public class ConsumableProjectileAlteration : AlterationBase
    {
        public ConsumableProjectileAlteration()
        {
        }

        protected override bool ValidateEffectInterfaceType()
        {
            return this.Effect is IConsumableProjectileAlterationEffect;
        }
    }
}
