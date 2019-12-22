using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using static Rogue.NET.Core.Math.Geometry.Metric;

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
        double CalculateConnection(Region<T> adjacentRegion, MetricType metricType);

        /// <summary>
        /// Sets a weight value calculated by another adjacent region
        /// </summary>
        void SetConnection(Region<T> adjacentRegion, Metric.MetricType metricType, T location, T adjacentLocation, double distance);

        /// <summary>
        /// Returns the connection point from the CalculateWeight(...) calculation
        /// </summary>
        T GetConnectionPoint(Region<T> adjacentRegion, MetricType metricType);

        /// <summary>
        /// Returns the connection point from this region to the adjacent region calculated
        /// during CalculateWeight(...)
        /// </summary>
        T GetAdjacentConnectionPoint(Region<T> adjacentRegion, MetricType metricType);
    }
}
