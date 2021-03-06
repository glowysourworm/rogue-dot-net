﻿using Rogue.NET.Core.Media.SymbolEffect.Utility;

using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect
{
    /// <summary>
    /// Applies a hard color set to the image
    /// </summary>
    public class ClampEffect : SymbolEffectBase
    {
        public Color Color { get; set; }

        public ClampEffect(string color)
        {
            this.Color = Utility.ColorOperations.Convert(color);
        }

        public override Color ApplyFilter(Color inputColor)
        {
            return this.Color;
        }
    }
}
