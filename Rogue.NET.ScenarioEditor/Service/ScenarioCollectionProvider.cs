using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace Rogue.NET.ScenarioEditor.Service
{
    [Export(typeof(IScenarioCollectionProvider))]
    public class ScenarioCollectionProvider : IScenarioCollectionProvider
    {
        readonly IRogueEventAggregator _eventAggregator;

        // Level Design Collections
        public ObservableCollection<LevelTemplateViewModel> Levels { get; private set; }

        // Asset Collections
        public ObservableCollection<PlayerTemplateViewModel> PlayerClasses { get; private set; }
        public ObservableCollection<LayoutTemplateViewModel> Layouts { get; private set; }
        public ObservableCollection<EnemyTemplateViewModel> Enemies { get; private set; }
        public ObservableCollection<FriendlyTemplateViewModel> Friendlies { get; private set; }
        public ObservableCollection<EquipmentTemplateViewModel> Equipment { get; private set; }
        public ObservableCollection<ConsumableTemplateViewModel> Consumables { get; private set; }
        public ObservableCollection<DoodadTemplateViewModel> Doodads { get; private set; }
        public ObservableCollection<SkillSetTemplateViewModel> SkillSets { get; private set; }

        // Shared Non-Calculated "General" Assets
        public ObservableCollection<AlteredCharacterStateTemplateViewModel> AlteredCharacterStates { get; private set; }
        public ObservableCollection<AttackAttributeTemplateViewModel> AttackAttributes { get; private set; }
        public ObservableCollection<SymbolPoolItemTemplateViewModel> SymbolPool { get; private set; }

        // "General Assets" - these are calculated and provided by the ScenarioEditorModule
        public ObservableCollection<BrushTemplateViewModel> Brushes { get; private set; }

        [ImportingConstructor]
        public ScenarioCollectionProvider(
                IRogueEventAggregator eventAggregator)
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
            // Level Design Collections
            this.Levels = configurationData.Configuration.ScenarioDesign.LevelDesigns;

            // Assets
            this.PlayerClasses = configurationData.Configuration.PlayerTemplates;
            this.Layouts = configurationData.Configuration.LayoutTemplates;
            this.Enemies = configurationData.Configuration.EnemyTemplates;
            this.Friendlies = configurationData.Configuration.FriendlyTemplates;
            this.Equipment = configurationData.Configuration.EquipmentTemplates;
            this.Consumables = configurationData.Configuration.ConsumableTemplates;
            this.Doodads = configurationData.Configuration.DoodadTemplates;
            this.SkillSets = configurationData.Configuration.SkillTemplates;

            // Shared Non-calculated "General" Assets
            this.AlteredCharacterStates = configurationData.Configuration.AlteredCharacterStates;
            this.AttackAttributes = configurationData.Configuration.AttackAttributes;
            this.SymbolPool = configurationData.Configuration.SymbolPool;

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
