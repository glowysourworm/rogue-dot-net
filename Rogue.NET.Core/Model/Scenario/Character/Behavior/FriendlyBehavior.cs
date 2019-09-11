using System;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Friendly;

namespace Rogue.NET.Core.Model.Scenario.Character.Behavior
{
    [Serializable]
    public class FriendlyBehavior : Behavior
    {
        public override AlterationCostTemplate SkillAlterationCost
        {
            get { return this.SkillAlteration?.Cost; }
        }

        public FriendlyAlterationTemplate SkillAlteration { get; set; }

        public FriendlyBehavior()
        {

        }
    }
}
