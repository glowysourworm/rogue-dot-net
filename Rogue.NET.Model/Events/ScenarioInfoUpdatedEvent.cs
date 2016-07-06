using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Model.Events
{
    public class ScenarioInfoUpdatedEvent : CompositePresentationEvent<ScenarioInfoUpdatedEvent>
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
}
