using Rogue.NET.Common.Extension.Event;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface
{
    public interface IScenarioAssetViewModel : IScenarioAssetReadonlyViewModel
    {
        ICommand RemoveAssetCommand { get; set; }
        ICommand CopyAssetCommand { get; set; }
        ICommand RenameAssetCommand { get; set; }
    }
}
