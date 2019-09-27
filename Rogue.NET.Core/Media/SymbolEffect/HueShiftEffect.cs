using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Media.SymbolEffect
{
    public class HueShiftEffect : SymbolEffectBase
    {
        public double Radians { get; private set; }
        public HueShiftEffect(double radians)
        {
            this.Radians = radians;
        }
    }
}
