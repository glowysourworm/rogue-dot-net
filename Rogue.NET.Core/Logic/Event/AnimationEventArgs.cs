using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Event
{
    public class AnimationEventArgs : EventArgs
    {
        public IEnumerable<AnimationTemplate> Animations { get; set; }
        public CellPoint SourceLocation { get; set; }
        public IEnumerable<CellPoint> TargetLocations { get; set; }
    }
}
