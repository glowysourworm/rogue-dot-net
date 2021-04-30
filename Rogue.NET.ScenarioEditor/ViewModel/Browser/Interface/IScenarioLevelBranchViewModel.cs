using Rogue.NET.Common.ViewModel;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface
{
    public interface IScenarioLevelBranchViewModel
    {
        string Name { get; set; }
        bool HasObjectiveAssets { get; set; }
        bool HasEquipmentObjectiveAssets { get; set; }
        bool HasConsumableObjectiveAssets { get; set; }
        bool HasEnemyObjectiveAssets { get; set; }
        bool HasFriendlyObjectiveAssets { get; set; }
        bool HasDoodadObjectiveAssets { get; set; }

        // Readonly Assets don't have Remove / Rename / Copy / Delete commands (At this Tree-view level)
        //
        // NOTE*** These tree-view branches are just to show the assets in the level branch. Clicking should
        //         just load the asset into the editor - which WILL allow editing; but the tree-view shouldn't
        //         be able to rename / copy / add / remove any of the assets at THIS level. 
        NotifyingObservableCollection<IScenarioAssetReadonlyViewModel> Layouts { get; set; }
        NotifyingObservableCollection<IScenarioAssetReadonlyViewModel> Equipment { get; set; }
        NotifyingObservableCollection<IScenarioAssetReadonlyViewModel> Consumables { get; set; }
        NotifyingObservableCollection<IScenarioAssetReadonlyViewModel> Enemies { get; set; }
        NotifyingObservableCollection<IScenarioAssetReadonlyViewModel> Friendlies { get; set; }
        NotifyingObservableCollection<IScenarioAssetReadonlyViewModel> Doodads { get; set; }
        ICommand RemoveLevelBranchCommand { get; set; }
        ICommand LoadLevelBranchAssetsCommand { get; set; }
        ICommand CopyLevelBranchCommand { get; set; }
        ICommand RenameLevelBranchCommand { get; set; }
        ICommand PreviewLevelBranchCommand { get; set; }
    }
}
