using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Consumable
{
    [Serializable]
    public class ConsumableProjectileAlteration : AlterationContainer
    {
        public ConsumableProjectileAlteration()
        {
        }

        public override Type EffectInterfaceType
        {
            get { return typeof(IConsumableProjectileAlterationEffect); }
        }
    }
}
