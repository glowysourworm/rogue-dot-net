using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System.Collections.ObjectModel;


namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    public class SkillSetTemplateViewModel : DungeonObjectTemplateViewModel
    {
        private int _levelLearned;
        private bool _hasReligiousAffiliationRequirement;
        private ReligiousAffiliationRequirementTemplateViewModel _religiousAffiliationRequirement;

        public ObservableCollection<SkillTemplateViewModel> Skills { get; set; }

        public int LevelLearned
        {
            get { return _levelLearned; }
            set { this.RaiseAndSetIfChanged(ref _levelLearned, value); }
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

        public SkillSetTemplateViewModel()
        {
            this.Skills = new ObservableCollection<SkillTemplateViewModel>();
            this.ReligiousAffiliationRequirement = new ReligiousAffiliationRequirementTemplateViewModel();
        }
        public SkillSetTemplateViewModel(DungeonObjectTemplateViewModel obj)
            : base(obj)
        {
            this.Skills = new ObservableCollection<SkillTemplateViewModel>();
            this.ReligiousAffiliationRequirement = new ReligiousAffiliationRequirementTemplateViewModel();
        }
    }
}
