using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect
{
    public class LightingEffect : SymbolEffectBase
    {
        public Light Lighting { get; private set; }

        public LightingEffect(Light lighting)
        {
            this.Lighting = lighting;
        }

        public override Color ApplyFilter(Color inputColor)
        {
            return LightOperations.ApplyLightingEffect(inputColor, this.Lighting);
        }
    }
}
