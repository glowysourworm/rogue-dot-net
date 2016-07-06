using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Events.ScenarioEditor;
using Rogue.NET.ScenarioEditor.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        ICommand ScoreCommand { get; }
    }
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
                    _eventAggregator.GetEvent<ExitScenarioEditorEvent>().Publish(new ExitScenarioEditorEvent());
                });
            }
        }

        public ICommand LoadBuiltInCommand
        {
            get
            {
                return new DelegateCommand<string>((name) =>
                {
                    _eventAggregator.GetEvent<LoadBuiltInScenarioEvent>().Publish(new LoadBuiltInScenarioEvent() { ScenarioName = name });
                });
            }
        }

        public ICommand NewCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    _eventAggregator.GetEvent<NewScenarioConfigEvent>().Publish(new NewScenarioConfigEvent());
                });
            }
        }

        public ICommand ScoreCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    _eventAggregator.GetEvent<ScoreScenarioEvent>().Publish(new ScoreScenarioEvent());
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
                        Publish(new Rogue.NET.ScenarioEditor.Events.SaveScenarioEvent()
                        {
                            ScenarioName = name
                        });
                });
            }
        }

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
                this.ScenarioName = e.Payload.DungeonTemplate.Name;
            });
        }
    }
}
