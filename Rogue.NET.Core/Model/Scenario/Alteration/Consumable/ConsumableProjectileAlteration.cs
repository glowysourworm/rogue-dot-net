using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Consumable
{
    [Serializable]
    public class ConsumableProjectileAlteration : RogueBase
    {
        public AnimationContainer Animation { get; set; }
        public IConsumableProjectileAlterationEffect Effect { get; set; }

        public ConsumableProjectileAlteration()
        {
            this.Animation = new AnimationContainer();
        }
    }
}
