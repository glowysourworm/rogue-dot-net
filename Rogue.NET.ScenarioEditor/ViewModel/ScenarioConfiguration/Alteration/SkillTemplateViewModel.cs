using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    public class SkillTemplateViewModel : TemplateViewModel
    {
        int _levelRequirement;
        int _pointRequirement;
        bool _hasReligiousAffiliationRequirement;
        ReligiousAffiliationRequirementTemplateViewModel _religiousAffiliationRequirement;
        SpellTemplateViewModel _alteration;

        public int LevelRequirement
        {
            get { return _levelRequirement; }
            set { this.RaiseAndSetIfChanged(ref _levelRequirement, value); }
        }
        public int PointRequirement
        {
            get { return _pointRequirement; }
            set { this.RaiseAndSetIfChanged(ref _pointRequirement, value); }
        }
        public bool HasReligiousAffiliationRequirement
        {
            get { return _hasReligiousAffiliationRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasReligiousAffiliationRequirement, value); }
        }
        public ReligiousAffiliationRequirementTemplateViewModel ReligiousAffiliationRequirement
        {
            get { return _religiousAffiliationRequirement; }
            set { this.RaiseAndSetIfChanged(ref _religiousAffiliationRequirement, value); }
        }
        public SpellTemplateViewModel Alteration
        {
            get { return _alteration; }
            set { this.RaiseAndSetIfChanged(ref _alteration, value); }
        }

        public SkillTemplateViewModel()
        {
            this.HasReligiousAffiliationRequirement = false;
            this.ReligiousAffiliationRequirement = new ReligiousAffiliationRequirementTemplateViewModel();
            this.Alteration = new SpellTemplateViewModel();
        }
    }
}
