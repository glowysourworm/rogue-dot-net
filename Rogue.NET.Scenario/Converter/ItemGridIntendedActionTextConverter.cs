using Rogue.NET.Core.Utility;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Rogue.NET.Scenario.Converter
{
    public class ItemGridIntendedActionTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            return TextUtility.CamelCaseToTitleCase(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
