using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.ScenarioEditor.Events.Browser
{
    public class LoadLevelBranchAssetsEventData
    {
        public string LevelBranchName { get; set; }
        public Type AssetType { get; set; }
    }
    public class LoadLevelBranchAssetsEvent : RogueEvent<LoadLevelBranchAssetsEventData>
    {

    }
}
