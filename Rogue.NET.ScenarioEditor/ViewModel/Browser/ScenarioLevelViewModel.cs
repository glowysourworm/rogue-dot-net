using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser
{
    public class ScenarioLevelViewModel : NotifyViewModel, IScenarioLevelViewModel
    {
        string _name;

        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        public ICommand RemoveLevelCommand { get; set; }
        public ICommand LoadLevelCommand { get; set; }
        public ObservableCollection<IScenarioLevelBranchViewModel> LevelBranches { get; set; }
        public ScenarioLevelViewModel(IRogueEventAggregator eventAggregator, LevelTemplateViewModel templateViewModel)
        {
            this.Name = templateViewModel.Name;
            this.LevelBranches = new ObservableCollection<IScenarioLevelBranchViewModel>();

            // Listen for branch change events
            templateViewModel.LevelBranches.CollectionChanged += (sender, e) => Rebuild(eventAggregator, templateViewModel);

            this.LoadLevelCommand = new SimpleCommand(() =>
            {
                eventAggregator.GetEvent<LoadLevelEvent>()
                               .Publish(this);
            });
            this.RemoveLevelCommand = new SimpleCommand(() =>
            {
                eventAggregator.GetEvent<RemoveLevelEvent>()
                               .Publish(this);
            });

            Rebuild(eventAggregator, templateViewModel);
        }

        private void Rebuild(IRogueEventAggregator eventAggregator, LevelTemplateViewModel templateViewModel)
        {
            this.LevelBranches.Clear();

            this.LevelBranches.AddRange(
                templateViewModel
                    .LevelBranches
                    .Select(levelBranch => new ScenarioLevelBranchViewModel(eventAggregator, templateViewModel, levelBranch.LevelBranch)));
        }
    }
}
