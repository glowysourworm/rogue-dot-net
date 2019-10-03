using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect.Utility
{
    public static class BrushFilter
    {
        /// <summary>
        /// Color used as a mask for applying effects
        /// </summary>
        private static Color ColorMask = Colors.Red;

        /// <summary>
        /// Applies HSL filter to the specified brush - HSL(0,0,0) being the neutral position - shifting positive
        /// or negative.
        /// </summary>
        public static Brush ShiftHSL(Brush brush, double hueRadians, double saturation, double lightness, bool useColorMask)
        {
            return ApplyFilter(brush, color => 
            {
                if (!useColorMask)
                    return ColorFilter.ShiftHSL(color, hueRadians, saturation, lightness);

                else
                {
                    if (color == ColorMask)
                        return ColorFilter.ShiftHSL(color, hueRadians, saturation, lightness);

                    else
                        return color;
                }
            });
        }

        /// <summary>
        /// Maps input color from brush to the specified output color
        /// </summary>
        public static Brush MapColor(Brush brush, Color inputColor, Color outputColor)
        {
            return ApplyFilter(brush, color =>
            {
                if (color == inputColor)
                    return outputColor;

                return color;
            });
        }

        /// <summary>
        /// Applies a desaturation effect to the brush
        /// </summary>
        /// <param name="brush">Input Brush</param>
        public static Brush Clamp(Brush brush, Color color)
        {
            return ApplyFilter(brush, inputColor => color);
        }

        private static Brush ApplyFilter(Brush brush, Func<Color, Color> filter)
        {
            // NOTE*** Using brush clones to apply changes to avoid freezable issues

            if (brush is SolidColorBrush)
            {
                var solidColorBrush = brush.Clone() as SolidColorBrush;

                solidColorBrush.Color = filter(solidColorBrush.Color);

                return solidColorBrush;
            }
            else if (brush is LinearGradientBrush)
            {
                var linearBrush = brush.Clone() as LinearGradientBrush;

                foreach (var stop in linearBrush.GradientStops)
                    stop.Color = filter(stop.Color);

                return linearBrush;
            }
            else if (brush is RadialGradientBrush)
            {
                var radialBrush = brush.Clone() as RadialGradientBrush;

                foreach (var stop in radialBrush.GradientStops)
                    stop.Color = filter(stop.Color);

                return radialBrush;
            }
            else
                throw new Exception("Unknown Brush Type");
        }
    }
}
