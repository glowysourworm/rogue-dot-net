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

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    [Export(typeof(IScenarioAssetGroupViewModel))]
    public class ScenarioAssetGroupViewModel : IScenarioAssetGroupViewModel
    {
        public ObservableCollection<IScenarioAssetViewModel> Assets { get; set; }
        public string AssetType { get; set; }

        public ICommand AddAssetCommand { get; private set; }

        protected readonly string[] NO_SYMBOL_TYPES = new string[] { "Layout", "Spell", "Animation", "Brush" };

        [ImportingConstructor]
        public ScenarioAssetGroupViewModel(IEventAggregator eventAggregator)
        {
            this.Assets = new ObservableCollection<IScenarioAssetViewModel>();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe((e) =>
            {
                this.Assets.Clear();
                switch (this.AssetType)
                {
                    case "Layout":
                        foreach (var v in e.DungeonTemplate.LayoutTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel() { Name = v.Name, Type = this.AssetType });
                        break;
                    case "AttackAttribute":
                        foreach (var v in e.AttackAttributes)
                            this.Assets.Add(new ScenarioAssetViewModel() { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Enemy":
                        foreach (var v in e.EnemyTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel() { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Equipment":
                        foreach (var v in e.EquipmentTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel() { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Consumable":
                        foreach (var v in e.ConsumableTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel() { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Doodad":
                        foreach (var v in e.DoodadTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel() { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Spell":
                        foreach (var v in e.MagicSpells)
                            this.Assets.Add(new ScenarioAssetViewModel() { Name = v.Name, Type = this.AssetType });
                        break;
                    case "SkillSet":
                        foreach (var v in e.SkillTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel() { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Animation":
                        foreach (var v in e.AnimationTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel() { Name = v.Name, Type = this.AssetType });
                        break;
                    case "Brush":
                        foreach (var v in e.BrushTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel() { Name = v.Name, Type = this.AssetType });
                        break;
                }

                // Add
                this.AddAssetCommand = new DelegateCommand(() =>
                {
                    var uniqueName = NameGenerator.Get(this.Assets.Select(z => z.Name), "New " + this.AssetType);

                    eventAggregator.GetEvent<AddAssetEvent>().Publish(new AddAssetEventArgs()
                    {
                        AssetType = this.AssetType,
                        AssetUniqueName = uniqueName
                    });

                    if (NO_SYMBOL_TYPES.Contains(this.AssetType))
                        this.Assets.Add(new ScenarioAssetViewModel() { Name = uniqueName, Type = this.AssetType });

                    else
                        this.Assets.Add(new ScenarioAssetViewModel() { Name = uniqueName, Type = this.AssetType, SymbolDetails = new SymbolDetailsTemplateViewModel() });
                });

                // Load, Remove
                foreach (var asset in this.Assets)
                {
                    asset.LoadAssetEvent += (sender, args) =>
                    {
                        eventAggregator.GetEvent<LoadAssetEvent>().Publish(asset);
                    };
                    asset.RemoveAssetEvent += (sender, args) =>
                    {
                        this.Assets.Remove(args);

                        eventAggregator.GetEvent<RemoveAssetEvent>().Publish(asset);
                    };
                }
            });
        }
    }
}
