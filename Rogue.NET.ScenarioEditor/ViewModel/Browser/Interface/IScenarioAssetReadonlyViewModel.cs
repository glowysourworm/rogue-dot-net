using Rogue.NET.Common.Extension.Event;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface
{
    public interface IScenarioAssetReadonlyViewModel : INotifyPropertyChanged
    {
        string Name { get; set; }
        bool IsSelectedAsset { get; set; }

        string Type { get; }
        string SubType { get; }
        Type AssetType { get; }
        bool IsObjective { get; }
        bool IsUnique { get; }
        bool IsCursed { get; }
        SymbolDetailsTemplateViewModel SymbolDetails { get; }
        ICommand LoadAssetCommand { get; set; }
    }
}
