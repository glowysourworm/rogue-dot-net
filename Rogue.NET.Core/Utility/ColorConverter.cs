using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Rogue.NET.Core.Utility
{
    public static class ColorConverter
    {
        public static Color Convert(string colorString)
        {
            return (Color)System.Windows.Media.ColorConverter.ConvertFromString(colorString);
        }

        public static string Add(string colorString1, string colorString2)
        {
            var color1 = Convert(colorString1);
            var color2 = Convert(colorString2);
            
            return Color.Add(color1, color2).ToString();
        }
    }
}
