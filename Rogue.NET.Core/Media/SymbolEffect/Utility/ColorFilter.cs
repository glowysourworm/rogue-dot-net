using Rogue.NET.Common.ViewModel;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System;
using Rogue.NET.Common.Extension;
using System.ComponentModel;
using Rogue.NET.Core.Model;

namespace Rogue.NET.Core.Media.SymbolEffect.Utility
{
    public static class ColorFilter
    {
        public static Color Convert(string colorString)
        {
            return (Color)ColorConverter.ConvertFromString(colorString);
        }
        public static string ConvertBack(Color color)
        {
            return color.ToString();
        }
        public static Color Inverse(Color color)
        {
            var result = new Color();
            result.A = System.Convert.ToByte(255);
            result.R = System.Convert.ToByte(255 - color.R);
            result.G = System.Convert.ToByte(255 - color.G);
            result.B = System.Convert.ToByte(255 - color.B);

            return result;
        }
        public static string Add(string colorString1, string colorString2)
        {
            var color1 = Convert(colorString1);
            var color2 = Convert(colorString2);

            var result = new Color();
            result.A = System.Convert.ToByte(System.Math.Min((color1.A + color2.A) / 2.0D, 255));
            result.R = System.Convert.ToByte(System.Math.Min((color1.R + color2.R) / 2.0D, 255));
            result.G = System.Convert.ToByte(System.Math.Min((color1.G + color2.G) / 2.0D, 255));
            result.B = System.Convert.ToByte(System.Math.Min((color1.B + color2.B) / 2.0D, 255));

            return result.ToString();
        }
        
        /// <summary>
        /// Uses a tinting effect to apply lighting to a base color
        /// </summary>
        public static Color AddLightingEffect(Color baseColor, Color light, bool discretize = true)
        {
            // Alpha Blending:  https://en.wikipedia.org/wiki/Alpha_compositing#Alpha_blending
            // Blend Modes:     https://en.wikipedia.org/wiki/Blend_modes
            // Tinting:         https://softwarebydefault.com/2013/04/12/bitmap-color-tint/

            //// Tinting:  
            //var red = baseColor.R + (((0xFF - baseColor.R) * light.R) / 255.0);
            //var green = baseColor.G + (((0xFF - baseColor.G) * light.G) / 255.0);
            //var blue = baseColor.B + (((0xFF - baseColor.B) * light.B) / 255.0);

            //// Invent a "darkness" value that subtracts light to simulate darkening
            //var darkness = (1 - (light.A / 255.0)).Clip(0, 0.4);

            //// Create the color from the tinted value
            //var lightedColor = Color.FromArgb(baseColor.A, (byte)(int)red, (byte)(int)green, (byte)(int)blue);

            //// Create a darkened color from the tinted color
            //var darkenedColor = ShiftHSL(lightedColor, 0, 0, -1 * darkness);

            //// Discretize the output to save cache space
            //return Discretize(darkenedColor, ModelConstants.ColorChannelDiscretization);

            // TODO:TERRAIN - BYPASSING FILTER
            return baseColor;
        }
        public static IEnumerable<ColorViewModel> CreateColors()
        {
            var colorProperties = typeof(Colors).GetProperties();
            var brushProperties = typeof(Brushes).GetProperties();

            return colorProperties.Select(colorProperty =>
            {
                var matchingBrushProperty = brushProperties.First(x => x.Name == colorProperty.Name);
                var color = (Color)colorProperty.GetValue(typeof(Colors));

                return new ColorViewModel()
                {
                    Name = colorProperty.Name,
                    Color = color,
                    Brush = (Brush)matchingBrushProperty.GetValue(typeof(Brushes)),
                    ColorString = color.ToString()
                };
            });
        }

        public static Color ShiftHSL(Color color, double hueRadians, double saturation, double lightness)
        {
            var huePrime = CalculateHue(color) + hueRadians;
            var saturationPrime = CalculateSaturation(color) + saturation;
            var lightnessPrime = CalculateLightness(color) + lightness;

            // Clip values
            huePrime = huePrime % (System.Math.PI * 2);
            saturationPrime = saturationPrime.Clip(0, 1);
            lightnessPrime = lightnessPrime.Clip(0, 1);

            return FromHSL(color.A / 255.0, huePrime, saturationPrime, lightnessPrime);
        }

