using Rogue.NET.Common.Extension.Prism.EventAggregator;

namespace Rogue.NET.Core.Event.Scenario
{
    public class NewScenarioEventArgs : System.EventArgs
    {
        public string ScenarioName { get; set; }
        public string RogueName { get; set; }
        public string CharacterClassName { get; set; }
        public int Seed { get; set; }
        public bool SurvivorMode { get; set; }
    }
    public class NewScenarioEvent : RogueEvent<NewScenarioEventArgs>
    {
    }
}
