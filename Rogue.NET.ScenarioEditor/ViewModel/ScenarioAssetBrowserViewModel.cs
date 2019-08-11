using Prism.Commands;
using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.Constant;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    [Export(typeof(IScenarioAssetBrowserViewModel))]
    public class ScenarioAssetBrowserViewModel : IScenarioAssetBrowserViewModel
    {
        public IScenarioAssetGroupViewModel LayoutGroup { get; set; }
        public IScenarioAssetGroupViewModel EnemyGroup { get; set; }
        public IScenarioAssetGroupViewModel EquipmentGroup { get; set; }
        public IScenarioAssetGroupViewModel ConsumableGroup { get; set; }
        public IScenarioAssetGroupViewModel DoodadGroup { get; set; }
        public IScenarioAssetGroupViewModel SpellGroup { get; set; }
        public IScenarioAssetGroupViewModel SkillSetGroup { get; set; }
        public IScenarioAssetGroupViewModel AnimationGroup { get; set; }
        public IScenarioAssetGroupViewModel BrushGroup { get; set; }

        public ICommand CollapseAssetTreeCommand { get; set; }

        [ImportingConstructor]
        public ScenarioAssetBrowserViewModel(
            IEventAggregator eventAggregator)
        {
            this.CollapseAssetTreeCommand = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<CollapseAssetTreeEvent>().Publish();
            });

            this.LayoutGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = AssetType.Layout };
            this.EnemyGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = AssetType.Enemy };
            this.EquipmentGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = AssetType.Equipment };
            this.ConsumableGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = AssetType.Consumable };
            this.DoodadGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = AssetType.Doodad };
            // TODO:ALTERATION
            //this.SpellGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = AssetType.Spell };
            this.SkillSetGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = AssetType.SkillSet };
            this.AnimationGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = AssetType.Animation };
            this.BrushGroup = new ScenarioAssetGroupViewModel(eventAggregator) { AssetType = AssetType.Brush };
        }
    }
}