        // https://en.wikipedia.org/wiki/HSL_and_HSV
        public static Color FromHSL(double opacity, double hue, double saturation, double lightness)
        {
            // Recover RGB from H'SL using the function (Described in the wikipedia entry)
            //

            var alpha = System.Convert.ToByte(opacity * 255);
            var rPrime = FromHSLSub(0.0, hue, saturation, lightness);
            var gPrime = FromHSLSub(8.0, hue, saturation, lightness);
            var bPrime = FromHSLSub(4.0, hue, saturation, lightness);

            return Color.FromArgb(alpha, rPrime, gPrime, bPrime);
        }

        private static byte FromHSLSub(double n, double hue, double saturation, double lightness)
        {
            var k = (n + ((hue * 6.0) / System.Math.PI)) % 12.0;
            var a = saturation * System.Math.Min(lightness, 1 - lightness);

            var result = lightness - (a * System.Math.Max(System.Math.Min(System.Math.Min(k - 3.0, 9.0 - k), 1.0), -1.0));

            // Go ahead and convert back to a byte [0, 1) -> [0, 256)

            return System.Convert.ToByte(result * 255);
        }

        public static double CalculateHue(Color color)
        {
            var r = System.Convert.ToInt32(color.R) / 255.0D;
            var g = System.Convert.ToInt32(color.G) / 255.0D;
            var b = System.Convert.ToInt32(color.B) / 255.0D;

            var max = System.Math.Max(System.Math.Max(r, g), b);
            var min = System.Math.Min(System.Math.Min(r, g), b);

            return    (System.Math.PI / 3.0) * (max == min ? 0.0 :
                                         max == r ? (0.0 + ((g - b) / (max - min))) :
                                         max == g ? (2.0 + ((b - r) / (max - min))) :
                                         max == b ? (4.0 + ((r - g) / (max - min))) : 0.0);
        }
        public static double CalculateLightness(Color color)
        {
            var r = System.Convert.ToInt32(color.R) / 255.0D;
            var g = System.Convert.ToInt32(color.G) / 255.0D;
            var b = System.Convert.ToInt32(color.B) / 255.0D;

            var max = System.Math.Max(System.Math.Max(r, g), b);
            var min = System.Math.Min(System.Math.Min(r, g), b);

            return (max + min) / 2.0D;
        }
        public static double CalculateSaturation(Color color)
        {
            var r = System.Convert.ToInt32(color.R) / 255.0D;
            var g = System.Convert.ToInt32(color.G) / 255.0D;
            var b = System.Convert.ToInt32(color.B) / 255.0D;

            var max = System.Math.Max(System.Math.Max(r, g), b);
            var min = System.Math.Min(System.Math.Min(r, g), b);

            // var lightness = CalculateLightness(color);

            var lightness = (max + min) / 2.0;

            return max == 0 ? 0 :
                   min == 1 ? 0 :
                 ((max - lightness) / (System.Math.Min(lightness, 1 - lightness)));
        }

        /// <summary>
        /// Creates a step-function discretization of the alpha channel to create opacity "levels"
        /// </summary>
        /// <param name="color">Input color</param>
        /// <param name="numberOfLevels">Number of levels in the digitization</param>
        /// <returns>A filtered color with an alpha channel that has been set to a particular discrete value</returns>
        public static Color Discretize(Color color, int numberOfLevels)
        {
            if (numberOfLevels < 2)
                throw new ArgumentException("ColorFilter.Digitize must have a number of levels greater than or equal to two");

            // Divide the alpha channel (byte) into a discrete set of levels - each with the scaled size
            var channelSize = 256.0 / numberOfLevels;

            var alpha = ((int)(color.A / channelSize)) * channelSize;
            var red   = ((int)(color.R / channelSize)) * channelSize;
            var green = ((int)(color.G / channelSize)) * channelSize;
            var blue  = ((int)(color.B / channelSize)) * channelSize;

            return Color.FromArgb((byte)(int)alpha, (byte)(int)red, (byte)(int)green, (byte)(int)blue);
        }
    }
}
