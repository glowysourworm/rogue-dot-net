using Rogue.NET.Common.Extension;
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
        const int PADDING = 2;

        [ImportingConstructor]
        public RegionGeometryCreator(IRandomSequenceGenerator randomSequenceGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public IEnumerable<RegionBoundary> CreateGridRectangularRegions(int numberRegionColumns, int numberRegionRows, int regionPadding, Range<int> regionWidthRange, Range<int> regionHeightRange)
        {
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

        public IEnumerable<RegionBoundary> CreateRandomRectangularRegions(RegionBoundary placementBoundary, int numberOfRegions, Range<int> regionWidthRange, Range<int> regionHeightRange, int regionSpread)
        {
            var regions = new List<RegionBoundary>();

            // Create region boundaries
            for (int i = 0; i < numberOfRegions; i++)
            {
                // First, generate the region height and width
                var regionWidth = _randomSequenceGenerator.Get(regionWidthRange.Low, regionWidthRange.High + 1);
                var regionHeight = _randomSequenceGenerator.Get(regionHeightRange.Low, regionHeightRange.High + 1);

                // Choose placement of region
                //
                var column = _randomSequenceGenerator.Get(PADDING + placementBoundary.Left, placementBoundary.Right - (regionWidth + PADDING) + 1);
                var row = _randomSequenceGenerator.Get(PADDING + placementBoundary.Top, placementBoundary.Bottom - (regionHeight + PADDING) + 1);

                regions.Add(new RegionBoundary(new GridLocation(column, row), regionWidth, regionHeight));
            }

            return regions;
        }

        public IEnumerable<RegionBoundary> CreateGaussianRectangularRegions(RegionBoundary placementBoundary, int numberOfRegions, Range<int> regionWidth, Range<int> regionHeight, int regionSpread)
        {
            // Calculate Gaussian parameters
            var standardDeviation = regionSpread;
            var meanColumn = placementBoundary.CellWidth / 2.0;
            var meanRow = placementBoundary.CellHeight / 2.0;

            var regions = new List<RegionBoundary>();

            // Create regions
            for (int i = 0; i < numberOfRegions; i++)
            {
                // First, generate the region height and width
                var width = _randomSequenceGenerator.Get(regionWidth.Low, regionWidth.High + 1);
                var height = _randomSequenceGenerator.Get(regionHeight.Low, regionHeight.High + 1);

                // Choose placement of region
                //
                var column = (int)_randomSequenceGenerator.GetGaussian(meanColumn, standardDeviation).Clip(placementBoundary.Left + PADDING, placementBoundary.Right - (width + PADDING));
                var row = (int)_randomSequenceGenerator.GetGaussian(meanRow, standardDeviation).Clip(placementBoundary.Top + PADDING, placementBoundary.Bottom - (height + PADDING));

                regions.Add(new RegionBoundary(new GridLocation(column, row), width, height));
            }

            return regions;
        }
    }
}
