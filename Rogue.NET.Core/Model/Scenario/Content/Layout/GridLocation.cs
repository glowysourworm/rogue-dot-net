using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using System;
using System.Runtime.Serialization;

using static Rogue.NET.Core.Math.Geometry.Metric;
using static Rogue.NET.Core.Model.Scenario.Content.Layout.Interface.IGridLocator;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class GridLocation : IGridLocator
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public MetricType Type { get; set; }

        public GridLocation()
        {
            this.Type = MetricType.Rogue;
        }
        public GridLocation(GridLocation copy)
        {
            this.Row = copy.Row;
            this.Column = copy.Column;
            this.Type = MetricType.Rogue;
        }
        public GridLocation(IGridLocator locator)
        {
            this.Row = locator.Row;
            this.Column = locator.Column;
            this.Type = MetricType.Rogue;
        }
        public GridLocation(int column, int row)
        {
            Row = row;
            Column = column;
            this.Type = MetricType.Rogue;
        }
        public GridLocation(SerializationInfo info, StreamingContext context)
        {
            this.Column = info.GetInt32("Column");
            this.Row = info.GetInt32("Row");

            this.Type = MetricType.Rogue;
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Column", this.Column);
            info.AddValue("Row", this.Row);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is GridLocation)
            {
                var location = (GridLocation)obj;
                return location.Column == this.Column && location.Row == this.Row && location.Type == this.Type;
            }
            else
                throw new Exception("Trying to compare GridLocation to non-compatible type");
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return "Column=" + Column.ToString() + " Row=" + Row.ToString() + " Type=" + this.Type.ToString();
        }

    }
}
