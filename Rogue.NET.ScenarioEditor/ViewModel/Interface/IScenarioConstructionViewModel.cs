using Prism.Commands;

namespace Rogue.NET.ScenarioEditor.ViewModel.Interface
{
    public interface IScenarioConstructionViewModel
    {
        DelegateCommand<string> LoadConstructionCommand { get; }
    }
}
