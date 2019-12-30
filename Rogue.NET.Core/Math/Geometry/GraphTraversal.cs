using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Math.Geometry
{
    [Serializable]
    public class GraphTraversal : ISerializable
    {
        List<GraphVertex> _route;

        public GraphVertex Source { get; set; }
        public GraphVertex Destination { get; set; }
        public IEnumerable<GraphVertex> Route { get { return _route; } }

        public GraphTraversal(GraphVertex source, GraphVertex destination, IEnumerable<GraphVertex> route)
        {
            this.Source = source;
            this.Destination = destination;

            _route = route.ToList();
        }

        public GraphTraversal(SerializationInfo info, StreamingContext context)
        {
            this.Source = (GraphVertex)info.GetValue("Source", typeof(GraphVertex));
            this.Destination = (GraphVertex)info.GetValue("Destination", typeof(GraphVertex));
            _route = (List<GraphVertex>)info.GetValue("Route", typeof(List<GraphVertex>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Source", this.Source);
            info.AddValue("Destination", this.Destination);
            info.AddValue("Route", _route);
        }
    }
}
