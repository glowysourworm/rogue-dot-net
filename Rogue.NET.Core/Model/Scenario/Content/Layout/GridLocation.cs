using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Runtime.Serialization;

using static Rogue.NET.Core.Math.Geometry.Metric;

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

        public GridLocation(IPropertyReader reader)
        {
            this.Column = reader.Read<int>("Column");
            this.Row = reader.Read<int>("Row");

            this.Type = MetricType.Rogue;
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("Column", this.Column);
            writer.Write("Row", this.Row);
        }

        // SHOULD BEHAVE LIKE A VALUE TYPE
        public static bool operator ==(GridLocation location1, GridLocation location2)
        {
            if (ReferenceEquals(location1, location2))
                return true;

            else if (ReferenceEquals(location1, null))
                return ReferenceEquals(location2, null);

            else if (ReferenceEquals(location2, null))
                return false;

            else
                return location1.Equals(location2);
        }

        public static bool operator !=(GridLocation location1, GridLocation location2)
        {
            if (ReferenceEquals(location1, location2))
                return false;

            else if (ReferenceEquals(location1, null))
                return !ReferenceEquals(location2, null);

            else if (ReferenceEquals(location2, null))
                return true;

            else
                return !location1.Equals(location2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            var location = obj as GridLocation;

            if (!ReferenceEquals(location, null))
            {
                return location.Column == this.Column && 
                       location.Row == this.Row && 
                       location.Type == this.Type;
            }
            else
                throw new Exception("Trying to compare GridLocation to non-compatible type");
        }
        public override int GetHashCode()
        {
            return this.CreateHashCode(this.Column, this.Row, this.Type);
        }
        public override string ToString()
        {
            return "Column=" + Column.ToString() + " Row=" + Row.ToString() + " Type=" + this.Type.ToString();
        }
    }
}
