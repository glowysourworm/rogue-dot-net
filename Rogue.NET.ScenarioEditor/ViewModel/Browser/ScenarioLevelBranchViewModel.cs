using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser
{
    public class ScenarioLevelBranchViewModel : NotifyViewModel, IScenarioLevelBranchViewModel
    {
        string _name;
        bool _hasObjectiveAssets;
        bool _hasEquipmentObjectiveAssets;
        bool _hasConsumableObjectiveAssets;
        bool _hasEnemyObjectiveAssets;
        bool _hasFriendlyObjectiveAssets;
        bool _hasDoodadObjectiveAssets;
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
        public bool HasEquipmentObjectiveAssets
        {
            get { return _hasEquipmentObjectiveAssets; }
            set { this.RaiseAndSetIfChanged(ref _hasEquipmentObjectiveAssets, value); }
        }
        public bool HasConsumableObjectiveAssets
        {
            get { return _hasConsumableObjectiveAssets; }
            set { this.RaiseAndSetIfChanged(ref _hasConsumableObjectiveAssets, value); }
        }
        public bool HasEnemyObjectiveAssets
        {
            get { return _hasEnemyObjectiveAssets; }
            set { this.RaiseAndSetIfChanged(ref _hasEnemyObjectiveAssets, value); }
        }
        public bool HasFriendlyObjectiveAssets
        {
            get { return _hasFriendlyObjectiveAssets; }
            set { this.RaiseAndSetIfChanged(ref _hasFriendlyObjectiveAssets, value); }
        }
        public bool HasDoodadObjectiveAssets
        {
            get { return _hasDoodadObjectiveAssets; }
            set { this.RaiseAndSetIfChanged(ref _hasDoodadObjectiveAssets, value); }
        }
        public NotifyingObservableCollection<IScenarioAssetReadonlyViewModel> Layouts { get; set; }
        public NotifyingObservableCollection<IScenarioAssetReadonlyViewModel> Equipment { get; set; }
        public NotifyingObservableCollection<IScenarioAssetReadonlyViewModel> Consumables { get; set; }
        public NotifyingObservableCollection<IScenarioAssetReadonlyViewModel> Enemies { get; set; }
        public NotifyingObservableCollection<IScenarioAssetReadonlyViewModel> Friendlies { get; set; }
        public NotifyingObservableCollection<IScenarioAssetReadonlyViewModel> Doodads { get; set; }
        public ICommand RemoveLevelBranchCommand { get; set; }
        public ICommand LoadLevelBranchAssetsCommand { get; set; }
        public ICommand CopyLevelBranchCommand { get; set; }
        public ICommand RenameLevelBranchCommand { get; set; }
        public ICommand PreviewLevelBranchCommand { get; set; }

        public ScenarioLevelBranchViewModel(
                IRogueEventAggregator eventAggregator, 
                LevelTemplateViewModel levelTemplateViewModel,
                LevelBranchTemplateViewModel branchTemplateViewModel)
        {
            this.Name = branchTemplateViewModel.Name;

            this.Consumables = new NotifyingObservableCollection<IScenarioAssetReadonlyViewModel>();
            this.Doodads = new NotifyingObservableCollection<IScenarioAssetReadonlyViewModel>();
            this.Enemies = new NotifyingObservableCollection<IScenarioAssetReadonlyViewModel>();
            this.Equipment = new NotifyingObservableCollection<IScenarioAssetReadonlyViewModel>();
            this.Friendlies = new NotifyingObservableCollection<IScenarioAssetReadonlyViewModel>();
            this.Layouts = new NotifyingObservableCollection<IScenarioAssetReadonlyViewModel>();

            // Listen for objective flag change
            this.Consumables.ItemPropertyChanged += OnAssetPropertyChanged;
            this.Doodads.ItemPropertyChanged += OnAssetPropertyChanged;
            this.Enemies.ItemPropertyChanged += OnAssetPropertyChanged;
            this.Equipment.ItemPropertyChanged += OnAssetPropertyChanged;
            this.Friendlies.ItemPropertyChanged += OnAssetPropertyChanged;

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
            this.PreviewLevelBranchCommand = new SimpleCommand(() =>
            {
                eventAggregator.GetEvent<PreviewLevelBranchEvent>()
                               .Publish(branchTemplateViewModel);
            });

            Rebuild(eventAggregator, branchTemplateViewModel);
        }

        private void OnAssetPropertyChanged(
                        NotifyingObservableCollection<IScenarioAssetReadonlyViewModel> collection, 
                        IScenarioAssetReadonlyViewModel asset, 
                        PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsObjective")
            {
                CalculateHasObjectiveAssets();
            }
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

            CalculateHasObjectiveAssets();
        }

        private void CalculateHasObjectiveAssets()
        {
            this.HasConsumableObjectiveAssets = this.Consumables.Any(x => x.IsObjective);
            this.HasDoodadObjectiveAssets = this.Doodads.Any(x => x.IsObjective);
            this.HasEnemyObjectiveAssets = this.Enemies.Any(x => x.IsObjective);
            this.HasEquipmentObjectiveAssets = this.Equipment.Any(x => x.IsObjective);
            this.HasFriendlyObjectiveAssets = this.Friendlies.Any(x => x.IsObjective);

            this.HasObjectiveAssets = this.HasConsumableObjectiveAssets ||
                                      this.HasDoodadObjectiveAssets ||
                                      this.HasEnemyObjectiveAssets ||
                                      this.HasEquipmentObjectiveAssets ||
                                      this.HasFriendlyObjectiveAssets;
        }
    }
}
