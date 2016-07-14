using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;
using Rogue.NET.Common;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Model.Events;
using Rogue.NET.Model.Generation;
using Rogue.NET.Model.Logic;
using Rogue.NET.Model.Scenario;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Rogue.NET.Model
{
    /// <summary>
    /// Stateful model managing controller
    /// </summary>
    public interface IModelController
    {
        // scenario loading
        void New(ScenarioConfiguration config, string rogueName, int seed, bool survivorMode);
        void Open(string file);
        void Save();

        void LoadCurrentLevel();
        void LoadFirstLevel();

        // TODO: move this 
        Dictionary<string, string> GetGameDisplayStats();
    }

    public class ModelController : IModelController
    {
        readonly IResourceService _resourceService;
        readonly IUnityContainer _unityContainer;
        readonly IEventAggregator _eventAggregator;
        readonly IDungeonGenerator _dungeonGenerator;

        private ScenarioContainer _scenarioContainer = null;
        private ScenarioFile _scenarioFile = null;

        private bool _saveHasOccurred = false;

        //TODO: Make into 2 events for aggregator
        public bool ObjectiveAcheived
        {
            get { return _scenarioContainer.ObjectiveAcheived; }
            set { _scenarioContainer.ObjectiveAcheived = value; }
        }

        public ModelController(
            IResourceService resourceService,
            IUnityContainer unityContainer,
            IEventAggregator eventAggregator,
            IDungeonGenerator dungeonGenerator)
        {
            _resourceService = resourceService;
            _unityContainer = unityContainer;
            _eventAggregator = eventAggregator;
            _dungeonGenerator = dungeonGenerator;

            Initialize();
        }

        private void Initialize()
        {
            //Make sure save directory exists
            try
            {
                if (!Directory.Exists("..\\save"))
                    Directory.CreateDirectory("..\\save");
            }
            catch (Exception)
            {
                _eventAggregator.GetEvent<MessageBoxEvent>().Publish(new MessageBoxEvent()
                {
                    Message = "Error creating save directory - will not be able to save game progress"
                });
            }

            _eventAggregator.GetEvent<NewScenarioEvent>().Subscribe((e) =>
            {
                var config = _resourceService.GetScenarioConfigurations().FirstOrDefault(c => c.DungeonTemplate.Name == e.ScenarioName);
                if (config != null)
                    New(config, e.RogueName, e.Seed, e.SurvivorMode);
            }, true);

            _eventAggregator.GetEvent<OpenScenarioEvent>().Subscribe((e) =>
            {
                _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEvent()
                {
                    SplashAction = SplashAction.Show,
                    SplashType = SplashEventType.Open
                });

                Open(Constants.SAVED_GAMES_DIR + "\\" + e.ScenarioName + "." + Constants.SCENARIO_EXTENSION);

                _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEvent()
                {
                    SplashAction = SplashAction.Hide,
                    SplashType = SplashEventType.Open
                });
            });

            _eventAggregator.GetEvent<SaveScenarioEvent>().Subscribe((e) =>
            {
                _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEvent()
                {
                    SplashAction = SplashAction.Show,
                    SplashType = SplashEventType.Save
                });

                Save();

                _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEvent()
                {
                    SplashAction = SplashAction.Hide,
                    SplashType = SplashEventType.Save
                });

                _eventAggregator.GetEvent<ScenarioSavedEvent>().Publish(new ScenarioSavedEvent());
            });

            _eventAggregator.GetEvent<LoadLevelEvent>().Subscribe((e) =>
            {
                LoadLevel(e.LevelNumber, e.StartLocation);
            });

            _eventAggregator.GetEvent<ContinueScenarioEvent>().Subscribe((e) =>
            {
                //Unpack snapshot from file
                _scenarioContainer = _scenarioFile.Unpack();

                //Check current level
                if (_scenarioContainer.CurrentLevel < 1 || _scenarioContainer.CurrentLevel > _scenarioContainer.StoredConfig.DungeonTemplate.NumberOfLevels)
                    _scenarioContainer.CurrentLevel = 1;

                LoadCurrentLevel();
            });

            _eventAggregator.GetEvent<DeleteScenarioEvent>().Subscribe((e) =>
            {
                var result = MessageBox.Show("Are you sure you want to delete this scenario?", "Delete " + e.ScenarioName, MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes || result == MessageBoxResult.OK)
                {
                    var path = Path.Combine(Constants.SAVED_GAMES_DIR, e.ScenarioName + "." + Constants.SCENARIO_EXTENSION);
                    if (File.Exists(path))
                        File.Delete(path);

                    _eventAggregator.GetEvent<ScenarioDeletedEvent>().Publish(new ScenarioDeletedEvent());
                }
            });

            _eventAggregator.GetEvent<ScenarioTickEvent>().Subscribe((e) =>
            {
                _scenarioContainer.TotalTicks++;
                UpdateScenarioInfo();
            });
        }

        public void New(ScenarioConfiguration c, string characterName, int seed, bool survivorMode)
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEvent()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.NewScenario
            });

            if (_scenarioContainer != null)
            {
                _unityContainer.Teardown(_scenarioContainer);
                _scenarioContainer = null;
            }

            // send character color and initialize display message
            _eventAggregator.GetEvent<CreatingScenarioEvent>().Publish(new CreatingScenarioEvent()
            {
                Message = "Creating " + c.DungeonTemplate.Name + " Scenario...",
                Progress = 10,
                ScenarioName = c.DungeonTemplate.Name,
                SmileyBodyColor = (Color)ColorConverter.ConvertFromString(c.PlayerTemplate.SymbolDetails.SmileyBodyColor),
                SmileyLineColor = (Color)ColorConverter.ConvertFromString(c.PlayerTemplate.SymbolDetails.SmileyLineColor)
            });

            //Create expanded dungeon contents in memory
            _scenarioContainer = (characterName.ToUpper() == "DEBUG") ? _dungeonGenerator.CreateDebugScenario(c) :
                _dungeonGenerator.CreateScenario(c, seed, survivorMode);

            _scenarioContainer.Seed = seed;
            _scenarioContainer.Player1.RogueName = characterName;
            _scenarioContainer.StoredConfig = c;
            _scenarioContainer.SurvivorMode = survivorMode;
            _scenarioContainer.StartTime = DateTime.Now;

            _eventAggregator.GetEvent<CreatingScenarioEvent>().Publish(new CreatingScenarioEvent()
            {
                Message = "Compressing Scenario in Memory...",
                Progress = 90
            });

            //Compress to dungeon file
            _scenarioFile = ScenarioFile.Create(_scenarioContainer);

            _eventAggregator.GetEvent<CreatingScenarioEvent>().Publish(new CreatingScenarioEvent()
            {
                Message = "Loading First Level...",
                Progress = 95
            });

            //Unpack bare minimum to dungeon object - get levels as needed
            _scenarioContainer = _scenarioFile.Unpack();

            LoadFirstLevel();

            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEvent()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.NewScenario
            });

            _saveHasOccurred = false;
        }
        public void Open(string file)
        {
            if (_scenarioContainer != null)
            {
                _unityContainer.Teardown(_scenarioContainer);
                _scenarioContainer = null;
            }

            //Read dungeon file
            _scenarioFile = ScenarioFile.Open(file);
            if (_scenarioFile == null)
                return;

            //Unpack bare minimum to dungeon object - get levels as needed
            _scenarioContainer = _scenarioFile.Unpack();

            //Check current level
            if (_scenarioContainer.CurrentLevel < 1 || _scenarioContainer.CurrentLevel > _scenarioContainer.StoredConfig.DungeonTemplate.NumberOfLevels)
                _scenarioContainer.CurrentLevel = 1;

            _saveHasOccurred = false;

            LoadLevel(_scenarioContainer.CurrentLevel, PlayerStartLocation.AtCurrent);
        }
        public void Save()
        {
            _scenarioFile.Update(_scenarioContainer);
            _scenarioFile.Save(Constants.SAVED_GAMES_DIR + "\\" + _scenarioContainer.Player1.RogueName + "." + Constants.SCENARIO_EXTENSION);

            _saveHasOccurred = true;
        }

        public void LoadCurrentLevel()
        {
            LoadLevel(_scenarioContainer.CurrentLevel, _saveHasOccurred ? PlayerStartLocation.AtCurrent : PlayerStartLocation.StairsUp);
        }
        public void LoadFirstLevel()
        {
            LoadLevel(1, _saveHasOccurred ? PlayerStartLocation.AtCurrent : PlayerStartLocation.StairsUp);
        }
        private void LoadLevel(int number, PlayerStartLocation location)
        {
            if (number <= _scenarioContainer.StoredConfig.DungeonTemplate.NumberOfLevels && number > 0)
            {
                _scenarioContainer.CurrentLevel = number;

                //If level is not loaded - must load it from the dungeon file
                var nextLevel = _scenarioContainer.LoadedLevels.FirstOrDefault(level => level.Number == number);
                if (nextLevel == null)
                {
                    nextLevel = _scenarioFile.Checkout(number);
                    _scenarioContainer.LoadedLevels.Add(nextLevel);
                }

                if (_unityContainer.IsRegistered<LevelData>())
                    _unityContainer.Teardown(_unityContainer.Resolve<LevelData>());

                // Register instance of level data object in the container
                _unityContainer.RegisterInstance<LevelData>(new LevelData()
                {
                    Level = nextLevel,
                    Player = _scenarioContainer.Player1,
                    Encyclopedia = _scenarioContainer.ItemEncyclopedia,
                    ShopConsumables = _scenarioContainer.ShopConsumables,
                    ShopEquipment = _scenarioContainer.ShopEquipment,
                    ObjectiveDescription = _scenarioContainer.StoredConfig.DungeonTemplate.ObjectiveDescription,
                    Config = _scenarioContainer.StoredConfig,
                    Seed = _scenarioContainer.Seed, 
                    StartLocation = location
                }, new ContainerControlledLifetimeManager());

                // Publish event containing level - subscribers operate on level as necessary
                _eventAggregator.GetEvent<LevelLoadedEvent>().Publish(new LevelLoadedEvent()
                {
                    Data = _unityContainer.Resolve<LevelData>()
                });

                UpdateScenarioInfo();
            }
        }

        public void SaveKeyFile()
        {
            //Save datetime
            _scenarioContainer.CompletedTime = DateTime.Now;

            DifficultyKey existingKey = ResourceManager.OpenKeyFile();
            if (existingKey == null)
                existingKey = new DifficultyKey();


            ResourceManager.SaveKeyFile(existingKey);
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

            stats.Add("Percentage Cleared", (((double)_scenarioContainer.ItemEncyclopedia.Count(z => z.Value.IsIdentified) / (double)_scenarioContainer.ItemEncyclopedia.Count()) * 100.0D).ToString("N0"));
            stats.Add("Steps Taken", _scenarioContainer.LoadedLevels.Sum(z => z.StepsTaken).ToString());
            stats.Add("Monsters Killed", _scenarioContainer.LoadedLevels.Sum(z => z.MonstersKilled.Sum(y => y.Value)).ToString());
            stats.Add("Items Found", _scenarioContainer.LoadedLevels.Sum(z => z.ItemsFound.Sum(y => y.Value)).ToString());
            stats.Add("Unique Items Found", _scenarioContainer.LoadedLevels.Sum(z => z.ItemsFound.Where(y => _scenarioContainer.ItemEncyclopedia[y.Key].IsUnique).Sum(x => x.Value)).ToString());
            stats.Add("Unique Monsters Found", _scenarioContainer.LoadedLevels.Sum(z => z.MonstersKilled.Where(y => _scenarioContainer.ItemEncyclopedia[y.Key].IsUnique).Sum(x => x.Value)).ToString());
            stats.Add("Total Score", _scenarioContainer.LoadedLevels.Sum(z => z.ItemScore + z.MonsterScore).ToString());

            return stats;
        }

        /// <summary>
        /// Updates game info view data
        /// </summary>
        public void UpdateScenarioInfo()
        {
            _eventAggregator.GetEvent<ScenarioInfoUpdatedEvent>().Publish(new ScenarioInfoUpdatedEvent()
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
