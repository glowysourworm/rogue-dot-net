﻿using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect.Utility
{
    public static class DrawingFilter
    {
        public static void ApplyEffect(DrawingGroup drawing, SymbolEffectBase effect)
        {
            if (effect is HSLEffect)
            {
                RecursiveHSL(drawing, effect as HSLEffect);
            }
            else if (effect is ColorMapEffect)
            {
                RecursiveColorMap(drawing, effect as ColorMapEffect);
            }
            else if (effect is ClampEffect)
            {
                RecursiveClamp(drawing, effect as ClampEffect);
            }
            else
                throw new Exception("Unhandled Symbol Effect Type");
        }
        private static void RecursiveHSL(DrawingGroup group, HSLEffect effect)
        {
            foreach (var child in group.Children)
            {
                if (child is DrawingGroup)
                    RecursiveHSL(child as DrawingGroup, effect);

                else if (child is Drawing)
                    (child as GeometryDrawing).Brush = BrushFilter.ShiftHSL((child as GeometryDrawing).Brush, effect.Hue, effect.Saturation, effect.Lightness);

                else
                    throw new Exception("Unknown Drawing Type DrawingIterator.RecursiveHueShift");
            }
        }
        private static void RecursiveColorMap(DrawingGroup group, ColorMapEffect effect)
        {
            foreach (var child in group.Children)
            {
                if (child is DrawingGroup)
                    RecursiveColorMap(child as DrawingGroup, effect);

                else if (child is Drawing)
                    (child as GeometryDrawing).Brush = BrushFilter.MapColor((child as GeometryDrawing).Brush, effect.FromColor, effect.ToColor);

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
