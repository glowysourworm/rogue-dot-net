using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Enemy
{
    [Serializable]
    public class EnemyAlteration : AlterationBase
    {
        public AlterationTargetType TargetType { get; set; }

        public EnemyAlteration()
        {
        }
        public EnemyAlteration(string guid) : base(guid)
        {
        }

        protected override bool ValidateEffectInterfaceType()
        {
            return this.Effect is IEnemyAlterationEffect;
        }
    }
}
