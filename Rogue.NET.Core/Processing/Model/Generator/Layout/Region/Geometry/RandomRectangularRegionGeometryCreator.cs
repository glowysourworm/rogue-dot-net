using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Geometry
{
    public static class RandomRectangularRegionGeometryCreator
    {
        static readonly IRandomSequenceGenerator _randomSequenceGenerator;

        // Region padding - must be applied to the level grid
        const int PADDING = 1;

        static RandomRectangularRegionGeometryCreator()
        {
            _randomSequenceGenerator = ServiceLocator.Current.GetInstance<IRandomSequenceGenerator>();
        }

        public static IEnumerable<RegionBoundary> CreateRegionGeometry(LayoutTemplate template)
        {
            var bounds = new RegionBoundary(new GridLocation(0, 0), template.Width, template.Height);
            var regions = new List<RegionBoundary>();

            // Create region cells
            for (int i = 0; i < template.RandomRoomCount; i++)
            {
                // First, generate the region height and width
                var regionWidth = _randomSequenceGenerator.Get(template.RoomWidthMin, template.RoomWidthLimit + 1);
                var regionHeight = _randomSequenceGenerator.Get(template.RoomHeightMin, template.RoomHeightLimit + 1);

                // Choose placement of region
                //
                var column = _randomSequenceGenerator.Get(PADDING, bounds.CellWidth - regionWidth - PADDING);
                var row = _randomSequenceGenerator.Get(PADDING, bounds.CellHeight - regionHeight - PADDING);

                regions.Add(new RegionBoundary(new GridLocation(column, row), regionWidth, regionHeight));
            }

            return regions;
        }
    }
}
