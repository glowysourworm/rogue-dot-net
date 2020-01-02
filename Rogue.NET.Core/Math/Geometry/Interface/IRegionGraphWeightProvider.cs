using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Interface;

namespace Rogue.NET.Core.Math.Geometry.Interface
{
    /// <summary>
    /// Provides a mechanism to inject weighted distances between nodes in a graph
    /// </summary>
    public interface IRegionGraphWeightProvider<T> where T : class, IGridLocator
    {
        /// <summary>
        /// Calculates the connection parameters for the region - and the recipricol - and stores them
        /// on the two involved regions. Returns the distance for the connection - with the specified metric type
        /// </summary>
        void CalculateConnection(ConnectedRegion<T> adjacentRegion, IRandomSequenceGenerator randomSequenceGenerator);

        /// <summary>
        /// Sets a weight value calculated by another adjacent region
        /// </summary>
        void SetConnection(ConnectedRegion<T> adjacentRegion, T location, T adjacentLocation, double distance);

        /// <summary>
        /// Returns the connection point from the CalculateWeight(...) calculation
        /// </summary>
        RegionConnection<T> GetConnection(ConnectedRegion<T> adjacentRegion);
    }
}
