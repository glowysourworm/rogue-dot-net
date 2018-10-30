using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Enemy
{
    public partial class EnemyItems : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public EnemyItems()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(EnemyMetadata); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;

            var enemy = model as EnemyTemplate;
            var config = containerViewModel.SecondaryPayload as ScenarioConfigurationContainer;
            if (config != null && enemy != null)
            {
                this.ConsumablesLB.SourceLB.ItemsSource = config.ConsumableTemplates.Select(x => new ProbabilityConsumableTemplate() { Name = x.Name, TheTemplate = x });
                this.EquipmentLB.SourceLB.ItemsSource = config.EquipmentTemplates.Select(x => new ProbabilityEquipmentTemplate() { Name = x.Name, TheTemplate = x });

                this.ConsumablesLB.SourceLB.DisplayMemberPath = "TheTemplate.Name";
                this.EquipmentLB.SourceLB.DisplayMemberPath = "TheTemplate.Name";

                var consumableCollection = new ObservableCollection<ProbabilityConsumableTemplate>(enemy.StartingConsumables);
                var equipmentCollection = new ObservableCollection<ProbabilityEquipmentTemplate>(enemy.StartingEquipment);

                consumableCollection.CollectionChanged += (obj, e) =>
                {
                    enemy.StartingConsumables.Clear();
                    foreach (var item in consumableCollection)
                        enemy.StartingConsumables.Add(item);
                };

                equipmentCollection.CollectionChanged += (obj, e) =>
                {
                    enemy.StartingEquipment.Clear();
                    foreach (var item in equipmentCollection)
                        enemy.StartingEquipment.Add(item);
                };

                this.ConsumablesLB.DestinationLB.ItemsSource = consumableCollection;
                this.EquipmentLB.DestinationLB.ItemsSource = equipmentCollection;
            }
        }
    }
}
