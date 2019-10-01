using Rogue.NET.Common.Extension.Event;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface
{
    public interface IScenarioLevelViewModel
    {
        string Name { get; set; }
        ObservableCollection<IScenarioLevelBranchViewModel> LevelBranches { get; set; }

        /// <summary>
        ///  Command to remove the level
        /// </summary>
        ICommand RemoveLevelCommand { get; set; }

        /// <summary>
        ///  Command to load the level
        /// </summary>
        ICommand LoadLevelCommand { get; set; }
    }
}
