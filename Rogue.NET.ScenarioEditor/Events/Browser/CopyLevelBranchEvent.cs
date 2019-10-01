using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;

namespace Rogue.NET.ScenarioEditor.Events.Browser
{
    public class CopyLevelBranchEventData
    {
        public string LevelName { get; set; }
        public string LevelBranchName { get; set; }
    }
    public class CopyLevelBranchEvent : RogueEvent<CopyLevelBranchEventData>
    {
    }
}
