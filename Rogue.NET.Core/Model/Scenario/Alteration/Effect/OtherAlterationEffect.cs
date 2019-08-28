using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    [AlterationBlockable]
    [AlterationCostSpecifier(AlterationCostType.OneTime,
                             typeof(IConsumableAlterationEffect),
                             typeof(IDoodadAlterationEffect),
                             typeof(ISkillAlterationEffect))]
    public class OtherAlterationEffect
        : RogueBase, IConsumableAlterationEffect,
                     IDoodadAlterationEffect,
                     ISkillAlterationEffect
    {
        public AlterationOtherEffectType Type { get; set; }

        public OtherAlterationEffect()
        {

        }
    }
}
