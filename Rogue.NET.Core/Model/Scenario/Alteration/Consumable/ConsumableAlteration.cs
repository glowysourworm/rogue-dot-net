using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Consumable
{
    [Serializable]
    public class ConsumableAlteration : Common.AlterationContainer
    {        
        public ConsumableAlteration()
        {
            this.AnimationGroup = new AnimationGroup();
        }

        public override Type EffectInterfaceType
        {
            get { return typeof(IConsumableAlterationEffect); }
        }
    }
}
