using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System.Collections.Generic;
using System.Windows;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IAnimationEvent
    {
        IEnumerable<AnimationTemplate> Animations { get; set; }
        Point SourceLocation { get; set; }
        IEnumerable<Point> TargetLocations { get; set; }
    }
}
