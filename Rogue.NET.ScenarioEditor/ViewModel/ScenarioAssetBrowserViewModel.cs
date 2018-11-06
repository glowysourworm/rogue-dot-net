using Prism.Events;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    [Export(typeof(IScenarioAssetBrowserViewModel))]
    public class ScenarioAssetBrowserViewModel : IScenarioAssetBrowserViewModel
    {
        public IScenarioAssetGroupViewModel LayoutGroup { get; set; }
        public IScenarioAssetGroupViewModel AttackAttributeGroup { get; set; }
        public IScenarioAssetGroupViewModel EnemyGroup { get; set; }
        public IScenarioAssetGroupViewModel EquipmentGroup { get; set; }
        public IScenarioAssetGroupViewModel ConsumableGroup { get; set; }
        public IScenarioAssetGroupViewModel DoodadGroup { get; set; }
        public IScenarioAssetGroupViewModel SpellGroup { get; set; }
        public IScenarioAssetGroupViewModel SkillSetGroup { get; set; }
        public IScenarioAssetGroupViewModel AnimationGroup { get; set; }
        public IScenarioAssetGroupViewModel BrushGroup { get; set; }
        public IScenarioAssetGroupViewModel PenGroup { get; set; }

        [ImportingConstructor]
        public ScenarioAssetBrowserViewModel(
            IEventAggregator eventAggregator)
        {
            this.LayoutGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = "Layout" };
            this.AttackAttributeGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = "AttackAttribute" };
            this.EnemyGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = "Enemy" };
            this.EquipmentGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = "Equipment" };
            this.ConsumableGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = "Consumable" };
            this.DoodadGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = "Doodad" };
            this.SpellGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = "Spell" };
            this.SkillSetGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = "SkillSet" };
            this.AnimationGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = "Animation" };
            this.BrushGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = "Brush" };
            this.PenGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = "Pen" };
        }
    }
}
