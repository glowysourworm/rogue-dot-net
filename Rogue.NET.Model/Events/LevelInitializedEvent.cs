using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Model.Events
{
    /// <summary>
    /// Occurs after player is placed
    /// </summary>
    public class LevelInitializedEvent : CompositePresentationEvent<LevelInitializedEvent>
    {
        public LevelData Data { get; set; }
    }
}
