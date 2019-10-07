using Rogue.NET.Common.Extension.Prism.EventAggregator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class LoadRegionEventData
    {
        /// <summary>
        /// View to load into the region
        /// </summary>
        public Type ViewType { get; set; }
    }
    public class LoadRegionEvent : RogueRegionEvent<LoadRegionEventData>
    {
    }
}
