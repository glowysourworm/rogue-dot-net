using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Consumable
{
    [Serializable]
    public class ConsumableAlteration : RogueBase
    {
        public AnimationContainer AnimationGroup { get; set; }
        public AlterationCost Cost { get; set; }
        public IConsumableAlterationEffect Effect { get; set; }

        public ConsumableAlteration()
        {
            this.AnimationGroup = new AnimationContainer();
            this.Cost = new AlterationCost()
            {
                Type = AlterationCostType.OneTime
            };
        }
    }
}
