using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Rogue.NET.Common.Events.ScenarioEditor;
using Rogue.NET.ScenarioEditor.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    public interface IScenarioAssetGroupViewModel
    {
        ICommand AddAssetCommand { get; }

        void RemoveAsset(IScenarioAssetViewModel asset);
    }
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
                        foreach (var v in e.Payload.DungeonTemplate.LayoutTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType });
                        break;
                    case "CreatureClass":
                        foreach (var v in e.Payload.CharacterClasses)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType });
                        break;
                    case "AttackAttribute":
                        foreach (var v in e.Payload.AttackAttributes)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Enemy":
                        foreach (var v in e.Payload.EnemyTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Equipment":
                        foreach (var v in e.Payload.EquipmentTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Consumable":
                        foreach (var v in e.Payload.ConsumableTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Doodad":
                        foreach (var v in e.Payload.DoodadTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Spell":
                        foreach (var v in e.Payload.MagicSpells)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType });
                        break;
                    case "SkillSet":
                        foreach (var v in e.Payload.SkillTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType, SymbolDetails = v.SymbolDetails });
                        break;
                    case "Animation":
                        foreach (var v in e.Payload.AnimationTemplates)
                            this.Assets.Add(new ScenarioAssetViewModel(this, _controller) { Name = v.Name, Type = this.AssetType });
                        break;
                    case "Brush":
                        foreach (var v in e.Payload.BrushTemplates)
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
