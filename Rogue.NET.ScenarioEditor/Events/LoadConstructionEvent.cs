using Prism.Events;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class LoadConstructionEventArgs : System.EventArgs
    {
        public string ConstructionName { get; set; }
    }
    public class LoadConstructionEvent : PubSubEvent<LoadConstructionEventArgs>
    {
        
    }
}
