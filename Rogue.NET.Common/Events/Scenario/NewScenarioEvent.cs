using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Events.Scenario
{
    public class NewScenarioEvent : CompositePresentationEvent<NewScenarioEvent>
    {
        public string ScenarioName { get; set; }
        public string RogueName { get; set; }
        public int Seed { get; set; }
        public bool SurvivorMode { get; set; }
    }
}
