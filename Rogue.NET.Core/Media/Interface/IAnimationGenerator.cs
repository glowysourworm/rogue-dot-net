using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System.Collections.Generic;
using System.Windows;

namespace Rogue.NET.Core.Media.Interface
{
    public interface IAnimationGenerator
    {
        ITimedGraphic CreateAnimation(
            AnimationTemplate animationTemplate, 
            Rect bounds, 
            Point sourcePoint, 
            Point[] targetPoints);

        IEnumerable<ITimedGraphic> CreateAnimation(
            IEnumerable<AnimationTemplate> animationTemplates,
            Rect bounds,
            Point sourcePoint,
            Point[] targetPoints);

        IEnumerable<ITimedGraphic> CreateTargetingAnimation(Point[] points);

        /// <summary>
        /// Returns the total run-time in milli-seconds
        /// </summary>
        int CalculateRunTime(AnimationTemplate animationTemplate, Point sourcePoint, Point[] targetPoints);
    }
}
