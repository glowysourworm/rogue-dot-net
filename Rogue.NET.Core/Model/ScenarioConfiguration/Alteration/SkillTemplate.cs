using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    [Serializable]
    public class SkillTemplate : Template
    {
        int _levelRequirement;
        int _pointRequirement;
        bool _hasReligiousAffiliationRequirement;
        ReligiousAffiliationRequirementTemplate _religiousAffiliationRequirement;
        SpellTemplate _alteration;

        public int LevelRequirement
        {
            get { return _levelRequirement; }
            set
            {
                if (_levelRequirement != value)
                {
                    _levelRequirement = value;
                    OnPropertyChanged("LevelRequirement");
                }
            }
        }
        public int PointRequirement
        {
            get { return _pointRequirement; }
            set
            {
                if (_pointRequirement != value)
                {
                    _pointRequirement = value;
                    OnPropertyChanged("PointRequirement");
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
        public SpellTemplate Alteration
        {
            get { return _alteration; }
            set
            {
                if (_alteration != value)
                {
                    _alteration = value;
                    OnPropertyChanged("Alteration");
                }
            }
        }

        public SkillTemplate()
        {
            this.HasReligiousAffiliationRequirement = false;
            this.ReligiousAffiliationRequirement = new ReligiousAffiliationRequirementTemplate();
            this.Alteration = new SpellTemplate();
        }
    }
}
