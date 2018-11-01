using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Core.Controller.Interface;
using Rogue.NET.Core.IO;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Core.Utility;
using Rogue.NET.Model.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rogue.NET.Core.Controller
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IModelController))]
    public class ModelController : IModelController
    {
        readonly IScenarioResourceService _resourceService;
        readonly IEventAggregator _eventAggregator;
        readonly IScenarioGenerator _scenarioGenerator;
        readonly IModelService _modelService;

        // PRIMARY MODEL STATE
        ScenarioFile _scenarioFile;
        ScenarioContainer _scenarioContainer;

        [ImportingConstructor]
        public ModelController(
            IScenarioResourceService resourceService,
            IEventAggregator eventAggregator,
            IScenarioGenerator scenarioGenerator,
            IModelService modelService)
        {
            _resourceService = resourceService;
            _eventAggregator = eventAggregator;
            _scenarioGenerator = scenarioGenerator;
            _modelService = modelService;
        }

        public void Initialize()
        {
            //Make sure save directory exists
            try
            {
                if (!Directory.Exists("..\\save"))
                    Directory.CreateDirectory("..\\save");
            }
            catch (Exception)
            {
                _eventAggregator.GetEvent<MessageBoxEvent>().Publish("Error creating save directory - will not be able to save game progress");
            }

            _eventAggregator.GetEvent<NewScenarioEvent>().Subscribe((e) =>
            {
                var config = _resourceService.GetScenarioConfigurations().FirstOrDefault(c => c.DungeonTemplate.Name == e.ScenarioName);
                if (config != null)
                    New(config, e.RogueName, e.Seed, e.SurvivorMode);
            }, true);

            _eventAggregator.GetEvent<OpenScenarioEvent>().Subscribe((e) =>
            {
                _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventArgs()
                {
                    SplashAction = SplashAction.Show,
                    SplashType = SplashEventType.Open
                });

                Open(e.ScenarioName);

                _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventArgs()
                {
                    SplashAction = SplashAction.Hide,
                    SplashType = SplashEventType.Open
                });
            });

            _eventAggregator.GetEvent<SaveScenarioEvent>().Subscribe(() =>
            {
                _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventArgs()
                {
                    SplashAction = SplashAction.Show,
                    SplashType = SplashEventType.Save
                });

                Save();

                _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventArgs()
                {
                    SplashAction = SplashAction.Hide,
                    SplashType = SplashEventType.Save
                });

                _eventAggregator.GetEvent<ScenarioSavedEvent>().Publish();
            });

            _eventAggregator.GetEvent<LoadLevelEvent>().Subscribe((e) =>
            {
                LoadLevel(e.LevelNumber, e.StartLocation);
            });

            _eventAggregator.GetEvent<ContinueScenarioEvent>().Subscribe(() =>
            {
                //Unpack snapshot from file
                var scenario = _scenarioFile.Unpack();

                //Check current level
                if (scenario.CurrentLevel < 1 || scenario.CurrentLevel > scenario.StoredConfig.DungeonTemplate.NumberOfLevels)
                    scenario.CurrentLevel = 1;

                LoadCurrentLevel();
            });

            // TODO MOVE THIS
            _eventAggregator.GetEvent<DeleteScenarioEvent>().Subscribe((e) =>
            {
                //var result = MessageBox.Show("Are you sure you want to delete this scenario?", "Delete " + e.ScenarioName, MessageBoxButton.YesNoCancel);
                //if (result == MessageBoxResult.Yes || result == MessageBoxResult.OK)
                //{
                //    var path = Path.Combine(Constants.SAVED_GAMES_DIR, e.ScenarioName + "." + Constants.SCENARIO_EXTENSION);
                //    if (File.Exists(path))
                //        File.Delete(path);

                //    _eventAggregator.GetEvent<ScenarioDeletedEvent>().Publish();
                //}
            });

            _eventAggregator.GetEvent<ScenarioTickEvent>().Subscribe(() =>
            {
                _scenarioContainer.TotalTicks++;
                UpdateScenarioInfo();
            });
        }

        public void New(ScenarioConfigurationContainer configuration, string characterName, int seed, bool survivorMode)
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventArgs()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.NewScenario
            });
            // send character color and initialize display message
            _eventAggregator.GetEvent<CreatingScenarioEvent>().Publish(new CreatingScenarioEventArgs()
            {
                Message = "Creating " + configuration.DungeonTemplate.Name + " Scenario...",
                Progress = 10,
                ScenarioName = configuration.DungeonTemplate.Name,
                SmileyBodyColor = ColorConverter.Convert(configuration.PlayerTemplate.SymbolDetails.SmileyBodyColor),
                SmileyLineColor = ColorConverter.Convert(configuration.PlayerTemplate.SymbolDetails.SmileyLineColor)
            });


            // TODO: Handle unloading state
            //if (_scenarioContainer != null)
            //{
            //    _scenarioContainer = null;
            //}

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
            _scenarioContainer.StartTime = DateTime.Now;

            _eventAggregator.GetEvent<CreatingScenarioEvent>().Publish(new CreatingScenarioEventArgs()
            {
                Message = "Compressing Scenario in Memory...",
                Progress = 90
            });

            //Compress to dungeon file
            _scenarioFile = ScenarioFile.Create(_scenarioContainer);

            _eventAggregator.GetEvent<CreatingScenarioEvent>().Publish(new CreatingScenarioEventArgs()
            {
                Message = "Loading First Level...",
                Progress = 95
            });

            //Unpack bare minimum to dungeon object - get levels as needed
            _scenarioContainer = _scenarioFile.Unpack();

            LoadCurrentLevel();

            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventArgs()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.NewScenario
            });

            _eventAggregator.GetEvent<LevelInitializedEvent>().Publish();
        }
        public void Open(string file)
        {
            if (_scenarioContainer != null)
            {
                _scenarioContainer = null;
            }

            //Read dungeon file - TODO: Handle exceptions
            _scenarioFile = ScenarioFile.Open(File.ReadAllBytes(file));
            if (_scenarioFile == null)
                return;

            //Unpack bare minimum to dungeon object - get levels as needed
            _scenarioContainer = _scenarioFile.Unpack();

            LoadLevel(_scenarioContainer.CurrentLevel, PlayerStartLocation.AtCurrent);
        }
        public void Save()
        {
            _scenarioFile.Update(_scenarioContainer);

            // TODO
            //_scenarioFile.Save(Constants.SAVED_GAMES_DIR + "\\" + _scenarioContainer.Player1.RogueName + "." + Constants.SCENARIO_EXTENSION);
        }

        public void LoadCurrentLevel()
        {
            LoadLevel(_scenarioContainer.SaveLevel, _scenarioContainer.SaveLocation);
        }
        private void LoadLevel(int levelNumber, PlayerStartLocation location)
        {
            if (levelNumber <= _scenarioContainer.StoredConfig.DungeonTemplate.NumberOfLevels && levelNumber > 0)
            {
                _scenarioContainer.CurrentLevel = levelNumber;

                //If level is not loaded - must load it from the dungeon file
                var nextLevel = _scenarioContainer.LoadedLevels.FirstOrDefault(level => level.Number == levelNumber);
                if (nextLevel == null)
                {
                    nextLevel = _scenarioFile.Checkout(levelNumber);
                    _scenarioContainer.LoadedLevels.Add(nextLevel);
                }

                //if (_unityContainer.IsRegistered<LevelData>())
                //    _unityContainer.Teardown(_unityContainer.Resolve<LevelData>());

                // Register instance of level data object in the container
                //_unityContainer.RegisterInstance<LevelData>(new LevelData()
                //{
                //    Level = nextLevel,
                //    Player = _scenarioContainer.Player1,
                //    Encyclopedia = _scenarioContainer.ItemEncyclopedia,
                //    ObjectiveDescription = _scenarioContainer.StoredConfig.DungeonTemplate.ObjectiveDescription,
                //    Config = _scenarioContainer.StoredConfig,
                //    Seed = _scenarioContainer.Seed, 
                //    StartLocation = location
                //}, new ContainerControlledLifetimeManager());

                // Publish event containing level - subscribers operate on level as necessary

                // TODO - NEED TO PUBLISH LEVEL DATA
                _eventAggregator.GetEvent<LevelLoadedEvent>().Publish();

                UpdateScenarioInfo();
            }
        }

        public Dictionary<string, string> GetGameDisplayStats()
        {
            Dictionary<string, string> stats = new Dictionary<string, string>();

            //Make sure levels are checked out
            for (int i = 1; i <= _scenarioContainer.StoredConfig.DungeonTemplate.NumberOfLevels; i++)
            {
                if (!_scenarioContainer.LoadedLevels.Any(z => z.Number == i))
                    _scenarioContainer.LoadedLevels.Add(_scenarioFile.Checkout(i));
            }

            stats.Add("Percentage Cleared", (((double)_scenarioContainer.ScenarioEncyclopedia.Count(z => z.Value.IsIdentified) / (double)_scenarioContainer.ScenarioEncyclopedia.Count()) * 100.0D).ToString("N0"));
            stats.Add("Steps Taken", _scenarioContainer.LoadedLevels.Sum(z => z.StepsTaken).ToString());
            stats.Add("Monsters Killed", _scenarioContainer.LoadedLevels.Sum(z => z.MonstersKilled.Sum(y => y.Value)).ToString());
            stats.Add("Items Found", _scenarioContainer.LoadedLevels.Sum(z => z.ItemsFound.Sum(y => y.Value)).ToString());
            stats.Add("Unique Items Found", _scenarioContainer.LoadedLevels.Sum(z => z.ItemsFound.Where(y => _scenarioContainer.ScenarioEncyclopedia[y.Key].IsUnique).Sum(x => x.Value)).ToString());
            stats.Add("Unique Monsters Found", _scenarioContainer.LoadedLevels.Sum(z => z.MonstersKilled.Where(y => _scenarioContainer.ScenarioEncyclopedia[y.Key].IsUnique).Sum(x => x.Value)).ToString());
            stats.Add("Total Score", _scenarioContainer.LoadedLevels.Sum(z => z.MonsterScore).ToString());

            return stats;
        }

        /// <summary>
        /// Updates game info view data
        /// </summary>
        public void UpdateScenarioInfo()
        {
            _eventAggregator.GetEvent<ScenarioInfoUpdatedEvent>().Publish(new ScenarioInfoUpdatedEventArgs()
            {
                Ticks = _scenarioContainer.TotalTicks,
                CurrentLevel = _scenarioContainer.CurrentLevel,
                ObjectiveAcheived = _scenarioContainer.ObjectiveAcheived,
                PlayerName = _scenarioContainer.Player1.RogueName,
                ScenarioName = _scenarioContainer.StoredConfig.DungeonTemplate.Name,
                Seed = _scenarioContainer.Seed,
                StartTime = _scenarioContainer.StartTime,
                SurvivorMode = _scenarioContainer.SurvivorMode
            });
        }
    }
}
