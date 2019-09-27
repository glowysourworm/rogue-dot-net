using Rogue.NET.Core.Media.SymbolEffect.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect
{
    /// <summary>
    /// Effect that maps one color to another
    /// </summary>
    public class ColorMapEffect : SymbolEffectBase
    {
        public Color FromColor { get; private set; }
        public Color ToColor { get; private set; }

        public ColorMapEffect(string fromColor, string toColor)
        {
            this.FromColor = ColorFilter.Convert(fromColor);
            this.ToColor = ColorFilter.Convert(toColor);
        }
    }
}
