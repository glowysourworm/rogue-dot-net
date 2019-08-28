using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    [AlterationBlockable]
    [AlterationCostSpecifier(AlterationCostType.None)]
    public class RunAwayAlterationEffect : RogueBase, IEnemyAlterationEffect
    {
        public RunAwayAlterationEffect() { }
    }
}
