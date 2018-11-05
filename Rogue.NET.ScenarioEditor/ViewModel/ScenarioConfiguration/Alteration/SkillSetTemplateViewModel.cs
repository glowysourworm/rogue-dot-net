using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

using System.Collections.ObjectModel;

using ReactiveUI;


namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    public class SkillSetTemplateViewModel : DungeonObjectTemplateViewModel
    {
        private int _levelLearned;

        public ObservableCollection<SpellTemplateViewModel> Spells { get; set; }

        public int LevelLearned
        {
            get { return _levelLearned; }
            set { this.RaiseAndSetIfChanged(ref _levelLearned, value); }
        }

        public SkillSetTemplateViewModel()
        {
            this.Spells = new ObservableCollection<SpellTemplateViewModel>();
        }
        public SkillSetTemplateViewModel(DungeonObjectTemplateViewModel obj)
            : base(obj)
        {
            this.Spells = new ObservableCollection<SpellTemplateViewModel>();
        }
    }
}
