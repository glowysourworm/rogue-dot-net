using Rogue.NET.Common.Extension.Prism.EventAggregator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Processing.Event.Content
{
    public class ZoomEventData
    {
        public double OldZoomFactor { get; set; }
        public double NewZoomFactor { get; set; }
    }
    public class ZoomEvent : RogueEvent<ZoomEventData>
    {
    }
}
