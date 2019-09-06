using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.DialogMode;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Scenario.Converter
{
    // IValueConverter that converts Item Grid View Model type to visibility. 
    // Collapsed for multi-selection mode / Visible for single-selection mode
    public class ItemGridViewModelTypeMultipleSelectionVisibilityInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                value == DependencyProperty.UnsetValue)
                return Binding.DoNothing;

            if (value is EquipmentTransmuteItemGridViewModel)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
