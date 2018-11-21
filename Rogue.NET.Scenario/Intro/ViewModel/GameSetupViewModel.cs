using Prism.Commands;
using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Event.Core;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Service.Interface;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using System.Windows.Media;

namespace Rogue.NET.Intro.ViewModel
{
    [Export]
    public class GameSetupViewModel : NotifyViewModel
    {
        readonly IScenarioResourceService _scenarioResourceService;
        readonly IScenarioFileService _scenarioFileService;
        readonly IEventAggregator _eventAggregator;

        #region Nested Classes
        public class ScenarioSelectionViewModel
        {
            readonly IEventAggregator _eventAggregator;

            public string Name { get; set; }
            public Color SmileyColor { get; set; }
            public Color SmileyLineColor { get; set; }
            public string Description { get; set; }
            public int NumberOfLevels { get; set; }
            public int CurrentLevel { get; set; }
            public bool ObjectiveAcheived { get; set; }

            public ICommand DeleteScenarioCommand
            {
                get
                {
                    return new DelegateCommand(() =>
                    {
                        _eventAggregator.GetEvent<DeleteScenarioEvent>().Publish(new DeleteScenarioEventArgs()
                        {
                            ScenarioName = this.Name
                        });
                    });
                }
            }
            public ICommand StartScenarioCommand
            {
                get
                {
                    return new DelegateCommand(() =>
                    {
                        _eventAggregator.GetEvent<OpenScenarioEvent>().Publish(new OpenScenarioEventArgs()
                        {
                            ScenarioName = this.Name
                        });
                    });
                }
            }

            public ScenarioSelectionViewModel(IEventAggregator eventAggregator)
            {
                _eventAggregator = eventAggregator;
            }
        }
        #endregion

        #region Properties
        public ObservableCollection<ScenarioSelectionViewModel> Scenarios { get; set; }
        public ObservableCollection<ScenarioSelectionViewModel> Configs { get; set; }

        string _config = "";
        string _rogueName = "";
        int _seed = 1;
        bool _survivorMode = false;
        AttributeEmphasis _emphasis = AttributeEmphasis.Strength;
        Color _bodyColor = Colors.Yellow;
        Color _lineColor = Colors.Black;

        string _loadingMessage = "";
        double _loadingProgress = 0;

        public string ScenarioName
        {
            get { return _config; }
            set
            {
                _config = value;
                OnPropertyChanged("ScenarioName");
            }
        }
        public string RogueName
        {
            get { return _rogueName; }
            set
            {
                _rogueName = value;
                OnPropertyChanged("RogueName");
            }
        }
        public Color SmileyColor
        {
            get {return _bodyColor;}
            set
            {
                _bodyColor = value;
                OnPropertyChanged("SmileyBodyColor");
            }
        }
        public Color SmileyLineColor
        {
            get {return _lineColor;}
            set
            {
                _lineColor = value;
                OnPropertyChanged("SmileyLineColor");
            }
        }
        public AttributeEmphasis AttributeEmphasis
        {
            get { return _emphasis; }
            set
            {
                _emphasis = value;
                OnPropertyChanged("AttributeEmphasis");
            }
        }
        public int Seed
        {
            get { return _seed; }
            set
            {
                _seed = value;
                OnPropertyChanged("Seed");
            }
        }
        public bool SurvivorMode
        {
            get { return _survivorMode; }
            set
            {
                _survivorMode = value;
                OnPropertyChanged("SurvivorMode");
            }
        }

        public string LoadingMessage
        {
            get { return _loadingMessage; }
            set
            {
                _loadingMessage = value;
                OnPropertyChanged("LoadingMessage");
            }
        }
        public double LoadingProgress
        {
            get { return _loadingProgress; }
            set
            {
                _loadingProgress = value;
                OnPropertyChanged("LoadingProgress");
            }
        }
        #endregion

        [ImportingConstructor]
        public GameSetupViewModel(IScenarioResourceService scenarioResourceService, IScenarioFileService scenarioFileService, IEventAggregator eventAggregator)
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

            this.Scenarios = new ObservableCollection<ScenarioSelectionViewModel>();
            this.Configs = new ObservableCollection<ScenarioSelectionViewModel>();
            Reinitialize();
        }

        public void Reinitialize()
        {
            this.Scenarios.Clear();
            this.Configs.Clear();

            foreach (var header in _scenarioFileService.GetScenarioHeaders())
            {
                this.Scenarios.Add(new ScenarioSelectionViewModel(_eventAggregator)
                {
                    Name = header.Key,
                    SmileyColor = (Color)ColorConverter.ConvertFromString(header.Value.SmileyBodyColor),
                    SmileyLineColor = (Color)ColorConverter.ConvertFromString(header.Value.SmileyLineColor)
                });
            }

            foreach (var config in _scenarioResourceService.GetScenarioConfigurations())
            {
                this.Configs.Add(new ScenarioSelectionViewModel(_eventAggregator)
                {
                    Name = config.DungeonTemplate.Name,
                    SmileyColor = (Color)ColorConverter.ConvertFromString(config.PlayerTemplate.SymbolDetails.SmileyBodyColor),
                    SmileyLineColor = (Color)ColorConverter.ConvertFromString(config.PlayerTemplate.SymbolDetails.SmileyLineColor),
                    Description = config.DungeonTemplate.ObjectiveDescription,
                    NumberOfLevels = config.DungeonTemplate.NumberOfLevels
                });
            }
        }
    }
}
