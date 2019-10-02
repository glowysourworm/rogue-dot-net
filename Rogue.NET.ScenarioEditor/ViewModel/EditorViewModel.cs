using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;

using System.ComponentModel.Composition;
using System.Windows.Input;

using Prism.Commands;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using System;
using Microsoft.Win32;
using System.IO;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Event.ScenarioEditor;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    [Export(typeof(IEditorViewModel))]
    public class EditorViewModel : NotifyViewModel, IEditorViewModel
    {
        readonly IRogueEventAggregator _eventAggregator;
        readonly IScenarioConfigurationUndoService _rogueUndoService;

        ScenarioConfigurationContainerViewModel _configuration;
        string _scenarioName;


        // Required configuration reference for binding global collections
        public ScenarioConfigurationContainerViewModel Configuration
        {
            get { return _configuration; }
            private set { this.RaiseAndSetIfChanged(ref _configuration, value); }
        }
        public string ScenarioName
        {
            get { return _scenarioName; }
            set { this.RaiseAndSetIfChanged(ref _scenarioName, value); }
        }
        public ICommand ExitCommand { get; private set; }
        public ICommand LoadBuiltInCommand { get; private set; }
        public ICommand SaveBuiltInCommand { get; private set; }
        public ICommand RunBuiltInSaveCycleCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }
        public DelegateCommand UndoCommand { get; private set; }
        public DelegateCommand RedoCommand { get; private set; }

        [ImportingConstructor]
        public EditorViewModel(IRogueEventAggregator eventAggregator, IScenarioConfigurationUndoService rogueUndoService)
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
                this.Configuration = e.Configuration;
                this.ScenarioName = e.Configuration.ScenarioDesign.Name;
            });

            // Commands           
            this.SaveCommand = new DelegateCommand<string>((name) =>
            {
                _eventAggregator.GetEvent<Rogue.NET.ScenarioEditor.Events.SaveScenarioEvent>().Publish();
            });
            this.OpenCommand = new DelegateCommand(() =>
            {
                var dialog = new OpenFileDialog();
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == true)
                {
                    // Scenario name
                    var scenarioName = Path.GetFileNameWithoutExtension(dialog.FileName);

                    // Load the Scenario (event)
                    _eventAggregator.GetEvent<LoadScenarioEvent>().Publish(scenarioName);
                }
            });
            this.NewCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<NewScenarioConfigEvent>().Publish();
            });
            this.LoadBuiltInCommand = new DelegateCommand<string>((scenarioName) =>
            {
                var configResource = (ConfigResources)Enum.Parse(typeof(ConfigResources), scenarioName);

                _eventAggregator.GetEvent<LoadBuiltInScenarioEvent>().Publish(configResource);
            });
            this.SaveBuiltInCommand = new DelegateCommand<string>((scenarioName) =>
            {
                var configResource = (ConfigResources)Enum.Parse(typeof(ConfigResources), scenarioName);

                _eventAggregator.GetEvent<SaveBuiltInScenarioEvent>().Publish(configResource);
            });
            this.RunBuiltInSaveCycleCommand = new DelegateCommand(() =>
            {
                // Runs an open / save for all built in scenarios
                foreach (var configResource in Enum.GetValues(typeof(ConfigResources)))
                {
                    _eventAggregator.GetEvent<LoadBuiltInScenarioEvent>().Publish((ConfigResources)configResource);
                    _eventAggregator.GetEvent<SaveBuiltInScenarioEvent>().Publish((ConfigResources)configResource);
                }
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
