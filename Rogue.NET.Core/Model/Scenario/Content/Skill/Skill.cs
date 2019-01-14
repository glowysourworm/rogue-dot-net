using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Content.Skill
{
    [Serializable]
    public class Skill : RogueBase
    {
        public int LevelRequirement { get; set; }
        public int SkillPointRequirement { get; set; }
        public double RequiredAffiliationLevel { get; set; }
        public Spell Alteration { get; set; }

        public bool IsLearned { get; set; }

        public Skill()
        {
            this.Alteration = new Spell();
        }
    }
}
