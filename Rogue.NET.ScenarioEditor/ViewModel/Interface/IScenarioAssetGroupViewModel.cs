using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Interface
{
    public interface IScenarioAssetGroupViewModel
    {
        ICommand AddAssetCommand { get; }

        void RemoveAsset(IScenarioAssetViewModel asset);
    }
}
