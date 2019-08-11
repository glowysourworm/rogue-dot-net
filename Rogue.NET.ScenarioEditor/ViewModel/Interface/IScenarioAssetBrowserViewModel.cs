using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Interface
{
    public interface IScenarioAssetBrowserViewModel
    {
        IScenarioAssetGroupViewModel LayoutGroup { get; set; }
        IScenarioAssetGroupViewModel EnemyGroup { get; set; }
        IScenarioAssetGroupViewModel EquipmentGroup { get; set; }
        IScenarioAssetGroupViewModel ConsumableGroup { get; set; }
        IScenarioAssetGroupViewModel DoodadGroup { get; set; }
        IScenarioAssetGroupViewModel SpellGroup { get; set; }
        IScenarioAssetGroupViewModel SkillSetGroup { get; set; }
        IScenarioAssetGroupViewModel AnimationGroup { get; set; }
        IScenarioAssetGroupViewModel BrushGroup { get; set; }

        ICommand CollapseAssetTreeCommand { get; set; }
    }
}
