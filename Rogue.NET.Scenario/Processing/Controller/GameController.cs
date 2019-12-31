using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.Core.Processing.Event.Level;
using Rogue.NET.Core.Processing.Event.Scenario;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Scenario.Processing.Controller.Interface;
using Rogue.NET.Scenario.Processing.Event.Content;
using Rogue.NET.Scenario.Processing.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Scenario.Processing.Controller
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IGameController))]
    public class GameController : IGameController
    {
        readonly IScenarioResourceService _scenarioResourceService;
        readonly IScenarioStatisticsService _statisticsService;
        readonly IScenarioObjectiveService _scenarioObjectiveService;
        readonly IRogueEventAggregator _eventAggregator;
        readonly IScenarioGenerator _scenarioGenerator;
        readonly ICommandRouter _gameRouter;
        readonly IModelService _modelService;

        // PRIMARY MODEL STATE
        ScenarioContainer _scenarioContainer;

        [ImportingConstructor]
        public GameController(
            IScenarioResourceService resourceService,
            IScenarioStatisticsService scenarioStatisticsService,
            IScenarioObjectiveService scenarioObjectiveService,
            IRogueEventAggregator eventAggregator,
            IScenarioGenerator scenarioGenerator,
            ICommandRouter gameRouter,
            IModelService modelService)
        {
            _statisticsService = scenarioStatisticsService;
            _scenarioObjectiveService = scenarioObjectiveService;
            _scenarioResourceService = resourceService;
            _eventAggregator = eventAggregator;
            _scenarioGenerator = scenarioGenerator;
            _gameRouter = gameRouter;
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

            // Fetch scenario container from the cache
            _scenarioContainer = _scenarioResourceService.GetScenario(playerName);

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

            // Update the compressed level from the loaded one
            var loadedLevel = _modelService.Level;
            var compressedLevel = _scenarioContainer.Levels.First(level => level.Key == loadedLevel.Parameters.Number);

            compressedLevel.Checkin(loadedLevel);

            // Update the Save Level
            _scenarioContainer.SaveLevel = _scenarioContainer.CurrentLevel;
            _scenarioContainer.SaveLocation = PlayerStartLocation.SavePoint;

            // Update UI Parameters
            _scenarioContainer.ZoomFactor = _modelService.ZoomFactor;

            // Save scenario file to disk
            _scenarioResourceService.SaveScenario(_scenarioContainer);

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

            else if (levelNumber > _scenarioContainer.Configuration.ScenarioDesign.LevelDesigns.Count)
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
                // First halt processing of messages
                _gameRouter.Stop();

                // Update the level number in the container
                _scenarioContainer.CurrentLevel = levelNumber;

                //If level is not loaded - must load it from the dungeon file
                var nextLevel = _scenarioContainer.Levels.FirstOrDefault(level => level.Key == levelNumber);

                if (nextLevel == null)
                    throw new Exception("Level " + levelNumber.ToString() + " not found in the Scenario Container");

                // Unload current model - pass extractable contents to next level with Player
                IEnumerable<ScenarioObject> extractedContent = null;

                // Propagate UI Parameters - Set default to saved zoom factor
                double zoomFactor = _scenarioContainer.ZoomFactor;

                if (_modelService.IsLoaded)
                {
                    // Get Level to Compress back to memory
                    var loadedLevel = _modelService.Level;

                    // Find compressed buffer to update
                    var compressedLevel = _scenarioContainer.Levels.First(level => level.Key == loadedLevel.Parameters.Number);

                    // UPDATE COMPRESSED BUFFER
                    compressedLevel.Checkin(loadedLevel);

                    // Content moved to next level
                    extractedContent = _modelService.Unload();

                    // UI Parameters
                    zoomFactor = _modelService.ZoomFactor;
                }

                // Register next level data with the model service
                _modelService.Load(
                    _scenarioContainer.Player,
                    location,
                    nextLevel.Checkout(),
                    zoomFactor,
                    extractedContent ?? new ScenarioObject[] { },
                    _scenarioContainer.Encyclopedia,
                    _scenarioContainer.Configuration);

                // Notify Listeners - Level Loaded -> Game Update
                _eventAggregator.GetEvent<LevelLoadedEvent>().Publish();

                // Enable backend processing
                _gameRouter.Start();

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
