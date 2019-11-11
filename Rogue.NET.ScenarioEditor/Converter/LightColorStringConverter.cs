using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.Converter
{
    public class LightColorStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var light = value as LightTemplateViewModel;
            if (light == null)
                return Binding.DoNothing;

            var color = Color.FromArgb(0xFF, light.Red, light.Green, light.Blue);

            return ColorFilter.ConvertBack(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorString = value as string;
            if (string.IsNullOrEmpty(colorString))
                return Binding.DoNothing;

            var color = ColorFilter.Convert(colorString);

            return new LightTemplateViewModel()
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B,
                Intensity = 1.0
            };
        }
    }
}
