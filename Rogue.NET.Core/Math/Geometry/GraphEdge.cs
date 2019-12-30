using System;
using static Rogue.NET.Core.Math.Geometry.Metric;

namespace Rogue.NET.Core.Math.Geometry
{
    [Serializable]
    public class GraphEdge
    {
        public GraphVertex Point1 { get; set; }
        public GraphVertex Point2 { get; set; }
        public MetricType MetricType { get; set; }

        public double Distance
        {
            get
            {
                switch (this.MetricType)
                {
                    case MetricType.Roguian:
                        return Metric.RoguianDistance(this.Point1, this.Point2);
                    case MetricType.Euclidean:
                        return Metric.EuclideanDistance(this.Point1, this.Point2);
                    case MetricType.TaxiCab:
                        return Metric.TaxiCabDistance(this.Point1, this.Point2);
                    default:
                        throw new Exception("Unhandled metric type GraphEdge");
                }
            }
        }

        public GraphEdge(GraphVertex point1, GraphVertex point2, MetricType metricType)
        {
            this.Point1 = point1;
            this.Point2 = point2;
            this.MetricType = metricType;
        }



        public override bool Equals(object obj)
        {
            if (obj is GraphEdge)
            {
                var edge = obj as GraphEdge;

                return Equals(edge.Point1, edge.Point2) && edge.MetricType == this.MetricType;
            }

            return false;
        }

        public bool Equals(GraphVertex point1, GraphVertex point2)
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
            hash = (17 * hash) + this.MetricType.GetHashCode();

            return hash;
        }

        public override string ToString()
        {
            return "(" + this.Point1.ToString() + ") -> (" + this.Point2.ToString() + ")";
        }
    }
}
