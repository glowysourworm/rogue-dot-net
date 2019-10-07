using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData
{
    public class AnimationEventData
    {
        public AnimationSequence Animation { get; set; }
        public GridLocation SourceLocation { get; set; }
        public IEnumerable<GridLocation> TargetLocations { get; set; }
    }
}
