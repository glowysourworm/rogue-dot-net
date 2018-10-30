using Prism.Events;
using System;

namespace Rogue.NET.Model.Events
{
    public class ScenarioInfoUpdatedEventArgs : System.EventArgs
    {
        public string ScenarioName { get; set; }
        public string PlayerName { get; set; }
        public int Ticks { get; set; }
        public int Seed { get; set; }
        public int CurrentLevel { get; set; }
        public bool SurvivorMode { get; set; }
        public bool ObjectiveAcheived { get; set; }
        public DateTime StartTime { get; set; }
    }

    public class ScenarioInfoUpdatedEvent : PubSubEvent<ScenarioInfoUpdatedEventArgs>
    {

    }
}
