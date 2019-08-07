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
                Animations = template.Animations.Select(x => new AnimationData()
                {
                    AccelerationRatio = x.AccelerationRatio,
                    AnimationTime = x.AnimationTime,
                    AutoReverse = x.AutoReverse,
                    BaseType = x.BaseType,
                    ChildCount = x.ChildCount,
                    ConstantVelocity = x.ConstantVelocity,
                    Erradicity = x.Erradicity,
                    FillTemplate = x.FillTemplate,
                    Height1 = x.Height1,
                    Height2 = x.Height2,
                    Opacity1 = x.Opacity1,
                    Opacity2 = x.Opacity2,
                    RadiusFromFocus = x.RadiusFromFocus,
                    RepeatCount = x.RepeatCount,
                    RoamRadius = x.RoamRadius,
                    SpiralRate = x.SpiralRate,
                    StrokeTemplate = x.StrokeTemplate,
                    StrokeThickness = x.StrokeThickness,
                    TargetType = x.TargetType, 
                    Velocity = x.Velocity,
                    Width1 = x.Width1,
                    Width2 = x.Width2
                }).ToList()
            };
        }
    }
}
