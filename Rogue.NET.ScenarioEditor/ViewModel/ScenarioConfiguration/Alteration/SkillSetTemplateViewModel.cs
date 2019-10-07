using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.Views.Assets;
using System.Collections.ObjectModel;


namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    [UIType(DisplayName = "Skill Set",
            Description = "Set of skills that the Player can use",
            ViewType = typeof(SkillSet),
            BaseType = UITypeAttributeBaseType.Asset)]
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
