using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout;
using System.Collections.Generic;
using System.Windows;

namespace Rogue.NET.Core.Processing.Model.Algorithm.Interface
{
    public interface IVisibilityCalculator
    {
        /// <summary>
        /// Calculates visibility from the vantage point of the input location. The return value is the set of locations that are lit up from the 
        /// vantage point of the input location. 
        /// </summary>
        /// <param name="grid">The level grid</param>
        /// <param name="location">The input location</param>
        IEnumerable<DistanceLocation> CalculateVisibility(LevelGrid grid, GridLocation location);

        /// <summary>
        /// Calculates visibility from the vantage point of the input location. The return value is the set of locations that are lit up from the 
        /// vantage point of the input location. 
        /// </summary>
        /// <param name="grid">The grid used during scenario generation</param>
        /// <param name="location">The input location</param>
        IEnumerable<DistanceLocation> CalculateVisibility(GridCellInfo[,] grid, GridLocation location);
    }
}
