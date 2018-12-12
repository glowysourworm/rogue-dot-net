using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface
{
    public interface IDifficultyAssetGroupViewModel
    {
        ObservableCollection<IDifficultyAssetViewModel> Assets { get; set; }
        string AssetType { get; set; }
    }
}
