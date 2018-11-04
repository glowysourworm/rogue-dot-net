using Prism.Commands;
using Prism.Events;
using Rogue.NET.Common.Events.ScenarioEditor;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.Events;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    public interface IEditorViewModel
    {
        string ScenarioName { get; set; }

        ICommand ExitCommand { get; }

        ICommand LoadBuiltInCommand { get; }
        ICommand SaveCommand { get; }
        ICommand NewCommand { get; }
        ICommand ShowDifficultyCommand { get; }
    }
    [Export(typeof(IEditorViewModel))]
    public class EditorViewModel : NotifyViewModel, IEditorViewModel
    {
        readonly IEventAggregator _eventAggregator;

        string _scenarioName;

        public string ScenarioName
        {
            get { return _scenarioName; }
            set
            {
                _scenarioName = value;
                OnPropertyChanged("ScenarioName");
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    _eventAggregator.GetEvent<ExitScenarioEditorEvent>().Publish();
                });
            }
        }

        public ICommand LoadBuiltInCommand
        {
            get
            {
                return new DelegateCommand<string>((name) =>
                {
                    _eventAggregator.GetEvent<LoadBuiltInScenarioEvent>().Publish(new LoadBuiltInScenarioEventArgs() { ScenarioName = name });
                });
            }
        }

        public ICommand NewCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    _eventAggregator.GetEvent<NewScenarioConfigEvent>().Publish();
                });
            }
        }

        public ICommand ShowDifficultyCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    _eventAggregator.GetEvent<ScoreScenarioEvent>().Publish();
                });
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new DelegateCommand<string>((name) =>
                {
                    _eventAggregator.GetEvent<Rogue.NET.ScenarioEditor.Events.SaveScenarioEvent>().
                        Publish(new Rogue.NET.ScenarioEditor.Events.SaveScenarioEventArgs()
                        {
                            ScenarioName = name
                        });
                });
            }
        }
        [ImportingConstructor]
        public EditorViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            this.ScenarioName = "My Scenario";

            Initialize();
        }

        private void Initialize()
        {
            // Listen to scenario config loaded event
            _eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe((e) =>
            {
                this.ScenarioName = e.DungeonTemplate.Name;
            });
        }
    }
}
