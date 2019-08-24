using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class GridLocation
    {
        public static GridLocation Empty = new GridLocation(-1, -1);

        public int Row { get; set; }
        public int Column { get; set; }

        public GridLocation() { }
        public GridLocation(GridLocation copy)
        {
            this.Row = copy.Row;
            this.Column = copy.Column;
        }
        public GridLocation(int row, int col)
        {
            Row = row;
            Column = col;
        }
        public override bool Equals(object obj)
        {
            if (obj is GridLocation)
            {
                var cellPoint = obj as GridLocation;
                return cellPoint.Column == this.Column && cellPoint.Row == this.Row;
            }

            return base.Equals(obj);
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
