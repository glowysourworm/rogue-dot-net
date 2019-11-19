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
        /// <returns></returns>
        IEnumerable<DistanceLocation> CalculateVisibility(LevelGrid grid, GridLocation location);
    }
}
