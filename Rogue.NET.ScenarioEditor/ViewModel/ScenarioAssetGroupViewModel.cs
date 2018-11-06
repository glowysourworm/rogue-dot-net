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

        protected readonly string[] NO_SYMBOL_TYPES = new string[] { "Layout", "Spell", "Animation", "Brush" };

        [ImportingConstructor]
        public ScenarioAssetGroupViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            this.Assets = new ObservableCollection<IScenarioAssetViewModel>();

            _eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe((e) =>
            {
                _hasSymbol = !NO_SYMBOL_TYPES.Contains(this.AssetType);

                this.Assets.Clear();
                switch (this.AssetType)
                {
                    case "Layout":
                        e.DungeonTemplate.LayoutTemplates.CollectionChanged += OnConfigurationCollectionChanged;
                        OnConfigurationCollectionChanged(e.DungeonTemplate.LayoutTemplates, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        break;
                    case "AttackAttribute":
                        e.AttackAttributes.CollectionChanged += OnConfigurationCollectionChanged;
                        OnConfigurationCollectionChanged(e.AttackAttributes, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        break;
                    case "Enemy":
                        e.EnemyTemplates.CollectionChanged += OnConfigurationCollectionChanged;
                        OnConfigurationCollectionChanged(e.EnemyTemplates, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        break;
                    case "Equipment":
                        e.EquipmentTemplates.CollectionChanged += OnConfigurationCollectionChanged;
                        OnConfigurationCollectionChanged(e.EquipmentTemplates, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        break;
                    case "Consumable":
                        e.ConsumableTemplates.CollectionChanged += OnConfigurationCollectionChanged;
                        OnConfigurationCollectionChanged(e.ConsumableTemplates, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        break;
                    case "Doodad":
                        e.DoodadTemplates.CollectionChanged += OnConfigurationCollectionChanged;
                        OnConfigurationCollectionChanged(e.DoodadTemplates, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        break;
                    case "Spell":
                        e.MagicSpells.CollectionChanged += OnConfigurationCollectionChanged;
                        OnConfigurationCollectionChanged(e.MagicSpells, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        break;
                    case "SkillSet":
                        e.SkillTemplates.CollectionChanged += OnConfigurationCollectionChanged;
                        OnConfigurationCollectionChanged(e.SkillTemplates, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        break;
                    case "Animation":
                        e.AnimationTemplates.CollectionChanged += OnConfigurationCollectionChanged;
                        OnConfigurationCollectionChanged(e.AnimationTemplates, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        break;
                    case "Brush":
                        e.BrushTemplates.CollectionChanged += OnConfigurationCollectionChanged;
                        OnConfigurationCollectionChanged(e.BrushTemplates, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        break;
                }
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
                    this.Assets.Add(new ScenarioAssetViewModel() { Name = item.Name, Type = this.AssetType, SymbolDetails = item.SymbolDetails });
            }
            else
            {
                foreach (var item in collection.Cast<TemplateViewModel>())
                    this.Assets.Add(new ScenarioAssetViewModel() { Name = item.Name, Type = this.AssetType });
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
    }
}
