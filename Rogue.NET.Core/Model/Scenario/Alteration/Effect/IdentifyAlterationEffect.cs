using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class IdentifyAlterationEffect : RogueBase, IConsumableAlterationEffect,
                                                       IDoodadAlterationEffect,
                                                       ISkillAlterationEffect
    {
        public bool IdentifyAll { get; set; }

        public IdentifyAlterationEffect() { }
    }
}
