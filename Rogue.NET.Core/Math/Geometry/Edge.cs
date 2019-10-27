using System;

namespace Rogue.NET.Core.Math.Geometry
{
    public struct Edge
    {
        public Vertex Point1 { get; set; }
        public Vertex Point2 { get; set; }

        public double Length
        {
            get { return this.Point2.Subtract(this.Point1).Magnitude; }
        }

        public Edge(Vertex point1, Vertex point2)
        {
            this.Point1 = point1;
            this.Point2 = point2;
        }

        public bool Intersects(Ray ray, double tolerance, out Vertex intersection)
        {
            // https://rootllama.wordpress.com/2014/06/20/ray-line-segment-intersection-test-in-2d/
            //

            // Check: Cast line segment and ray as parametric vectors and do some vector math to find
            //        constraints on the parameter for intersection.
            //
            var rayAsUnitVector = ray.UnitVector();

            var vector1 = ray.Origin.Subtract(this.Point1);
            var vector2 = this.Point2.Subtract(this.Point1);
            var vector3 = new Vector(-1 * rayAsUnitVector.Y, rayAsUnitVector.X);

            // ADDING A STEP:  If vector2.Dot(vector3) == 0, then the ray is parallel to this edge.
            //
            // SOLUTION:       Two lines created from this edge and the ray would share the same 
            //                 slope and intercept if they intersect.
            // 
            //                 If they share the same intercept - then see if they lie on the same
            //                 parametric line x(t) - such that x(t == this.Point1) <= x(t == ray.Origin) <= x(t == this.Point2)
            //
            if (System.Math.Abs(vector2.Dot(vector3)) < tolerance)
            {
                // Also, check to see if the slope is infinity - and handle that case
                var slope = (System.Math.Abs(rayAsUnitVector.X) < tolerance) ? double.NaN : 
                                                                        rayAsUnitVector.Y / rayAsUnitVector.X;

                // Vertical line:  Just check for the y-value of the ray to see that it's between the segment's 
                //                 endpoints' y-values
                //
                if (slope == double.NaN &&
                    ray.Origin.Y >= System.Math.Min(this.Point1.Y, this.Point2.Y) &&
                    ray.Origin.Y <= System.Math.Max(this.Point1.Y, this.Point2.Y))
                {
                    intersection = ray.Origin;
                    return true;
                }

                // Check the y-intercepts to see that they match. If they do - then make sure that the distance
                // from a -> o -> b is the same as a -> b.
                //
                else
                {
                    var intercept1 = ray.Origin.Y - (slope * ray.Origin.X);
                    var intercept2 = this.Point1.Y - (slope * this.Point1.X);

                    // Conditions for intersection:  
                    //
                    // 1) Intercepts match
                    // 2) |o - a| + |o - b| = |a - b|
                    //
                    if ((System.Math.Abs(intercept2 - intercept1) < tolerance) &&
                        (ray.Origin.Subtract(this.Point1).Magnitude + 
                         this.Point2.Subtract(ray.Origin).Magnitude) <= this.Length)
                    {
                        intersection = ray.Origin;
                        return true;
                    }
                }
            }

            // Calculate constraints on the parameter
            //
            var const1 = (vector2.Cross(vector1)) / vector2.Dot(vector3);
            var const2 = (vector1.Dot(vector3)) / (vector2.Dot(vector3));

            // Condition for intersection
            //
            var intersects = (const1 >= (-1 * tolerance)) &&              // const1 >= 0
                             (const2 >= (-1 * tolerance)) &&              // const2 >= 0
                             (const2 <= (1.0 + tolerance));               // 0 <= const2 <= 1

            // No intersection found
            //
            if (!intersects)
            {
                intersection = new Vertex(-1, -1);
                return intersects;
            }
            
            // Intersection found - use parametric form of the equations to calculate the intersection point
            //
            else
            {
                // x(t_intersection) = o + dt (where o, d are vectors)
                //
                // o := Origin Vector
                // d := Unit Vector along the ray
                // t_intersection := const1
                //
                intersection = new Vertex(ray.Origin.X + (rayAsUnitVector.X * const1),
                                          ray.Origin.Y + (rayAsUnitVector.Y * const1));
                return intersects;
            }
        }

        public static bool operator ==(Edge edge1, Edge edge2)
        {
            return edge1.Equals(edge2);
        }

        public static bool operator !=(Edge edge1, Edge edge2)
        {
            return !edge1.Equals(edge2);
        }

        public override bool Equals(object obj)
        {
            if (obj is Edge)
            {
                var edge = (Edge)obj;

                // Check both orientations
                return (edge.Point1 == this.Point1 && edge.Point2 == this.Point2) ||
                       (edge.Point2 == this.Point1 && edge.Point1 == this.Point2);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + this.Point1.ToString() + ") -> (" + this.Point2.ToString() + ")";
        }
    }
}
