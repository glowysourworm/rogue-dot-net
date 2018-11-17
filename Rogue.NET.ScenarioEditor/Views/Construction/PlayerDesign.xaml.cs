using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    [Export]
    public partial class PlayerDesign : UserControl
    {
        [ImportingConstructor]
        public PlayerDesign(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe((configuration) =>
            {
                this.ConsumablesLB.SourceItemsSource = configuration.ConsumableTemplates;
                this.EquipmentLB.SourceItemsSource = configuration.EquipmentTemplates;
            });

            this.ConsumablesLB.AddEvent += OnAddConsumable;
            this.EquipmentLB.AddEvent += OnAddEquipment;

            this.ConsumablesLB.RemoveEvent += OnRemoveConsumable;
            this.EquipmentLB.RemoveEvent += OnRemoveEquipment;
        }
        private void OnAddConsumable(object sender, object consumable)
        {
            var player = this.DataContext as PlayerTemplateViewModel;
            var consumableTemplate = consumable as ConsumableTemplateViewModel;
            player.StartingConsumables.Add(new ProbabilityConsumableTemplateViewModel()
            {
                Name = consumableTemplate.Name,
                TheTemplate = consumableTemplate
            });
        }
        private void OnAddEquipment(object sender, object equipment)
        {
            var player = this.DataContext as PlayerTemplateViewModel;
            var equipmentTemplate = equipment as EquipmentTemplateViewModel;
            player.StartingEquipment.Add(new ProbabilityEquipmentTemplateViewModel()
            {
                Name = equipmentTemplate.Name,
                TheTemplate = equipmentTemplate
            });
        }
        private void OnRemoveConsumable(object sender, object consumable)
        {
            var player = this.DataContext as PlayerTemplateViewModel;
            var probabilityConsumableTemplate = consumable as ProbabilityConsumableTemplateViewModel;
            player.StartingConsumables.Remove(probabilityConsumableTemplate);
        }
        private void OnRemoveEquipment(object sender, object equipment)
        {
            var player = this.DataContext as PlayerTemplateViewModel;
            var probabilityEquipmentTemplate = equipment as ProbabilityEquipmentTemplateViewModel;
            player.StartingEquipment.Remove(probabilityEquipmentTemplate);
        }
    }
}
