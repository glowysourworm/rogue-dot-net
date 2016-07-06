using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Rogue.NET.Common;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Model;
using Rogue.NET.Model.Generation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Rogue.NET.Intro.ViewModel
{
    public class GameSetupViewModel : INotifyPropertyChanged
    {
        readonly IResourceService _resourceService;
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
                        _eventAggregator.GetEvent<DeleteScenarioEvent>().Publish(new DeleteScenarioEvent()
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
                        _eventAggregator.GetEvent<OpenScenarioEvent>().Publish(new OpenScenarioEvent()
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

        public GameSetupViewModel(IResourceService resourceService, IEventAggregator eventAggregator)
        {
            _resourceService = resourceService;
            _eventAggregator = eventAggregator;

            // subscribe to opening / loading events
            _eventAggregator.GetEvent<SplashUpdateEvent>().Subscribe(e =>
            {
                this.LoadingMessage = e.Message;
                this.LoadingProgress = e.Progress;
            });

            _eventAggregator.GetEvent<ScenarioDeletedEvent>().Subscribe((e) =>
            {
                Reinitialize();
            });
            _eventAggregator.GetEvent<ScenarioSavedEvent>().Subscribe((e) =>
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

            foreach (var header in _resourceService.GetScenarioHeaders())
            {
                this.Scenarios.Add(new ScenarioSelectionViewModel(_eventAggregator)
                {
                    Name = header.Key,
                    SmileyColor = (Color)ColorConverter.ConvertFromString(header.Value.SmileyBodyColor),
                    SmileyLineColor = (Color)ColorConverter.ConvertFromString(header.Value.SmileyLineColor)
                });
            }

            foreach (var config in _resourceService.GetScenarioConfigurations())
            {
                var smiley = TemplateGenerator.GenerateSymbol(config.PlayerTemplate.SymbolDetails);
                this.Configs.Add(new ScenarioSelectionViewModel(_eventAggregator)
                {
                    Name = config.DungeonTemplate.Name,
                    SmileyColor = (Color)ColorConverter.ConvertFromString(smiley.SmileyBodyColor),
                    SmileyLineColor = (Color)ColorConverter.ConvertFromString(smiley.SmileyLineColor),
                    Description = config.DungeonTemplate.ObjectiveDescription,
                    NumberOfLevels = config.DungeonTemplate.NumberOfLevels
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
