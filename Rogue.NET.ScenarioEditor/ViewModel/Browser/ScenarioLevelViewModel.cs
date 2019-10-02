using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser
{
    public class ScenarioLevelViewModel : NotifyViewModel, IScenarioLevelViewModel
    {
        string _name;
        bool _hasObjectiveAssets;

        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
        public bool HasObjectiveAssets
        {
            get { return _hasObjectiveAssets; }
            set { this.RaiseAndSetIfChanged(ref _hasObjectiveAssets, value); }
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
            // Unhook listeners
            foreach (var branchViewModel in this.LevelBranches.Cast<INotifyPropertyChanged>())
                branchViewModel.PropertyChanged -= OnBranchPropertyChanged;

            this.LevelBranches.Clear();

            foreach (var branchTemplate in templateViewModel.LevelBranches)
            {
                // Create new branch view model
                var branchViewModel = new ScenarioLevelBranchViewModel(eventAggregator, templateViewModel, branchTemplate.LevelBranch);

                // Hook listener
                branchViewModel.PropertyChanged += OnBranchPropertyChanged;

                this.LevelBranches.Add(branchViewModel);
            }

            this.HasObjectiveAssets = this.LevelBranches.Any(x => x.HasObjectiveAssets);
        }

        private void OnBranchPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // TODO: Get rid of string descriptor (?)
            if (e.PropertyName == "HasObjectiveAssets")
                this.HasObjectiveAssets = this.LevelBranches.Any(x => x.HasObjectiveAssets);
        }
    }
}
