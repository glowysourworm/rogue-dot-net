using Prism.Events;
using Rogue.NET.ScenarioEditor.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    [Export(typeof(IScenarioAssetBrowserViewModel))]
    public class ScenarioAssetBrowserViewModel : IScenarioAssetBrowserViewModel
    {
        readonly IEventAggregator _eventAggregator;
        readonly IScenarioEditorController _controller;

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
