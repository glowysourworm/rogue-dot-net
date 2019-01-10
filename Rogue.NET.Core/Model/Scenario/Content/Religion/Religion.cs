using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Content.Religion
{
    [Serializable]
    public class Religion : ScenarioImage
    {
        public string FollowerName { get; set; }

        public bool HasAttributeBonus { get; set; }
        public bool HasBonusAttackAttributes { get; set; }
        public bool HasBonusSkillSet { get; set; }
        public bool AllowsRenunciation { get; set; }
        public bool AllowsReAffiliation { get; set; }

        public AlterationEffect AttributeAlteration { get; set; }
        public AlterationEffect AttackAttributeAlteration { get; set; }

        public List<ReligiousAffiliationAttackParameters> AttackParameters { get; set; }
        public List<AnimationTemplate> RenunciationAnimations { get; set; }

        public SkillSet SkillSet { get; set; }

        public Religion()
        {
            this.AttackAttributeAlteration = new AlterationEffect();
            this.AttributeAlteration = new AlterationEffect();
            this.AttackParameters = new List<ReligiousAffiliationAttackParameters>();
            this.RenunciationAnimations = new List<AnimationTemplate>();
            this.SkillSet = new SkillSet();
        }
    }
}
