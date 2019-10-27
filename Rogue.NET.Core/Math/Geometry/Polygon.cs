using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Math.Geometry
{
    public struct Polygon
    {
        private const double TRIG_NOISE_TOLERANCE = 1e-10;

        public IEnumerable<Vertex> OrderedVerticies { get; private set; }
        public IEnumerable<Edge> OrderedEdges { get; private set; }
        public Rectangle Boundary { get; private set; }

        public Polygon(IEnumerable<Vertex> orderedVertices)
        {
            if (orderedVertices.Count() < 3)
                throw new Exception("Improperly formed polygon (must have more than 2 verticies)");

            this.OrderedVerticies = orderedVertices;
            this.Boundary = new Rectangle(orderedVertices);

            // Calculate edges
            this.OrderedEdges = new List<Edge>();

            for (int i=1;i<orderedVertices.Count();i++)
                ((List<Edge>)this.OrderedEdges).Add(new Edge(orderedVertices.ElementAt(i - 1), orderedVertices.ElementAt(i)));

            if (orderedVertices.Any())
                ((List<Edge>)this.OrderedEdges).Add(new Edge(orderedVertices.Last(), orderedVertices.First()));
        }

        /// <summary>
        /// Uses point-in-polygon routine to calculate whether specified point lies in the interior of
        /// the polygon.
        /// </summary>
        public bool Contains(Vertex point, out bool liesOnEdge)
        {
            // Procedure
            //
            // - Cast a ray through the point
            //
            // - If intersects a vertex directly, then check to see that that is 
            //   taken as data to return the right result
            //
            // - If the total number of edge intersections with the polygon is odd, 
            //   then the result is true. Else, false.
            //

            // Initialize the edge detector
            liesOnEdge = false;

            // Choose some angle to cast the ray
            //
            var ray = new Ray(point, 1.0);
            var intersections = new List<Vertex>();

            // Intersections
            foreach (var edge in this.OrderedEdges)
            {
                // If the point lies on a vertex of the edge - go ahead and
                // return true
                //
                if (point == edge.Point1 || point == edge.Point2)
                {
                    liesOnEdge = true;
                    return true;
                }

                var intersection = new Vertex(-1, -1);
                if (edge.Intersects(ray, TRIG_NOISE_TOLERANCE, out intersection))
                    intersections.Add(intersection);

                // NOTE*** Allowing for tolerance because of noisy trig functions
                //
                if (System.Math.Abs(intersection.X - point.X) <= TRIG_NOISE_TOLERANCE &&
                    System.Math.Abs(intersection.Y - point.Y) <= TRIG_NOISE_TOLERANCE)
                {
                    liesOnEdge |= true;
                    return true;
                }
            }

            // Check this exception to make sure our routine is understood
            //
            if (intersections.Distinct().Count() != intersections.Count())
                throw new Exception("Found non-distinct intersections with polygon");

            // Calculate Answer
            //
            if (intersections.Count() > 0)
                return intersections.Count() % 2 == 1;

            else
                return false;
        }
    }
}
