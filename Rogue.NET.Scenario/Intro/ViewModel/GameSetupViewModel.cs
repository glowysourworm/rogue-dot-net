using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Event.Core;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Scenario.Intro.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Media;
using System.Linq;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using Rogue.NET.Common.Extension;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using Rogue.NET.Core.Model.Scenario.Content;
using System.Collections.Generic;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;

namespace Rogue.NET.Intro.ViewModel
{
    [Export]
    public class GameSetupViewModel : NotifyViewModel
    {
        readonly IScenarioResourceService _scenarioResourceService;
        readonly IScenarioFileService _scenarioFileService;
        readonly IAttackAttributeGenerator _attackAttributeGenerator;
        readonly IScenarioMetaDataGenerator _scenarioMetaDataGenerator;
        readonly IEventAggregator _eventAggregator;

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
                IAttackAttributeGenerator attackAttributeGenerator,
                IScenarioMetaDataGenerator scenarioMetaDataGenerator,
                IEventAggregator eventAggregator)
        {
            _scenarioFileService = scenarioFileService;
            _scenarioResourceService = scenarioResourceService;
            _attackAttributeGenerator = attackAttributeGenerator;
            _scenarioMetaDataGenerator = scenarioMetaDataGenerator;
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
                    SmileyColor = (Color)ColorConverter.ConvertFromString(header.Value.SmileyBodyColor),
                    SmileyLineColor = (Color)ColorConverter.ConvertFromString(header.Value.SmileyLineColor)
                });
            }

            foreach (var config in _scenarioResourceService.GetScenarioConfigurations())
            {
                // Identify Skill Sets for showing on the startup screen
                var skillSets = config.SkillTemplates.Select(x => _scenarioMetaDataGenerator.CreateScenarioMetaData(x))
                                                     .Actualize();

                skillSets.ForEach(x => x.IsIdentified = true);

                this.Configurations.Add(new ScenarioViewModel()
                {
                    Name = config.DungeonTemplate.Name,
                    SmileyColor = (Color)ColorConverter.ConvertFromString(config.PlayerTemplate.SymbolDetails.SmileyBodyColor),
                    SmileyLineColor = (Color)ColorConverter.ConvertFromString(config.PlayerTemplate.SymbolDetails.SmileyLineColor),
                    Description = config.DungeonTemplate.ObjectiveDescription,
                    CharacterClasses = new ObservableCollection<Scenario.Intro.ViewModel.CharacterClassSelectionViewModel>(config.CharacterClasses
                                                                                        .Select(x => 
                    new Scenario.Intro.ViewModel.CharacterClassSelectionViewModel()
                    {
                        HasBonusAttackAttributes = x.HasBonusAttackAttributes,
                        HasBonusAttribute = x.HasAttributeBonus,
                        BonusAttackAttributes = new ObservableCollection<AttackAttribute>(x.BonusAttackAttributes
                                                                                           .Where(z => z.Attack.IsSet() || z.Resistance.IsSet() || z.Weakness.IsSet())
                                                                                           .Select(z => _attackAttributeGenerator.GenerateAttackAttribute(z))),
                        BonusAttribute = x.BonusAttribute,
                        BonusAttributeValue = x.BonusAttributeValue,
                        MetaData = new ScenarioMetaDataViewModel(_scenarioMetaDataGenerator.CreateScenarioMetaData(x), _scenarioResourceService),
                        RogueName = x.Name,

                        Source = _scenarioResourceService.GetImageSource(x.SymbolDetails)
                    }))
                });
            }
        }
    }
}
