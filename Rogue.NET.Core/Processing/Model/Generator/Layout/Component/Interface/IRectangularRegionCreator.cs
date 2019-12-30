using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Construction;

using System;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface
{
    public interface IRectangularRegionCreator
    {
        /// <summary>
        /// Creates cells within the given boundary - with option to overwrite existing cells. 
        /// </summary>
        /// <exception cref="Exception">Trying to overwrite existing cell involuntarily</exception>
        void CreateCells(GridCellInfo[,] grid, RegionBoundary boundary, bool overwrite);

        /// <summary>
        /// Create cells within the given boundary - but NOT intersecting any other boundaries. It will
        /// accomplish this by checking adjacent cells when plotting; and leaving a padding between
        /// boundaries. Zero padding will leave one empty cell for a wall. Separation will occur if the
        /// number of cells OUTSIDE the intersection exceeds the provided separation ratio.
        /// </summary>
        void CreateCellsXOR(GridCellInfo[,] grid, RegionBoundary boundary, int padding, double separationRatio);
    }
}
