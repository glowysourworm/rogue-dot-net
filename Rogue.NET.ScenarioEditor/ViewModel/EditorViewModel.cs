using Prism.Events;
using Rogue.NET.Common.Events.ScenarioEditor;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;

using System.ComponentModel.Composition;
using System.Windows.Input;

using Prism.Commands;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    [Export(typeof(IEditorViewModel))]
    public class EditorViewModel : NotifyViewModel, IEditorViewModel
    {
        readonly IEventAggregator _eventAggregator;
        readonly IRogueUndoService _rogueUndoService;

        string _scenarioName;

        public string ScenarioName
        {
            get { return _scenarioName; }
            set { this.RaiseAndSetIfChanged(ref _scenarioName, value); }
        }
        public ICommand ExitCommand { get; private set; }
        public ICommand LoadBuiltInCommand { get; private set; }
        public ICommand SaveBuiltInCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand ShowDifficultyCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public DelegateCommand UndoCommand { get; private set; }
        public DelegateCommand RedoCommand { get; private set; }

        [ImportingConstructor]
        public EditorViewModel(IEventAggregator eventAggregator, IRogueUndoService rogueUndoService)
        {
            _eventAggregator = eventAggregator;
            _rogueUndoService = rogueUndoService;

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

            // Commands           
            this.SaveCommand = new DelegateCommand<string>((name) =>
            {
                _eventAggregator.GetEvent<Rogue.NET.ScenarioEditor.Events.SaveScenarioEvent>().Publish();
            });
            this.ShowDifficultyCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<ScoreScenarioEvent>().Publish();
            });
            this.NewCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<NewScenarioConfigEvent>().Publish();
            });
            this.LoadBuiltInCommand = new DelegateCommand<string>((scenarioName) =>
            {
                _eventAggregator.GetEvent<LoadBuiltInScenarioEvent>().Publish(scenarioName);
            });
            this.SaveBuiltInCommand = new DelegateCommand<string>((scenarioName) =>
            {
                var configResource = (ConfigResources)Enum.Parse(typeof(ConfigResources), scenarioName);

                _eventAggregator.GetEvent<SaveBuiltInScenarioEvent>().Publish(configResource);
            });
            this.ExitCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<ExitScenarioEditorEvent>().Publish();
            });

            this.UndoCommand = new DelegateCommand(() => _rogueUndoService.Undo(), () => _rogueUndoService.CanUndo());
            this.RedoCommand = new DelegateCommand(() => _rogueUndoService.Redo(), () => _rogueUndoService.CanRedo());

            _rogueUndoService.ChangeEvent += (sender, e) =>
            {
                this.UndoCommand.RaiseCanExecuteChanged();
                this.RedoCommand.RaiseCanExecuteChanged();
            };
        }
    }
}
