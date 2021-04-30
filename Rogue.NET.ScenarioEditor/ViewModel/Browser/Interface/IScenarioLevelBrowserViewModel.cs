using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface
{
    public interface IScenarioLevelBrowserViewModel
    {
        ICommand AddLevelCommand { get; set; }
        ICommand CollapseLevelTreeCommand { get; set; }
        ObservableCollection<IScenarioLevelViewModel> Levels { get; set; }
    }
}
