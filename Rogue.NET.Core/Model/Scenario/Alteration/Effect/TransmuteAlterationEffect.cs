using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class TransmuteAlterationEffect : RogueBase, IConsumableAlterationEffect,
                                                        IDoodadAlterationEffect,
                                                        ISkillAlterationEffect
    {
        public double ProbabilityOfSuccess { get; set; }

        public List<TransmuteAlterationEffectItem> TransmuteItems { get; set; }

        public TransmuteAlterationEffect()
        {
            this.TransmuteItems = new List<TransmuteAlterationEffectItem>();
        }
    }
}
