using Prism.Events;

namespace Rogue.NET.Model.Events
{
    public class ScenarioMessageEventArgs : System.EventArgs
    {
        public string Message { get; set; }
    }
    public class ScenarioMessageEvent : PubSubEvent<ScenarioMessageEventArgs>
    {
        
    }
}
