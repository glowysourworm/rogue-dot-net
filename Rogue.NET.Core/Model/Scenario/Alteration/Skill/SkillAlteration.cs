using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Skill
{
    [Serializable]
    public class SkillAlteration : RogueBase
    {
        public AnimationContainer Animation { get; set; }
        public AlterationCost Cost { get; set; }
        public ISkillAlterationEffect Effect { get; set; }
        public AlterationBlockType BlockType { get; set; }

        // TODO:ALTERATION
        public AuraSourceParameters AuraParameters { get; set; }

        public SkillAlteration()
        {
            this.Animation = new AnimationContainer();
            this.Cost = new AlterationCost();
            this.AuraParameters = new AuraSourceParameters();
        }
    }
}
