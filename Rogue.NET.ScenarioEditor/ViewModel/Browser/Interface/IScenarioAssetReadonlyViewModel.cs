using Rogue.NET.Common.Extension.Event;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface
{
    public interface IScenarioAssetReadonlyViewModel
    {
        string Name { get; set; }
        string Type { get; }
        string SubType { get; }
        Type AssetType { get; }
        SymbolDetailsTemplateViewModel SymbolDetails { get; }
        ICommand LoadAssetCommand { get; set; }
    }
}
