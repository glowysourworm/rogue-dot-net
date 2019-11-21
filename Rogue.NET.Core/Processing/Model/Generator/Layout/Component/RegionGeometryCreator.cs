using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRegionGeometryCreator))]
    public class RegionGeometryCreator : IRegionGeometryCreator
    {
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        // Region padding - must be applied to the level grid
        const int PADDING = 1;

        [ImportingConstructor]
        public RegionGeometryCreator(IRandomSequenceGenerator randomSequenceGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public IEnumerable<RegionBoundary> CreateGridRectangularRegions(int width, int height, int numberRegionColumns, int numberRegionRows, int regionPadding, Range<int> regionWidthRange, Range<int> regionHeightRange)
        {
            var bounds = new RegionBoundary(new GridLocation(0, 0), width, height);
            var gridDivisionWidth = regionWidthRange.High + (2 * regionPadding);
            var gridDivisionHeight = regionHeightRange.High + (2 * regionPadding);

            var regions = new List<RegionBoundary>();

            // Create regions according to template parameters
            for (int i = 0; i < numberRegionColumns; i++)
            {
                var divisionColumn = i * gridDivisionWidth;

                for (int j = 0; j < numberRegionRows; j++)
                {
                    var divisionRow = j * gridDivisionHeight;

                    // Draw a random region size
                    var regionWidth = _randomSequenceGenerator.Get(regionWidthRange.Low, regionWidthRange.High + 1);
                    var regionHeight = _randomSequenceGenerator.Get(regionHeightRange.Low, regionHeightRange.High + 1);

                    // Generate the upper left-hand corner for the region
                    var column = divisionColumn +
                                 _randomSequenceGenerator.Get(regionPadding,
                                                              gridDivisionWidth - (regionWidth + regionPadding) + 1);

                    var row = divisionRow +
                              _randomSequenceGenerator.Get(regionPadding,
                                                           gridDivisionHeight - (regionHeight + regionPadding) + 1);

                    // Call method to iterate grid to create region
                    //
                    var region = new RegionBoundary(new GridLocation(column, row), regionWidth, regionHeight);

                    regions.Add(region);
                }
            }

            return regions;
        }

        public IEnumerable<RegionBoundary> CreateRandomRectangularRegions(int width, int height, int numberOfRegions, Range<int> regionWidthRange, Range<int> regionHeightRange, int regionSpread)
        {
            var bounds = new RegionBoundary(new GridLocation(0, 0), width, height);
            var regions = new List<RegionBoundary>();

            // Create region cells
            for (int i = 0; i < numberOfRegions; i++)
            {
                // First, generate the region height and width
                var regionWidth = _randomSequenceGenerator.Get(regionWidthRange.Low, regionWidthRange.High + 1);
                var regionHeight = _randomSequenceGenerator.Get(regionHeightRange.Low, regionHeightRange.High + 1);

                // Choose placement of region
                //
                var column = _randomSequenceGenerator.Get(PADDING, bounds.CellWidth - regionWidth - PADDING);
                var row = _randomSequenceGenerator.Get(PADDING, bounds.CellHeight - regionHeight - PADDING);

                regions.Add(new RegionBoundary(new GridLocation(column, row), regionWidth, regionHeight));
            }

            return regions;
        }

        public IEnumerable<IEnumerable<RegionBoundary>> GroupOverlappingBoundaries(IEnumerable<RegionBoundary> boundaries)
        {
            var result = new List<List<RegionBoundary>>();

            foreach (var boundary in boundaries)
            {
                // Any contiguous region boundaries will have already been
                // identified and added to one of the result lists
                if (result.Any(list => list.Contains(boundary)))
                    continue;

                // First, get the regions that overlap this region.
                var overlappingBoundaries = boundaries.Where(x => x != boundary && x.Touches(boundary))
                                                      .ToList();

                // Add the boundary in question to this list to start
                overlappingBoundaries.Add(boundary);

                IEnumerable<RegionBoundary> nextOverlappingBoundaries = null;

                do
                {
                    // While there are other boundaries to overlap with this subset - continue iterating
                    nextOverlappingBoundaries = boundaries.Where(x => overlappingBoundaries.Any(y => y.Touches(x) &&
                                                                     !overlappingBoundaries.Contains(x)));

                    if (nextOverlappingBoundaries.Any())
                        overlappingBoundaries.AddRange(nextOverlappingBoundaries);

                } while (nextOverlappingBoundaries.Any());

                // Add overlapping boundaries to the result
                result.Add(overlappingBoundaries);
            }

            if (result.Sum(x => x.Count) != boundaries.Count())
                throw new Exception("Improperly calculated contiguous region");

            return result;
        }
    }
}
