using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rogue.NET.Core.Math.Geometry.Metric;

namespace Rogue.NET.Core.Math.Geometry.Interface
{
    /// <summary>
    /// Provides a mechanism to inject weighted distances between nodes in a graph
    /// </summary>
    public interface IRegionGraphWeightProvider
    {
        /// <summary>
        /// Returns calculated weight to the adjacent node given the specified metric
        /// </summary>
        double CalculateWeight(Region adjacentRegion, MetricType metricType);

        /// <summary>
        /// Returns the connection point from the CalculateWeight(...) calculation
        /// </summary>
        GridLocation GetConnectionPoint(Region adjacentRegion, MetricType metricType);

        /// <summary>
        /// Returns the connection point from this region to the adjacent region calculated
        /// during CalculateWeight(...)
        /// </summary>
        GridLocation GetAdjacentConnectionPoint(Region adjacentRegion, MetricType metricType);
    }
}
