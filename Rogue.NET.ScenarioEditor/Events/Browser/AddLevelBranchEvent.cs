using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.Events.Browser
{
    public class AddLevelBranchEventData
    {
        public string LevelName { get; set; }
        public string LevelBranchUniqueName { get; set; }
    }

    public class AddLevelBranchEvent : RogueEvent<AddLevelBranchEventData>
    {

    }
}
