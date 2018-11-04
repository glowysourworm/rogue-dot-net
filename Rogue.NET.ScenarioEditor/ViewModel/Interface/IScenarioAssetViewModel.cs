using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Interface
{
    public interface IScenarioAssetViewModel
    {
        string Name { get; set; }
        string Type { get; set; }
        SymbolDetailsTemplate SymbolDetails { get; set; }
        bool IsSelected { get; set; }

        ICommand RemoveAssetCommand { get; }
        ICommand LoadAssetCommand { get; }
    }
}
