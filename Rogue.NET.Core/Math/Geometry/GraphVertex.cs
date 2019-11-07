using Rogue.NET.Core.Model.Scenario.Content.Layout;

namespace Rogue.NET.Core.Math.Geometry
{
    public class GraphVertex<T> where T : Region
    {
        /// <summary>
        /// The reference object for the vertex
        /// </summary>
        public T Reference { get; private set; }

        /// <summary>
        /// The vertex associated with the reference
        /// </summary>
        public GridLocation Vertex { get; private set; }

        public GraphVertex(T reference, GridLocation vertex)
        {
            this.Reference = reference;
            this.Vertex = vertex;
        }
    }
}
