using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Religion;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Content.Skill
{
    [Serializable]
    public class SkillSet : ScenarioImage
    {
        public IList<Skill> Skills { get; set; }

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

        public bool HasReligiousAffiliationRequirement { get; set; }
        public ReligiousAffiliationRequirement ReligiousAffiliationRequirement { get; set; }

        public Spell GetCurrentSkillAlteration()
        {
            if (this.Level < this.Skills.Count)
                return this.Skills[this.Level].Alteration;

            return null;
        }

        public SkillSet()
        {
            this.IsActive = false;
            this.IsLearned = false;
            this.Emphasis = 0;
            this.Skills = new List<Skill>();
            this.ReligiousAffiliationRequirement = new ReligiousAffiliationRequirement();
        } 
    }
}
