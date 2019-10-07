using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Consumable
{
    [Serializable]
    public class ConsumableProjectileAlterationTemplate : AlterationTemplate
    {
        public ConsumableProjectileAlterationTemplate()
        {
            this.Animation = new AnimationSequenceTemplate()
            {
                TargetType = AlterationTargetType.Target
            };
        }
    }
}
