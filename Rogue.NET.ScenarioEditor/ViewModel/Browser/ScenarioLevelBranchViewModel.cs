using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser
{
    public class ScenarioLevelBranchViewModel : NotifyViewModel, IScenarioLevelBranchViewModel
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
        public ObservableCollection<IScenarioAssetReadonlyViewModel> Layouts { get; set; }
        public ObservableCollection<IScenarioAssetReadonlyViewModel> Equipment { get; set; }
        public ObservableCollection<IScenarioAssetReadonlyViewModel> Consumables { get; set; }
        public ObservableCollection<IScenarioAssetReadonlyViewModel> Enemies { get; set; }
        public ObservableCollection<IScenarioAssetReadonlyViewModel> Friendlies { get; set; }
        public ObservableCollection<IScenarioAssetReadonlyViewModel> Doodads { get; set; }
        public ICommand RemoveLevelBranchCommand { get; set; }
        public ICommand LoadLevelBranchAssetsCommand { get; set; }
        public ICommand CopyLevelBranchCommand { get; set; }
        public ICommand RenameLevelBranchCommand { get; set; }
        public ScenarioLevelBranchViewModel(
                IRogueEventAggregator eventAggregator, 
                LevelTemplateViewModel levelTemplateViewModel,
                LevelBranchTemplateViewModel branchTemplateViewModel)
        {
            this.Name = branchTemplateViewModel.Name;

            this.Consumables = new ObservableCollection<IScenarioAssetReadonlyViewModel>();
            this.Doodads = new ObservableCollection<IScenarioAssetReadonlyViewModel>();
            this.Enemies = new ObservableCollection<IScenarioAssetReadonlyViewModel>();
            this.Equipment = new ObservableCollection<IScenarioAssetReadonlyViewModel>();
            this.Friendlies = new ObservableCollection<IScenarioAssetReadonlyViewModel>();
            this.Layouts = new ObservableCollection<IScenarioAssetReadonlyViewModel>();

            // Listen for changes to the collection
            branchTemplateViewModel.Consumables.CollectionChanged += (sender, e) => Rebuild(eventAggregator, branchTemplateViewModel);
            branchTemplateViewModel.Doodads.CollectionChanged += (sender, e) => Rebuild(eventAggregator, branchTemplateViewModel);
            branchTemplateViewModel.Enemies.CollectionChanged += (sender, e) => Rebuild(eventAggregator, branchTemplateViewModel);
            branchTemplateViewModel.Equipment.CollectionChanged += (sender, e) => Rebuild(eventAggregator, branchTemplateViewModel);
            branchTemplateViewModel.Friendlies.CollectionChanged += (sender, e) => Rebuild(eventAggregator, branchTemplateViewModel);
            branchTemplateViewModel.Layouts.CollectionChanged += (sender, e) => Rebuild(eventAggregator, branchTemplateViewModel);

            this.CopyLevelBranchCommand = new SimpleCommand(() =>
            {
                eventAggregator.GetEvent<CopyLevelBranchEvent>()
                               .Publish(new CopyLevelBranchEventData()
                               {
                                   LevelName = levelTemplateViewModel.Name,
                                   LevelBranchName = this.Name
                               });
            });
            this.LoadLevelBranchAssetsCommand = new SimpleCommand<Type>((assetType) =>
            {
                eventAggregator.GetEvent<LoadLevelBranchAssetsEvent>()
                               .Publish(new LoadLevelBranchAssetsEventData()
                               {
                                   AssetType = assetType,
                                   LevelBranchName = this.Name
                               });
            });
            this.RemoveLevelBranchCommand = new SimpleCommand(() =>
            {
                eventAggregator.GetEvent<RemoveLevelBranchEvent>()
                               .Publish(new RemoveLevelBranchEventData()
                               {
                                   LevelName = levelTemplateViewModel.Name,
                                   LevelBranchName = this.Name
                               });
            });
            this.RenameLevelBranchCommand = new SimpleCommand(() =>
            {
                eventAggregator.GetEvent<RenameLevelBranchEvent>()
                               .Publish(this);
            });

            Rebuild(eventAggregator, branchTemplateViewModel);
        }

        private void Rebuild(IRogueEventAggregator eventAggregator, LevelBranchTemplateViewModel templateViewModel)
        {
            this.Consumables.Clear();
            this.Doodads.Clear();
            this.Enemies.Clear();
            this.Equipment.Clear();
            this.Friendlies.Clear();
            this.Layouts.Clear();

            this.Consumables.AddRange(templateViewModel
                .Consumables
                .Select(consumable => new ScenarioAssetReadonlyViewModel(eventAggregator, consumable.Asset)));

            this.Doodads.AddRange(templateViewModel
                .Doodads
                .Select(doodad => new ScenarioAssetReadonlyViewModel(eventAggregator, doodad.Asset)));

            this.Enemies.AddRange(templateViewModel
                .Enemies
                .Select(enemy => new ScenarioAssetReadonlyViewModel(eventAggregator, enemy.Asset)));

            this.Equipment.AddRange(templateViewModel
                .Equipment
                .Select(equipment => new ScenarioAssetReadonlyViewModel(eventAggregator, equipment.Asset)));

            this.Friendlies.AddRange(templateViewModel
                .Friendlies
                .Select(friendly => new ScenarioAssetReadonlyViewModel(eventAggregator, friendly.Asset)));

            this.Layouts.AddRange(templateViewModel
                .Layouts
                .Select(layout => new ScenarioAssetReadonlyViewModel(eventAggregator, layout.Asset)));
        }
    }
}
