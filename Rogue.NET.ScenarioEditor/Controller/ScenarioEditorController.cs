using Prism.Events;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System;
using System.ComponentModel.Composition;

namespace Rogue.NET.Controller.ScenarioEditor
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioEditorController))]
    public class ScenarioEditorController : IScenarioEditorController
    {
        readonly IEventAggregator _eventAggregator;
        readonly IScenarioAssetReferenceService _scenarioAssetReferenceService;
        readonly IScenarioConfigurationUndoService _rogueUndoService;
        readonly IScenarioResourceService _scenarioResourceService;
        readonly IScenarioFileService _scenarioFileService;

        readonly ScenarioConfigurationMapper _configurationMapper;

        ScenarioConfigurationContainerViewModel _config;

        [ImportingConstructor]
        public ScenarioEditorController(
            IEventAggregator eventAggregator,
            IScenarioAssetReferenceService scenarioAssetReferenceService,
            IScenarioConfigurationUndoService rogueUndoService,
            IScenarioResourceService scenarioResourceService,
            IScenarioFileService scenarioFileService)
        {
            _eventAggregator = eventAggregator;
            _rogueUndoService = rogueUndoService;
            _scenarioAssetReferenceService = scenarioAssetReferenceService;
            _scenarioResourceService = scenarioResourceService;
            _scenarioFileService = scenarioFileService;

            _configurationMapper = new ScenarioConfigurationMapper();

            Initialize();
        }

        public ScenarioConfigurationContainerViewModel CurrentConfig { get { return _config; } }

        private void Initialize()
        {
            _eventAggregator.GetEvent<LoadBuiltInScenarioEvent>().Subscribe((scenarioName) =>
            {
                Open(scenarioName, true);
            });

            _eventAggregator.GetEvent<Rogue.NET.ScenarioEditor.Events.SaveScenarioEvent>().Subscribe(() =>
            {
                Save();
            });

            _eventAggregator.GetEvent<SaveBuiltInScenarioEvent>().Subscribe((configResource) =>
            {
                Save(true, configResource);
            });

            _eventAggregator.GetEvent<NewScenarioConfigEvent>().Subscribe(() =>
            {
                New();
            });
        }

        public void New()
        {
            // Have to keep Undo Service in sync with the configuration
            if (_config != null)
                _rogueUndoService.Clear();

            // Create new Scenario Configuration
            _config = new ScenarioConfigurationContainerViewModel();

            // Register with the Undo Service
            _rogueUndoService.Register(_config);

            // Publish the Scenario Configuration
            _eventAggregator.GetEvent<ScenarioLoadedEvent>().Publish(_config);

            PublishOutputMessage("Created Scenario " + _config.DungeonTemplate.Name);
        }

        public void Open(string name, bool builtIn)
        {
            // Show Splash Screen
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Open
            });

            // Have to keep Undo Service in sync with the configuration
            if (_config != null)
                _rogueUndoService.Clear();

            // Open the Scenario Configuration from file
            ScenarioConfigurationContainer config;
            if (builtIn)
                config = _scenarioResourceService.GetScenarioConfiguration((ConfigResources)Enum.Parse(typeof(ConfigResources), name));
            else
                config = _scenarioFileService.OpenConfiguration(name);

            // Map to the view model
            _config = _configurationMapper.Map(config);

            // Update all config references to match them by name
            _scenarioAssetReferenceService.UpdateAll(_config);

            // Register with the Undo Service
            _rogueUndoService.Register(_config);

            // Publish configuration
            _eventAggregator.GetEvent<ScenarioLoadedEvent>().Publish(_config);

            // Hide Splash Screen
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.Open
            });

            PublishOutputMessage("Opened Scenario " + name);
        }

        public void Save()
        {
            Save(false);
        }
        public void Save(bool builtInScenario = false, ConfigResources builtInScenarioType = ConfigResources.Fighter)
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Save
            });

            PublishOutputMessage("Saving " + _config.DungeonTemplate.Name + " Scenario File...");

            // Map back to the model namespace
            var config = _configurationMapper.MapBack(_config);

            // Save the configuration
            if (builtInScenario)
                _scenarioFileService.EmbedConfiguration(builtInScenarioType, config);
            else
                _scenarioFileService.SaveConfiguration(_config.DungeonTemplate.Name, config);

            // Clear the Undo stack
            _rogueUndoService.Clear();

            PublishOutputMessage("Save complete");

            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.Save
            });

            _eventAggregator.GetEvent<ScenarioLoadedEvent>().Publish(_config);
        }

        public void Validate()
        {
            //TODO
        }

        private void PublishOutputMessage(string msg)
        {
            _eventAggregator.GetEvent<ScenarioEditorMessageEvent>().Publish(new ScenarioEditorMessageEventArgs()
            {
                Message = msg
            });
        }
    }
}
