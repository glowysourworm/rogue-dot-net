using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Collections.ObjectModel;


namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    public class SkillSetTemplateViewModel : DungeonObjectTemplateViewModel
    {
        public ObservableCollection<SkillTemplateViewModel> Skills { get; set; }

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
