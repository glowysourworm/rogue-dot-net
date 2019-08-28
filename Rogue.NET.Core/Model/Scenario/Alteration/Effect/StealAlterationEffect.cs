using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    [AlterationBlockable]
    [AlterationCostSpecifier(AlterationCostType.OneTime,
                             typeof(IEnemyAlterationEffect),
                             typeof(ISkillAlterationEffect))]
    public class StealAlterationEffect
        : RogueBase, IEnemyAlterationEffect,
                     ISkillAlterationEffect
    {
        public StealAlterationEffect() { }
    }
}
