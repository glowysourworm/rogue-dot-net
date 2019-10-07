using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Consumable
{
    [Serializable]
    public class ConsumableAlterationTemplate : AlterationTemplate
    {
        public ConsumableAlterationTemplate()
        {
            this.Animation = new AnimationSequenceTemplate();
            this.Cost = new AlterationCostTemplate();
        }
    }
}
