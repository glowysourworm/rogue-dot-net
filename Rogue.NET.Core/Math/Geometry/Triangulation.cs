using Rogue.NET.Common.Extension;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Math.Geometry
{
    public class Triangulation<T> where T : class
    {
        List<ReferencedTriangle<T>> _triangles;

        public IEnumerable<ReferencedTriangle<T>> Triangles
        {
            get { return _triangles; }
        }

        public Triangulation()
        {
            _triangles = new List<ReferencedTriangle<T>>();
        }

        public void AddTriangle(ReferencedTriangle<T> triangle)
        {
            _triangles.Add(triangle);
        }
        public void RemoveTriangle(ReferencedTriangle<T> triangle)
        {
            _triangles.Remove(triangle);
        }

        /// <summary>
        /// Returns set of distinct edges in the triangulation
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReferencedEdge<T>> GetEdges()
        {
            return _triangles.SelectMany(triangle => triangle.Edges).DistinctWith((edge1, edge2) =>
            {
                       // Oriented Similar
                return (edge1.Point1.Reference == edge2.Point1.Reference && edge1.Point2.Reference == edge2.Point2.Reference) ||

                       // Oriented Opposing
                       (edge1.Point1.Reference == edge2.Point2.Reference && edge1.Point2.Reference == edge2.Point1.Reference);
            }).Actualize();
        }

        /// <summary>
        /// Returns distinct set of vertices in the triangulation
        /// </summary>
        public IEnumerable<ReferencedVertex<T>> GetVertices()
        {
            return _triangles.SelectMany(triangle => triangle.Vertices).DistinctBy(vertex => vertex.Reference).Actualize();
        }
    }
}
