using Rogue.NET.Common.Extension;

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter
{
    public class EnumDisplayAttributeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            if (value == DependencyProperty.UnsetValue)
                return Binding.DoNothing;

            if (!value.GetType().IsEnum)
                throw new Exception("Enum must be specified for EnumDisplayAttributeNameConverter");

            return (value as Enum).GetAttribute<DisplayAttribute>().Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
