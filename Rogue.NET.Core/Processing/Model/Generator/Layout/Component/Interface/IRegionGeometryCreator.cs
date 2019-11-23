using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface
{
    public interface IRegionGeometryCreator
    {
        /// <summary>
        /// Creates randomly placed rectangular regions with the provided parameters
        /// </summary>
        /// <param name="placementBoundary">Placement region for generating the rectangular regions</param>
        /// <param name="numberOfRegions">Number of regions to generate</param>
        /// <param name="regionHeight">Region height range</param>
        /// <param name="regionWidth">Region width range</param>
        /// <param name="regionSpread">Spread parameter - generates space in between regions</param>
        IEnumerable<RegionBoundary> CreateRandomRectangularRegions(RegionBoundary placementBoundary, int numberOfRegions, Range<int> regionWidth, Range<int> regionHeight, int regionSpread);

        /// <summary>
        /// Creates regularly placed regions using a virtual grid with the specified parameters
        /// </summary>        
        /// <param name="numberRegionColumns">Total number of subdivided virtual columns</param>
        /// <param name="numberRegionRows">Total number of subdivided virtual rows</param>
        /// <param name="regionPadding">Amount of padding added to space out regions</param>
        /// <param name="regionHeight">Region height range (inside the virtual grid cell)</param>
        /// <param name="regionWidth">Region width range (inside the virtual grid cell)</param>
        IEnumerable<RegionBoundary> CreateGridRectangularRegions(int numberRegionColumns, int numberRegionRows, int regionPadding, Range<int> regionWidthRange, Range<int> regionHeightRange);

        /// <summary>
        /// Creates rectangular regions placed near the center of the map using gaussian distributed locations. NOTE*** THESE CAN GO PARTIALLY OFF THE MAP.
        /// </summary>
        /// <param name="placementBoundary">Placement region for generating the rectangular regions</param>
        /// <param name="numberOfRegions">Number of regions to generate</param>
        /// <param name="regionHeight">Region height range</param>
        /// <param name="regionWidth">Region width range</param>
        /// <param name="regionSpread">Spread parameter - generates space in between regions</param>
        IEnumerable<RegionBoundary> CreateGaussianRectangularRegions(RegionBoundary placementBoundary, int numberOfRegions, Range<int> regionWidth, Range<int> regionHeight, int regionSpread);
    }
}
