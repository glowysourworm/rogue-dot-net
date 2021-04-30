using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class PreviewLevelEventData
    {
        public Level Level { get; set; }
        public LayoutTemplate LayoutTemplate { get;set; }
    }
    public class PreviewLevelEvent : RogueEvent<PreviewLevelEventData>
    {
    }
}
