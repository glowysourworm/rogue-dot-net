using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Events.Scenario
{
    public class LoadLevelEvent : CompositePresentationEvent<LoadLevelEvent>
    {
        public int LevelNumber { get; set; }
        public PlayerStartLocation StartLocation { get; set; }
    }
}
