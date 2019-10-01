using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Extension;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class Player : UserControl
    {
        [ImportingConstructor]
        public Player(
                IRogueEventAggregator eventAggregator, 
                IScenarioCollectionProvider scenarioCollectionProvider)
        {
            InitializeComponent();
            Initialize(scenarioCollectionProvider);

            eventAggregator.GetEvent<ScenarioUpdateEvent>()
                           .Subscribe(provider =>
                           {
                               Initialize(provider);
                           });

            this.ConsumablesLB.AddEvent += OnAddConsumable;
            this.EquipmentLB.AddEvent += OnAddEquipment;
            this.SkillsLB.AddEvent += OnAddSkill;

            this.ConsumablesLB.RemoveEvent += OnRemoveConsumable;
            this.EquipmentLB.RemoveEvent += OnRemoveEquipment;
            this.SkillsLB.RemoveEvent += OnRemoveSkill;
        }

        private void Initialize(IScenarioCollectionProvider provider)
        {
            this.ConsumablesLB.SourceItemsSource = provider.Consumables.CreateDefaultView();
            this.EquipmentLB.SourceItemsSource = provider.Equipment.CreateDefaultView();
            this.SkillsLB.SourceItemsSource = provider.SkillSets.CreateDefaultView();
        }

        private void OnAddConsumable(object sender, object consumable)
        {
            var player = (this.DataContext as PlayerTemplateViewModel);
            var consumableTemplate = consumable as ConsumableTemplateViewModel;
            player.StartingConsumables.Add(new ProbabilityConsumableTemplateViewModel()
            {
                Name = consumableTemplate.Name,
                TheTemplate = consumableTemplate
            });
        }
        private void OnAddEquipment(object sender, object equipment)
        {
            var player = (this.DataContext as PlayerTemplateViewModel);
            var equipmentTemplate = equipment as EquipmentTemplateViewModel;
            player.StartingEquipment.Add(new ProbabilityEquipmentTemplateViewModel()
            {
                Name = equipmentTemplate.Name,
                TheTemplate = equipmentTemplate
            });
        }
        private void OnAddSkill(object sender, object skill)
        {
            var player = (this.DataContext as PlayerTemplateViewModel);
            var skillSetTemplate = skill as SkillSetTemplateViewModel;
            player.Skills.Add(skillSetTemplate);
        }
        private void OnRemoveConsumable(object sender, object consumable)
        {
            var player = (this.DataContext as PlayerTemplateViewModel);
            var probabilityConsumableTemplate = consumable as ProbabilityConsumableTemplateViewModel;
            player.StartingConsumables.Remove(probabilityConsumableTemplate);
        }
        private void OnRemoveEquipment(object sender, object equipment)
        {
            var player = (this.DataContext as PlayerTemplateViewModel);
            var probabilityEquipmentTemplate = equipment as ProbabilityEquipmentTemplateViewModel;
            player.StartingEquipment.Remove(probabilityEquipmentTemplate);
        }
        private void OnRemoveSkill(object sender, object skill)
        {
            var player = (this.DataContext as PlayerTemplateViewModel);
            var skillSetTemplate = skill as SkillSetTemplateViewModel;
            player.Skills.Remove(skillSetTemplate);
        }
    }
}
