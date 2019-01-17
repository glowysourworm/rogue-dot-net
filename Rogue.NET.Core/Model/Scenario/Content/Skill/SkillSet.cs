using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Religion;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Content.Skill
{
    [Serializable]
    public class SkillSet : ScenarioImage
    {
        public IList<Skill> Skills { get; set; }

        public int LevelLearned { get; set; }
        public bool IsActive { get; set; }
        public bool IsTurnedOn { get; set; }
        public bool IsLearned { get; set; }

        public bool HasReligiousAffiliationRequirement { get; set; }
        public ReligiousAffiliationRequirement ReligiousAffiliationRequirement { get; set; }

        public Skill SelectedSkill { get; private set; }

        #region (public) Methods
        public void SelectSkillUp()
        {
            if (this.SelectedSkill == null)
                this.SelectedSkill = this.Skills.FirstOrDefault(x => x.IsLearned);

            else
            {
                this.SelectedSkill = this.Skills.FirstOrDefault(x => x.IsLearned &&
                                                                     x != this.SelectedSkill) ?? this.SelectedSkill;
            }
        }
        public void SelectSkillDown()
        {
            if (this.SelectedSkill == null)
                this.SelectedSkill = this.Skills.FirstOrDefault(x => x.IsLearned);

            else
            {
                this.SelectedSkill = this.Skills.FirstOrDefault(x => x.IsLearned &&
                                                                     x != this.SelectedSkill) ?? this.SelectedSkill;
            }
        }
        public void SelectSkill(string skillId)
        {
            this.SelectedSkill = this.Skills.FirstOrDefault(x => x.Id == skillId);
        }
        public Spell GetCurrentSkillAlteration()
        {
            if (this.SelectedSkill != null)
                return this.SelectedSkill.Alteration;

            return null;
        }
        #endregion

        public SkillSet()
        {
            this.IsActive = false;
            this.IsLearned = false;
            this.Skills = new List<Skill>();
            this.ReligiousAffiliationRequirement = new ReligiousAffiliationRequirement();
        } 
    }
}
