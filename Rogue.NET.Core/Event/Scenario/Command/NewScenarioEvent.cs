using Prism.Events;

namespace Rogue.NET.Common.Events.Scenario
{
    public class NewScenarioEventArgs : System.EventArgs
    {
        public string ScenarioName { get; set; }
        public string RogueName { get; set; }
        public int Seed { get; set; }
        public bool SurvivorMode { get; set; }
    }
    public class NewScenarioEvent : PubSubEvent<NewScenarioEventArgs>
    {
    }
}
