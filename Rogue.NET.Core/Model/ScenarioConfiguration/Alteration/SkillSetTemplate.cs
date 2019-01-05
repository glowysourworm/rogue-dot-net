using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    [Serializable]
    public class SkillSetTemplate : DungeonObjectTemplate
    {
        private int _levelLearned;
        private bool _hasReligiousAffiliationRequirement;
        private ReligiousAffiliationRequirementTemplate _religiousAffiliationRequirement;

        public List<SkillTemplate> Skills { get; set; }

        public int LevelLearned
        {
            get { return _levelLearned; }
            set
            {
                if (_levelLearned != value)
                {
                    _levelLearned = value;
                    OnPropertyChanged("LevelLearned");
                }
            }
        }
        public bool HasReligiousAffiliationRequirement
        {
            get { return _hasReligiousAffiliationRequirement; }
            set
            {
                if (_hasReligiousAffiliationRequirement != value)
                {
                    _hasReligiousAffiliationRequirement = value;
                    OnPropertyChanged("HasReligiousAffiliationRequirement");
                }
            }
        }
        public ReligiousAffiliationRequirementTemplate ReligiousAffiliationRequirement
        {
            get { return _religiousAffiliationRequirement; }
            set
            {
                if (_religiousAffiliationRequirement != value)
                {
                    _religiousAffiliationRequirement = value;
                    OnPropertyChanged("ReligiousAffiliationRequirement");
                }
            }
        }

        public SkillSetTemplate()
        {
            this.Skills = new List<SkillTemplate>();
            this.HasReligiousAffiliationRequirement = false;
            this.ReligiousAffiliationRequirement = new ReligiousAffiliationRequirementTemplate();
        }
        public SkillSetTemplate(DungeonObjectTemplate obj)
            : base(obj)
        {
            this.Skills = new List<SkillTemplate>();
            this.HasReligiousAffiliationRequirement = false;
            this.ReligiousAffiliationRequirement = new ReligiousAffiliationRequirementTemplate();
        }
    }
}
