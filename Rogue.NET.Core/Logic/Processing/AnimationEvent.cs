using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System.Collections.Generic;
using System.Windows;

namespace Rogue.NET.Core.Logic.Processing
{
    public class AnimationEvent : IAnimationEvent
    {
        public IEnumerable<AnimationTemplate> Animations { get; set; }
        public Point SourceLocation { get; set; }
        public IEnumerable<Point> TargetLocations { get; set; }
    }
}
