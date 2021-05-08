using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class RegionBoundary
    {
        public int Column { get; set; }
        public int Row { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public RegionBoundary()
        {
            this.Column = -1;
            this.Row = -1;
            this.Width = -1;
            this.Height = -1;
        }
        public RegionBoundary(IGridLocator location, int cellwidth, int cellheight)
        {
            this.Column = location.Column;
            this.Row = location.Row;
            this.Height = cellheight;
            this.Width = cellwidth;
        }
        public RegionBoundary(int column, int row, int cellwidth, int cellheight)
        {
            this.Column = column;
            this.Row = row;
            this.Height = cellheight;
            this.Width = cellwidth;
        }

        public int Left { get { return this.Column; } }
        public int Right { get { return (this.Column + this.Width) - 1; } }
        public int Top { get { return this.Row; } }
        public int Bottom { get { return (this.Row + this.Height) - 1; } }

        public GridLocation GetCenter()
        {
            return new GridLocation(this.Left + (this.Width / 2),
                                    this.Top + (this.Height / 2));
        }

        public IEnumerable<GridLocation> GetCorners()
        {
            return new GridLocation[]
            {
                new GridLocation(this.Left, this.Top),
                new GridLocation(this.Right, this.Top),
                new GridLocation(this.Left, this.Bottom),
                new GridLocation(this.Right, this.Bottom)
            };
        }

        public override string ToString()
        {
            return "X=" + Column + " Y=" + Row + " Width=" + Width + " Height=" + Height;
        }

        public override bool Equals(object obj)
        {
            if (obj is RegionBoundary)
            {
                var boundary = obj as RegionBoundary;

                return this.Column == boundary.Column &&
                       this.Row == boundary.Row &&
                       this.Height == boundary.Height &&
                       this.Width == boundary.Width;
            }

            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Includes the edge
        /// </summary>
        public bool Contains(IGridLocator location)
        {
            return Contains(location.Column, location.Row);
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
        public bool Contains(RegionBoundary cellRectangle)
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
        public bool Overlaps(RegionBoundary boundary)
        {
            if (boundary.Left > this.Right)
                return false;

            if (boundary.Right < this.Left)
                return false;

            if (boundary.Top > this.Bottom)
                return false;

            if (boundary.Bottom < this.Top)
                return false;

            return true;
        }

        public bool Touches(RegionBoundary boundary)
        {
            if (Overlaps(boundary))
                return true;

            // Expand by one cell and re-calculate overlaps
            //
            var topLeft = new GridLocation(this.Left - 1, this.Top - 1);
            var biggerBoundary = new RegionBoundary(topLeft, this.Width + 2, this.Height + 2);

            return biggerBoundary.Overlaps(boundary);
        }

        public bool IsLeftOf(RegionBoundary boundary)
        {
            return this.Right < boundary.Left;
        }
        public bool IsRightOf(RegionBoundary boundary)
        {
            return this.Left > boundary.Right;
        }
        public bool IsBelow(RegionBoundary boundary)
        {
            return this.Top > boundary.Bottom;
        }
        public bool IsAbove(RegionBoundary boundary)
        {
            return this.Bottom < boundary.Top;
        }
    }
}
