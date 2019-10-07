using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System.Linq;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [Export(typeof(IAnimationGenerator))]
    public class AnimationGenerator : IAnimationGenerator
    {
        public AnimationGenerator() { }

        public AnimationSequence GenerateAnimation(AnimationSequenceTemplate template)
        {
            return new AnimationSequence()
            {
                DarkenBackground = template.DarkenBackground,
                TargetType = template.TargetType,
                Animations = template.Animations.Select(x => GenerateAnimation(x)).ToList()
            };
        }

        private AnimationBase GenerateAnimation(AnimationBaseTemplate template)
        {
            if (template is AnimationAuraTemplate)
            {
                var aura = template as AnimationAuraTemplate;
                var animation = new AnimationAura();

                // Generate base parameters
                GenerateAnimation(aura, animation);

                animation.AnimationTime = aura.AnimationTime;
                animation.AutoReverse = aura.AutoReverse;
                animation.RepeatCount = aura.RepeatCount;

                return animation;
            }
            else if (template is AnimationBarrageTemplate)
            {
                var barrage = template as AnimationBarrageTemplate;
                var animation = new AnimationBarrage();

                // Generate base parameters
                GenerateAnimation(barrage, animation);

                animation.AnimationTime = barrage.AnimationTime;
                animation.ChildCount = barrage.ChildCount;
                animation.Erradicity = barrage.Erradicity;
                animation.Radius = barrage.Radius;
                animation.Reverse = barrage.Reverse;

                return animation;
            }
            else if (template is AnimationBlinkTemplate)
            {
                var blink = template as AnimationBlinkTemplate;
                var animation = new AnimationBlink();

                animation.AnimationTime = blink.AnimationTime;
                animation.AutoReverse = blink.AutoReverse;
                animation.FillTemplate = blink.FillTemplate;
                animation.Opacity1 = blink.Opacity1;
                animation.Opacity2 = blink.Opacity2;
                animation.PointTargetType = blink.PointTargetType;
                animation.RepeatCount = blink.RepeatCount;

                return animation;
            }
            else if (template is AnimationBubblesTemplate)
            {
                var bubbles = template as AnimationBubblesTemplate;
                var animation = new AnimationBubbles();

                // Generate base parameters
                GenerateAnimation(bubbles, animation);

                animation.AnimationTime = bubbles.AnimationTime;
                animation.ChildCount = bubbles.ChildCount;
                animation.Erradicity = bubbles.Erradicity;
                animation.Radius = bubbles.Radius;
                animation.RoamRadius = bubbles.RoamRadius;

                return animation;
            }
            else if (template is AnimationChainConstantVelocityTemplate)
            {
                var chain = template as AnimationChainConstantVelocityTemplate;
                var animation = new AnimationChainConstantVelocity();

                // Generate base parameters
                GenerateAnimation(chain, animation);

                animation.AutoReverse = chain.AutoReverse;
                animation.Erradicity = chain.Erradicity;
                animation.RepeatCount = chain.RepeatCount;
                animation.Reverse = chain.Reverse;
                animation.Velocity = chain.Velocity;

                return animation;
            }
            else if (template is AnimationChainTemplate)
            {
                var chain = template as AnimationChainTemplate;
                var animation = new AnimationChain();

                // Generate base parameters
                GenerateAnimation(chain, animation);

                animation.AutoReverse = chain.AutoReverse;
                animation.Erradicity = chain.Erradicity;
                animation.RepeatCount = chain.RepeatCount;
                animation.Reverse = chain.Reverse;
                animation.AnimationTime = chain.AnimationTime;

                return animation;
            }
            else if (template is AnimationLightningChainTemplate)
            {
                var chain = template as AnimationLightningChainTemplate;
                var animation = new AnimationLightningChain();

                animation.AnimationTime = chain.AnimationTime;
                animation.FillTemplate = chain.FillTemplate;
                animation.HoldEndTime = chain.HoldEndTime;
                animation.IncrementHeightLimit = chain.IncrementHeightLimit;
                animation.IncrementWidthLimit = chain.IncrementWidthLimit;
                animation.PointTargetType = chain.PointTargetType;

                return animation;
            }
            else if (template is AnimationLightningTemplate)
            {
                var lightning = template as AnimationLightningTemplate;
                var animation = new AnimationLightning();

                animation.AnimationTime = lightning.AnimationTime;
                animation.FillTemplate = lightning.FillTemplate;
                animation.HoldEndTime = lightning.HoldEndTime;
                animation.IncrementHeightLimit = lightning.IncrementHeightLimit;
                animation.IncrementWidthLimit = lightning.IncrementWidthLimit;
                animation.PointTargetType = lightning.PointTargetType;

                return animation;
            }
            else if (template is AnimationProjectileConstantVelocityTemplate)
            {
                var projectile = template as AnimationProjectileConstantVelocityTemplate;
                var animation = new AnimationProjectileConstantVelocity();

                // Generate base parameters
                GenerateAnimation(projectile, animation);

                animation.AutoReverse = projectile.AutoReverse;
                animation.Erradicity = projectile.Erradicity;
                animation.RepeatCount = projectile.RepeatCount;
                animation.Velocity = projectile.Velocity;
                animation.Reverse = projectile.Reverse;

                return animation;
            }
            else if (template is AnimationProjectileTemplate)
            {
                var projectile = template as AnimationProjectileTemplate;
                var animation = new AnimationProjectile();

                // Generate base parameters
                GenerateAnimation(projectile, animation);

                animation.AutoReverse = projectile.AutoReverse;
                animation.Erradicity = projectile.Erradicity;
                animation.RepeatCount = projectile.RepeatCount;
                animation.AnimationTime = projectile.AnimationTime;
                animation.Reverse = projectile.Reverse;

                return animation;
            }
            else if (template is AnimationSpiralTemplate)
            {
                var spiral = template as AnimationSpiralTemplate;
                var animation = new AnimationSpiral();

                // Generate base parameters
                GenerateAnimation(spiral, animation);

                animation.AnimationTime = spiral.AnimationTime;
                animation.ChildCount = spiral.ChildCount;
                animation.Clockwise = spiral.Clockwise;
                animation.Erradicity = spiral.Erradicity;
                animation.Radius = spiral.Radius;
                animation.RadiusChangeRate = spiral.RadiusChangeRate;
                animation.RotationRate = spiral.RotationRate;

                return animation;
            }
            else
                throw new Exception("Unhandled animation type AnimationGenerator");
        }

        private void GenerateAnimation(AnimationEllipseBaseTemplate source, AnimationEllipseBase dest)
        {
            // Generate base parameters for the animation
            dest.EasingType = source.EasingType;
            dest.EasingAmount = source.EasingAmount;
            dest.FillTemplate = source.FillTemplate;
            dest.Height1 = source.Height1;
            dest.Height2 = source.Height2;
            dest.Opacity1 = source.Opacity1;
            dest.Opacity2 = source.Opacity2;
            dest.PointTargetType = source.PointTargetType;
            dest.Width1 = source.Width1;
            dest.Width2 = source.Width2;
        }
    }
}
