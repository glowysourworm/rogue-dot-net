using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Interface
{
    public interface IScenarioAssetViewModel
    {
        string Name { get; set; }
        string Type { get; set; }
        SymbolDetailsTemplateViewModel SymbolDetails { get; set; }
        bool IsSelected { get; set; }

        ICommand RemoveAssetCommand { get; }
        ICommand LoadAssetCommand { get; }

        event EventHandler<IScenarioAssetViewModel> RemoveAssetEvent;
        event EventHandler<IScenarioAssetViewModel> LoadAssetEvent;
    }
}
