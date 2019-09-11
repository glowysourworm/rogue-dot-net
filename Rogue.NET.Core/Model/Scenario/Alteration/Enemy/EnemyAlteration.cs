using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Enemy
{
    [Serializable]
    public class EnemyAlteration : Common.AlterationContainer
    {
        public EnemyAlteration()
        {
        }

        public override Type EffectInterfaceType
        {
            get { return typeof(IEnemyAlterationEffect); }
        }
    }
}
