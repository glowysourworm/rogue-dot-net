using Prism.Commands;
using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using System.Linq;
using System.Collections.Specialized;
using System.Collections;

using AssetTypeConst = Rogue.NET.ScenarioEditor.ViewModel.Constant.AssetType;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    [Export(typeof(IScenarioAssetGroupViewModel))]
    public class ScenarioAssetGroupViewModel : IScenarioAssetGroupViewModel
    {
        readonly IEventAggregator _eventAggregator;

        public ObservableCollection<IScenarioAssetViewModel> Assets { get; set; }
        public string AssetType { get; set; }
        public ICommand AddAssetCommand { get; private set; }

        bool _hasSymbol = false;

        [ImportingConstructor]
        public ScenarioAssetGroupViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            this.Assets = new ObservableCollection<IScenarioAssetViewModel>();

            // Scenario loaded
            _eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe((configuration) =>
            {
                _hasSymbol = AssetTypeConst.HasSymbol(this.AssetType);

                // Obtain INotifyCollectionChanged reference
                var notifyCollectionChanged = (INotifyCollectionChanged)ConfigurationCollectionResolver.GetAssetCollection(configuration, this.AssetType);

                // Clear out assets
                this.Assets.Clear();

                // Hook event to monitor collection
                notifyCollectionChanged.CollectionChanged += OnConfigurationCollectionChanged;

                // Run the handler once to refresh the Assets
                OnConfigurationCollectionChanged(notifyCollectionChanged, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });

            // Add
            this.AddAssetCommand = new DelegateCommand(() =>
            {
                var uniqueName = NameGenerator.Get(this.Assets.Select(z => z.Name), "New " + this.AssetType);

                _eventAggregator.GetEvent<AddAssetEvent>().Publish(new AddAssetEventArgs()
                {
                    AssetType = this.AssetType,
                    AssetUniqueName = uniqueName,
                });
            });
        }

        private void HookAssetEvents()
        {
            // Load, Remove
            foreach (var asset in this.Assets)
            {
                asset.LoadAssetEvent -= OnLoadAsset;
                asset.LoadAssetEvent += OnLoadAsset;
                asset.RemoveAssetEvent -= OnRemoveAsset;
                asset.RemoveAssetEvent += OnRemoveAsset;
                asset.RenameAssetEvent -= OnRenameAsset;
                asset.RenameAssetEvent += OnRenameAsset;
                asset.CopyAssetEvent -= OnCopyAsset;
                asset.CopyAssetEvent += OnCopyAsset;
            }
        }

        private void OnConfigurationCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var collection = sender as IList;            

            this.Assets.Clear();

            if (_hasSymbol)
            {
                foreach (var item in collection.Cast<DungeonObjectTemplateViewModel>())
                {
                    this.Assets.Add(new ScenarioAssetViewModel()
                    {
                        Name = item.Name,
                        Type = this.AssetType,
                        SubType = AssetTypeConst.GetSubType(item),
                        SymbolDetails = item.SymbolDetails,
                        IsLevelPlacement = AssetTypeConst.HasLevelPlacement(this.AssetType),
                        Level = item.Level
                    });
                }
            }
            else
            {
                foreach (var item in collection.Cast<TemplateViewModel>())
                    this.Assets.Add(new ScenarioAssetViewModel()
                    {
                        Name = item.Name,
                        Type = this.AssetType,
                        SubType = AssetTypeConst.GetSubType(item),
                        IsLevelPlacement = AssetTypeConst.HasLevelPlacement(this.AssetType),
                        Level = AssetTypeConst.HasLevelPlacement(this.AssetType) ? ((LayoutTemplateViewModel)item).Level : null
                    });
            }

            HookAssetEvents();
        }

        private void OnLoadAsset(object sender, IScenarioAssetViewModel asset)
        {
            _eventAggregator.GetEvent<LoadAssetEvent>().Publish(asset);
        }
        private void OnRemoveAsset(object sender, IScenarioAssetViewModel asset)
        {
            this.Assets.Remove(asset);

            _eventAggregator.GetEvent<RemoveAssetEvent>().Publish(asset);
        }
        private void OnRenameAsset(object sender, IScenarioAssetViewModel asset)
        {
            _eventAggregator.GetEvent<RenameAssetEvent>().Publish(asset);
        }
        private void OnCopyAsset(object sender, IScenarioAssetViewModel asset)
        {
            var uniqueName = NameGenerator.Get(this.Assets.Select(z => z.Name), asset.Name + " - Copy");

            _eventAggregator.GetEvent<CopyAssetEvent>().Publish(new CopyAssetEventArgs()
            {
                AssetName = asset.Name,
                AssetNewName = uniqueName,
                AssetType = asset.Type
            });
        }
    }
}
