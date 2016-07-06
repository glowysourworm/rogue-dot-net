﻿using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Events.Scenario
{
    public class DeleteScenarioEvent : CompositePresentationEvent<DeleteScenarioEvent>
    {
        public string ScenarioName { get; set; }
    }
}
