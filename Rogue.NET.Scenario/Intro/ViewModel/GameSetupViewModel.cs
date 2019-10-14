using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Processing.Event.Scenario;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Scenario.Intro.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;

namespace Rogue.NET.Intro.ViewModel
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public class GameSetupViewModel : NotifyViewModel
    {
        readonly IScenarioResourceService _scenarioResourceService;
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
                IRogueEventAggregator eventAggregator)
        {
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

            this.Scenarios = new ObservableCollection<SavedGameViewModel>();
            this.Configurations = new ObservableCollection<ScenarioViewModel>();
            Reinitialize();
        }

        public void Reinitialize()
        {
            this.Scenarios.Clear();
            this.Configurations.Clear();

            // Get all saved games
            foreach (var scenarioInfo in _scenarioResourceService.GetScenarioInfos())
            {
                this.Scenarios.Add(new SavedGameViewModel()
                {
                    Name = scenarioInfo.Name,
                    SmileyExpression = scenarioInfo.SmileyExpression,
                    SmileyBodyColor = scenarioInfo.SmileyBodyColor,
                    SmileyLineColor = scenarioInfo.SmileyLineColor
                });
            }

            // Get all scenarios
            foreach (var scenarioConfigurationInfo in _scenarioResourceService.GetScenarioConfigurationInfos())
            {
                this.Configurations.Add(new ScenarioViewModel()
                {
                    Name = scenarioConfigurationInfo.Name,
                    Description = scenarioConfigurationInfo.Description,
                    SmileyExpression = scenarioConfigurationInfo.SmileyExpression,
                    SmileyBodyColor = scenarioConfigurationInfo.SmileyBodyColor,
                    SmileyLineColor = scenarioConfigurationInfo.SmileyLineColor,
                    CharacterClasses = new ObservableCollection<CharacterClassSelectionViewModel>(scenarioConfigurationInfo.PlayerTemplates.Select(x =>
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
