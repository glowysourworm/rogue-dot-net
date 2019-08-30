using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace Rogue.NET.ScenarioEditor.Service
{
    [Export(typeof(IScenarioCollectionProvider))]
    public class ScenarioCollectionProvider : IScenarioCollectionProvider
    {
        public ObservableCollection<EnemyTemplateViewModel> Enemies { get; private set; }
        public ObservableCollection<EquipmentTemplateViewModel> Equipment { get; private set; }
        public ObservableCollection<ConsumableTemplateViewModel> Consumables { get; private set; }
        public ObservableCollection<SkillSetTemplateViewModel> SkillSets { get; private set; }
        public ObservableCollection<AlteredCharacterStateTemplateViewModel> AlteredCharacterStates { get; private set; }
        public ObservableCollection<PlayerTemplateViewModel> CharacterClasses { get; private set; }

        [ImportingConstructor]
        public ScenarioCollectionProvider(
                IRogueEventAggregator eventAggregator,
                IScenarioEditorController scenarioEditorController)
        {
            if (scenarioEditorController.CurrentConfig != null)
                Set(scenarioEditorController.CurrentConfig, eventAggregator);

            // New / Open (Scenario)
            eventAggregator.GetEvent<ScenarioLoadedEvent>()
                           .Subscribe(configuration =>
                           {
                               Set(configuration, eventAggregator);
                           });
        }

        private void Set(ScenarioConfigurationContainerViewModel configuration, IRogueEventAggregator eventAggregator)
        {
            this.Enemies = configuration.EnemyTemplates;
            this.Equipment = configuration.EquipmentTemplates;
            this.Consumables = configuration.ConsumableTemplates;
            this.SkillSets = configuration.SkillTemplates;
            this.AlteredCharacterStates = configuration.AlteredCharacterStates;
            this.CharacterClasses = configuration.PlayerTemplates;

            // Publish Update Event
            eventAggregator.GetEvent<ScenarioUpdateEvent>().Publish(this);
        }
    }
}
