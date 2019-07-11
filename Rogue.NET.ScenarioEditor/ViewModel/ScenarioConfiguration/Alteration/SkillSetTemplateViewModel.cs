using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System.Collections.ObjectModel;


namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    public class SkillSetTemplateViewModel : DungeonObjectTemplateViewModel
    {
        private int _levelLearned;
        private bool _hasReligionRequirement;
        private ReligionTemplateViewModel _religion;

        public ObservableCollection<SkillTemplateViewModel> Skills { get; set; }

        public int LevelLearned
        {
            get { return _levelLearned; }
            set { this.RaiseAndSetIfChanged(ref _levelLearned, value); }
        }
        public bool HasReligionRequirement
        {
            get { return _hasReligionRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasReligionRequirement, value); }
        }
        public ReligionTemplateViewModel Religion
        {
            get { return _religion; }
            set { this.RaiseAndSetIfChanged(ref _religion, value); }
        }

        public SkillSetTemplateViewModel()
        {
            this.Skills = new ObservableCollection<SkillTemplateViewModel>();
            this.Religion = new ReligionTemplateViewModel();
        }
        public SkillSetTemplateViewModel(DungeonObjectTemplateViewModel obj)
            : base(obj)
        {
            this.Skills = new ObservableCollection<SkillTemplateViewModel>();
            this.Religion = new ReligionTemplateViewModel();
        }
    }
}
