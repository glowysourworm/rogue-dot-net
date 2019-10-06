using Rogue.NET.Core.Model.Scenario.Animation;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.Animation.Interface
{
    public interface IAnimationCreator
    {
        AnimationQueue CreateAnimation(AnimationData animation, Rect bounds, Point sourceLocation, Point[] targetLocations);

        AnimationQueue CreateTargetingAnimation(Point point, Color fillColor, Color strokeColor);
    }
}
