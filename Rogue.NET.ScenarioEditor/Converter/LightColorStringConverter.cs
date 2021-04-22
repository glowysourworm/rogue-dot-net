using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.Converter
{
    public class LightColorStringConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                value == DependencyProperty.UnsetValue)
                return Binding.DoNothing;

            var light = (LightTemplateViewModel)value;

            return Color.FromArgb(1, light.Red, light.Green, light.Blue).ToString();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                value == DependencyProperty.UnsetValue)
                return Binding.DoNothing;

            var color = ColorOperations.Convert(value.ToString());

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
