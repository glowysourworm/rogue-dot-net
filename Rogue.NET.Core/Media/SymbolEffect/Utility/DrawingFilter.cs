using Rogue.NET.Core.Media.SymbolEffect.Utility;
using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect.Utility
{
    public static class DrawingFilter
    {
        public static void ApplyEffect(DrawingGroup drawing, SymbolEffectBase effect)
        {
            if (effect is HueShiftEffect)
            {
                RecursiveHueShift(drawing, effect as HueShiftEffect);
            }
            else if (effect is SaturationEffect)
            {
                RecursiveDesaturate(drawing, effect as SaturationEffect);
            }
            else if (effect is ClampEffect)
            {
                RecursiveClamp(drawing, effect as ClampEffect);
            }
            else
                throw new Exception("Unhandled Symbol Effect Type");
        }
        private static void RecursiveHueShift(DrawingGroup group, HueShiftEffect effect)
        {
            foreach (var child in group.Children)
            {
                if (child is DrawingGroup)
                    RecursiveHueShift(child as DrawingGroup, effect);

                else if (child is Drawing)
                    (child as GeometryDrawing).Brush = BrushFilter.ShiftHue((child as GeometryDrawing).Brush, effect.Radians);

                else
                    throw new Exception("Unknown Drawing Type DrawingIterator.RecursiveHueShift");
            }
        }
        private static void RecursiveDesaturate(DrawingGroup group, SaturationEffect effect)
        {
            foreach (var child in group.Children)
            {
                if (child is DrawingGroup)
                    RecursiveDesaturate(child as DrawingGroup, effect);

                else if (child is Drawing)
                    (child as GeometryDrawing).Brush = BrushFilter.Saturate((child as GeometryDrawing).Brush, effect.Saturation);

                else
                    throw new Exception("Unknown Drawing Type DrawingIterator.RecursiveHueShift");
            }
        }
        private static void RecursiveClamp(DrawingGroup group, ClampEffect effect)
        {
            foreach (var child in group.Children)
            {
                if (child is DrawingGroup)
                    RecursiveClamp(child as DrawingGroup, effect);

                else if (child is Drawing)
                    (child as GeometryDrawing).Brush = BrushFilter.Clamp((child as GeometryDrawing).Brush, effect.Color);

                else
                    throw new Exception("Unknown Drawing Type DrawingIterator.RecursiveHueShift");
            }
        }
    }
}
