﻿using System;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter.AttackAttribute
{
    public class AttackAttributeVibilityCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            if (value is int)
                return (int)value <= 0 ? Visibility.Collapsed : Visibility.Visible;

            return (double)value <= 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
