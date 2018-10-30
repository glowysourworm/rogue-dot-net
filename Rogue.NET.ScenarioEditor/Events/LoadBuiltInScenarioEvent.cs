using Prism.Events;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class LoadBuiltInScenarioEventArgs : System.EventArgs
    {
        public string ScenarioName { get; set; }
    }
    public class LoadBuiltInScenarioEvent : PubSubEvent<LoadBuiltInScenarioEventArgs>
    {
        
    }
}
