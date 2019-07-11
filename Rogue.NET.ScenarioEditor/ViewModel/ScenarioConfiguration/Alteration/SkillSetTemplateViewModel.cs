using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Collections.ObjectModel;


namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    public class SkillSetTemplateViewModel : DungeonObjectTemplateViewModel
    {
        private int _levelLearned;
        private bool _hasReligionRequirement;

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
        public SkillSetTemplateViewModel()
        {
            this.Skills = new ObservableCollection<SkillTemplateViewModel>();
        }
        public SkillSetTemplateViewModel(DungeonObjectTemplateViewModel obj)
            : base(obj)
        {
            this.Skills = new ObservableCollection<SkillTemplateViewModel>();
        }
    }
}
