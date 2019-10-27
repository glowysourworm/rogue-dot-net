using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Geometry
{
    public static class RectangularGridRegionGeometryCreator
    {
        static readonly IRandomSequenceGenerator _randomSequenceGenerator;

        static RectangularGridRegionGeometryCreator()
        {
            _randomSequenceGenerator = ServiceLocator.Current.GetInstance<IRandomSequenceGenerator>();
        }

        public static IEnumerable<RegionBoundary> CreateRegionGeometry(LayoutTemplate template)
        {
            var bounds = new RegionBoundary(new GridLocation(0, 0), template.Width, template.Height);
            var gridDivisionWidth = template.RoomWidthLimit + (2 * template.RectangularGridPadding);
            var gridDivisionHeight = template.RoomHeightLimit + (2 * template.RectangularGridPadding);

            var regions = new List<RegionBoundary>();

            // Create regions according to template parameters
            for (int i = 0; i < template.NumberRoomCols; i++)
            {
                var divisionColumn = i * gridDivisionWidth;

                for (int j = 0; j < template.NumberRoomRows; j++)
                {
                    var divisionRow = j * gridDivisionHeight;

                    // Draw a random region size
                    var regionWidth = _randomSequenceGenerator.Get(template.RoomWidthMin, template.RoomWidthLimit + 1);
                    var regionHeight = _randomSequenceGenerator.Get(template.RoomHeightMin, template.RoomHeightLimit + 1);

                    // Generate the upper left-hand corner for the region
                    var column = divisionColumn +
                                 _randomSequenceGenerator.Get(template.RectangularGridPadding,
                                                              gridDivisionWidth - (regionWidth + template.RectangularGridPadding) + 1);

                    var row = divisionRow +
                              _randomSequenceGenerator.Get(template.RectangularGridPadding,
                                                           gridDivisionHeight - (regionHeight + template.RectangularGridPadding) + 1);

                    // Call method to iterate grid to create region
                    //
                    var region = new RegionBoundary(new GridLocation(column, row), regionWidth, regionHeight);

                    regions.Add(region);
                }
            }

            return regions;
        }
    }
}
