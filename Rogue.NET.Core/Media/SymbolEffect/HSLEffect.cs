using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Media.SymbolEffect
{
    public class HSLEffect : SymbolEffectBase
    {
        /// <summary>
        /// Represents hue shift (-2*PI, 2*PI)
        /// </summary>
        public double Hue { get; private set; }

        /// <summary>
        /// Represents saturation shift [-1, 1]
        /// </summary>
        public double Saturation { get; private set; }

        /// <summary>
        /// Represents lightness shift [-1, 1]
        /// </summary>
        public double Lightness { get; private set; }

        /// <summary>
        /// Transforms only Red (#FF0000) 
        /// </summary>
        public bool UseColorMask { get; private set; }

        public HSLEffect(double hue, double saturation, double lightness, bool useColorMask)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Lightness = lightness;
            this.UseColorMask = useColorMask;
        }
    }
}
