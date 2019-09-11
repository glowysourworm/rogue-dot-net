using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class ChangeLevelAlterationEffect
        : RogueBase, IConsumableAlterationEffect,
                     IDoodadAlterationEffect,
                     ISkillAlterationEffect
    {
        public int LevelChange { get; set; }

        public ChangeLevelAlterationEffect()
        {
        }
    }
}
