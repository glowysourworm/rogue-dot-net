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
        /*
            <Binding Path="Height" />
            <Binding Path="Width" />                                    
            <Binding Path="Type" />
            <Binding Path="SymmetryType" />
            <Binding Path="RegionWidthRange.Low" />
            <Binding Path="RegionWidthRange.High" />
            <Binding Path="RegionHeightRange.Low" />
            <Binding Path="RegionHeightRange.High" />
            <Binding Path="NumberRoomCols" />
            <Binding Path="NumberRoomRows" />
            <Binding Path="RectangularGridPadding" />
            <Binding Path="GaussianRoomSpread" />
            <Binding Path="RandomRoomCount" />
            <Binding Path="RandomRoomSpread" />
            <Binding Path="MakeSymmetric" />
        */

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 14 || values.Any(x => x == DependencyProperty.UnsetValue))
                return Binding.DoNothing;

            var height = (int)values[0];
            var width = (int)values[1];
            var type = (LayoutType)values[2];
            var symmetryType = (LayoutSymmetryType)values[3];
            var regionWidthLow = (int)values[4];
            var regionWidthHigh = (int)values[5];
            var regionHeightLow = (int)values[6];
            var regionHeightHigh = (int)values[7];
            var numberRoomCols = (int)values[8];
            var numberRoomRows = (int)values[9];
            var rectangularGridPadding = (int)values[10];
            var randomRoomCount = (int)values[11];
            var randomRoomSpread = (int)values[12];
            var makeSymmetric = (bool)values[13];

            // Symmetry Multiplier:  Generate level 2x or 4x
            var symmetryMultiplier = 1;

            switch (symmetryType)
            {
                case LayoutSymmetryType.LeftRight:
                    symmetryMultiplier = 1;
                    break;
                case LayoutSymmetryType.Quadrant:
                    symmetryMultiplier = makeSymmetric ? 2 : 1;
                    break;
                default:
                    break;
            }

            switch (type)
            {
                case LayoutType.RectangularRegion:
                    return symmetryMultiplier * (numberRoomRows * (regionHeightHigh + (2 * rectangularGridPadding)));
                case LayoutType.RandomRectangularRegion:
                    return symmetryMultiplier * (regionHeightHigh + (2 * randomRoomSpread));
                case LayoutType.MazeMap:
                case LayoutType.ElevationMap:
                case LayoutType.CellularAutomataMap:
                default:
                    return height;
            }
        }

        public int Convert(LayoutTemplate template)
        {
            return (int)Convert(new object[]
            {
                template.Height,
                template.Width,
                template.Type,
                template.SymmetryType,
                template.RegionWidthRange.Low,
                template.RegionWidthRange.High,
                template.RegionHeightRange.Low,
                template.RegionHeightRange.High,
                template.NumberRoomCols,
                template.NumberRoomRows,
                template.RectangularGridPadding,
                template.RandomRoomCount,
                template.RandomRoomSpread,
                template.MakeSymmetric
            }, null, null, null);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Change the width only when updating the control's value
            return new object[] {value,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing,
                                 Binding.DoNothing};
        }
    }
}
