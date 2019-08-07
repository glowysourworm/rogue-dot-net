using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Consumable
{
    [Serializable]
    public class ConsumableAlteration : AlterationBase
    {        
        public AlterationTargetType TargetType { get; set; }

        public ConsumableAlteration()
        {
            this.AnimationGroup = new AnimationGroup();
        }

        public ConsumableAlteration(string guid) : base(guid)
        {

        }

        protected override bool ValidateEffectInterfaceType()
        {
            return this.Effect is IConsumableAlterationEffect;
        }
    }
}
