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
            return (Color)System.Windows.Media.ColorConverter.ConvertFromString(colorString);
        }
        public static string ConvertBack(Color color)
        {
            return color.ToString();
        }
        public static string Add(string colorString1, string colorString2)
        {
            var color1 = Convert(colorString1);
            var color2 = Convert(colorString2);
            
            return Color.Add(color1, color2).ToString();
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
