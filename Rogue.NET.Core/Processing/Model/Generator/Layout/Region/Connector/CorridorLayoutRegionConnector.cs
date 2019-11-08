using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Connector
{
    public static class CorridorLayoutRegionConnector
    {
        static readonly IRandomSequenceGenerator _randomSequenceGenerator;

        static CorridorLayoutRegionConnector()
        {
            _randomSequenceGenerator = ServiceLocator.Current.GetInstance<IRandomSequenceGenerator>();
        }

        public static void Connect(GridCellInfo[,] grid, GridCellInfo cell1, GridCellInfo cell2, LayoutTemplate template)
        {
            // Check for neighboring (cardinal) or same-identity cell. For this case, just return an empty array
            if (cell1.Location.Equals(cell2.Location) ||
                grid.GetCardinalAdjacentElements(cell1.Location.Column, cell1.Location.Row)
                    .Any(x => x.Equals(cell2.Location)))
                return;

            var corridor = new List<GridCellInfo>();
            var createDoors = template.ConnectionType == LayoutConnectionType.CorridorWithDoors;

            // Procedure
            //
            // 0) Create ray from center of cell1 to the center of cell2
            // 1) Create integer "grid lines" to locate intersections
            // 2) Add intersecting cells to the result

            var physicalLocation1 = TransformToPhysicalLayout(cell1.Location);
            var physicalLocation2 = TransformToPhysicalLayout(cell2.Location);

            var minX = (int)System.Math.Min(physicalLocation1.X, physicalLocation2.X);
            var minY = (int)System.Math.Min(physicalLocation1.Y, physicalLocation2.Y);
            var rangeX = (int)System.Math.Abs(physicalLocation2.X - physicalLocation1.X);
            var rangeY = (int)System.Math.Abs(physicalLocation2.Y - physicalLocation1.Y);

            // Offset to the center of the cell
            physicalLocation1.X += ModelConstants.CellWidth / 2.0F;
            physicalLocation1.Y += ModelConstants.CellHeight / 2.0F;

            physicalLocation2.X += ModelConstants.CellWidth / 2.0F;
            physicalLocation2.Y += ModelConstants.CellHeight / 2.0F;

            // Calculate ray parameters
            var slope = (physicalLocation2.Y - physicalLocation1.Y) / ((double)(physicalLocation2.X - physicalLocation1.X));
            var intercept = physicalLocation1.Y - (slope * physicalLocation1.X);

            var directionX = System.Math.Sign(physicalLocation2.X - physicalLocation1.X);
            var directionY = System.Math.Sign(physicalLocation2.Y - physicalLocation1.Y);

            // Grid lines don't include the originating cells
            var xLines = new List<float>();
            var yLines = new List<float>();

            // START ONE OFF THE MIN (Have to draw this out to see why...)
            var xCounter = minX + ModelConstants.CellWidth;
            var yCounter = minY + ModelConstants.CellHeight;

            while (xCounter <= minX + rangeX)
            {
                xLines.Add(xCounter);
                xCounter += ModelConstants.CellWidth;
            }

            while (yCounter <= minY + rangeY)
            {
                yLines.Add(yCounter);
                yCounter += ModelConstants.CellHeight;
            }

            // Store intersections in a list and use them to calculate the corridor
            var intersections = new List<Vertex>();

            // Calculate all intersections of the ray defined above with the grid lines
            foreach (var xLine in xLines)
            {
                if (!double.IsInfinity(slope))
                    intersections.Add(new Vertex(xLine, (float)((slope * xLine) + intercept)));
            }
            foreach (var yLine in yLines)
            {
                // For this case - add the horizontal lines as intersections
                if (double.IsInfinity(slope))
                    intersections.Add(new Vertex(physicalLocation1.X, yLine));

                else if (slope != 0D)
                    intersections.Add(new Vertex((float)((yLine - intercept) / slope), yLine));
            }


            // Order the intersections by their distance from the first point
            intersections = intersections.Where(point =>
            {
                // Need to filter out intersections that fell outside the bounds of the grid. This 
                // can happen because of an extraneous grid line. Extraneous grid lines are 
                // off-by-one errors from calculating the horizontal / vertical lines. These are
                // automatically filtered out below because the algorithm checks for cells already
                // alive in the grid before creating new ones. Also, no extra cells are built because
                // the error is only ever off-by-one.
                //
                // However, for small or near-infinity slopes, off-by-one errors can add a point that
                // is outside the bounds of the grid. So, those must filtered off prior to entering 
                // the loop below.
                var location = new GridLocation((int)System.Math.Floor(point.X / ModelConstants.CellWidth),
                                                (int)System.Math.Floor(point.Y / ModelConstants.CellHeight));

                if (location.Column < 0 ||
                    location.Column >= grid.GetLength(0))
                    return false;

                else if (location.Row < 0 ||
                         location.Row >= grid.GetLength(1))
                    return false;

                return true;
            })
            .OrderBy(point => point.EuclideanSquareDistance(physicalLocation1))
            .ToList();

            // Trace along the ray and get the cells that are going to be part of the corridor
            foreach (var point in intersections)
            {
                // cell location of intersection
                var location = new GridLocation((int)System.Math.Floor(point.X / ModelConstants.CellWidth),
                                                (int)System.Math.Floor(point.Y / ModelConstants.CellHeight));

                // intersection landed on one of the vertical lines
                var horizontal = (point.X % ModelConstants.CellWidth) <= double.Epsilon;

                GridCellInfo gridCell1 = null;
                GridCellInfo gridCell2 = null;
                GridCellInfo corridor1 = null;
                GridCellInfo corridor2 = null;

                // Using the ray direction and the intersection - calculate the next two points to include in the
                // result
                if (horizontal)
                {
                    var location1 = directionX > 0 ? new GridLocation(location.Column - 1, location.Row) :
                                                     new GridLocation(location.Column, location.Row);

                    var location2 = directionX > 0 ? new GridLocation(location.Column, location.Row) :
                                                     new GridLocation(location.Column - 1, location.Row);

                    gridCell1 = grid[location1.Column, location1.Row];
                    gridCell2 = grid[location2.Column, location2.Row];

                    corridor1 = gridCell1 ??
                                corridor.FirstOrDefault(cell => cell.Location.Equals(location1)) ??
                                new GridCellInfo(location1) { IsWall = false };

                    corridor2 = gridCell2 ??
                                corridor.FirstOrDefault(cell => cell.Location.Equals(location2)) ??
                                new GridCellInfo(location2) { IsWall = false };
                }
                else
                {
                    var location1 = directionY > 0 ? new GridLocation(location.Column, location.Row - 1) :
                                                     new GridLocation(location.Column, location.Row);

                    var location2 = directionY > 0 ? new GridLocation(location.Column, location.Row) :
                                                     new GridLocation(location.Column, location.Row - 1);

                    gridCell1 = grid[location1.Column, location1.Row];
                    gridCell2 = grid[location2.Column, location2.Row];

                    corridor1 = gridCell1 ??
                                corridor.FirstOrDefault(cell => cell.Location.Equals(location1)) ??
                                new GridCellInfo(location1) { IsWall = false };

                    corridor2 = gridCell2 ??
                                corridor.FirstOrDefault(cell => cell.Location.Equals(location2)) ??
                                new GridCellInfo(location2) { IsWall = false };
                }

                // Create a door (if the location is on the edge of a room)
                if (createDoors)
                {
                    // Criteria for door
                    //
                    // 1) Grid cell is not yet in the corridor
                    // 2) Cardinally adjacent cell (N,S,E,W) is in a room (not the corridor)
                    if (gridCell1 == null)
                    {
                        foreach (var adjacentRoomCell in grid.GetCardinalAdjacentElements(corridor1.Location.Column, corridor1.Location.Row)
                                                             .Where(cell => grid[cell.Location.Column, cell.Location.Row] != null))
                        {
                            var direction = GridUtility.GetDirectionOfAdjacentLocation(corridor1.Location, adjacentRoomCell.Location);
                            var oppositeDirection = GridUtility.GetOppositeDirection(direction);

                            // Set up hidden door parameters for both "sides" of the door.
                            var hiddenDoor = _randomSequenceGenerator.Get() < template.HiddenDoorProbability;
                            var counter = hiddenDoor ? _randomSequenceGenerator.Get(1, 20) : 0;

                            // Set door on the corridor and the adjacent room cell
                            corridor1.IsDoor = true;
                            corridor1.IsWall = false;
                            corridor1.DoorSearchCounter = counter;
                        }
                    }
                    if (gridCell2 == null)
                    {
                        foreach (var adjacentRoomCell in grid.GetCardinalAdjacentElements(corridor2.Location.Column, corridor2.Location.Row)
                                                             .Where(cell => grid[cell.Location.Column, cell.Location.Row] != null))
                        {
                            var direction = GridUtility.GetDirectionOfAdjacentLocation(corridor2.Location, adjacentRoomCell.Location);
                            var oppositeDirection = GridUtility.GetOppositeDirection(direction);

                            // Set up hidden door parameters for both "sides" of the door.
                            var hiddenDoor = _randomSequenceGenerator.Get() < template.HiddenDoorProbability;
                            var counter = hiddenDoor ? _randomSequenceGenerator.Get(1, 20) : 0;

                            // Set door on the corridor and the adjacent room cell
                            corridor2.IsDoor = true;
                            corridor2.IsWall = false;
                            corridor2.DoorSearchCounter = counter;
                        }
                    }
                }

                // Punch out the walls
                corridor1.IsWall = false;
                corridor2.IsWall = false;

                // Store the results
                if (!corridor.Any(cell => cell == corridor1) && gridCell1 == null)
                    corridor.Add(corridor1);

                if (!corridor.Any(cell => cell == corridor2) && gridCell2 == null)
                    corridor.Add(corridor2);

                // Add to the grid if they're not yet there
                grid[corridor1.Location.Column, corridor1.Location.Row] = corridor1;
                grid[corridor2.Location.Column, corridor2.Location.Row] = corridor2;
            }

            // Validating the corridor
            if (corridor.Any(x => x == cell1 || x == cell2))
                throw new Exception("Invalid corridor created");

            //if (createDoors && (!cell1.IsDoor || !cell2.IsDoor))
            //    throw new Exception("Invalid corridor created");

            if (grid[cell1.Location.Column, cell1.Location.Row] != cell1 ||
                grid[cell2.Location.Column, cell2.Location.Row] != cell2)
                throw new Exception("Invalid corridor created");
        }

        public static Vertex TransformToPhysicalLayout(GridLocation location)
        {
            float x = (float)(ModelConstants.CellWidth * location.Column);
            float y = (float)(ModelConstants.CellHeight * location.Row);
            return new Vertex(x, y);
        }
    }
}
