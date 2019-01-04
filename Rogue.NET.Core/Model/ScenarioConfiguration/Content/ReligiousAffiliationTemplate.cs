using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class ReligiousAffiliationTemplate : Template
    {
        ReligionTemplate _religion;
        Range<double> _affiliationLevel;

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
        public Range<double> AffiliationLevel
        {
            get { return _affiliationLevel; }
            set
            {
                if (_affiliationLevel != value)
                {
                    _affiliationLevel = value;
                    OnPropertyChanged("AffiliationLevel");
                }
            }
        }

        public ReligiousAffiliationTemplate()
        {
            this.Religion = new ReligionTemplate();
            this.AffiliationLevel = new Range<double>();
        }
    }
}
