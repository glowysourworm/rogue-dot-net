using Microsoft.Practices.Prism.Events;
using Rogue.NET.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class ScenarioLoadedEvent : CompositePresentationEvent<ScenarioLoadedEvent>
    {
        public ScenarioConfiguration Payload { get; set; }
    }
}
