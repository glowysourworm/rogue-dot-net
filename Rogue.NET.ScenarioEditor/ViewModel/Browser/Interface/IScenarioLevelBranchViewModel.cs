using Rogue.NET.Common.Extension.Event;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface
{
    public interface IScenarioLevelBranchViewModel
    {
        string Name { get; set; }

        // Readonly Assets don't have Remove / Rename / Copy / Delete commands (At this Tree-view level)
        //
        // NOTE*** These tree-view branches are just to show the assets in the level branch. Clicking should
        //         just load the asset into the editor - which WILL allow editing; but the tree-view shouldn't
        //         be able to rename / copy / add / remove any of the assets at THIS level. 
        ObservableCollection<IScenarioAssetReadonlyViewModel> Layouts { get; set; }
        ObservableCollection<IScenarioAssetReadonlyViewModel> Equipment { get; set; }
        ObservableCollection<IScenarioAssetReadonlyViewModel> Consumables { get; set; }
        ObservableCollection<IScenarioAssetReadonlyViewModel> Enemies { get; set; }
        ObservableCollection<IScenarioAssetReadonlyViewModel> Friendlies { get; set; }
        ObservableCollection<IScenarioAssetReadonlyViewModel> Doodads { get; set; }
        ICommand RemoveLevelBranchCommand { get; set; }
        ICommand LoadLevelBranchAssetsCommand { get; set; }
        ICommand CopyLevelBranchCommand { get; set; }
        ICommand RenameLevelBranchCommand { get; set; }
    }
}
