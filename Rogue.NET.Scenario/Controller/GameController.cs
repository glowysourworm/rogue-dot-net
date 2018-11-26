﻿using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.IO;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Core.Utility;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Controller.Interface;
using Rogue.NET.Scenario.Events.Content;
using Rogue.NET.Scenario.Service.Interface;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Controller
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IGameController))]
    public class GameController : IGameController
    {
        readonly IScenarioResourceService _scenarioResourceService;
        readonly IScenarioFileService _scenarioFileService;
        readonly IScenarioStatisticsService _statisticsService;
        readonly IEventAggregator _eventAggregator;
        readonly IScenarioGenerator _scenarioGenerator;
        readonly IScenarioController _scenarioController;
        readonly IModelService _modelService;

        // PRIMARY MODEL STATE
        ScenarioFile _scenarioFile;
        ScenarioContainer _scenarioContainer;

        [ImportingConstructor]
        public GameController(
            IScenarioResourceService resourceService,
            IScenarioFileService scenarioFileService,
            IScenarioStatisticsService scenarioStatisticsService,
            IEventAggregator eventAggregator,
            IScenarioGenerator scenarioGenerator,
            IScenarioController scenarioController,
            IModelService modelService)
        {
            _statisticsService = scenarioStatisticsService;
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
            _eventAggregator.GetEvent<NewScenarioEvent>().Subscribe(async (e) =>
            {
                var config = _scenarioResourceService.GetScenarioConfiguration(e.ScenarioName);
                if (config != null)
                    await New(config, e.RogueName, e.Seed, e.SurvivorMode);
            });

            // Open
            _eventAggregator.GetEvent<OpenScenarioEvent>().Subscribe((e) =>
            {
                Open(e.ScenarioName);
            });

            // Level Change / Save Events / Statistics
            _eventAggregator.GetEvent<ScenarioUpdateEvent>().Subscribe(update =>
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

        public async Task New(ScenarioConfigurationContainer configuration, string characterName, int seed, bool survivorMode)
        {
            await _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.NewScenario
            });
            // send character color and initialize display message
            await _eventAggregator.GetEvent<CreatingScenarioEvent>().Publish(new CreatingScenarioEventArgs()
            {
                Message = "Creating " + configuration.DungeonTemplate.Name + " Scenario...",
                Progress = 10,
                ScenarioName = configuration.DungeonTemplate.Name,
                SmileyBodyColor = ColorUtility.Convert(configuration.PlayerTemplate.SymbolDetails.SmileyBodyColor),
                SmileyLineColor = ColorUtility.Convert(configuration.PlayerTemplate.SymbolDetails.SmileyLineColor)
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
#if DEBUG
            _scenarioContainer = (characterName.ToUpper() == "DEBUG") ? 
                _scenarioGenerator.CreateDebugScenario(configuration) :
                _scenarioGenerator.CreateScenario(configuration, seed, survivorMode);
#else
            _scenarioContainer = _scenarioGenerator.CreateScenario(configuration, seed, survivorMode);
#endif

            _scenarioContainer.Seed = seed;
            _scenarioContainer.Player1.RogueName = characterName;
            _scenarioContainer.StoredConfig = configuration;
            _scenarioContainer.SurvivorMode = survivorMode;
            _scenarioContainer.Statistics.StartTime = DateTime.Now;

            await _eventAggregator.GetEvent<CreatingScenarioEvent>().Publish(new CreatingScenarioEventArgs()
            {
                Message = "Compressing Scenario in Memory...",
                Progress = 50
            });

            //Compress to dungeon file
            _scenarioFile = ScenarioFile.Create(_scenarioContainer);

            await _eventAggregator.GetEvent<CreatingScenarioEvent>().Publish(new CreatingScenarioEventArgs()
            {
                Message = "Loading First Level...",
                Progress = 80
            });

            //Unpack bare minimum to dungeon object - get levels as needed
            _scenarioContainer = _scenarioFile.Unpack();

            // Loads level and fires event to listeners
            LoadCurrentLevel();

            await _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.NewScenario
            });
        }
        public async Task Open(string playerName)
        {
            // Show Splash
            await _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Open
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

            await _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.Open
            });

            LoadCurrentLevel();
        }
        public async Task Save()
        {
            // Splash Show
            await _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Save
            });

            // Update the Save Level
            _scenarioContainer.SaveLevel = _scenarioContainer.CurrentLevel;
            _scenarioContainer.SaveLocation = PlayerStartLocation.SavePoint;

            // Update the ScenarioFile with unpacked levels
            _scenarioFile.Update(_scenarioContainer);

            // Save scenario file to disk
            _scenarioFileService.SaveScenarioFile(_scenarioFile, _scenarioContainer.Player1.RogueName);

            // Hide Splash
            await _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.Save
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

            else if (levelNumber > _scenarioContainer.StoredConfig.DungeonTemplate.NumberOfLevels)
                throw new ApplicationException("Trying to load level " + levelNumber.ToString());

            // Check for Scenario Completed
            else if (levelNumber == 0)
            {
                // Player has WON! :)
                if (_scenarioContainer.IsObjectiveAcheived())
                {
                    // Publish scenario completed event
                    _eventAggregator.GetEvent<ScenarioUpdateEvent>().Publish(new ScenarioUpdate()
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
                    _scenarioContainer.Player1,
                    location,
                    nextLevel, 
                    _scenarioContainer.ScenarioEncyclopedia, 
                    _scenarioContainer.StoredConfig);

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
                ScenarioName = _scenarioContainer.StoredConfig.DungeonTemplate.Name,
                IsObjectiveAcheived = _scenarioContainer.IsObjectiveAcheived(),
                IsSurvivorMode = _scenarioContainer.SurvivorMode,
                Statistics = _scenarioContainer.Statistics,
                Seed = _scenarioContainer.Seed,
                LevelNumber = _scenarioContainer.CurrentLevel
            });
        }
    }
}
