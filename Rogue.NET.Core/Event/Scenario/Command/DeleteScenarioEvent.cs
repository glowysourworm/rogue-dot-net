using Prism.Events;

namespace Rogue.NET.Common.Events.Scenario
{
    public class DeleteScenarioEventArgs : System.EventArgs
    {
        public string ScenarioName { get; set; }
    }
    public class DeleteScenarioEvent : PubSubEvent<DeleteScenarioEventArgs>
    {
        
    }
}
