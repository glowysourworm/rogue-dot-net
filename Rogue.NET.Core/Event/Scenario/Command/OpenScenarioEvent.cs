using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;

namespace Rogue.NET.Common.Events.Scenario
{
    public class OpenScenarioEventArgs : System.EventArgs
    {
        public string ScenarioName { get; set; }
    }
    public class OpenScenarioEvent : RogueEvent<OpenScenarioEventArgs>
    {
        
    }
}
