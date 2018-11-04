using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Interface
{
    public interface IEditorViewModel
    {
        string ScenarioName { get; set; }

        ICommand ExitCommand { get; }

        ICommand LoadBuiltInCommand { get; }
        ICommand SaveCommand { get; }
        ICommand NewCommand { get; }
        ICommand ShowDifficultyCommand { get; }
    }
}
