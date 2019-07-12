using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Skill
{
    [Serializable]
    public class Skill : RogueBase
    {
        public int LevelRequirement { get; set; }
        public int SkillPointRequirement { get; set; }
        public Spell Alteration { get; set; }

        public bool IsLearned { get; set; }

        public Skill()
        {
            this.Alteration = new Spell();
        }
    }
}
