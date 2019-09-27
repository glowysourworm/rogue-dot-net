using Rogue.NET.Common.Extension;

namespace Rogue.NET.Core.Media.SymbolEffect
{
    public class SaturationEffect : SymbolEffectBase
    {
        public double Saturation { get; private set; }
        public SaturationEffect(double saturation)
        {
            this.Saturation = saturation.Clip(0,1);
        }
    }
}
