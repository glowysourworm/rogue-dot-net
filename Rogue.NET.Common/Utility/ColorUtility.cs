using Rogue.NET.Common.ViewModel;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;

namespace Rogue.NET.Core.Utility
{
    public static class ColorUtility
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
            result.A = System.Convert.ToByte(color1.A + color2.A / 2.0D);
            result.R = System.Convert.ToByte(color1.R + color2.R / 2.0D);
            result.G = System.Convert.ToByte(color1.G + color2.G / 2.0D);
            result.B = System.Convert.ToByte(color1.B + color2.B / 2.0D);

            return result.ToString();
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
    }
}
