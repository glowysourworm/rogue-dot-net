using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

namespace Rogue.NET.Core.Math.Geometry
{
    public class GraphVertex<T, V> where T : class, IGridLocator
    {
        /// <summary>
        /// The reference object for the vertex
        /// </summary>
        public V Reference { get; private set; }

        /// <summary>
        /// The vertex associated with the reference
        /// </summary>
        public T Vertex { get; private set; }

        public GraphVertex(V reference, T vertex)
        {
            this.Reference = reference;
            this.Vertex = vertex;
        }
    }
}
