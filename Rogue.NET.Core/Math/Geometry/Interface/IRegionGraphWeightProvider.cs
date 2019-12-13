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
        /// Returns calculated weight to the adjacent node given the specified metric
        /// </summary>
        double CalculateWeight(Region<T> adjacentRegion, MetricType metricType);

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
