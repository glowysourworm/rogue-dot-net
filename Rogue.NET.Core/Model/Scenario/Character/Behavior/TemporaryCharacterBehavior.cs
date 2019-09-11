using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.TemporaryCharacter;

namespace Rogue.NET.Core.Model.Scenario.Character.Behavior
{
    [Serializable]
    public class TemporaryCharacterBehavior : Behavior
    {
        public override AlterationCostTemplate SkillAlterationCost
        {
            get { return this.SkillAlteration?.Cost; }
        }

        public TemporaryCharacterAlterationTemplate SkillAlteration { get; set; }

        public TemporaryCharacterBehavior()
        {
        }
    }
}
