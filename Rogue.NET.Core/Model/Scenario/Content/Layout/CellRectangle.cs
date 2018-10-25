using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class CellRectangle
    {
        public CellPoint Location { get; set; }
        public int CellHeight { get; set; }
        public int CellWidth { get; set; }

        public CellRectangle()
        {
            this.Location = new CellPoint(-1, -1);
            this.CellWidth = -1;
            this.CellHeight = -1;
        }
        public CellRectangle(CellPoint location, int cellwidth, int cellheight)
        {
            this.Location = location;
            this.CellHeight = cellheight;
            this.CellWidth = cellwidth;
        }
        public int Left { get { return this.Location.Column; } }
        public int Right { get { return this.Location.Column + this.CellWidth; } }
        public int Top { get { return this.Location.Row; } }
        public int Bottom { get { return this.Location.Row + this.CellHeight; } }

        public override string ToString()
        {
            return "X=" + Location.Column + "Y=" + Location.Row + "Width=" + CellWidth + "Height=" + CellHeight;
        }
        public static CellRectangle Join(CellRectangle r1, CellRectangle r2)
        {
            var topLeft = new CellPoint(Math.Min(r1.Location.Row, r2.Location.Row), Math.Min(r1.Location.Column, r2.Location.Column));
            var bottomRight = new CellPoint(Math.Max(r1.Location.Row + r1.CellHeight, r2.Location.Row + r2.CellHeight), Math.Max(r1.Location.Column + r1.CellWidth, r2.Location.Column + r2.CellWidth));
            return new CellRectangle(topLeft, bottomRight.Column - topLeft.Column, bottomRight.Row - topLeft.Row);
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(CellRectangle))
            {
                return (this.Location.Equals(((CellRectangle)obj).Location)
                    && this.CellHeight == ((CellRectangle)obj).CellHeight && this.CellWidth == ((CellRectangle)obj).CellWidth);
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// Includes Boundary
        /// </summary>
        public bool Contains(CellPoint cellPoint)
        {
            if (cellPoint.Column < Location.Column)
                return false;

            if (cellPoint.Column > Location.Column + CellWidth)
                return false;

            if (cellPoint.Row < Location.Row)
                return false;

            if (cellPoint.Row > Location.Row + CellHeight)
                return false;

            return true;
        }
        public bool Contains(CellRectangle cellRectangle)
        {
            if (!Contains(cellRectangle.Location))
                return false;

            if (cellRectangle.Location.Column + cellRectangle.CellWidth >= this.Location.Column + this.CellWidth)
                return false;

            if (cellRectangle.Location.Row + cellRectangle.CellHeight >= this.Location.Row + this.CellHeight)
                return false;

            return true;
        }
    }
}
