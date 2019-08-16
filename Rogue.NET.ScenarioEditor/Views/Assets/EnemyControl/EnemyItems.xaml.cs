﻿using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Service.Interface;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EnemyControl
{
    [Export]
    public partial class EnemyItems : UserControl
    {
        [ImportingConstructor]
        public EnemyItems(IRogueEventAggregator eventAggregator, IScenarioCollectionProvider scenarioCollectionProvider)
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

            this.ConsumablesLB.RemoveEvent += OnRemoveConsumable;
            this.EquipmentLB.RemoveEvent += OnRemoveEquipment;
        }

        private void Initialize(IScenarioCollectionProvider provider)
        {
            this.ConsumablesLB.SourceItemsSource = provider.Consumables;
            this.EquipmentLB.SourceItemsSource = provider.Equipment;
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
        private void OnRemoveConsumable(object sender, object consumable)
        {
            var enemy = this.DataContext as EnemyTemplateViewModel;
            var probabilityConsumableTemplate = consumable as ProbabilityConsumableTemplateViewModel;
            enemy.StartingConsumables.Remove(probabilityConsumableTemplate);
        }
        private void OnRemoveEquipment(object sender, object equipment)
        {
            var enemy = this.DataContext as EnemyTemplateViewModel;
            var probabilityEquipmentTemplate = equipment as ProbabilityEquipmentTemplateViewModel;
            enemy.StartingEquipment.Remove(probabilityEquipmentTemplate);
        }
    }
}
