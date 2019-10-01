using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.Events.Browser
{
    public class RemoveLevelBranchEventData
    {
        public string LevelName { get; set; }
        public string LevelBranchName { get; set; }
    }
    public class RemoveLevelBranchEvent : RogueEvent<RemoveLevelBranchEventData>
    {
    }
}
