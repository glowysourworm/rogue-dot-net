using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Scenario;

namespace Rogue.NET.Core.Processing.Event.Scenario
{
    public class DeleteScenarioEventData
    {
        public string ScenarioName { get; set; }
    }

    public class DeleteScenarioEvent : RogueEvent<DeleteScenarioEventData>
    {
        
    }
}
