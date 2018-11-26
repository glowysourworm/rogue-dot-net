using Rogue.NET.Scenario.Content.ViewModel.Content;
using System;
using System.Linq;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Rogue.NET.Scenario.Converter
{
    public class SkillSetProgressConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Binding.DoNothing;

            else if (values.Any(x => x == DependencyProperty.UnsetValue))
                return Binding.DoNothing;

            else if (values.Length != 3)
                return Binding.DoNothing;

            var level = (int)values[0];
            var levelMax = (int)values[1];
            var progress = (double)values[2];

            return level < levelMax ? progress : 1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
