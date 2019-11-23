using Rogue.NET.Core.Model.Enums;
using System;
using System.Linq;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Rogue.NET.ScenarioEditor.Converter
{
    public class LayoutConnectionGeometryVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO:TERRAIN
            //if (values == null || values.Length != 2 || values.Any(x => x == DependencyProperty.UnsetValue))
            //    return Binding.DoNothing;

            //var type = (LayoutType)values[0];
            //var roomConnectionType = (LayoutConnectionType)values[1];

            //return ((type == LayoutType.ConnectedRectangularRooms ||
            //         type == LayoutType.ConnectedCellularAutomata) &&
            //       (roomConnectionType == LayoutConnectionType.Corridor) ||
            //       (roomConnectionType == LayoutConnectionType.CorridorWithDoors)) ? Visibility.Visible : Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
