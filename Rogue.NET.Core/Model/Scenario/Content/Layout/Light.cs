using System;

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

        /// <summary>
        /// Simulates no light - effectively RGB black with zero intensity.
        /// </summary>
        public static Light None = new Light(0x00, 0x00, 0x00, 0.0);

        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public double Intensity { get; set; }

        public Light()
        {
            this.Red = 0x00;
            this.Green = 0x00;
            this.Blue = 0x00;
            this.Intensity = 0.0;
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
