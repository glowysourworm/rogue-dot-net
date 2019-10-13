using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class DetectAlterationAlterationEffect : RogueBase, IConsumableAlterationEffect,
                                                               IDoodadAlterationEffect,
                                                               ISkillAlterationEffect
    {
        public AlterationCategory AlterationCategory { get; set; }

        public DetectAlterationAlterationEffect()
        {

        }
    }
}
