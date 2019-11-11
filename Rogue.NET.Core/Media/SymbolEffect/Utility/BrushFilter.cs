using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect.Utility
{
    public static class BrushFilter
    {
        public static Brush ApplyFilter(Brush brush, SymbolEffectBase effect)
        {
            return ApplyFilter(brush, (color) => effect.ApplyFilter(color));
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
