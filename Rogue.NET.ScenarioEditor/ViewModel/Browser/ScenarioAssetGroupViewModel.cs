using Prism.Commands;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.Common.ViewModel;

using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.Specialized;
using System.Collections;
using System;
using Rogue.NET.Common.Extension;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using System.ComponentModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser
{
    public class ScenarioAssetGroupViewModel : NotifyViewModel, IScenarioAssetGroupViewModel
    {
        NotifyingObservableCollection<IScenarioAssetViewModel> _assets;
        bool _hasObjectiveAssets;

        public NotifyingObservableCollection<IScenarioAssetViewModel> Assets
        {
            get { return _assets; }
            set { this.RaiseAndSetIfChanged(ref _assets, value); }
        }
        public Type AssetType { get; private set; }
        public bool HasObjectiveAssets
        {
            get { return _hasObjectiveAssets; }
            set { this.RaiseAndSetIfChanged(ref _hasObjectiveAssets, value); }
        }
        public ICommand AddAssetCommand { get; private set; }
        public ScenarioAssetGroupViewModel(
                Type assetType,
                IRogueEventAggregator eventAggregator)
        {
            this.Assets = new NotifyingObservableCollection<IScenarioAssetViewModel>();
            this.AssetType = assetType;

            // Listen for objective change
            this.Assets.ItemPropertyChanged += OnAssetCollectionItemChanged;

            // Listen to scenario updates
            eventAggregator.GetEvent<ScenarioUpdateEvent>().Subscribe(scenarioCollectionProvider =>
            {
                // Obtain INotifyCollectionChanged reference
                var collection = GetAssetCollection(scenarioCollectionProvider);

                // Run the handler once to refresh the Assets
                Rebuild(eventAggregator, collection);
            });

            // Add
            this.AddAssetCommand = new DelegateCommand(() =>
            {
                var uniqueName = NameGenerator.Get(this.Assets.Select(z => z.Name), "New " + this.AssetType.GetAttribute<UITypeAttribute>().DisplayName);

                eventAggregator.GetEvent<AddAssetEvent>().Publish(new AddAssetEventArgs()
                {
                    AssetType = this.AssetType,
                    AssetUniqueName = uniqueName,
                });
            });
        }
        private void Rebuild(IRogueEventAggregator eventAggregator, IEnumerable collection)
        {
            this.Assets.Clear();

            foreach (var item in collection)
            {
                if (item is DungeonObjectTemplateViewModel)
                    this.Assets.Add(new ScenarioAssetViewModel(eventAggregator, item as DungeonObjectTemplateViewModel));

                else
                    this.Assets.Add(new ScenarioAssetViewModel(eventAggregator, item as TemplateViewModel));
            }



            CalculateHasObjectiveAssets();
        }

        private void OnAssetCollectionItemChanged(
                        NotifyingObservableCollection<IScenarioAssetViewModel> collection, 
                        IScenarioAssetViewModel asset, 
                        PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsObjective")
                CalculateHasObjectiveAssets();
        }

        private void CalculateHasObjectiveAssets()
        {
            this.HasObjectiveAssets = this.Assets.Any(x => x.IsObjective);
        }

        private IEnumerable GetAssetCollection(IScenarioCollectionProvider collectionProvider)
        {
            if (this.AssetType == typeof(PlayerTemplateViewModel))
                return collectionProvider.PlayerClasses;

            else if (this.AssetType == typeof(LayoutTemplateViewModel))
                return collectionProvider.Layouts;

            else if (this.AssetType == typeof(EnemyTemplateViewModel))
                return collectionProvider.Enemies;

            else if (this.AssetType == typeof(FriendlyTemplateViewModel))
                return collectionProvider.Friendlies;

            else if (this.AssetType == typeof(EquipmentTemplateViewModel))
                return collectionProvider.Equipment;

            else if (this.AssetType == typeof(ConsumableTemplateViewModel))
                return collectionProvider.Consumables;

            else if (this.AssetType == typeof(DoodadTemplateViewModel))
                return collectionProvider.Doodads;

            else if (this.AssetType == typeof(SkillSetTemplateViewModel))
                return collectionProvider.SkillSets;

            else
                throw new Exception("Unhandled Asset Type ScenarioAssetGroupViewModel");
        }
    }
}
