using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
