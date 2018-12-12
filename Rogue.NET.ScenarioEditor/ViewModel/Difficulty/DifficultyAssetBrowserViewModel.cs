using System.Collections.Generic;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.Constant;
using Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System.Linq;

namespace Rogue.NET.ScenarioEditor.ViewModel.Difficulty
{
    public class DifficultyAssetBrowserViewModel : NotifyViewModel, IDifficultyAssetBrowserViewModel
    {
        public IDifficultyAssetGroupViewModel LayoutGroup { get; set; }
        public IDifficultyAssetGroupViewModel EnemyGroup { get; set; }
        public IDifficultyAssetGroupViewModel EquipmentGroup { get; set; }
        public IDifficultyAssetGroupViewModel ConsumableGroup { get; set; }

        public IEnumerable<IDifficultyAssetViewModel> Assets
        {
            get
            {
                return  this.LayoutGroup
                            .Assets
                            .Union(this.EnemyGroup.Assets)
                            .Union(this.EquipmentGroup.Assets)
                            .Union(this.ConsumableGroup.Assets);
            }
        }

        public DifficultyAssetBrowserViewModel(ScenarioConfigurationContainerViewModel scenarioConfiguration)
        {
            this.LayoutGroup = new DifficultyAssetGroupViewModel(AssetType.Layout, scenarioConfiguration.DungeonTemplate.LayoutTemplates);
            this.EnemyGroup = new DifficultyAssetGroupViewModel(AssetType.Enemy, scenarioConfiguration.EnemyTemplates);
            this.EquipmentGroup = new DifficultyAssetGroupViewModel(AssetType.Equipment, scenarioConfiguration.EquipmentTemplates);
            this.ConsumableGroup = new DifficultyAssetGroupViewModel(AssetType.Consumable, scenarioConfiguration.ConsumableTemplates);
        }
    }
}
