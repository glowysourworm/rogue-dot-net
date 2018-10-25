using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Content.Skill
{
    [Serializable]
    public class SkillSet : ScenarioImage
    {
        public IList<Spell> Skills { get; set; }

        public int Level { get; set; }
        public int DisplayLevel
        {
            get { return this.Level + 1; }
        }
        public int LevelLearned { get; set; }
        public int Emphasis { get; set; }
        public bool IsActive { get; set; }
        public bool IsTurnedOn { get; set; }
        public bool IsLearned { get; set; }
        public double SkillProgress { get; set; }

        public Spell GetCurrentSkill()
        {
            if (this.Level < this.Skills.Count)
                return this.Skills[this.Level];

            return null;
        }

        public SkillSet()
        {
            this.IsActive = false;
            this.IsLearned = false;
            this.Emphasis = 0;
            this.Skills = new List<Spell>();
        } 
    }
}
