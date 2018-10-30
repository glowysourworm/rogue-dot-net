using Prism.Commands;
using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    public interface IScenarioAssetGroupViewModel
    {
        ICommand AddAssetCommand { get; }

        void RemoveAsset(IScenarioAssetViewModel asset);
    }
    [Export(typeof(IScenarioAssetGroupViewModel))]
    public class ScenarioAssetGroupViewModel : IScenarioAssetGroupViewModel
    {
        readonly IScenarioEditorController _controller;
        readonly IEventAggregator _eventAggregator;

        public ObservableCollection<IScenarioAssetViewModel> Assets { get; set; }
        public string AssetType { get; set; }

        public ICommand AddAssetCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var assetName = _controller.AddAsset(this.AssetType);
                    this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { 
                        Name = assetName, 
                        Type = this.AssetType
                    });
                });
            }
        }

        [ImportingConstructor]
        public ScenarioAssetGroupViewModel(IEventAggregator eventAggregator, IScenarioEditorController controller)
        {
            _eventAggregator = eventAggregator;
            _controller = controller;

            this.Assets = new ObservableCollection<IScenarioAssetViewModel>();

            _eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe((e) =>
            {
                this.Assets.Clear();
                switch (this.AssetType)
                {
                    case "Layout":
                        foreach (var v in e.DungeonTemplate.LayoutTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType });
                        break;
                    case "CreatureClass":
                        foreach (var v in e.CharacterClasses)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType });
                        break;
                    case "AttackAttribute":
                        foreach (var v in e.AttackAttributes)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Enemy":
                        foreach (var v in e.EnemyTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Equipment":
                        foreach (var v in e.EquipmentTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Consumable":
                        foreach (var v in e.ConsumableTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Doodad":
                        foreach (var v in e.DoodadTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Spell":
                        foreach (var v in e.MagicSpells)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType });
                        break;
                    case "SkillSet":
                        foreach (var v in e.SkillTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Animation":
                        foreach (var v in e.AnimationTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType });
                        break;
                    case "Brush":
                        foreach (var v in e.BrushTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType });
                        break;
                }
            });
        }


        public void RemoveAsset(IScenarioAssetViewModel asset)
        {
            _controller.RemoveAsset(this.AssetType, asset.Name);
            this.Assets.Remove(asset);
        }
    }
}
