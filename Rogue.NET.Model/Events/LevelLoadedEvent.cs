using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common;
using Rogue.NET.Common.Collections;
using Rogue.NET.Model.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Model.Events
{
    public class LevelLoadedEvent : CompositePresentationEvent<LevelLoadedEvent>
    {
        public LevelData Data { get; set; }
    }
}
