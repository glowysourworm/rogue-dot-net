using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface
{
    public interface IScenarioAssetGroupViewModel
    {
        ObservableCollection<IScenarioAssetViewModel> Assets { get; }
        Type AssetType { get; }
        ICommand AddAssetCommand { get; }
    }
}
