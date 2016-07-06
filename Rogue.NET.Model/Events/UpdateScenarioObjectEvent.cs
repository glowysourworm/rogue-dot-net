using Microsoft.Practices.Prism.Events;
using Rogue.NET.Model.Scenario;
using System;

namespace Rogue.NET.Model.Events.Scenario
{
    public class UpdateScenarioObjectEvent : CompositePresentationEvent<UpdateScenarioObjectEvent>
    {
        public ScenarioObject ScenarioObject { get; set; }
    }
}
