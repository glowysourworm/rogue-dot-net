using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class UncurseAlterationEffect : RogueBase, IConsumableAlterationEffect,
                                                      IDoodadAlterationEffect,
                                                      ISkillAlterationEffect
    {
        public bool UncurseAll { get; set; }

        public UncurseAlterationEffect() { }
    }
}
