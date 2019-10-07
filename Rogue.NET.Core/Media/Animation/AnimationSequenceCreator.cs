using Rogue.NET.Core.Media.Animation.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.Animation
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IAnimationSequenceCreator))]
    public class AnimationSequenceCreator : IAnimationSequenceCreator
    {
        readonly IAnimationCreator _animationCreator;

        [ImportingConstructor]
        public AnimationSequenceCreator(IAnimationCreator animationCreator)
        {
            _animationCreator = animationCreator;
        }

        public IAnimationPlayer CreateAnimation(AnimationSequence animation, Rect bounds, Point sourceLocation, Point[] targetLocations)
        {
            var animationGroups = animation.Animations.Select(x => _animationCreator.CreateAnimation(x, bounds, sourceLocation, targetLocations));

            return new AnimationQueue(animationGroups);
        }

        public IAnimationPlayer CreateTargetingAnimation(Point point, Color fillColor, Color strokeColor)
        {
            return new AnimationQueue(new AnimationPrimitiveGroup[] { _animationCreator.CreateTargetingAnimation(point, fillColor)});
        }
    }
}
