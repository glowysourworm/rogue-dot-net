using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Alteration.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Skill;

namespace Rogue.NET.Core.Model.Scenario.Content.Skill
{
    [Serializable]
    public class SkillSet : ScenarioImage
    {
        public IList<Skill> Skills { get; set; }

        public bool IsActive { get; set; }
        public bool IsTurnedOn { get; set; }

        public Skill SelectedSkill { get; private set; }

        #region (public) Methods
        public void SelectSkillUp(Player player)
        {
            if (this.SelectedSkill == null)
                this.SelectedSkill = this.Skills.FirstOrDefault(x => x.IsLearned);

            else
            {
                this.SelectedSkill = this.Skills.FirstOrDefault(x => x.IsLearned &&
                                                                     x != this.SelectedSkill) ?? this.SelectedSkill;
            }
        }
        public void SelectSkillDown(Player player)
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
        public void DeSelectSkill()
        {
            this.SelectedSkill = null;
        }
        public SkillAlterationTemplate GetCurrentSkillAlteration()
        {
            if (this.SelectedSkill != null)
                return this.SelectedSkill.Alteration;

            return null;
        }
        #endregion

        public SkillSet()
        {
            this.IsActive = false;
            this.Skills = new List<Skill>();
        } 
    }
}
