using Prism.Commands;
using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    [Export(typeof(IScenarioAssetGroupViewModel))]
    public class ScenarioAssetGroupViewModel : IScenarioAssetGroupViewModel
    {
        public ObservableCollection<IScenarioAssetViewModel> Assets { get; set; }
        public string AssetType { get; set; }

        public ICommand AddAssetCommand { get; private set; }

        [ImportingConstructor]
        public ScenarioAssetGroupViewModel(IEventAggregator eventAggregator)
        {
            this.Assets = new ObservableCollection<IScenarioAssetViewModel>();

            this.AddAssetCommand = new DelegateCommand(() =>
            {
                // TODO: Bubble command up
            });

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe((e) =>
            {
                this.Assets.Clear();
                switch (this.AssetType)
                {
                    case "Layout":
                        foreach (var v in e.DungeonTemplate.LayoutTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(eventAggregator) { Name = v.Name, Type = this.AssetType });
                        break;
                    case "AttackAttribute":
                        foreach (var v in e.AttackAttributes)
                            this.Assets.Add(new ScenarioAssetViewModel(eventAggregator) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Enemy":
                        foreach (var v in e.EnemyTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(eventAggregator) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Equipment":
                        foreach (var v in e.EquipmentTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(eventAggregator) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Consumable":
                        foreach (var v in e.ConsumableTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(eventAggregator) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Doodad":
                        foreach (var v in e.DoodadTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(eventAggregator) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Spell":
                        foreach (var v in e.MagicSpells)
                            this.Assets.Add(new ScenarioAssetViewModel(eventAggregator) { Name = v.Name, Type = this.AssetType });
                        break;
                    case "SkillSet":
                        foreach (var v in e.SkillTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(eventAggregator) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Animation":
                        foreach (var v in e.AnimationTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(eventAggregator) { Name = v.Name, Type = this.AssetType });
                        break;
                    case "Brush":
                        foreach (var v in e.BrushTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(eventAggregator) { Name = v.Name, Type = this.AssetType });
                        break;
                }
            });
        }
    }
}
