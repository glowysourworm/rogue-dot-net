using Rogue.NET.Common.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface
{
    public interface IScenarioAssetGroupViewModel
    {
        NotifyingObservableCollection<IScenarioAssetViewModel> Assets { get; }
        Type AssetType { get; }
        bool HasObjectiveAssets { get; }
        ICommand AddAssetCommand { get; }
    }
}
