using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface
{
    public interface IRegionGeometryCreator
    {
        /// <summary>
        /// Creates randomly placed (overlapping) rectangular regions. 
        /// </summary>
        /// <param name="width">Width of parent grid</param>
        /// <param name="height">Height of parent grid</param>
        /// <param name="regionFillRatio">A [0,1] parameter to specify how much of the region is filled with rectangles</param>
        /// <param name="regionSize">A [0,1] parameter to specify how relatively large each region is</param>
        /// <param name="regionSizeErradicity">A [0,1] parameter to specify how "different" region height and width can be</param>
        IEnumerable<RegionBoundary> CreateRandomRectangularRegions(int width, int height, double regionFillRatio, double regionSize, double regionSizeErradicity);

        /// <summary>
        /// Creates regularly placed regions using a virtual grid with the specified parameters
        /// </summary>        
        /// <param name="height">Height of parent grid</param>
        /// <param name="width">Width of parent grid</param>
        /// <param name="numberRegionColumns">Number of region columns</param>
        /// <param name="numberRegionRows">Number of region rows</param>
        /// <param name="regionSize">Region size [0,1]</param>
        /// <param name="regionFillRatio">Region fill ratio [0,1] (ratio of regions within the sub-divisions to fill)</param>
        /// <param name="regionSizeErradicity">Ratio [0,1] of erradic width / height parameters for the room sizes</param>
        IEnumerable<RegionBoundary> CreateGridRectangularRegions(int width, int height, int numberRegionColumns, int numberRegionRows, double regionSize, double regionFillRatio, double regionSizeErradicity);
    }
}
