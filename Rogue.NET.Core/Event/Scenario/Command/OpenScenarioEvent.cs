using Prism.Events;

namespace Rogue.NET.Common.Events.Scenario
{
    public class OpenScenarioEventArgs : System.EventArgs
    {
        public string ScenarioName { get; set; }
    }
    public class OpenScenarioEvent : PubSubEvent<OpenScenarioEventArgs>
    {
        
    }
}
