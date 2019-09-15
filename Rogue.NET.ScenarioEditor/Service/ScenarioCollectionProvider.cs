using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace Rogue.NET.ScenarioEditor.Service
{
    [Export(typeof(IScenarioCollectionProvider))]
    public class ScenarioCollectionProvider : IScenarioCollectionProvider
    {
        readonly IRogueEventAggregator _eventAggregator;

        public ObservableCollection<EnemyTemplateViewModel> Enemies { get; private set; }
        public ObservableCollection<FriendlyTemplateViewModel> Friendlies { get; private set; }
        public ObservableCollection<EquipmentTemplateViewModel> Equipment { get; private set; }
        public ObservableCollection<ConsumableTemplateViewModel> Consumables { get; private set; }
        public ObservableCollection<SkillSetTemplateViewModel> SkillSets { get; private set; }
        public ObservableCollection<AlteredCharacterStateTemplateViewModel> AlteredCharacterStates { get; private set; }
        public ObservableCollection<PlayerTemplateViewModel> CharacterClasses { get; private set; }

        // "General Assets" - these are calculated and provided by the ScenarioEditorModule
        public ObservableCollection<BrushTemplateViewModel> Brushes { get; private set; }

        [ImportingConstructor]
        public ScenarioCollectionProvider(
                IRogueEventAggregator eventAggregator,
                IScenarioEditorController scenarioEditorController)
        {
            _eventAggregator = eventAggregator;

            this.Brushes = new ObservableCollection<BrushTemplateViewModel>();

            // New / Open (Scenario)
            eventAggregator.GetEvent<ScenarioLoadedEvent>()
                           .Subscribe(configurationData =>
                           {
                               Set(configurationData);
                           });
        }

        private void Set(ScenarioConfigurationData configurationData)
        {
            this.Enemies = configurationData.Configuration.EnemyTemplates;
            this.Friendlies = configurationData.Configuration.FriendlyTemplates;
            this.Equipment = configurationData.Configuration.EquipmentTemplates;
            this.Consumables = configurationData.Configuration.ConsumableTemplates;
            this.SkillSets = configurationData.Configuration.SkillTemplates;
            this.AlteredCharacterStates = configurationData.Configuration.AlteredCharacterStates;
            this.CharacterClasses = configurationData.Configuration.PlayerTemplates;

            // "General (calculated) Assets"
            this.Brushes.Clear();
            this.Brushes.AddRange(configurationData.ConfigurationBrushes);

            // Publish Update Event
            _eventAggregator.GetEvent<ScenarioUpdateEvent>().Publish(this);
        }

        public void AddBrush(BrushTemplateViewModel brush)
        {
            this.Brushes.Add(brush);

            // Publish Update Event
            _eventAggregator.GetEvent<ScenarioUpdateEvent>().Publish(this);
        }
        public void RemoveBrush(BrushTemplateViewModel brush)
        {
            this.Brushes.Remove(brush);

            // Publish Update Event
            _eventAggregator.GetEvent<ScenarioUpdateEvent>().Publish(this);
        }
    }
}
