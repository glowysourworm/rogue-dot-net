using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.Core.Processing.Event.Level;
using Rogue.NET.Core.Processing.Event.Scenario;
using Rogue.NET.Core.Processing.IO;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Scenario.Processing.Controller.Interface;
using Rogue.NET.Scenario.Processing.Event.Content;
using Rogue.NET.Scenario.Processing.Service.Interface;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Scenario.Processing.Controller
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IGameController))]
    public class GameController : IGameController
    {
        readonly IScenarioResourceService _scenarioResourceService;
        readonly IScenarioFileService _scenarioFileService;
        readonly IScenarioStatisticsService _statisticsService;
        readonly IScenarioObjectiveService _scenarioObjectiveService;
        readonly IRogueEventAggregator _eventAggregator;
        readonly IScenarioGenerator _scenarioGenerator;
        readonly IBackendController _scenarioController;
        readonly IModelService _modelService;

        // PRIMARY MODEL STATE
        ScenarioFile _scenarioFile;
        ScenarioContainer _scenarioContainer;

        [ImportingConstructor]
        public GameController(
            IScenarioResourceService resourceService,
            IScenarioFileService scenarioFileService,
            IScenarioStatisticsService scenarioStatisticsService,
            IScenarioObjectiveService scenarioObjectiveService,
            IRogueEventAggregator eventAggregator,
            IScenarioGenerator scenarioGenerator,
            IBackendController scenarioController,
            IModelService modelService)
        {
            _statisticsService = scenarioStatisticsService;
            _scenarioObjectiveService = scenarioObjectiveService;
            _scenarioFileService = scenarioFileService;
            _scenarioResourceService = resourceService;
            _eventAggregator = eventAggregator;
            _scenarioGenerator = scenarioGenerator;
            _scenarioController = scenarioController;
            _modelService = modelService;
        }

        public void Initialize()
        {
            // New
            _eventAggregator.GetEvent<NewScenarioEvent>().Subscribe((e) =>
            {
                var config = _scenarioResourceService.GetScenarioConfiguration(e.ScenarioName);
                if (config != null)
                    New(config, e.RogueName, e.CharacterClassName, e.Seed, e.SurvivorMode);
            });

            // Open
            _eventAggregator.GetEvent<OpenScenarioEvent>().Subscribe((e) =>
            {
                Open(e.ScenarioName);
            });

            // Level Change / Save Events / Statistics
            _eventAggregator.GetEvent<ScenarioEvent>().Subscribe(update =>
            {
                switch (update.ScenarioUpdateType)
                {
                    case ScenarioUpdateType.LevelChange:
                        LoadLevel(update.LevelNumber, update.StartLocation);
                        break;
                    case ScenarioUpdateType.Save:
                        Save();
                        break;
                    case ScenarioUpdateType.StatisticsTick:
                        _statisticsService.ProcessScenarioTick(_scenarioContainer.Statistics);
                        PublishGameUpdate();
                        break;
                    case ScenarioUpdateType.StatisticsDoodadUsed:
                        _statisticsService.ProcessDoodadStatistics(_scenarioContainer.Statistics, _modelService.ScenarioEncyclopedia[update.ContentRogueName]);
                        PublishGameUpdate();
                        break;
                    case ScenarioUpdateType.StatisticsEnemyDeath:
                        _statisticsService.ProcessEnemyStatistics(_scenarioContainer.Statistics, _modelService.ScenarioEncyclopedia[update.ContentRogueName]);
                        PublishGameUpdate();
                        break;
                    case ScenarioUpdateType.StatisticsItemFound:
                        _statisticsService.ProcessItemStatistics(_scenarioContainer.Statistics, _modelService.ScenarioEncyclopedia[update.ContentRogueName]);
                        PublishGameUpdate();
                        break;
                }
            });

            // Continue Scenario
            _eventAggregator.GetEvent<ContinueScenarioEvent>().Subscribe(() =>
            {
                //Unpack snapshot from file
                _scenarioContainer = _scenarioFile.Unpack();

                LoadCurrentLevel();
            });
        }

        public void New(ScenarioConfigurationContainer configuration, string characterName, string characterClassName, int seed, bool survivorMode)
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventData()
            {                
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Loading
            });

            // Unload current scenario container
            if (_scenarioContainer != null)
            {
                // Unload old model
                _modelService.Unload();

                // Null-out container to indicate unloaded model
                _scenarioContainer = null;
            }

            //Create expanded dungeon contents in memory
            _scenarioContainer = _scenarioGenerator.CreateScenario(configuration, characterClassName, seed, survivorMode);

            _scenarioContainer.Seed = seed;
            _scenarioContainer.Player.RogueName = characterName;
            _scenarioContainer.Configuration = configuration;
            _scenarioContainer.SurvivorMode = survivorMode;
            _scenarioContainer.Statistics.StartTime = DateTime.Now;

            //Compress to dungeon file
            _scenarioFile = ScenarioFile.Create(_scenarioContainer);

            //Unpack bare minimum to dungeon object - get levels as needed
            _scenarioContainer = _scenarioFile.Unpack();

            // Loads level and fires event to listeners
            LoadCurrentLevel();

            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventData()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.Loading
            });
        }
        public void Open(string playerName)
        {
            // Show Splash
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventData()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Loading
            });

            // Unload current scenario container
            if (_scenarioContainer != null)
            {
                // Unload old model
                _modelService.Unload();

                // Null-out container to indicate unloaded model
                _scenarioContainer = null;
            }

            //Read dungeon file - TODO: Handle exceptions
            _scenarioFile = _scenarioFileService.OpenScenarioFile(playerName);

            if (_scenarioFile == null)
                return;

            //Unpack bare minimum to dungeon object - get levels as needed
            _scenarioContainer = _scenarioFile.Unpack();

            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventData()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.Loading
            });

            LoadCurrentLevel();
        }
        public void Save()
        {
            // Splash Show
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventData()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Loading
            });

            // Update the Save Level
            _scenarioContainer.SaveLevel = _scenarioContainer.CurrentLevel;
            _scenarioContainer.SaveLocation = PlayerStartLocation.SavePoint;

            // Update the ScenarioFile with unpacked levels
            _scenarioFile.Update(_scenarioContainer);

            // Save scenario file to disk
            _scenarioFileService.SaveScenarioFile(_scenarioFile, _scenarioContainer.Player.RogueName);

            // Hide Splash
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventData()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.Loading
            });

            // Notify listeners
            _eventAggregator.GetEvent<ScenarioSavedEvent>().Publish();
        }

        public void LoadCurrentLevel()
        {
            if (_scenarioContainer.SaveLevel != 0)
                LoadLevel(_scenarioContainer.SaveLevel, _scenarioContainer.SaveLocation);

            else
                LoadLevel(1, PlayerStartLocation.StairsUp);
        }
        private void LoadLevel(int levelNumber, PlayerStartLocation location)
        {
            if (levelNumber < 0)
                throw new ApplicationException("Trying to load level " + levelNumber.ToString());

            else if (levelNumber > _scenarioContainer.Configuration.DungeonTemplate.NumberOfLevels)
                throw new ApplicationException("Trying to load level " + levelNumber.ToString());

            // Check for Scenario Completed
            else if (levelNumber == 0)
            {
                // Player has WON! :)
                if (_scenarioObjectiveService.IsObjectiveAcheived(_scenarioContainer))
                {
                    // Publish scenario completed event
                    _eventAggregator.GetEvent<ScenarioEvent>().Publish(new ScenarioEventData()
                    {
                        ScenarioUpdateType = ScenarioUpdateType.ScenarioCompleted
                    });
                }
            }

            // Level number is valid
            else
            {
                // First halt processing of backend
                _scenarioController.Stop();

                // Update the level number in the container
                _scenarioContainer.CurrentLevel = levelNumber;

                //If level is not loaded - must load it from the dungeon file
                var nextLevel = _scenarioContainer.LoadedLevels.FirstOrDefault(level => level.Number == levelNumber);
                if (nextLevel == null)
                {
                    nextLevel = _scenarioFile.Checkout(levelNumber);
                    _scenarioContainer.LoadedLevels.Add(nextLevel);
                }

                // Unload current model
                _modelService.Unload();

                // Register next level data with the model service
                _modelService.Load(
                    _scenarioContainer.Player,
                    location,
                    nextLevel, 
                    _scenarioContainer.ScenarioEncyclopedia, 
                    _scenarioContainer.Configuration);

                // Notify Listeners - Level Loaded -> Game Update
                _eventAggregator.GetEvent<LevelLoadedEvent>().Publish();

                // Enable backend processing
                _scenarioController.Start();

                PublishGameUpdate();
            }
        }

        private void PublishGameUpdate()
        {
            _eventAggregator.GetEvent<GameUpdateEvent>().Publish(new GameUpdateEventArgs()
            {
                IsObjectiveAcheived = _scenarioObjectiveService.IsObjectiveAcheived(_scenarioContainer),
                IsSurvivorMode = _scenarioContainer.SurvivorMode,
                Statistics = _scenarioContainer.Statistics,
                Seed = _scenarioContainer.Seed,
                LevelNumber = _scenarioContainer.CurrentLevel,
                ScenarioObjectiveUpdates = _scenarioObjectiveService.GetScenarioObjectiveUpdates(_scenarioContainer),
            });
        }
    }
}
