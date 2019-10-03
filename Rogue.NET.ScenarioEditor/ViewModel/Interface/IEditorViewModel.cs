using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Interface
{
    public interface IEditorViewModel
    {
        ScenarioConfigurationContainerViewModel Configuration { get; }

        string ScenarioName { get; set; }
        bool HasChanges { get; set; }

        ICommand ExitCommand { get; }
        ICommand LoadBuiltInCommand { get; }
        ICommand SaveCommand { get; }
        ICommand NewCommand { get; }
    }
}
