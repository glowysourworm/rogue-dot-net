using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System.Linq;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(IAnimationGenerator))]
    public class AnimationGenerator : IAnimationGenerator
    {
        public AnimationGenerator() { }

        public AnimationGroup GenerateAnimationGroup(AnimationGroupTemplate template)
        {
            return new AnimationGroup()
            {
                Animations = template.Animations.Select(x => GenerateAnimation(x)).ToList(),
                TargetType = template.TargetType
            };
        }

        public AnimationData GenerateAnimation(AnimationTemplate template)
        {
            return new AnimationData()
            {
                AccelerationRatio = template.AccelerationRatio,
                AnimationTime = template.AnimationTime,
                AutoReverse = template.AutoReverse,
                BaseType = template.BaseType,
                ChildCount = template.ChildCount,
                ConstantVelocity = template.ConstantVelocity,
                Erradicity = template.Erradicity,
                FillTemplate = template.FillTemplate,
                Height1 = template.Height1,
                Height2 = template.Height2,
                Opacity1 = template.Opacity1,
                Opacity2 = template.Opacity2,
                PointTargetType = template.PointTargetType,
                RadiusFromFocus = template.RadiusFromFocus,
                RepeatCount = template.RepeatCount,
                RoamRadius = template.RoamRadius,
                SpiralRate = template.SpiralRate,
                StrokeThickness = template.StrokeThickness,
                Velocity = template.Velocity,
                Width1 = template.Width1,
                Width2 = template.Width2
            };
        }
    }
}
