using Rogue.NET.Core.Model.Scenario.Animation;
using System.Collections.Generic;
using System.Windows;

namespace Rogue.NET.Core.Media.Interface
{
    public interface IAnimationCreator
    {
        IEnumerable<AnimationQueue> CreateAnimation(AnimationData animation, Rect bounds, Point sourceLocation, Point[] targetLocations);

        IEnumerable<AnimationQueue> CreateTargetingAnimation(Point[] points);
    }
}
