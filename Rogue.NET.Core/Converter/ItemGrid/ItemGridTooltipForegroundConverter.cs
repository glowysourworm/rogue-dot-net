﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.Core.Converter.ItemGrid
{
    public class ItemGridTooltipForegroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 4 || values.All(x => x == DependencyProperty.UnsetValue))
                return Brushes.White;

            bool equiped = (bool)values[0];
            bool cursed = (bool)values[1];
            bool objective = (bool)values[2];
            bool unique = (bool)values[3];

            if (cursed)
                return equiped ? Brushes.Purple : Brushes.Red;

            else if (objective)
                return Brushes.Cyan;

            else if (unique)
                return Brushes.Goldenrod;

            else
                return Brushes.White;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}