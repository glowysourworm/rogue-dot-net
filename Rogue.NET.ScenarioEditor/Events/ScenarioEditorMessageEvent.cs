using Prism.Events;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class ScenarioEditorMessageEventArgs : System.EventArgs
    {
        public string Message { get; set; }
    }
    public class ScenarioEditorMessageEvent : PubSubEvent<ScenarioEditorMessageEventArgs>
    {

    }
}
