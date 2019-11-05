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
    public interface IGraphWeightProvider<T>
    {
        double CalculateWeight(T adjacentNode, MetricType metricType);
    }
}
