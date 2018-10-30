using Prism.Events;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class SaveScenarioEventArgs : System.EventArgs
    {
        public string ScenarioName { get; set; }
    }

    public class SaveScenarioEvent : PubSubEvent<SaveScenarioEventArgs>
    {
        
    }
}
