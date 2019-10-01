using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioLevelBrowserViewModel))]
    public class ScenarioLevelBrowserViewModel : NotifyViewModel, IScenarioLevelBrowserViewModel
    {
        ObservableCollection<IScenarioLevelViewModel> _levels;
        public ICommand AddLevelCommand { get; set; }
        public ICommand CollapseLevelTreeCommand { get; set; }
        public ObservableCollection<IScenarioLevelViewModel> Levels
        {
            get { return _levels; }
            set { this.RaiseAndSetIfChanged(ref _levels, value); }
        }

        [ImportingConstructor]
        public ScenarioLevelBrowserViewModel(IRogueEventAggregator eventAggregator)
        {
            this.Levels = new ObservableCollection<IScenarioLevelViewModel>();

            this.CollapseLevelTreeCommand = new SimpleCommand(() =>
            {
                eventAggregator.GetEvent<CollapseLevelTreeEvent>().Publish();
            });

            eventAggregator.GetEvent<ScenarioUpdateEvent>().Subscribe(scenarioCollectionProvider =>
            {
                // Initialize level collection
                Rebuild(eventAggregator, scenarioCollectionProvider.Levels);
            });

            this.AddLevelCommand = new SimpleCommand(() =>
            {
                eventAggregator.GetEvent<AddLevelEvent>()
                               .Publish();
            });
        }
        private void Rebuild(IRogueEventAggregator eventAggregator, IEnumerable<LevelTemplateViewModel> levels)
        {
            this.Levels.Clear();

            this.Levels.AddRange(levels.Select(level => new ScenarioLevelViewModel(eventAggregator, level)));
        }
    }
}
