using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData
{
    public class AnimationEventData
    {
        public IEnumerable<Model.Scenario.Animation.AnimationData> Animations { get; set; }
        public GridLocation SourceLocation { get; set; }
        public IEnumerable<GridLocation> TargetLocations { get; set; }
    }
}
