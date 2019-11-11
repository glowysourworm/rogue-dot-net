using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect.Utility
{
    public static class DrawingFilter
    {
        public static void ApplyEffect(DrawingGroup drawing, SymbolEffectBase effect)
        {
            RecursiveApply(drawing, effect);
        }
        private static void RecursiveApply(DrawingGroup group, SymbolEffectBase effect)
        {
            foreach (var child in group.Children)
            {
                if (child is DrawingGroup)
                    RecursiveApply(child as DrawingGroup, effect);

                else if (child is Drawing)
                {
                    (child as GeometryDrawing).Brush = BrushFilter.ApplyFilter((child as GeometryDrawing).Brush, effect);
                }

                else
                    throw new Exception("Unknown Drawing Type DrawingFilter.RecursiveApply");
            }
        }
    }
}
