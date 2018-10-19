﻿using System;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Common.Converters
{
    public class BoolInverseVisibilityHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(bool)value)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((Visibility)value)
            {
                case Visibility.Collapsed:
                case Visibility.Hidden:
                    return false;
                case Visibility.Visible:
                    return true;
            }
            return false;
        }
    }
}
