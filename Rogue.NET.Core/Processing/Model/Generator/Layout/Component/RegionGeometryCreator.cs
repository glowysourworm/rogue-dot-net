﻿using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRegionGeometryCreator))]
    public class RegionGeometryCreator : IRegionGeometryCreator
    {
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        const int ROOM_SIZE_MIN = 2;
        const int RECTANGULAR_DIV_COUNT_MAX = 10;
        const int RECTANGULAR_DIV_COUNT_MIN = 2;
        const int RECTANGULAR_DIV_MIN_SIZE = 4;
        const double RECTANGULAR_FILL_RATIO_MIN = 0.2;
        const double RANDOM_ROOM_FILL_RATIO_MAX = 0.2;
        const double RANDOM_ROOM_FILL_RATIO_MULTIPLIER = 2.0;

        [ImportingConstructor]
        public RegionGeometryCreator(IRandomSequenceGenerator randomSequenceGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public IEnumerable<RegionBoundary> CreateRandomRectangularRegions(int width, int height, double regionFillRatio, double regionSize, double regionSizeErradicity)
        {
            var padding = 2;

            // Calculate room size based on parameters
            var roomWidthLimit = width * RANDOM_ROOM_FILL_RATIO_MAX;
            var roomHeightLimit = height * RANDOM_ROOM_FILL_RATIO_MAX;

            var roomMaxWidth = (int)((regionSize * (roomWidthLimit - ROOM_SIZE_MIN)) + ROOM_SIZE_MIN);
            var roomMaxHeight = (int)((regionSize * (roomHeightLimit - ROOM_SIZE_MIN)) + ROOM_SIZE_MIN);

            var roomWidthMin = (int)(roomMaxWidth - (regionSizeErradicity * (roomMaxWidth - ROOM_SIZE_MIN)));
            var roomHeightMin = (int)(roomMaxHeight - (regionSizeErradicity * (roomMaxHeight - ROOM_SIZE_MIN)));

            // Add a correction to the number of regions calculation for erradicity [0,1] -> [0.5, 1]
            var erradicityCorrection = (0.5 * regionSizeErradicity) + 0.5;

            // Calculate number of regions based on "number of regions it would take to tile the area X times"
            var numberOfRegions = (int)(((width * height) / (roomMaxWidth * roomMaxHeight * erradicityCorrection)) * regionFillRatio * RANDOM_ROOM_FILL_RATIO_MULTIPLIER).LowLimit(2);

            if (roomWidthLimit < 2 ||
                roomHeightLimit < 2 ||
                roomMaxWidth < 2 ||
                roomMaxHeight < 2 ||
                roomWidthMin < 2 ||
                roomHeightMin < 2 ||
                numberOfRegions < 1)
                throw new Exception("Improper parameter set for random rectangular regions");

            var regions = new List<RegionBoundary>();

            // Create region boundaries
            for (int i = 0; i < numberOfRegions; i++)
            {
                // First, generate the region height and width
                var regionWidth = _randomSequenceGenerator.Get(roomWidthMin, roomMaxWidth + 1);
                var regionHeight = _randomSequenceGenerator.Get(roomHeightMin, roomMaxHeight + 1);

                // Choose placement of region
                //
                var column = _randomSequenceGenerator.Get(padding, width - (regionWidth + padding));
                var row = _randomSequenceGenerator.Get(padding, height - (regionHeight + padding));

                regions.Add(new RegionBoundary(new GridLocation(column, row), regionWidth, regionHeight));
            }

            return regions;
        }

        public IEnumerable<RegionBoundary> CreateGridRectangularRegions(int width, int height, double regionColumnRatio, double regionRowRatio,
                                                                        double regionSize, double regionFillRatio, double regionSizeErradicity)
        {
            var roomPadding = 1;

            // Calculate the min number of region columns / rows based on a min division size of 4
            var numberRegionColumnsMax = ((int)(width / (double)RECTANGULAR_DIV_MIN_SIZE)).HighLimit(RECTANGULAR_DIV_COUNT_MAX);
            var numberRegionRowsMax = ((int)(height / (double)RECTANGULAR_DIV_MIN_SIZE)).HighLimit(RECTANGULAR_DIV_COUNT_MAX);

            var numberRegionColumnsSafe = ((int)(numberRegionColumnsMax * regionColumnRatio)).Clip(RECTANGULAR_DIV_COUNT_MIN, numberRegionColumnsMax);
            var numberRegionRowsSafe = ((int)(numberRegionRowsMax * regionColumnRatio)).Clip(RECTANGULAR_DIV_COUNT_MIN, numberRegionRowsMax);

            var gridDivisionWidth = width / numberRegionColumnsSafe;
            var gridDivisionHeight = height / numberRegionRowsSafe;

            var regionFillRatioSafe = regionFillRatio.LowLimit(RECTANGULAR_FILL_RATIO_MIN);

            // Calculate room size based on parameters
            // var roomWidthLimit = gridDivisionWidth - (2 * roomPadding);
            // var roomHeightLimit = gridDivisionHeight - (2 * roomPadding);

            // UPDATE:  Use one padding to add just a single wall between rooms (at minimum)
            //          Then, check to see that any regions stay within the boundary - making size adjustments.
            //
            var roomWidthLimit = gridDivisionWidth - (roomPadding);
            var roomHeightLimit = gridDivisionHeight - (roomPadding);

            var roomWidthMax = (int)((regionSize * (roomWidthLimit - ROOM_SIZE_MIN)) + ROOM_SIZE_MIN);
            var roomWidthMin = (int)(roomWidthMax - (regionSizeErradicity * (roomWidthMax - ROOM_SIZE_MIN)));

            var roomHeightMax = (int)((regionSize * (roomHeightLimit - ROOM_SIZE_MIN)) + ROOM_SIZE_MIN);
            var roomHeightMin = (int)(roomHeightMax - (regionSizeErradicity * (roomHeightMax - ROOM_SIZE_MIN)));

            if (roomWidthLimit < 2 ||
                roomHeightLimit < 2 ||
                roomWidthMax < 2 ||
                roomHeightMax < 2 ||
                roomWidthMin < 2 ||
                roomHeightMin < 2)
                throw new Exception("Improper parameter set for random rectangular regions");

            var regions = new List<RegionBoundary>();

            // Create regions according to template parameters
            for (int i = 0; i < numberRegionColumnsSafe; i++)
            {
                var divisionColumn = i * gridDivisionWidth;

                for (int j = 0; j < numberRegionRowsSafe; j++)
                {
                    // Check random fill ratio for adding a region
                    if (_randomSequenceGenerator.Get() >= regionFillRatioSafe)
                        continue;

                    var divisionRow = j * gridDivisionHeight;

                    // Draw a random region size
                    var regionWidth = _randomSequenceGenerator.Get(roomWidthMin, roomWidthMax + 1);
                    var regionHeight = _randomSequenceGenerator.Get(roomHeightMin, roomHeightMax + 1);

                    // Generate the upper left-hand corner for the region
                    var column = divisionColumn + _randomSequenceGenerator.Get(roomPadding, gridDivisionWidth - (regionWidth + roomPadding) + 1);
                    var row = divisionRow + _randomSequenceGenerator.Get(roomPadding, gridDivisionHeight - (regionHeight + roomPadding) + 1);

                    // UPDATE: Check to see that region falls in bounds - cutting off the size if it's one over
                    if (column + regionWidth >= width)
                        regionWidth--;

                    if (row + regionHeight >= height)
                        regionHeight--;

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
