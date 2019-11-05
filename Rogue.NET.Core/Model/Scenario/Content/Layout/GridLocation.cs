using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class GridLocation
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public GridLocation()
        {

        }
        public GridLocation(GridLocation copy)
        {
            this.Row = copy.Row;
            this.Column = copy.Column;
        }
        public GridLocation(int column, int row)
        {
            Row = row;
            Column = column;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is GridLocation)
            {
                var location = (GridLocation)obj;
                return location.Column == this.Column && location.Row == this.Row;
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
            return "Column=" + Column.ToString() + " Row=" + Row.ToString();
        }
    }
}
