using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class CellPoint
    {
        public static CellPoint Empty = new CellPoint(-1, -1);

        public int Row { get; set; }
        public int Column { get; set; }

        public CellPoint() { }
        public CellPoint(CellPoint copy)
        {
            this.Row = copy.Row;
            this.Column = copy.Column;
        }
        public CellPoint(int row, int col)
        {
            Row = row;
            Column = col;
        }
        public override bool Equals(object obj)
        {
            if (obj is CellPoint)
            {
                var cellPoint = obj as CellPoint;
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
