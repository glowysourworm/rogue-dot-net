using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EnemyControl
{
    [Export]
    public partial class EnemyItems : UserControl
    {
        [ImportingConstructor]
        public EnemyItems(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe((configuration) =>
            {
                this.ConsumablesLB.SourceItemsSource = configuration.ConsumableTemplates;
                this.EquipmentLB.SourceItemsSource = configuration.EquipmentTemplates;
            });

            this.ConsumablesLB.AddEvent += OnAddConsumable;
            this.EquipmentLB.AddEvent += OnAddEquipment;
        }

        private void OnAddConsumable(object sender, object consumable)
        {
            var enemy = this.DataContext as EnemyTemplateViewModel;
            var consumableTemplate = consumable as ConsumableTemplateViewModel;
            enemy.StartingConsumables.Add(new ProbabilityConsumableTemplateViewModel()
            {
                Name = consumableTemplate.Name,
                TheTemplate = consumableTemplate
            });
        }
        private void OnAddEquipment(object sender, object equipment)
        {
            var enemy = this.DataContext as EnemyTemplateViewModel;
            var equipmentTemplate = equipment as EquipmentTemplateViewModel;
            enemy.StartingEquipment.Add(new ProbabilityEquipmentTemplateViewModel()
            {
                Name = equipmentTemplate.Name,
                TheTemplate = equipmentTemplate
            });
        }
    }
}
