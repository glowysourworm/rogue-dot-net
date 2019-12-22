using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rogue.NET.Core.Processing.Model.Content.Calculator
{
    /// <summary>
    /// Static methods dealing with some of the grid calculations
    /// </summary>
    public static class GridCalculator
    {
        public static Compass GetDirectionOfAdjacentLocation(GridLocation location, GridLocation adjacentLocation)
        {
            var north = (adjacentLocation.Row - location.Row) == -1;
            var south = (adjacentLocation.Row - location.Row) == 1;
            var east = (adjacentLocation.Column - location.Column) == 1;
            var west = (adjacentLocation.Column - location.Column) == -1;

            if (north && east) return Compass.NE;
            else if (north && west) return Compass.NW;
            else if (south && east) return Compass.SE;
            else if (south && west) return Compass.SW;
            else if (north) return Compass.N;
            else if (south) return Compass.S;
            else if (east) return Compass.E;
            else if (west) return Compass.W;
            else
                throw new Exception("Invalid adjacent cell GetDirectionOfAdjacentLocation");
        }

        public static Compass GetOppositeDirection(Compass direction)
        {
            switch (direction)
            {
                case Compass.N:
                    return Compass.S;
                case Compass.S:
                    return Compass.N;
                case Compass.E:
                    return Compass.W;
                case Compass.W:
                    return Compass.E;
                case Compass.NE:
                    return Compass.SW;
                case Compass.NW:
                    return Compass.SE;
                case Compass.SE:
                    return Compass.SW;
                case Compass.SW:
                    return Compass.SE;
                default:
                    return Compass.Null;
            }
        }

        public static bool IsCardinalDirection(Compass direction)
        {
            switch (direction)
            {
                case Compass.N:
                case Compass.S:
                case Compass.E:
                case Compass.W:
                    return true;

                case Compass.Null:
                case Compass.NW:
                case Compass.NE:
                case Compass.SE:
                case Compass.SW:
                default:
                    return false;
            }
        }

        public static Rect TransformToPhysicalLayout(RegionBoundary boundary)
        {
            float x = (float)(ModelConstants.CellWidth * boundary.Left);
            float y = (float)(ModelConstants.CellHeight * boundary.Top);
            return new Rect(x, y, boundary.Width * ModelConstants.CellWidth, boundary.Height * ModelConstants.CellHeight);
        }

        public static GridLocation TransformFromPhysicalLayout(double x, double y)
        {
            var column = (int)(x / ModelConstants.CellWidth);
            var row = (int)(y / ModelConstants.CellHeight);
            return new GridLocation(column, row);
        }
    }
}
