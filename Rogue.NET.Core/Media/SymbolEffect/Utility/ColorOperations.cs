using Rogue.NET.Common.Extension;
using System.Windows.Media;

namespace Rogue.NET.Core.Media.SymbolEffect.Utility
{
    public static class ColorOperations
    {
        public static Color Convert(string colorString)
        {
            return (Color)ColorConverter.ConvertFromString(colorString);
        }

        public static string ConvertBack(Color color)
        {
            return color.ToString();
        }

        public static string Add(string colorString1, string colorString2)
        {
            var color1 = ColorOperations.Convert(colorString1);
            var color2 = ColorOperations.Convert(colorString2);

            var result = new Color();
            result.A = System.Convert.ToByte(System.Math.Min((color1.A + color2.A) / 2.0D, 255));
            result.R = System.Convert.ToByte(System.Math.Min((color1.R + color2.R) / 2.0D, 255));
            result.G = System.Convert.ToByte(System.Math.Min((color1.G + color2.G) / 2.0D, 255));
            result.B = System.Convert.ToByte(System.Math.Min((color1.B + color2.B) / 2.0D, 255));

            return result.ToString();
        }

        public static Color ShiftHSL(Color color, double hueRadians, double saturation, double lightness)
        {
            var hslColor = HslColor.FromColor(color);

            hslColor.H = (hslColor.H + ((180.0 / System.Math.PI) * hueRadians)).Clip(0, 360);
            hslColor.S = (hslColor.S + saturation).Clip(0, 1);
            hslColor.L = (hslColor.L + lightness).Clip(0, 1);

            return hslColor.ToColor();
        }
    }
}
