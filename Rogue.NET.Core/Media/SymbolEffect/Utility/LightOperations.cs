using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;

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
            return new Light()
            {
                // Color channels are weighted average of both lights
                Red = (byte)(int)(((light1.Red * light1.Intensity) + (light2.Red * light2.Intensity)) / (light1.Intensity + light2.Intensity)).Clip(0, 255),
                Green = (byte)(int)(((light1.Green * light1.Intensity) + (light2.Green * light2.Intensity)) / (light1.Intensity + light2.Intensity)).Clip(0, 255),
                Blue = (byte)(int)(((light1.Blue * light1.Intensity) + (light2.Blue * light2.Intensity)) / (light1.Intensity + light2.Intensity)).Clip(0, 255),

                // Intensity is addition of both - clipped at the ceiling
                Intensity = (light1.Intensity + light2.Intensity).Clip(0, 1)
            };
        }

        public static Color ApplyLightingEffect(Color baseColor, Light light)
        {
            // Alpha Blending:  https://en.wikipedia.org/wiki/Alpha_compositing#Alpha_blending
            // Blend Modes:     https://en.wikipedia.org/wiki/Blend_modes
            // Tinting:         https://softwarebydefault.com/2013/04/12/bitmap-color-tint/

            // Tinting:  
            //var red = baseColor.R + (((0xFF - baseColor.R) * (light.Red * light.Intensity)) / 255.0);
            //var green = baseColor.G + (((0xFF - baseColor.G) * (light.Green * light.Intensity)) / 255.0);
            //var blue = baseColor.B + (((0xFF - baseColor.B) * (light.Blue * light.Intensity)) / 255.0);

            // Screening
            //var red = 0xFF - (0xFF - baseColor.R) * (0xFF - light.Red);
            //var green = 0xFF - (0xFF - baseColor.G) * (0xFF - light.Green);
            //var blue = 0xFF - (0xFF - baseColor.B) * (0xFF - light.Blue);

            // Multiply (Shade)
            var red = (baseColor.R * (light.Red * light.Intensity)) / (255.0 * light.Intensity);
            var green = (baseColor.G * (light.Green * light.Intensity)) / (255.0 * light.Intensity);
            var blue = (baseColor.B * (light.Blue * light.Intensity)) / (255.0 * light.Intensity);

            // Weighted Average
            //var red = (baseColor.R + (light.Red * light.Intensity)) / (1 + light.Intensity);
            //var green = (baseColor.G + (light.Green * light.Intensity)) / (1 + light.Intensity);
            //var blue = (baseColor.B + (light.Blue * light.Intensity)) / (1 + light.Intensity);

            // Invent a "darkness" value that subtracts light to simulate darkening (NOTE*** Darkness scale is [-0.5, 0.5])
            var darkness = (1 - light.Intensity) / 2.0;

            // Create the color from the tinted value
            var color = Color.FromArgb(baseColor.A, (byte)(int)red, (byte)(int)green, (byte)(int)blue);

            return color;

            // Create a darkened color from the tinted color
            // return ColorOperations.ShiftHSL(color, 0, 0, -1 * darkness);
        }
    }
}
