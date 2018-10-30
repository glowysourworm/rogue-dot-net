using Prism.Events;
using System.ComponentModel.Composition;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    public interface IScenarioAssetBrowserViewModel
    {
        ScenarioAssetGroupViewModel LayoutGroup { get; set; }
        ScenarioAssetGroupViewModel CreatureClassGroup { get; set; }
        ScenarioAssetGroupViewModel AttackAttributeGroup { get; set; }
        ScenarioAssetGroupViewModel EnemyGroup { get; set; }
        ScenarioAssetGroupViewModel EquipmentGroup { get; set; }
        ScenarioAssetGroupViewModel ConsumableGroup { get; set; }
        ScenarioAssetGroupViewModel DoodadGroup { get; set; }
        ScenarioAssetGroupViewModel SpellGroup { get; set; }
        ScenarioAssetGroupViewModel SkillSetGroup { get; set; }
        ScenarioAssetGroupViewModel AnimationGroup { get; set; }
        ScenarioAssetGroupViewModel BrushGroup { get; set; }
        ScenarioAssetGroupViewModel PenGroup { get; set; }
    }
    [Export(typeof(IScenarioAssetBrowserViewModel))]
    public class ScenarioAssetBrowserViewModel : IScenarioAssetBrowserViewModel
    {
        readonly IEventAggregator _eventAggregator;
        readonly IScenarioEditorController _controller;

        public ScenarioAssetGroupViewModel LayoutGroup { get; set; }
        public ScenarioAssetGroupViewModel CreatureClassGroup { get; set; }
        public ScenarioAssetGroupViewModel AttackAttributeGroup { get; set; }
        public ScenarioAssetGroupViewModel EnemyGroup { get; set; }
        public ScenarioAssetGroupViewModel EquipmentGroup { get; set; }
        public ScenarioAssetGroupViewModel ConsumableGroup { get; set; }
        public ScenarioAssetGroupViewModel DoodadGroup { get; set; }
        public ScenarioAssetGroupViewModel SpellGroup { get; set; }
        public ScenarioAssetGroupViewModel SkillSetGroup { get; set; }
        public ScenarioAssetGroupViewModel AnimationGroup { get; set; }
        public ScenarioAssetGroupViewModel BrushGroup { get; set; }
        public ScenarioAssetGroupViewModel PenGroup { get; set; }

        [ImportingConstructor]
        public ScenarioAssetBrowserViewModel(
            IEventAggregator eventAggregator,
            IScenarioEditorController controller)
        {
            _eventAggregator = eventAggregator;
            _controller = controller;

            Initialize();
        }

        private void Initialize()
        {
            this.LayoutGroup = new ScenarioAssetGroupViewModel(_eventAggregator, _controller) { AssetType = "Layout" };
            this.CreatureClassGroup = new ScenarioAssetGroupViewModel(_eventAggregator, _controller) { AssetType = "CreatureClass" };
            this.AttackAttributeGroup = new ScenarioAssetGroupViewModel(_eventAggregator, _controller) { AssetType = "AttackAttribute" };
            this.EnemyGroup = new ScenarioAssetGroupViewModel(_eventAggregator, _controller) { AssetType = "Enemy" };
            this.EquipmentGroup = new ScenarioAssetGroupViewModel(_eventAggregator, _controller) { AssetType = "Equipment" };
            this.ConsumableGroup = new ScenarioAssetGroupViewModel(_eventAggregator, _controller) { AssetType = "Consumable" };
            this.DoodadGroup = new ScenarioAssetGroupViewModel(_eventAggregator, _controller) { AssetType = "Doodad" };
            this.SpellGroup = new ScenarioAssetGroupViewModel(_eventAggregator, _controller) { AssetType = "Spell" };
            this.SkillSetGroup = new ScenarioAssetGroupViewModel(_eventAggregator, _controller) { AssetType = "SkillSet" };
            this.AnimationGroup = new ScenarioAssetGroupViewModel(_eventAggregator, _controller) { AssetType = "Animation" };
            this.BrushGroup = new ScenarioAssetGroupViewModel(_eventAggregator, _controller) { AssetType = "Brush" };
            this.PenGroup = new ScenarioAssetGroupViewModel(_eventAggregator, _controller) { AssetType = "Pen" };
        }
    }
}
