using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.Scenario.Content;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.Animation.Interface
{
    public interface IAnimationSequenceCreator
    {
        IAnimationPlayer CreateAnimation(AnimationSequence animation, Rect bounds, Point sourceLocation, Point[] targetLocations);

        IAnimationPlayer CreateTargetingAnimation(Point point, Color fillColor, Color strokeColor);

        IAnimationPlayer CreateThrowAnimation(ScenarioImage scenarioImage, Point sourceLocation, Point targetLocation);

        IAnimationPlayer CreateAmmoAnimation(ScenarioImage scenarioImage, Point sourceLocation, Point targetLocation);
    }
}
