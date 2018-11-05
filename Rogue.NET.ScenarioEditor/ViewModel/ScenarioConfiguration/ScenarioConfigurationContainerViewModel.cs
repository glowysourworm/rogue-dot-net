using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration
{
    public class ScenarioConfigurationContainerViewModel
    {
        public DungeonTemplateViewModel DungeonTemplate { get; set; }
        public PlayerTemplateViewModel PlayerTemplate { get; set; }

        public ObservableCollection<SkillSetTemplateViewModel> SkillTemplates { get; set; }
        public ObservableCollection<BrushTemplateViewModel> BrushTemplates { get; set; }
        public ObservableCollection<PenTemplateViewModel> PenTemplates { get; set; }
        public ObservableCollection<EnemyTemplateViewModel> EnemyTemplates { get; set; }
        public ObservableCollection<AnimationTemplateViewModel> AnimationTemplates { get; set; }
        public ObservableCollection<EquipmentTemplateViewModel> EquipmentTemplates { get; set; }
        public ObservableCollection<ConsumableTemplateViewModel> ConsumableTemplates { get; set; }
        public ObservableCollection<SpellTemplateViewModel> MagicSpells { get; set; }
        public ObservableCollection<DoodadTemplateViewModel> DoodadTemplates { get; set; }
        public ObservableCollection<DungeonObjectTemplateViewModel> AttackAttributes { get; set; }

        public ScenarioConfigurationContainerViewModel()
        {
            this.DungeonTemplate = new DungeonTemplateViewModel();
            this.MagicSpells = new ObservableCollection<SpellTemplateViewModel>();
            this.EnemyTemplates = new ObservableCollection<EnemyTemplateViewModel>();
            this.BrushTemplates = new ObservableCollection<BrushTemplateViewModel>();
            this.EquipmentTemplates = new ObservableCollection<EquipmentTemplateViewModel>();
            this.AnimationTemplates = new ObservableCollection<AnimationTemplateViewModel>();
            this.ConsumableTemplates = new ObservableCollection<ConsumableTemplateViewModel>();
            this.SkillTemplates = new ObservableCollection<SkillSetTemplateViewModel>();
            this.PlayerTemplate = new PlayerTemplateViewModel();
            this.DoodadTemplates = new ObservableCollection<DoodadTemplateViewModel>();
            this.PenTemplates = new ObservableCollection<PenTemplateViewModel>();
            this.AttackAttributes = new ObservableCollection<DungeonObjectTemplateViewModel>();
        }
    }
}
