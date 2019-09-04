using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Converter
{
    public class CharacterBaseAttributeForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                value == DependencyProperty.UnsetValue)
                return Binding.DoNothing;

            var baseAttribute = (CharacterBaseAttribute)value;

            switch (baseAttribute)
            {
                case CharacterBaseAttribute.Strength:
                    return Brushes.Tan;
                case CharacterBaseAttribute.Agility:
                    return Brushes.SpringGreen;
                case CharacterBaseAttribute.Intelligence:
                    return Brushes.CadetBlue;
                default:
                    return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
