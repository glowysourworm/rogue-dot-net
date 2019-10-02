﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.Core.Converter.ItemGrid
{
    public class ItemGridForegroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 4 || values.Any(x => x == DependencyProperty.UnsetValue))
                return Brushes.White;

            bool equiped = (bool)values[0];
            bool cursed = (bool)values[1];
            bool objective = (bool)values[2];
            bool unique = (bool)values[3];

            if (objective)
                return Brushes.Cyan;

            if (unique)
                return Brushes.Goldenrod;

            if (cursed)
                return Brushes.Red;

            if (equiped)
                return Brushes.GreenYellow;

            else
                return Brushes.White;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
