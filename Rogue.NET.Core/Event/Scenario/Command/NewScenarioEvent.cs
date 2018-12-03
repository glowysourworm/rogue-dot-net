using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Common.Events.Scenario
{
    public class NewScenarioEventArgs : System.EventArgs
    {
        public string ScenarioName { get; set; }
        public string RogueName { get; set; }
        public int Seed { get; set; }
        public bool SurvivorMode { get; set; }
        public AttributeEmphasis AttributeEmphasis { get; set; }
    }
    public class NewScenarioEvent : RogueEvent<NewScenarioEventArgs>
    {
    }
}
