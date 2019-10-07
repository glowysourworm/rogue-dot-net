using Rogue.NET.Core.Model.Enums;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Rogue.NET.Core.Media.Animation.Extension
{
    public class CustomEaseDoublePathAnimation : DoubleAnimationBase
    {
        public PathGeometry PathGeometry { get; private set; }
        public PathAnimationSource Source { get; private set; }
        public AnimationEasingType EasingType { get; private set; }
        public double EasingAmount { get; private set; }

        public CustomEaseDoublePathAnimation() { }

        public CustomEaseDoublePathAnimation(PathGeometry pathGeometry, PathAnimationSource source, AnimationEasingType easingType, double easingAmount, Duration duration)
        {
            this.PathGeometry = pathGeometry;
            this.Source = source;
            this.EasingType = easingType;
            this.EasingAmount = easingAmount;
            this.Duration = duration;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new CustomEaseDoublePathAnimation(this.PathGeometry, this.Source, this.EasingType, this.EasingAmount, this.Duration);
        }

        protected override double GetCurrentValueCore(double defaultOriginValue, double defaultDestinationValue, AnimationClock animationClock)
        {
            if (!animationClock.CurrentProgress.HasValue)
                return 0;

            // Get current progress ratio [0, 1]
            var fraction = EasingFunctions.CalculateEase(this.EasingType,
                                                         this.EasingAmount,
                                                         // Get current progress ratio [0, 1]
                                                         animationClock.CurrentProgress.Value,
                                                         0.0,
                                                         1.0);

            Point point, tangent;

            this.PathGeometry.GetPointAtFractionLength(fraction, out point, out tangent);

            switch (this.Source)
            {
                case PathAnimationSource.X:
                    return point.X;
                case PathAnimationSource.Y:
                    return point.Y;
                case PathAnimationSource.Angle:
                    return (180.0 / Math.PI) * Math.Atan2(tangent.Y, tangent.X);
                default:
                    throw new Exception("Unhandled Path Animation Source");
            }
        }
    }
}
