using Rogue.NET.Core.Processing.Model.Static;
using System;

using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class CellRectangle
    {
        public GridLocation Location { get; set; }
        public int CellHeight { get; set; }
        public int CellWidth { get; set; }

        public CellRectangle()
        {
            this.Location = new GridLocation(-1, -1);
            this.CellWidth = -1;
            this.CellHeight = -1;
        }
        public CellRectangle(GridLocation location, int cellwidth, int cellheight)
        {
            // Copy the cell point to avoid data corruption (should make CellPoint value type)
            this.Location = new GridLocation(location.Column, location.Row);
            this.CellHeight = cellheight;
            this.CellWidth = cellwidth;
        }
        public CellRectangle(int left, int top, int right, int bottom)
        {
            this.Location = new GridLocation(left, top);
            
            // Don't forget that the index math is off of the total dimensions by one.
            this.CellHeight = bottom - top + 1;
            this.CellWidth = right - left + 1;
        }
        
        public int Left { get { return this.Location.Column; } }
        public int Right { get { return (this.Location.Column + this.CellWidth) - 1; } }
        public int Top { get { return this.Location.Row; } }
        public int Bottom { get { return (this.Location.Row + this.CellHeight) - 1; } }

        public GridLocation TopLeft { get { return this.Location; } }
        public GridLocation TopRight { get { return new GridLocation(this.Right, this.Top); } }
        public GridLocation BottomRight { get { return new GridLocation(this.Right, this.Bottom); } }
        public GridLocation BottomLeft { get { return new GridLocation(this.Left, this.Bottom); } }

        public GridLocation[] Corners { get { return new GridLocation[] { this.TopLeft,
                                                                    this.TopRight,
                                                                    this.BottomRight,
                                                                    this.BottomLeft }; } }

        public GridLocation Center
        {
            get
            {
                var row = (int)((this.Top + this.Bottom) / 2D);
                var column = (int)((this.Left + this.Right) / 2D);

                if (this.CellHeight <= 1)
                    row = this.Top;

                if (this.CellWidth <= 1)
                    column = this.Left;

                return new GridLocation(column, row);
            }
        }

        public override string ToString()
        {
            return "X=" + Location.Column + " Y=" + Location.Row + " Width=" + CellWidth + " Height=" + CellHeight;
        }

        /// <summary>
        /// Expands rectangle to include provided location
        /// </summary>
        public void Expand(GridLocation location)
        {
            if (this.Left > location.Column)
                this.Location.Column = location.Column;

            if (this.Right < location.Column)
                this.CellWidth += (location.Column - this.Right);

            if (this.Top > location.Row)
                this.Location.Row = location.Row;

            if (this.Bottom < location.Row)
                this.CellHeight += (location.Row - this.Bottom);
        }
        /// <summary>
        /// Create a new cell rectangle by combining it with another - taking the extremum of both.
        /// </summary>
        /// <param name="rectangle"></param>
        public CellRectangle Union(CellRectangle rectangle)
        {
            var top = Math.Min(this.Top, rectangle.Top);
            var bottom = Math.Max(this.Bottom, rectangle.Bottom);
            var left = Math.Min(this.Left, rectangle.Left);
            var right = Math.Max(this.Right, rectangle.Right);

            return new CellRectangle(left, top, right, bottom);
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
        public bool Contains(GridLocation cellPoint)
        {
            if (cellPoint.Column < this.Left)
                return false;

            if (cellPoint.Column > this.Right)
                return false;

            if (cellPoint.Row < this.Top)
                return false;

            if (cellPoint.Row > this.Bottom)
                return false;

            return true;
        }
        /// <summary>
        /// Includes Boundary
        /// </summary>
        public bool Contains(int column, int row)
        {
            if (column < this.Left)
                return false;

            if (column > this.Right)
                return false;

            if (row < this.Top)
                return false;

            if (row > this.Bottom)
                return false;

            return true;
        }
        public bool Contains(CellRectangle cellRectangle)
        {
            if (cellRectangle.Right > this.Right)
                return false;

            if (cellRectangle.Left < this.Left)
                return false;

            if (cellRectangle.Top < this.Top)
                return false;

            if (cellRectangle.Bottom > this.Bottom)
                return false;

            return true;
        }
        public bool Overlaps(CellRectangle cellRectangle)
        {
            if (cellRectangle.Left > this.Right)
                return false;

            if (cellRectangle.Right < this.Left)
                return false;

            if (cellRectangle.Top > this.Bottom)
                return false;

            if (cellRectangle.Bottom < this.Top)
                return false;

            return true;
        }
        /// <summary>
        /// Calculates minimum distance between two rectangles - with overlapping rectangles defined as 0 
        /// distance
        /// </summary>
        public int RoguianDistance(CellRectangle cellRectangle)
        {
            var left = cellRectangle.Right < this.Left;
            var right = cellRectangle.Left > this.Right;
            var top = cellRectangle.Bottom < this.Top;
            var bottom = cellRectangle.Top > this.Bottom;

            if (left && top)
                return Calculator.RoguianDistance(this.Location, cellRectangle.BottomRight);
            else if (right && top)
                return Calculator.RoguianDistance(this.Location, cellRectangle.BottomLeft);
            else if (right && bottom)
                return Calculator.RoguianDistance(this.Location, cellRectangle.TopLeft);
            else if (left && bottom)
                return Calculator.RoguianDistance(this.Location, cellRectangle.TopRight);
            else if (left)
                return this.Left - cellRectangle.Right;
            else if (top)
                return this.Top - cellRectangle.Bottom;
            else if (right)
                return cellRectangle.Left - this.Right;
            else if (bottom)
                return cellRectangle.Top - this.Bottom;
            else
                return 0; // Overlapping
        }
        /// <summary>
        /// Calculates maximum possible distance between two cells within the rectangles.
        /// </summary>
        public int MaximumRoguianDistance(CellRectangle cellRectangle)
        {
            return this.Corners
                       .SelectMany(corner =>
                       {
                           return cellRectangle.Corners.Select(otherCorner => Calculator.RoguianDistance(corner, otherCorner));
                       })
                       .Max();
        }
    }
}
