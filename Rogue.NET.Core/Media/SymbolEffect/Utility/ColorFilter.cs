﻿using Rogue.NET.Common.ViewModel;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System;
using Rogue.NET.Common.Extension;
using System.ComponentModel;

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
        
        public static Color AddUsingAlphaChannels(Color front, Color back)
        {
            // https://stackoverflow.com/questions/726549/algorithm-for-additive-color-mixing-for-rgb-values
            // http://blog.johnnovak.net/2016/09/21/what-every-coder-should-know-about-gamma/#fn:osgamma

            //same as Markus Jarderot's answer
            var alpha = (0xFF - (0xFF - back.A) * (0xFF - front.A));
            var red = ((front.R * front.A) + ((back.R * back.A) * (0xFF - front.A))) / (double)alpha;
            var green = ((front.G * front.A) + ((back.G * back.A) * (0xFF - front.A))) / (double)alpha;
            var blue = ((front.B * front.A) + ((back.B * back.A) * (0xFF - front.A))) / (double)alpha;

            return Color.FromArgb((byte)alpha, (byte)(int)red, (byte)(int)green, (byte)(int)blue);
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
    }
}
