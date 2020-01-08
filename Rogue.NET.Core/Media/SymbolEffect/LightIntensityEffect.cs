using Rogue.NET.Core.Media.SymbolEffect.Utility;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect
{
    public class LightIntensityEffect : SymbolEffectBase
    {
        public double Intensity { get; private set; }

        public LightIntensityEffect(double intensity)
        {
            this.Intensity = intensity;
        }

        public override Color ApplyFilter(Color inputColor)
        {
            return LightOperations.ApplyLightIntensity(inputColor, this.Intensity);
        }
    }
}
