using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class ReligiousAffiliationRequirementTemplateViewModel : TemplateViewModel
    {
        ReligionTemplateViewModel _religion;
        double _requiredAffiliationLevel;

        public ReligionTemplateViewModel Religion
        {
            get { return _religion; }
            set { this.RaiseAndSetIfChanged(ref _religion, value); }
        }
        public double RequiredAffiliationLevel
        {
            get { return _requiredAffiliationLevel; }
            set { this.RaiseAndSetIfChanged(ref _requiredAffiliationLevel, value); }
        }

        public ReligiousAffiliationRequirementTemplateViewModel()
        {
            this.Religion = new ReligionTemplateViewModel();
            this.RequiredAffiliationLevel = 0D;
        }
    }
}
