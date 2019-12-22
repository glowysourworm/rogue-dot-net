using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Static;
using System;
using System.Runtime.Serialization;
using static Rogue.NET.Core.Math.Geometry.Metric;

namespace Rogue.NET.Core.Math.Geometry
{
    public class GridVector : IGridLocator
    {
        public double ColumnDimension { get; set; }
        public double RowDimension { get; set; }

        public int Column { get { return (int)this.ColumnDimension; } }
        public int Row { get { return (int)this.RowDimension; } }

        public double EuclideanMagnitude
        {
            get { return Metric.EuclideanDistance(this.ColumnDimension, this.RowDimension); }
        }

        public GridVector(double columnDimension, double rowDimension)
        {
            this.ColumnDimension = columnDimension;
            this.RowDimension = rowDimension;
        }

        public static GridVector Create(double angleRadians, double length)
        {
            return new GridVector(length * System.Math.Cos(angleRadians), length * System.Math.Sin(angleRadians));
        }

        public override bool Equals(object obj)
        {
            if (obj is GridVector)
            {
                var vector = (GridVector)obj;

                return vector.ColumnDimension == this.ColumnDimension && 
                       vector.RowDimension == this.RowDimension;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Column={0} Row={1}", this.ColumnDimension.ToString("F2"), this.RowDimension.ToString("F2"));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
