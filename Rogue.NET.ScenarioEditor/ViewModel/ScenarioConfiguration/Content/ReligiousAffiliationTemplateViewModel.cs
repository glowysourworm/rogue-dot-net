using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class ReligiousAffiliationTemplateViewModel : TemplateViewModel
    {
        ReligionTemplateViewModel _religion;
        RangeViewModel<double> _affiliationLevel;

        public ReligionTemplateViewModel Religion
        {
            get { return _religion; }
            set { this.RaiseAndSetIfChanged(ref _religion, value); }
        }

        public RangeViewModel<double> AffiliationLevel
        {
            get { return _affiliationLevel; }
            set { this.RaiseAndSetIfChanged(ref _affiliationLevel, value); }
        }

        public ReligiousAffiliationTemplateViewModel()
        {
            this.Religion = new ReligionTemplateViewModel();
            this.AffiliationLevel = new RangeViewModel<double>();
        }
    }
}
