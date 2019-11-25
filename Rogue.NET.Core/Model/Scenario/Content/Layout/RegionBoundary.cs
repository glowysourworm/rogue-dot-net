using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Math.Geometry.Interface;
using Rogue.NET.Core.Processing.Model.Static;
using System;

using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [Serializable]
    public class RegionBoundary
    {
        public GridLocation Location { get; set; }
        public int CellHeight { get; set; }
        public int CellWidth { get; set; }

        public RegionBoundary()
        {
            this.Location = new GridLocation(-1, -1);
            this.CellWidth = -1;
            this.CellHeight = -1;
        }
        public RegionBoundary(GridLocation location, int cellwidth, int cellheight)
        {
            // Copy the cell point to avoid data corruption (should make CellPoint value type)
            this.Location = new GridLocation(location.Column, location.Row);
            this.CellHeight = cellheight;
            this.CellWidth = cellwidth;
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

        public int Area
        {
            get { return this.CellWidth * this.CellHeight; }
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
                this.Location = new GridLocation(location.Column, this.Location.Row);

            if (this.Right < location.Column)
                this.CellWidth += (location.Column - this.Right);

            if (this.Top > location.Row)
                this.Location = new GridLocation(this.Location.Column, location.Row);

            if (this.Bottom < location.Row)
                this.CellHeight += (location.Row - this.Bottom);
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(RegionBoundary))
            {
                return (this.Location.Equals(((RegionBoundary)obj).Location)
                    && this.CellHeight == ((RegionBoundary)obj).CellHeight && this.CellWidth == ((RegionBoundary)obj).CellWidth);
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// Includes the edge
        /// </summary>
        public bool Contains(GridLocation cellPoint)
        {
            return Contains(cellPoint.Column, cellPoint.Row);
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
            var biggerBoundary = new RegionBoundary(topLeft, this.CellWidth + 2, this.CellHeight + 2);

            return biggerBoundary.Overlaps(boundary);
        }
    }
}
