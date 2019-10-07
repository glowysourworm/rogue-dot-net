using Rogue.NET.Core.Model.Enums;
using System.Windows;
using System.Windows.Media.Animation;

namespace Rogue.NET.Core.Media.Animation.Extension
{
    public class CustomEaseDoubleAnimation : DoubleAnimationBase
    {
        public AnimationEasingType EasingType { get; private set; }
        public double EasingAmount { get; private set; }

        double _from = 0;
        double _to = 0;

        public CustomEaseDoubleAnimation() { }

        public CustomEaseDoubleAnimation(double from, double to, AnimationEasingType easingType, double easingAmount, Duration duration)
        {
            _from = from;
            _to = to;

            this.EasingType = easingType;
            this.EasingAmount = easingAmount;
            this.Duration = duration;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new CustomEaseDoubleAnimation(_from, _to, this.EasingType, this.EasingAmount, this.Duration);
        }

        protected override double GetCurrentValueCore(double defaultOriginValue, 
                                                      double defaultDestinationValue, 
                                                      AnimationClock animationClock)
        {
            if (!animationClock.CurrentProgress.HasValue)
                return 0;

            return EasingFunctions.CalculateEase(this.EasingType, 
                                                 this.EasingAmount,
                                                 // Get current progress ratio [0, 1]
                                                 animationClock.CurrentProgress.Value, 
                                                 _from, 
                                                 _to);
        }
    }
}
