using Rogue.NET.Core.Media.SymbolEffect.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect
{
    public class AlphaBlendEffect : SymbolEffectBase
    {
        public Color Lighting { get; private set; }

        public AlphaBlendEffect(Color lighting)
        {
            this.Lighting = lighting;
        }

        public override Color ApplyFilter(Color inputColor)
        {
            return ColorFilter.AddLightingEffect(inputColor, this.Lighting);
        }
    }
}
