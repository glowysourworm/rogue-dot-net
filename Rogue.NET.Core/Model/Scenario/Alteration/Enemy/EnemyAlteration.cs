using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Enemy
{
    [Serializable]
    public class EnemyAlteration : AlterationContainer
    {
        public EnemyAlteration()
        {
        }

        protected override bool ValidateEffectType()
        {
            return this.Effect is IEnemyAlterationEffect;
        }
    }
}
