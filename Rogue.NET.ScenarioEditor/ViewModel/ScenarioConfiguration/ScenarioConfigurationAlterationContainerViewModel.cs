using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Doodad;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Enemy;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Equipment;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Skill;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration
{
    public class ScenarioConfigurationAlterationContainerViewModel
    {
        public ObservableCollection<ConsumableAlterationTemplateViewModel> ConsumableAlterations { get; set; }
        public ObservableCollection<ConsumableProjectileAlterationTemplateViewModel> ConsumableProjectileAlterations { get; set; }
        public ObservableCollection<DoodadAlterationTemplateViewModel> DoodadAlterations { get; set; }
        public ObservableCollection<EnemyAlterationTemplateViewModel> EnemyAlterations { get; set; }
        public ObservableCollection<EquipmentAttackAlterationTemplateViewModel> EquipmentAttackAlterations { get; set; }
        public ObservableCollection<EquipmentCurseAlterationTemplateViewModel> EquipmentCurseAlterations { get; set; }
        public ObservableCollection<EquipmentEquipAlterationTemplateViewModel> EquipmentEquipAlterations { get; set; }
        public ObservableCollection<SkillAlterationTemplateViewModel> SkillAlterations { get; set; }

        public ScenarioConfigurationAlterationContainerViewModel()
        {
            this.ConsumableAlterations = new ObservableCollection<ConsumableAlterationTemplateViewModel>();
            this.ConsumableProjectileAlterations = new ObservableCollection<ConsumableProjectileAlterationTemplateViewModel>();
            this.DoodadAlterations = new ObservableCollection<DoodadAlterationTemplateViewModel>();
            this.EnemyAlterations = new ObservableCollection<EnemyAlterationTemplateViewModel>();
            this.EquipmentAttackAlterations = new ObservableCollection<EquipmentAttackAlterationTemplateViewModel>();
            this.EquipmentCurseAlterations = new ObservableCollection<EquipmentCurseAlterationTemplateViewModel>();
            this.EquipmentEquipAlterations = new ObservableCollection<EquipmentEquipAlterationTemplateViewModel>();
            this.SkillAlterations = new ObservableCollection<SkillAlterationTemplateViewModel>();
        }
    }
}
