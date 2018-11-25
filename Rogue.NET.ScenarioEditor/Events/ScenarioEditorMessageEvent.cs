using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class ScenarioEditorMessageEventArgs : System.EventArgs
    {
        public string Message { get; set; }
    }
    public class ScenarioEditorMessageEvent : RogueEvent<ScenarioEditorMessageEventArgs>
    {

    }
}
