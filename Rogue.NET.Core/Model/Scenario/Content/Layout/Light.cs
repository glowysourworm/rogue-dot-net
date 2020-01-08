using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class Light
    {
        /// <summary>
        /// Transparent lighting will not affect the color channels of the object being colored. This was
        /// chosen as the default lighting to provide "white light" (which doesn't bias the colors). The intensity
        /// WILL affect the output color channels to darken the image (if it is less than 1.0).
        /// </summary>
        public static Light White = new Light(0xFF, 0xFF, 0xFF, 1.0);
        public static Light WhiteExplored = new Light(0xFF, 0xFF, 0xFF, ModelConstants.MinLightIntensity);
        public static Light WhiteRevealed = new Light(0xFF, 0xFF, 0xFF, ModelConstants.MaxLightIntensity);

        /// <summary>
        /// Simulates no light - effectively RGB black with zero intensity.
        /// </summary>
        public static Light None = new Light(0x00, 0x00, 0x00, 0.0);

        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public double Intensity { get; set; }

        public Color ToColor()
        {
            return Color.FromArgb(0xFF, this.Red, this.Green, this.Blue);
        }

        public override int GetHashCode()
        {
            var hash = 17;

            hash = (hash * 397) ^ this.Red.GetHashCode();
            hash = (hash * 397) ^ this.Green.GetHashCode();
            hash = (hash * 397) ^ this.Blue.GetHashCode();
            hash = (hash * 397) ^ this.Intensity.GetHashCode();

            return hash;
        }

        public Light()
        {
            this.Red = 0x00;
            this.Green = 0x00;
            this.Blue = 0x00;
            this.Intensity = 0.0;
        }
        public Light(Color color, double intensity)
        {
            this.Red = color.R;
            this.Green = color.G;
            this.Blue = color.B;
            this.Intensity = intensity;
        }
        public Light(Light light, double newIntensity)
        {
            this.Red = light.Red;
            this.Green = light.Green;
            this.Blue = light.Blue;
            this.Intensity = newIntensity;
        }
        public Light(byte red, byte green, byte blue, double intensity)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.Intensity = intensity;
        }
    }
}
