using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Enemy
{
    [Serializable]
    public class EnemyAlteration : RogueBase
    {
        public AnimationContainer Animation { get; set; }
        public AlterationCost Cost { get; set; }
        public IEnemyAlterationEffect Effect { get; set; }
        public AlterationBlockType BlockType { get; set; }

        public EnemyAlteration()
        {
            this.Animation = new AnimationContainer();
            this.Cost = new AlterationCost();
        }
    }
}
