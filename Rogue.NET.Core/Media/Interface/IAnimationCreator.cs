using Rogue.NET.Core.Model.Scenario.Animation;
using System.Collections.Generic;
using System.Windows;

namespace Rogue.NET.Core.Media.Interface
{
    public interface IAnimationCreator
    {
        ITimedGraphic CreateAnimation(AnimationData animation, Rect bounds, Point sourceLocation, Point[] targetLocations);

        IEnumerable<ITimedGraphic> CreateTargetingAnimation(Point[] points);

        /// <summary>
        /// Returns the total run-time in milli-seconds
        /// </summary>
        int CalculateRunTime(AnimationData animationTemplate, Point sourcePoint, Point[] targetPoints);
    }
}
