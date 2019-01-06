using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class ReligiousAffiliationRequirementTemplate : Template
    {
        ReligionTemplate _religion;
        double _requiredAffiliationLevel;

        public ReligionTemplate Religion
        {
            get { return _religion; }
            set
            {
                if (_religion != value)
                {
                    _religion = value;
                    OnPropertyChanged("Religion");
                }
            }
        }
        public double RequiredAffiliationLevel
        {
            get { return _requiredAffiliationLevel; }
            set
            {
                if (_requiredAffiliationLevel != value)
                {
                    _requiredAffiliationLevel = value;
                    OnPropertyChanged("RequiredAffiliationLevel");
                }
            }
        }

        public ReligiousAffiliationRequirementTemplate()
        {
            this.Religion = new ReligionTemplate();
            this.RequiredAffiliationLevel = 0.1D;
        }
    }
}
