﻿using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Model.Events
{
    public class ScenarioMessageEvent : CompositePresentationEvent<ScenarioMessageEvent>
    {
        public string Message { get; set; }
    }
}
