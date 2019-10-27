using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Media.Animation.Extension
{
    // MSFT SOURCE: https://github.com/microsoft/WPF-Samples/blob/master/Animation/CustomAnimation
    //              (some good examples)
    //
    public static class EasingFunctions
    {
        public static double CalculateEase(AnimationEasingType easingType, double easingAmount, double timeFraction, double from, double to)
        {
            switch (easingType)
            {
                case AnimationEasingType.None:
                    return (timeFraction * (to - from)) + from;
                case AnimationEasingType.BackEase:
                    {
                        // Examine function: x^(suppression) * -1 * (amplitude) sin( pi * x ) + x
                        //
                        // Can nicely characterize the dynamics with a single number. Want to keep
                        // suppression = amplitude + 1 (approx.). This will make the curve we want.
                        //
                        // For the amount - the ratio (suppression / amplitude) should remain nearly
                        // constant - with a variation between 1 - 10 (to make it more dramatic)
                        //

                        var suppression = easingAmount * 10.0;
                        var amplitude = System.Math.Max(0, suppression - 1.0);

                        return EasingFunctions.BackEase(timeFraction, from, to, amplitude, suppression);
                    }
                case AnimationEasingType.BounceEase:
                    {
                        var bounciness = (easingAmount * 10.0);
                        var bounces = (int)(easingAmount * 10.0);

                        return EasingFunctions.BounceEase(timeFraction, from, to, bounciness, bounces);
                    }
                case AnimationEasingType.ExponentialEase:
                    {
                        var exponent = easingAmount * 10.0;

                        return EasingFunctions.ExponentialEase(timeFraction, from, to, exponent);
                    }
                case AnimationEasingType.JoltEase:
                    {
                        var joltPlacement = easingAmount;

                        return EasingFunctions.JoltEase(timeFraction, from, to, joltPlacement);
                    }
                default:
                    throw new Exception("Unhandled easing type EasingFunctions");
            }
        }

        /// <summary>
        /// Returns a value according to a back ease function. This value is offset using your from, to values.
        /// </summary>
        /// <param name="timeFraction">Current fractional time of animation progress</param>
        /// <param name="amplitude">Amplitude of back ease</param>
        /// <param name="suppression">Delays the start of the effect</param>
        private static double BackEase(double timeFraction, double from, double to, double amplitude, double suppression)
        {
            var frequency = 0.5;
            var delta = to - from;

            // math magic: The sine gives us the right wave, the timeFraction * 0.5 (frequency) gives us only half 
            // of the full wave (flipped by multiplying by -1 so that we go "backwards" first), the amplitude gives 
            // us the relative height of the peak, and the exponent makes the effect start later by the "suppression" 
            // factor. 
            var returnValue = System.Math.Pow((timeFraction), suppression)
                              * amplitude * System.Math.Sin(2 * System.Math.PI * timeFraction * frequency) * -1 + timeFraction;
            returnValue = (returnValue * delta);
            returnValue += from;
            return returnValue;
        }

        private static double BounceEase(double timeFraction, double from, double to, double bounciness, int bounces)
        {
            var delta = to - from;

            // math magic: The cosine gives us the right wave, the timeFraction is the frequency of the wave, 
            // the absolute value keeps every value positive (so it "bounces" off the midpoint of the cosine 
            // wave, and the amplitude (the exponent) makes the sine wave get smaller and smaller at the end.
            var returnValue = System.Math.Abs(System.Math.Pow((1 - timeFraction), bounciness)
                                       * System.Math.Cos(2 * System.Math.PI * timeFraction * bounces));
            returnValue = delta - (returnValue * delta);
            returnValue += from;
            return returnValue;
        }

        private static double ExponentialEase(double timeFraction, double from, double to, double exponent)
        {
            var delta = to - from;

            // math magic: simple exponential decay
            var returnValue = System.Math.Pow(timeFraction, exponent);
            returnValue *= delta;
            returnValue = returnValue + from;
            return returnValue;
        }

        private static double JoltEase(double timeFraction, double from, double to, double joltPlacement)
        {
            // Starting from to, places a "triangle" spike at the critical point - then animates linearly from -> to.
            //

            var slope = (to - from) / (1.0 - joltPlacement);
            var intercept = from - (slope * joltPlacement);

            if (timeFraction < joltPlacement)
                return to;

            else
            {
                return (slope * timeFraction) + intercept;
            }
        }
    }
}
