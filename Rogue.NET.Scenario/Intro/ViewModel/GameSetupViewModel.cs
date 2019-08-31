using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Event.Core;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Scenario.Intro.ViewModel;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Utility;

using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Media;
using System.Linq;

namespace Rogue.NET.Intro.ViewModel
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public class GameSetupViewModel : NotifyViewModel
    {
        readonly IScenarioResourceService _scenarioResourceService;
        readonly IScenarioFileService _scenarioFileService;
        readonly IRogueEventAggregator _eventAggregator;

        #region Fields
        string _rogueName = "";
        int _seed = 1;
        bool _survivorMode = false;
        ScenarioViewModel _selectedConfiguration;
        SavedGameViewModel _selectedGame;
        CharacterClassSelectionViewModel _selectedCharacterClass;
        #endregion

        #region Properties
        public ObservableCollection<SavedGameViewModel> Scenarios { get; set; }
        public ObservableCollection<ScenarioViewModel> Configurations { get; set; }

        public ScenarioViewModel SelectedConfiguration
        {
            get { return _selectedConfiguration; }
            set { this.RaiseAndSetIfChanged(ref _selectedConfiguration, value); }
        }
        public SavedGameViewModel SelectedGame
        {
            get { return _selectedGame; }
            set { this.RaiseAndSetIfChanged(ref _selectedGame, value); }
        }
        public CharacterClassSelectionViewModel SelectedCharacterClass
        {
            get { return _selectedCharacterClass; }
            set { this.RaiseAndSetIfChanged(ref _selectedCharacterClass, value); }
        }

        public string RogueName
        {
            get { return _rogueName; }
            set { this.RaiseAndSetIfChanged(ref _rogueName, value); }
        }
        public int Seed
        {
            get { return _seed; }
            set { this.RaiseAndSetIfChanged(ref _seed, value); }
        }
        public bool SurvivorMode
        {
            get { return _survivorMode; }
            set { this.RaiseAndSetIfChanged(ref _survivorMode, value); }
        }
        #endregion

        [ImportingConstructor]
        public GameSetupViewModel(
                IScenarioResourceService scenarioResourceService, 
                IScenarioFileService scenarioFileService, 
                IRogueEventAggregator eventAggregator)
        {
            _scenarioFileService = scenarioFileService;
            _scenarioResourceService = scenarioResourceService;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<ScenarioDeletedEvent>().Subscribe(() =>
            {
                Reinitialize();
            });
            _eventAggregator.GetEvent<ScenarioSavedEvent>().Subscribe(() =>
            {
                Reinitialize();
            });
            _eventAggregator.GetEvent<ResourcesInitializedEvent>().Subscribe(() =>
            {
                Reinitialize();
            });

            this.Scenarios = new ObservableCollection<SavedGameViewModel>();
            this.Configurations = new ObservableCollection<ScenarioViewModel>();
            Reinitialize();
        }

        public void Reinitialize()
        {
            this.Scenarios.Clear();
            this.Configurations.Clear();

            foreach (var header in _scenarioFileService.GetScenarioHeaders())
            {
                this.Scenarios.Add(new SavedGameViewModel(_eventAggregator)
                {
                    Name = header.Key,
                    SmileyExpression = header.Value.SmileyExpression,
                    SmileyBodyColor = (Color)ColorConverter.ConvertFromString(header.Value.SmileyBodyColor),
                    SmileyLineColor = (Color)ColorConverter.ConvertFromString(header.Value.SmileyLineColor)
                });
            }

            // Select Configurations with at least ONE PLAYER TEMPLATE (Character Class)
            foreach (var config in _scenarioResourceService.GetScenarioConfigurations()
                                                           .Where(x => x.PlayerTemplates.Any()))
            {
                this.Configurations.Add(new ScenarioViewModel()
                {
                    Name = config.DungeonTemplate.Name,
                    Description = config.DungeonTemplate.ObjectiveDescription,
                    SmileyExpression = config.PlayerTemplates.First().SymbolDetails.SmileyExpression,
                    SmileyBodyColor = ColorUtility.Convert(config.PlayerTemplates.First().SymbolDetails.SmileyBodyColor),
                    SmileyLineColor = ColorUtility.Convert(config.PlayerTemplates.First().SymbolDetails.SmileyLineColor),
                    CharacterClasses = new ObservableCollection<CharacterClassSelectionViewModel>(config.PlayerTemplates
                                                                                                        .Select(x =>
                    new CharacterClassSelectionViewModel(x.SymbolDetails)
                    {
                        Description = x.LongDescription,
                        RogueName = x.Name
                    }))
                });
            }
        }
    }
}
