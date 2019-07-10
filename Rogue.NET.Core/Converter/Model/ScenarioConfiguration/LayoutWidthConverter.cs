using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter.Model.ScenarioConfiguration
{
    public class LayoutWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 8 || values.Any(x => x == DependencyProperty.UnsetValue))
                return Binding.DoNothing;

            var type = (LayoutType)values[0];
            var roomPlacementType = (LayoutRoomPlacementType)values[1];
            var roomWidthLimit = (int)values[2];
            var rectangularGridPadding = (int)values[3];
            var numberRoomCols = (int)values[4];
            var randomRoomCount = (int)values[5];
            var randomRoomSpread = (int)values[6];
            var width = (int)values[7];

            switch (type)
            {
                case LayoutType.Maze:
                case LayoutType.ConnectedCellularAutomata:
                    return width;
            }

            switch (roomPlacementType)
            {
                case LayoutRoomPlacementType.RectangularGrid:
                    return numberRoomCols * (roomWidthLimit + (2 * rectangularGridPadding));
                case LayoutRoomPlacementType.Random:
                    return roomWidthLimit + (2 * randomRoomSpread);
                default:
                    return width;
            }
        }

        public int Convert(LayoutTemplate template)
        {
            return (int)Convert(new object[]
            {
                template.Type,
                template.RoomPlacementType,
                template.RoomWidthLimit,
                template.RectangularGridPadding,
                template.NumberRoomCols,
                template.RandomRoomCount,
                template.RandomRoomSpread,
                template.Width
            }, null, null, null);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Change the width only when updating the control's value
            return new object[] {Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 value};
        }
    }
}
