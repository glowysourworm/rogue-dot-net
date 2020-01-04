using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Math.Geometry
{
    /// <summary>
    /// Component for planning a route between graph vertices
    /// </summary>
    public class GraphTraversal
    {
        public GraphVertex Source { get; private set; }
        public GraphVertex Destination { get; private set; }
        public IEnumerable<GraphVertex> Route { get; private set; }

        public GraphTraversal(GraphVertex source, GraphVertex destination, IEnumerable<GraphVertex> route)
        {
            this.Source = source;
            this.Destination = destination;
            this.Route = route;
        }
    }
}
