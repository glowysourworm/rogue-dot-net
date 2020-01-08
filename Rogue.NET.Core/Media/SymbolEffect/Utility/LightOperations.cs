using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect.Utility
{
    public static class LightOperations
    {
        /// <summary>
        /// Returns set of colors that are supported as lighting
        /// </summary>
        public static IEnumerable<Color> GetSupportedColors()
        {
            // https://www.1728.org/RGB3.htm
            return new Color[]
            {
                 // WHITE
                 Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF),

                 Color.FromArgb(0xFF, 0xFF, 0x00, 0x00),
                 Color.FromArgb(0xFF, 0xFF, 0x20, 0x00),
                 Color.FromArgb(0xFF, 0xFF, 0x40, 0x00),
                 Color.FromArgb(0xFF, 0xFF, 0x60, 0x00),
                 Color.FromArgb(0xFF, 0xFF, 0x80, 0x00),
                 Color.FromArgb(0xFF, 0xFF, 0xAA, 0x00),
                 Color.FromArgb(0xFF, 0xFF, 0xBF, 0x00),
                 Color.FromArgb(0xFF, 0xFF, 0xEE, 0x00),
                 Color.FromArgb(0xFF, 0xFF, 0xFF, 0x00),
                 Color.FromArgb(0xFF, 0xDD, 0xFF, 0x00),
                 Color.FromArgb(0xFF, 0xCC, 0xFF, 0x00),
                 Color.FromArgb(0xFF, 0xAA, 0xFF, 0x00),
                 Color.FromArgb(0xFF, 0x80, 0xFF, 0x00),
                 Color.FromArgb(0xFF, 0x60, 0xFF, 0x00),
                 Color.FromArgb(0xFF, 0x40, 0xFF, 0x00),
                 Color.FromArgb(0xFF, 0x20, 0xFF, 0x00),
                 Color.FromArgb(0xFF, 0x00, 0xFF, 0x00),
                 Color.FromArgb(0xFF, 0x00, 0xFF, 0x20),
                 Color.FromArgb(0xFF, 0x00, 0xFF, 0x40),
                 Color.FromArgb(0xFF, 0x00, 0xFF, 0x60),
                 Color.FromArgb(0xFF, 0x00, 0xFF, 0x80),
                 Color.FromArgb(0xFF, 0x00, 0xFF, 0xAA),
                 Color.FromArgb(0xFF, 0x00, 0xFF, 0xCC),
                 Color.FromArgb(0xFF, 0x00, 0xFF, 0xDD),
                 Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF),
                 Color.FromArgb(0xFF, 0x00, 0xDD, 0xFF),
                 Color.FromArgb(0xFF, 0x00, 0xCC, 0xFF),
                 Color.FromArgb(0xFF, 0x00, 0x99, 0xFF),
                 Color.FromArgb(0xFF, 0x00, 0x80, 0xFF),
                 Color.FromArgb(0xFF, 0x00, 0x60, 0xFF),
                 Color.FromArgb(0xFF, 0x00, 0x40, 0xFF),
                 Color.FromArgb(0xFF, 0x00, 0x20, 0xFF),
                 Color.FromArgb(0xFF, 0x00, 0x00, 0xFF),
                 Color.FromArgb(0xFF, 0x20, 0x00, 0xFF),
                 Color.FromArgb(0xFF, 0x40, 0x00, 0xFF),
                 Color.FromArgb(0xFF, 0x60, 0x00, 0xFF),
                 Color.FromArgb(0xFF, 0x80, 0x00, 0xFF),
                 Color.FromArgb(0xFF, 0xAA, 0x00, 0xFF),
                 Color.FromArgb(0xFF, 0xCC, 0x00, 0xFF),
                 Color.FromArgb(0xFF, 0xDD, 0x00, 0xFF),
                 Color.FromArgb(0xFF, 0xFF, 0x00, 0xFF),
                 Color.FromArgb(0xFF, 0xFF, 0x00, 0xEE),
                 Color.FromArgb(0xFF, 0xFF, 0x00, 0xCC),
                 Color.FromArgb(0xFF, 0xFF, 0x00, 0xAA),
                 Color.FromArgb(0xFF, 0xFF, 0x00, 0x80),
                 Color.FromArgb(0xFF, 0xFF, 0x00, 0x60),
                 Color.FromArgb(0xFF, 0xFF, 0x00, 0x40),
                 Color.FromArgb(0xFF, 0xFF, 0x00, 0x20)
            };
        }

        public static Light CombineLight(Light light1, Light light2)
        {
            var color1 = light1.ToColor();
            var color2 = light2.ToColor();

            var hslBase = HslColor.FromColor(color1);

            // Weighted Average
            var red = ((color1.R * light1.Intensity) + (color2.R * light2.Intensity)) / (light1.Intensity + light2.Intensity);
            var green = ((color1.G * light1.Intensity) + (color2.G * light2.Intensity)) / (light1.Intensity + light2.Intensity);
            var blue = ((color1.B * light1.Intensity) + (color2.B * light2.Intensity)) / (light1.Intensity + light2.Intensity);

            // Create BRIGHTENED color 
            var color = Color.FromArgb(0xFF, (byte)(int)red, (byte)(int)green, (byte)(int)blue);

            // Switch to HSL to calculate lightness change
            var hsl = HslColor.FromColor(color);

            // Lighten (or) Darken the image based on the lightness shift
            var lightnessChange = hsl.L - hslBase.L;

            var result = hsl.Lighten(lightnessChange * -1).ToColor();

            return new Light(result, (light1.Intensity + light2.Intensity) / 2.0);

            //return new Light()
            //{
            //    // Color channels are weighted average of both lights
            //    Red = (byte)(int)(((light1.Red * light1.Intensity) + (light2.Red * light2.Intensity)) / (light1.Intensity + light2.Intensity)).Clip(0, 255),
            //    Green = (byte)(int)(((light1.Green * light1.Intensity) + (light2.Green * light2.Intensity)) / (light1.Intensity + light2.Intensity)).Clip(0, 255),
            //    Blue = (byte)(int)(((light1.Blue * light1.Intensity) + (light2.Blue * light2.Intensity)) / (light1.Intensity + light2.Intensity)).Clip(0, 255),

            //    // Intensity is addition of both - clipped at the ceiling
            //    Intensity = (light1.Intensity + light2.Intensity).Clip(0, 1)
            //};
        }

        public static Color ApplyLightingEffect(Color baseColor, Light light)
        {
            // Alpha Blending:  https://en.wikipedia.org/wiki/Alpha_compositing#Alpha_blending
            // Blend Modes:     https://en.wikipedia.org/wiki/Blend_modes
            // Tinting:         https://softwarebydefault.com/2013/04/12/bitmap-color-tint/

            // Tried Methods:  Shading, Screening, Tinting, HSL darkening, and any combination of these...

            // The best method has been to use weighted averages to "Bring out" color channels - but need
            // to somehow normalize the lightness - which the HslColor does REALLY well.
            //
            var baseHsl = HslColor.FromColor(baseColor);

            // Weighted Average
            var red = (baseColor.R + (light.Red * light.Intensity)) / (1 + light.Intensity);
            var green = (baseColor.G + (light.Green * light.Intensity)) / (1 + light.Intensity);
            var blue = (baseColor.B + (light.Blue * light.Intensity)) / (1 + light.Intensity);

            // Create BRIGHTENED color 
            var color = Color.FromArgb(baseColor.A, (byte)(int)red, (byte)(int)green, (byte)(int)blue);

            // Switch to HSL to calculate lightness change
            var hsl = HslColor.FromColor(color);

            // Lighten (or) Darken the image based on the lightness shift
            var lightnessChange = hsl.L - baseHsl.L;

            return hsl.Lighten(lightnessChange * -1).ToColor();
        }

        public static Color ApplyLightIntensity(Color color, double intensity)
        {
            if (!intensity.Between(0, 1, true))
                throw new ArgumentException("Invalid light intensity LightOperations.ApplyLightIntensity");

            var hslColor = HslColor.FromColor(color);

            // Going to try scaling the lightness
            hslColor.L *= intensity;

            return hslColor.ToColor();
        }
    }
}
