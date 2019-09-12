using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace Rogue.NET.Controller.ScenarioEditor
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioEditorController))]
    public class ScenarioEditorController : IScenarioEditorController
    {
        readonly IRogueEventAggregator _eventAggregator;
        readonly IScenarioAssetReferenceService _scenarioAssetReferenceService;
        readonly IScenarioConfigurationUndoService _rogueUndoService;
        readonly IScenarioResourceService _scenarioResourceService;
        readonly IScenarioFileService _scenarioFileService;
        readonly IScenarioValidationService _scenarioValidationService;
        readonly IAlterationNameService _alterationNameService;

        readonly ScenarioConfigurationMapper _configurationMapper;

        ScenarioConfigurationContainerViewModel _config;

        [ImportingConstructor]
        public ScenarioEditorController(
            IRogueEventAggregator eventAggregator,
            IScenarioAssetReferenceService scenarioAssetReferenceService,
            IScenarioConfigurationUndoService rogueUndoService,
            IScenarioResourceService scenarioResourceService,
            IScenarioFileService scenarioFileService,
            IScenarioValidationService scenarioValidationService,
            IAlterationNameService alterationNameService)
        {
            _eventAggregator = eventAggregator;
            _rogueUndoService = rogueUndoService;
            _scenarioAssetReferenceService = scenarioAssetReferenceService;
            _scenarioResourceService = scenarioResourceService;
            _scenarioFileService = scenarioFileService;
            _scenarioValidationService = scenarioValidationService;
            _alterationNameService = alterationNameService;

            _configurationMapper = new ScenarioConfigurationMapper();

            Initialize();
        }

        public ScenarioConfigurationContainerViewModel CurrentConfig { get { return _config; } }

        private void Initialize()
        {
            _eventAggregator.GetEvent<LoadBuiltInScenarioEvent>().Subscribe((configResource) =>
            {
                Open(configResource.ToString(), true);
            });

            _eventAggregator.GetEvent<LoadScenarioEvent>().Subscribe((scenarioName) =>
            {
                Open(scenarioName, false);
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
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventData()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Loading
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

            // Register with the Undo Service
            _rogueUndoService.Register(_config);

            // Publish configuration
            _eventAggregator.GetEvent<ScenarioLoadedEvent>().Publish(_config);

            // Hide Splash Screen
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventData()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.Loading
            });

            PublishOutputMessage("Opened Scenario " + name);
        }

        public void Save()
        {
            Save(false);
        }
        public void Save(bool builtInScenario = false, ConfigResources builtInScenarioType = ConfigResources.Fighter)
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventData()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Loading
            });

            PublishOutputMessage("Saving " + _config.DungeonTemplate.Name + " Scenario File...");

            // SET ALTERATION EFFECT NAMES BEFORE MAPPING (THIS COULD BE REDESIGNED)
            _alterationNameService.Execute(_config);

            // Map back to the model namespace
            var config = _configurationMapper.MapBack(_config);

            // Validate - if invalid and user chooses not to proceed - then hide the splash display and
            //            return.
            if (!_scenarioValidationService.IsValid(config) &&
                 MessageBox.Show("Scenario is not valid and will not be playable. Save anyway?",
                                 "Scenario Invalid",
                                 MessageBoxButton.YesNoCancel) != MessageBoxResult.Yes)
            {
                _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventData()
                {
                    SplashAction = SplashAction.Hide,
                    SplashType = SplashEventType.Loading
                });

                PublishOutputMessage("Scenario Invalid - Save Terminated");

                return;
            }

            // Save the configuration
            if (builtInScenario)
                _scenarioFileService.EmbedConfiguration(builtInScenarioType, config);
            else
                _scenarioFileService.SaveConfiguration(_config.DungeonTemplate.Name, config);

            // Clear the Undo stack
            _rogueUndoService.Clear();

            PublishOutputMessage("Save complete");

            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventData()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.Loading
            });

            _eventAggregator.GetEvent<ScenarioLoadedEvent>().Publish(_config);
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
