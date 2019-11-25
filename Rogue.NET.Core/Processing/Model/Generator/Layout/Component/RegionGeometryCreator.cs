using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
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

        public IEnumerable<RegionBoundary> CreateCenteredRandomRectangularRegions(int width, int height, int numberOfRegions, Range<int> regionWidth, Range<int> regionHeight, int regionSpread)
        {
            var regions = new List<RegionBoundary>();

            var northWest = new RegionBoundary(new GridLocation(0, 0), width / 2, height / 2);
            var northEast = new RegionBoundary(new GridLocation(northWest.Right + 1, 0), (width / 2) - 1, height / 2);
            var southWest = new RegionBoundary(new GridLocation(0, northWest.Bottom + 1), width / 2, (height / 2) - 1);
            var southEast = new RegionBoundary(new GridLocation(northWest.Right + 1, northWest.Bottom + 1), (width / 2) - 1, (height / 2) - 1);

            regions.AddRange(CreatePackedRandomBoundary(northWest, Compass.SE, numberOfRegions / 4, 1, regionWidth, regionHeight));
            regions.AddRange(CreatePackedRandomBoundary(northEast, Compass.SW, numberOfRegions / 4, 1, regionWidth, regionHeight));
            regions.AddRange(CreatePackedRandomBoundary(southWest, Compass.NE, numberOfRegions / 4, 1, regionWidth, regionHeight));
            regions.AddRange(CreatePackedRandomBoundary(southEast, Compass.NW, numberOfRegions / 4, 1, regionWidth, regionHeight));

            return regions;
        }
        public IEnumerable<RegionBoundary> CreateAnchoredRandomRectangularRegions(int width, int height, int numberOfRegions, Range<int> regionWidth, Range<int> regionHeight, int regionSpread)
        {
            // Create a sub-division using euclidean geometry 
            //

            var placementBoundary = new RegionBoundary(new GridLocation(0, 0), width, height);
            var aspectRatio = ModelConstants.CellHeight / ModelConstants.CellWidth;

            var placementRatioY = 0.55;
            var placementRatioX = 0.30;

            var anchorLocationX = placementBoundary.CellWidth * placementRatioX;
            var anchorLocationY = placementBoundary.CellHeight * placementRatioY / aspectRatio;

            var anchorWidth = placementBoundary.CellWidth * (1 - (2 * placementRatioX));
            var anchorHeight = placementBoundary.CellHeight * (1 - placementRatioY) * aspectRatio;

            var westLocationX = 0;
            var eastLocationX = (int)(placementBoundary.CellWidth * (1 - placementRatioX)) + 1;

            // Transform back into grid coordinates
            var anchorLocation = new GridLocation((int)anchorLocationX, (int)anchorLocationY);
            var northWestLocation = new GridLocation(0, 0);
            var northEastLocation = new GridLocation(placementBoundary.Center.Column + 1, 0);
            var westLocation = new GridLocation(westLocationX, (int)anchorLocationY);
            var eastLocation = new GridLocation((int)eastLocationX, (int)anchorLocationY);

            var anchorRegion = new RegionBoundary(anchorLocation, (int)anchorWidth, (int)anchorHeight);
            var northWestRegion = new RegionBoundary(northWestLocation, (placementBoundary.CellWidth / 2) - 1, placementBoundary.CellHeight - anchorRegion.CellHeight - 2);
            var northEastRegion = new RegionBoundary(northEastLocation, (placementBoundary.CellWidth / 2) - 1, placementBoundary.CellHeight - anchorRegion.CellHeight - 2);
            var westRegion = new RegionBoundary(westLocation, ((placementBoundary.CellWidth - (int)anchorWidth) / 2) - 1, anchorRegion.CellHeight);
            var eastRegion = new RegionBoundary(eastLocation, ((placementBoundary.CellWidth - (int)anchorWidth) / 2) - 1, anchorRegion.CellHeight);

            var result = new List<RegionBoundary>();

            result.Add(anchorRegion);
            result.AddRange(CreatePackedRandomBoundary(northWestRegion, Compass.SE, (int)(numberOfRegions / 4.0), 1, regionWidth, regionHeight));
            result.AddRange(CreatePackedRandomBoundary(northEastRegion, Compass.SW, (int)(numberOfRegions / 4.0), 1, regionWidth, regionHeight));
            result.AddRange(CreatePackedRandomBoundary(westRegion, Compass.E, (int)(numberOfRegions / 4.0), 1, regionWidth, regionHeight));
            result.AddRange(CreatePackedRandomBoundary(eastRegion, Compass.W, (int)(numberOfRegions / 4.0), 1, regionWidth, regionHeight));

            return result;
        }

        private IEnumerable<RegionBoundary> CreatePackedRandomBoundary(RegionBoundary parentBoundary, Compass packingDirection, int numberOfRegions, int regionPadding, Range<int> regionWidthRange, Range<int> regionHeightRange)
        {
            var result = new List<RegionBoundary>();

            // Procedure
            //
            // - Pick one point at random favoring the side (or corner) of the region
            //      - Has no restrictions except to leave room for the region (height / width)
            //
            // - This point and the size form a rectangle - so just create a region which should be
            //   constrained to both the size requirements and the parent boundary
            //

            var packingSpread = 1;
            var northCutoff = (parentBoundary.Top + (parentBoundary.CellHeight * packingSpread)).Clip(parentBoundary.Top, parentBoundary.Bottom - regionHeightRange.High);
            var southCutoff = (parentBoundary.Top + (parentBoundary.CellHeight * (1 - packingSpread))).Clip(parentBoundary.Top, parentBoundary.Bottom - regionHeightRange.High);
            var eastCutoff = (parentBoundary.Left + (parentBoundary.CellWidth * (1 - packingSpread))).Clip(parentBoundary.Left, parentBoundary.Right - regionWidthRange.High);
            var westCutoff = (parentBoundary.Left + (parentBoundary.CellWidth * packingSpread)).Clip(parentBoundary.Left, parentBoundary.Right - regionWidthRange.High);

            for (int i = 0; i < numberOfRegions; i++)
            {
                GridLocation point = null;

                var height = _randomSequenceGenerator.GetRandomValue(regionHeightRange);
                var width = _randomSequenceGenerator.GetRandomValue(regionWidthRange);
                
                switch (packingDirection)
                {
                    case Compass.N:
                        point = new GridLocation(_randomSequenceGenerator.Get(parentBoundary.Left, parentBoundary.Right - width + 1),
                                            (int)_randomSequenceGenerator.GetTriangle(parentBoundary.Top, parentBoundary.Top, northCutoff));
                        break;
                    case Compass.S:
                        point = new GridLocation(_randomSequenceGenerator.Get(parentBoundary.Left, parentBoundary.Right - width + 1),
                                            (int)_randomSequenceGenerator.GetTriangle(southCutoff, parentBoundary.Bottom - height, parentBoundary.Bottom - height));
                        break;
                    case Compass.E:
                        point = new GridLocation((int)_randomSequenceGenerator.GetTriangle(eastCutoff, parentBoundary.Right - width, parentBoundary.Right - width),
                                                      _randomSequenceGenerator.Get(parentBoundary.Top, parentBoundary.Bottom - height + 1));
                        break;
                    case Compass.W:
                        point = new GridLocation((int)_randomSequenceGenerator.GetTriangle(parentBoundary.Left, parentBoundary.Left, westCutoff),
                                                      _randomSequenceGenerator.Get(parentBoundary.Top, parentBoundary.Bottom - height + 1));
                        break;
                    case Compass.NW:
                        point = new GridLocation((int)_randomSequenceGenerator.GetTriangle(parentBoundary.Left, parentBoundary.Left, westCutoff),
                                                 (int)_randomSequenceGenerator.GetTriangle(parentBoundary.Top, parentBoundary.Top, northCutoff));
                        break;
                    case Compass.NE:
                        point = new GridLocation((int)_randomSequenceGenerator.GetTriangle(eastCutoff, parentBoundary.Right - width, parentBoundary.Right - width),
                                                 (int)_randomSequenceGenerator.GetTriangle(parentBoundary.Top, parentBoundary.Top, northCutoff));
                        break;
                    case Compass.SE:
                        point = new GridLocation((int)_randomSequenceGenerator.GetTriangle(eastCutoff, parentBoundary.Right - width, parentBoundary.Right - width),
                                                 (int)_randomSequenceGenerator.GetTriangle(southCutoff, parentBoundary.Bottom - height, parentBoundary.Bottom - height));
                        break;
                    case Compass.SW:
                        point = new GridLocation((int)_randomSequenceGenerator.GetTriangle(parentBoundary.Left, parentBoundary.Left, westCutoff),
                                                 (int)_randomSequenceGenerator.GetTriangle(southCutoff, parentBoundary.Bottom - height, parentBoundary.Bottom - height));
                        break;
                    default:
                        throw new Exception("Unhandled packing direction RegionGeometryCreator");
                }

                result.Add(new RegionBoundary(point, width, height));
            }

            return result;
        }
    }
}
