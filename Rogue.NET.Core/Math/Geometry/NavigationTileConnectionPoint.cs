using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Math.Geometry
{
    /// <summary>
    /// Represents a single connection between two neighboring tiles. These MUST BORDER one-another with more than just
    /// a single vertex. There must be some edge overlap of at least one cell.
    /// </summary>
    public class NavigationTileConnectionPoint
    {
        List<int> _routeNumbers;

        /// <summary>
        /// Adjacent tile for this connection point
        /// </summary>
        public NavigationTile AdjacentTile { get; private set; }

        /// <summary>
        /// Connection point between the two tiles ON THE PRIMARY TILE
        /// </summary>
        public VertexInt ConnectionPoint { get; private set; }

        /// <summary>
        /// Direction to the adjacent tile
        /// </summary>
        public Compass Direction { get; private set; }

        public IEnumerable<int> RouteNumbers
        {
            get { return _routeNumbers; }
        }

        /// <summary>
        /// Creates a connection point to the specified adjacent tile using the connection point on
        /// THIS parent tile.
        /// </summary>
        /// <param name="adjacentTile">Tile adjacent to parent tile</param>
        /// <param name="connectionPoint">Point on the parent tile where it connects to the adjacent tile</param>
        /// <param name="direction">Direction of the adjacent tile</param>
        public NavigationTileConnectionPoint(NavigationTile adjacentTile, VertexInt connectionPoint, Compass direction)
        {
            switch (direction)
            {
                case Compass.N:
                case Compass.S:
                case Compass.E:
                case Compass.W:
                    break;
                default:
                    throw new ArgumentException("Trying to set non-cardinal direction for a navigation tile connection point");
            }

            this.AdjacentTile = adjacentTile;
            this.ConnectionPoint = connectionPoint;
            this.Direction = direction;

            _routeNumbers = new List<int>();
        }

        /// <summary>
        /// Adds a route number to this connection point
        /// </summary>
        /// <param name="number">A unqiue route number - this is to track which connection tile routes this tile is involved in.</param>
        public void IncludeRouteNumber(int number)
        {
            if (!_routeNumbers.Contains(number))
                _routeNumbers.Add(number);
        }

        public override string ToString()
        {
            return "Direction= {" + this.Direction + "} ConnectionPoint= {" + this.ConnectionPoint + "}";
        }
    }
}
