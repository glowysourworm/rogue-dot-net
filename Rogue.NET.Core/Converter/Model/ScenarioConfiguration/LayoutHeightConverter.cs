using Rogue.NET.Core.Model.Enums;
using System;
using System.Linq;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;

namespace Rogue.NET.Core.Converter.Model.ScenarioConfiguration
{
    public class LayoutHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 8 || values.Any(x => x == DependencyProperty.UnsetValue))
                return Binding.DoNothing;

            var type = (LayoutType)values[0];
            var roomPlacementType = (LayoutRoomPlacementType)values[1];
            var roomHeightLimit = (int)values[2];
            var rectangularGridPadding = (int)values[3];
            var numberRoomRows = (int)values[4];
            var randomRoomCount = (int)values[5];
            var randomRoomSpread = (int)values[6];
            var height = (int)values[7];

            switch (type)
            {
                case LayoutType.Maze:
                case LayoutType.ConnectedCellularAutomata:
                    return height;
            }

            switch (roomPlacementType)
            {
                case LayoutRoomPlacementType.RectangularGrid:
                    return numberRoomRows * (roomHeightLimit + (2 * rectangularGridPadding));
                case LayoutRoomPlacementType.Random:
                    return roomHeightLimit + (randomRoomSpread * 2);
                default:
                    return height;
            }
        }

        public int Convert(LayoutTemplate template)
        {
            return (int)Convert(new object[]
            {
                template.Type,
                template.RoomPlacementType,
                template.RoomHeightLimit,
                template.RectangularGridPadding,
                template.NumberRoomRows,
                template.RandomRoomCount,
                template.RandomRoomSpread,
                template.Height
            }, null, null, null);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Change the height only when updating the control's value
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
