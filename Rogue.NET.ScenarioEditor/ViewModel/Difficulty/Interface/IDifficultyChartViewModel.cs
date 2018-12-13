using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface
{
    public interface IDifficultyChartViewModel
    {
        string Title { get; set; }
        bool Show { get; set; }

        ICommand CalculateCommand { get; set; }
    }
}
