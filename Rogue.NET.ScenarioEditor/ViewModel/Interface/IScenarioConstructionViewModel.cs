using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Interface
{
    public interface IScenarioConstructionViewModel
    {
        ICommand LoadConstructionCommand { get; }
    }
}
