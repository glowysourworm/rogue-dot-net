using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Media.SymbolEffect.Interface;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ISymbolEffectFilter))]
    public class SymbolEffectFilter : ISymbolEffectFilter
    {
        public SymbolEffectFilter() { }

        public void ApplyEffect(DrawingGroup drawing, SymbolEffectBase effect)
        {
            RecursiveApplyEffect(drawing, effect);
        }

        private void RecursiveApplyEffect(DrawingGroup group, SymbolEffectBase effect)
        {
            foreach (var child in group.Children)
            {
                if (child is DrawingGroup)
                    RecursiveApplyEffect(child as DrawingGroup, effect);

                else if (child is Drawing)
                {
                    (child as GeometryDrawing).Brush = ApplyBrushFilter((child as GeometryDrawing).Brush, effect);
                }

                else
                    throw new Exception("Unknown Drawing Type DrawingFilter.RecursiveApply");
            }
        }

        private Brush ApplyBrushFilter(Brush brush, SymbolEffectBase effect)
        {
            return ApplyBrushFilter(brush, (color) => effect.ApplyFilter(color));
        }

        private Brush ApplyBrushFilter(Brush brush, Func<Color, Color> filter)
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
