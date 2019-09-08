using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout;
using System.Collections.Generic;
using System.Windows;

namespace Rogue.NET.Core.Processing.Model.Algorithm.Interface
{
    public interface IRayTracer
    {
        /// <summary>
        /// Calculates visibility from the vantage point of the input location. This is defined as two sets: The return value is
        /// the set of locations that are within the lightRadius from the input location. The other set is the line-of-sight
        /// collection of locations that are directly vantaged from the input location; but are NOT within the light radius of
        /// that location. 
        /// </summary>
        /// <param name="grid">The level grid</param>
        /// <param name="location">The input location</param>
        /// <param name="lightRadius">The input location light radius</param>
        /// <param name="lineOfSightLocations">The output set of line-of-sight locations</param>
        /// <returns></returns>
        IEnumerable<DistanceLocation> CalculateVisibility(LevelGrid grid, GridLocation location, double lightRadius, out IEnumerable<DistanceLocation> lineOfSightLocations);
    }
}
