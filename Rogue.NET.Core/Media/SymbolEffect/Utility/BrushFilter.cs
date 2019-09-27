using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect.Utility
{
    public static class BrushFilter
    {
        /// <summary>
        /// Shifts hue through the specified angle and returns the resulting brush.
        /// </summary>
        /// <param name="brush">Input Brush</param>
        /// <param name="radians">Angle in radians [0, 2 * pi)</param>
        public static Brush ShiftHue(Brush brush, double radians)
        {
            if (brush is SolidColorBrush)
            {
                var solidColorBrush = brush as SolidColorBrush;

                solidColorBrush.Color = ColorFilter.ShiftHue(solidColorBrush.Color, radians);

                return solidColorBrush;
            }
            else if (brush is LinearGradientBrush)
            {
                var linearBrush = brush as LinearGradientBrush;

                foreach (var stop in linearBrush.GradientStops)
                    stop.Color = ColorFilter.ShiftHue(stop.Color, radians);

                return linearBrush;
            }
            else if (brush is RadialGradientBrush)
            {
                var radialBrush = brush as RadialGradientBrush;

                foreach (var stop in radialBrush.GradientStops)
                    stop.Color = ColorFilter.ShiftHue(stop.Color, radians);

                return radialBrush;
            }
            else
                throw new Exception("Unknown Brush Type");
        }

        /// <summary>
        /// Applies a desaturation effect to the brush
        /// </summary>
        /// <param name="brush">Input Brush</param>
        public static Brush Saturate(Brush brush, double saturation)
        {
            if (brush is SolidColorBrush)
            {
                var solidColorBrush = brush as SolidColorBrush;

                solidColorBrush.Color = ColorFilter.Saturate(solidColorBrush.Color, saturation);

                return solidColorBrush;
            }
            else if (brush is LinearGradientBrush)
            {
                var linearBrush = brush as LinearGradientBrush;

                foreach (var stop in linearBrush.GradientStops)
                    stop.Color = ColorFilter.Saturate(stop.Color, saturation);

                return linearBrush;
            }
            else if (brush is RadialGradientBrush)
            {
                var radialBrush = brush as RadialGradientBrush;

                foreach (var stop in radialBrush.GradientStops)
                    stop.Color = ColorFilter.Saturate(stop.Color, saturation);

                return radialBrush;
            }
            else
                throw new Exception("Unknown Brush Type");
        }

        /// <summary>
        /// Applies a desaturation effect to the brush
        /// </summary>
        /// <param name="brush">Input Brush</param>
        public static Brush Clamp(Brush brush, Color color)
        {
            if (brush is SolidColorBrush)
            {
                var solidColorBrush = brush as SolidColorBrush;

                solidColorBrush.Color = color;

                return solidColorBrush;
            }
            else if (brush is LinearGradientBrush)
            {
                var linearBrush = brush as LinearGradientBrush;

                foreach (var stop in linearBrush.GradientStops)
                    stop.Color = color;

                return linearBrush;
            }
            else if (brush is RadialGradientBrush)
            {
                var radialBrush = brush as RadialGradientBrush;

                foreach (var stop in radialBrush.GradientStops)
                    stop.Color = color;

                return radialBrush;
            }
            else
                throw new Exception("Unknown Brush Type");
        }
    }
}
