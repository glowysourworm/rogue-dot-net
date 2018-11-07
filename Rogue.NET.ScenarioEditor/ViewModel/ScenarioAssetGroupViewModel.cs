using Prism.Commands;
using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using System.Linq;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
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
                OnAddAsset();
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
            }
        }

        private void OnAddAsset()
        {
            var uniqueName = NameGenerator.Get(this.Assets.Select(z => z.Name), "New " + this.AssetType);
            var symbolDetails = _hasSymbol ? new SymbolDetailsTemplateViewModel() : null;

            _eventAggregator.GetEvent<AddAssetEvent>().Publish(new AddAssetEventArgs()
            {
                AssetType = this.AssetType,
                AssetUniqueName = uniqueName,
                SymbolDetails = symbolDetails
            });

            this.Assets.Add(new ScenarioAssetViewModel() { Name = uniqueName, Type = this.AssetType, SymbolDetails = symbolDetails });

            HookAssetEvents();
        }

        private void OnConfigurationCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // These events are initiated from this class - so ignore these because they're already handled
            if (e.Action == NotifyCollectionChangedAction.Add)
                return;

            var collection = sender as IList;            

            this.Assets.Clear();

            if (_hasSymbol)
            {
                foreach (var item in collection.Cast<DungeonObjectTemplateViewModel>())                                                  
                    this.Assets.Add(new ScenarioAssetViewModel() { Name = item.Name, Type = this.AssetType, SubType = AssetTypeConst.GetSubType(item),  SymbolDetails = item.SymbolDetails });
            }
            else
            {
                foreach (var item in collection.Cast<TemplateViewModel>())
                    this.Assets.Add(new ScenarioAssetViewModel() { Name = item.Name, Type = this.AssetType, SubType = AssetTypeConst.GetSubType(item) });
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
    }
}
