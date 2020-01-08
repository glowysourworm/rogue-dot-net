using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.Converter
{
    public class LightColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                value == DependencyProperty.UnsetValue)
                return Binding.DoNothing;

            var light = (LightTemplateViewModel)value;

            return Color.FromArgb(0xFF, light.Red, light.Green, light.Blue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                value == DependencyProperty.UnsetValue)
                return Binding.DoNothing;

            var color = (Color)value;

            return new LightTemplateViewModel()
            {
                Blue = color.B,
                Green = color.G,
                Red = color.R,
                Intensity = 1.0
            };
        }
    }
}
