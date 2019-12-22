namespace Rogue.NET.Core.Math.Geometry
{
    public class GraphEdge<T> where T : class
    {
        public GraphVertex<T> Point1 { get; set; }
        public GraphVertex<T> Point2 { get; set; }

        public GraphEdge(GraphVertex<T> point1, GraphVertex<T> point2)
        {
            this.Point1 = point1;
            this.Point2 = point2;
        }

        public override bool Equals(object obj)
        {
            if (obj is GraphEdge<T>)
            {
                var edge = obj as GraphEdge<T>;

                return Equals(edge.Point1, edge.Point2);
            }

            return false;
        }

        public bool Equals(GraphVertex<T> point1, GraphVertex<T> point2)
        {
            return (point1.Equals(this.Point1) &&
                    point2.Equals(this.Point2)) ||
                   (point1.Equals(this.Point2) &&
                    point2.Equals(this.Point1));
        }

        public override int GetHashCode()
        {
            var hash = 397;

            hash = (17 * hash) + this.Point1.GetHashCode();
            hash = (17 * hash) + this.Point2.GetHashCode();

            return hash;
        }

        public override string ToString()
        {
            return "(" + this.Point1.ToString() + ") -> (" + this.Point2.ToString() + ")";
        }
    }
}
