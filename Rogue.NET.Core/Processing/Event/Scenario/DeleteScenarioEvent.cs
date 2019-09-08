using Rogue.NET.Common.Extension.Prism.EventAggregator;

namespace Rogue.NET.Core.Processing.Event.Scenario
{
    public class DeleteScenarioEventArgs : System.EventArgs
    {
        public string ScenarioName { get; set; }
    }
    public class DeleteScenarioEvent : RogueEvent<DeleteScenarioEventArgs>
    {
        
    }
}
