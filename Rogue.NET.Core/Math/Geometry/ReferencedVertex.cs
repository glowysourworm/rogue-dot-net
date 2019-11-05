using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Math.Geometry
{
    public class ReferencedVertex<T> where T : class
    {
        /// <summary>
        /// The reference object for the vertex
        /// </summary>
        public T Reference { get; private set; }

        /// <summary>
        /// The vertex associated with the reference
        /// </summary>
        public Vertex Vertex { get; private set; }

        public ReferencedVertex(T reference, Vertex vertex)
        {
            this.Reference = reference;
            this.Vertex = vertex;
        }
    }
}
