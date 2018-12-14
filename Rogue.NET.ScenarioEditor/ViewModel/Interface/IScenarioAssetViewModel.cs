using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Interface
{
    public interface IScenarioAssetViewModel
    {
        string Name { get; set; }
        string Type { get; set; }
        string SubType { get; set; }
        RangeViewModel<int> Level { get; set; }
        SymbolDetailsTemplateViewModel SymbolDetails { get; set; }
        bool IsSelected { get; set; }
        bool IsLevelPlacement { get; set; }

        ICommand RemoveAssetCommand { get; set; }
        ICommand LoadAssetCommand { get; set; }
        ICommand CopyAssetCommand { get; set; }
        ICommand RenameAssetCommand { get; set; }

        event EventHandler<IScenarioAssetViewModel> RemoveAssetEvent;
        event EventHandler<IScenarioAssetViewModel> LoadAssetEvent;
        event EventHandler<IScenarioAssetViewModel> CopyAssetEvent;
        event EventHandler<IScenarioAssetViewModel> RenameAssetEvent;
    }
}
