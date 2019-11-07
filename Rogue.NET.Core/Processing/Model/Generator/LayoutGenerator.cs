﻿using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Design;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Region;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Connector;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Creator;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Geometry;
using Rogue.NET.Core.Processing.Model.Static;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using RegionModel = Rogue.NET.Core.Model.Scenario.Content.Layout.Region;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ILayoutGenerator))]
    public class LayoutGenerator : ILayoutGenerator
    {
        private readonly IRandomSequenceGenerator _randomSequenceGenerator;

        // Size is great enough for an upstairs, downstairs, and 2 teleport pods (in and out of the room)
        private const int ROOM_SIZE_MIN = 4;

        // NOTE** Not used for all layout types. It was not required for certain types
        //        that had other padding involved (Rectangular Grid); or no padding (Maze).
        //
        //        MUST BE GREATER THAN OR EQUAL TO 2.
        private const int CELLULAR_AUTOMATA_PADDING = 2;
        private const int CELLULAR_AUTOMATA_ITERATIONS = 5;


        [ImportingConstructor]
        public LayoutGenerator(IRandomSequenceGenerator randomSequenceGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public IEnumerable<Level> CreateLayouts(IEnumerable<string> levelBranchNames, IEnumerable<LayoutGenerationTemplate> layoutTemplates)
        {
            var levels = new List<Level>();

            var levelNumber = 1;
            foreach (var template in layoutTemplates)
            {
                var levelBranchName = levelBranchNames.ElementAt(levelNumber - 1);

                var grid = CreateLayout(template.Asset);
                var level = new Level(levelBranchName,
                                      grid,
                                      template.Asset.Type,
                                      template.Asset.ConnectionType,
                                      levelNumber++,
                                      template.Asset.WallColor,
                                      template.Asset.DoorColor);
                levels.Add(level);
            }
            return levels;
        }

        private LevelGrid CreateLayout(LayoutTemplate template)
        {
            switch (template.Type)
            {
                case LayoutType.Maze:
                    return CreateMaze(template);
                case LayoutType.ConnectedRectangularRooms:
                    return CreateConnectedRectangularRooms(template);
                case LayoutType.ConnectedCellularAutomata:
                    return CreateCellularAutomata(template);
                default:
                    throw new Exception("Unhandled Layout Type");
            }
        }

        private LevelGrid CreateConnectedRectangularRooms(LayoutTemplate template)
        {
            switch (template.RoomPlacementType)
            {
                case LayoutRoomPlacementType.RectangularGrid:
                    return CreateRectangularGridRooms(template);
                case LayoutRoomPlacementType.Random:
                    return CreateRandomRooms(template);
                default:
                    throw new Exception("Unhandled Room Placement Type");
            }
        }

        private LevelGrid CreateRectangularGridRooms(LayoutTemplate template)
        {
            var grid = new Cell[template.Width, template.Height];

            // Create the room rectangles
            var roomBoundaries = RectangularGridRegionGeometryCreator.CreateRegionGeometry(template);

            // Create cells in the contiguous rectangle groups
            var regions = roomBoundaries.Select(boundary => GridUtility.CreateRectangularRegion(grid, boundary, false, true))
                                        .Actualize();

            return FinishLayoutRectilinear(grid, regions, template);
        }

        private LevelGrid CreateRandomRooms(LayoutTemplate template)
        {
            var grid = new Cell[template.Width, template.Height];

            // Create the room rectangles
            var roomBoundaries = RandomRectangularRegionGeometryCreator.CreateRegionGeometry(template);

            // Create contiguous rectangle groups
            var contiguousBoundaries = ContiguousRegionGeometryCreator.CreateContiguousRegionGeometry(roomBoundaries);

            // Create cells in the contiguous rectangle groups
            var regions = ContiguousRegionCreator.CreateRegions(grid, contiguousBoundaries, true);

            return FinishLayoutRectilinear(grid, regions, template);
        }

        private LevelGrid CreateCellularAutomata(LayoutTemplate template)
        {
            var grid = new Cell[template.Width, template.Height];

            // Create cells in the contiguous rectangle groups
            var regions = CellularAutomataRegionCreator.CreateRegions(grid, template.CellularAutomataType == LayoutCellularAutomataType.Filled, template.CellularAutomataFillRatio.Clip(0.4, 0.4));

            return FinishLayout(grid, regions, template);
        }

        private LevelGrid CreateMaze(LayoutTemplate template)
        {
            var grid = new Cell[template.Width, template.Height];

            var region = GridUtility.CreateRectangularRegion(grid, 0, 0, template.Width, template.Height, false, true);

            // Set region cells to be walls
            foreach (var cell in region.Cells)
                grid[cell.Column, cell.Row].SetWall(true);

            // Create maze by "punching out walls"
            //
            MazeRegionCreator.CreateMaze(grid, template.NumberExtraWallRemovals);

            return new LevelGrid(grid, new RegionModel[] { region }, new RegionModel[] { });
        }

        private LevelGrid CreateOpenWorldLayout(LayoutTemplate template)
        {
            var grid = new Cell[template.Width, template.Height];

            // To avoid extra iteration - use the callback to set up the grid cells
            var featureMap = NoiseGenerator.GeneratePerlinNoise(template.Width, template.Height, 0.7, new NoiseGenerator.PostProcessingFilterCallback((column, row, value) =>
            {
                // Use the loop to create the grid and save an iteration - mark valleys as "Walls" to be carved out later
                grid[column, row] = new Cell(column, row, value < 0);

                // Create "walls" for regions by weighting the result to prevent BFS from using these cells
                // return value > 0 ? 10000 : value;

                return value;
            }));

            var regions = grid.IdentifyRegions();

            // Calculate rectangles for use with Prim's algorithm to create MST
            var rectangleDict = regions.ToDictionary(region => new RectangleInt(new VertexInt(region.Bounds.TopLeft), new VertexInt(region.Bounds.BottomRight)), region => region);

            // Create MST
            var minimumSpanningTree = GeometryUtility.PrimsMinimumSpanningTree(rectangleDict.Keys.Select(rectangle => new ReferencedVertex<RectangleInt>(rectangle, new Vertex(rectangle.Center))), Metric.MetricType.Roguian);

            // Create connections by trying to "follow the valleys"
            foreach (var edge in minimumSpanningTree.Edges)
            {
                var region1 = rectangleDict[edge.Point1.Reference];
                var region2 = rectangleDict[edge.Point2.Reference];

                //var location1 = _randomSequenceGenerator.GetRandomElement(region1.EdgeCells);
                //var location2 = _randomSequenceGenerator.GetRandomElement(region2.EdgeCells);

                Cell region1Cell, region2Cell;
                CorridorLocationCalculator.CalculateNearestNeighborLocations(grid, template, region1, region2, out region1Cell, out region2Cell);

                var location1 = region1Cell.Location;
                var location2 = region2Cell.Location;

                // Creates dijkstra
                var dijkstraMap = featureMap.CreateDijkstraMap(new VertexInt(location1), new VertexInt(location2));

                var currentLocation = new VertexInt(location2);
                var goalLocation = new VertexInt(location1);

                // Find the "easiest" route to the goal
                while (!currentLocation.Equals(goalLocation))
                {
                    var column = currentLocation.X;
                    var row = currentLocation.Y;

                    var north = row - 1 >= 0;
                    var south = row + 1 < grid.GetLength(1);
                    var east = column + 1 < grid.GetLength(0);
                    var west = column - 1 >= 0;

                    double lowestWeight = double.MaxValue;
                    VertexInt lowestWeightLocation = currentLocation;

                    if (north && dijkstraMap[column, row - 1] < lowestWeight)
                    {
                        lowestWeightLocation = new VertexInt(column, row - 1);
                        lowestWeight = dijkstraMap[column, row - 1];
                    }

                    if (south && dijkstraMap[column, row + 1] < lowestWeight)
                    { 
                        lowestWeightLocation = new VertexInt(column, row + 1);
                        lowestWeight = dijkstraMap[column, row + 1];
                    }

                    if (east && dijkstraMap[column + 1, row] < lowestWeight)
                    { 
                        lowestWeightLocation = new VertexInt(column + 1, row);
                        lowestWeight = dijkstraMap[column + 1, row];
                    }

                    if (west && dijkstraMap[column - 1, row] < lowestWeight)
                    { 
                        lowestWeightLocation = new VertexInt(column - 1, row);
                        lowestWeight = dijkstraMap[column - 1, row];
                    }

                    if (north && east && dijkstraMap[column + 1, row - 1] < lowestWeight)
                    {
                        lowestWeightLocation = new VertexInt(column + 1, row - 1);
                        lowestWeight = dijkstraMap[column + 1, row - 1];
                    }

                    if (north && west && dijkstraMap[column - 1, row - 1] < lowestWeight)
                    {
                        lowestWeightLocation = new VertexInt(column - 1, row - 1);
                        lowestWeight = dijkstraMap[column - 1, row - 1];
                    }

                    if (south && east && dijkstraMap[column + 1, row + 1] < lowestWeight)
                    {
                        lowestWeightLocation = new VertexInt(column + 1, row + 1);
                        lowestWeight = dijkstraMap[column + 1, row + 1];
                    }

                    if (south && west && dijkstraMap[column - 1, row + 1] < lowestWeight)
                    {
                        lowestWeightLocation = new VertexInt(column - 1, row + 1);
                        lowestWeight = dijkstraMap[column - 1, row + 1];
                    }

                    if (lowestWeight == double.MaxValue)
                        throw new Exception("Mishandled Dijkstra Map LayoutGenerator.CreateOrganic");

                    currentLocation = lowestWeightLocation;

                    // Remove Wall from this cell
                    grid[column, row].SetWall(false);

                    // For diagonal movements - must also set one of the corresponding cardinal cells to be part of the corridor

                    // NE
                    if ((lowestWeightLocation.X == column + 1) && (lowestWeightLocation.Y == row - 1))
                    {
                        // Select the N or E cell to also remove the wall
                        if (dijkstraMap[column, row - 1] < dijkstraMap[column + 1, row])
                            grid[column, row - 1].SetWall(false);

                        else
                            grid[column + 1, row].SetWall(false);
                    }
                    // NW
                    else if ((lowestWeightLocation.X == column - 1) && (lowestWeightLocation.Y == row - 1))
                    {
                        // Select the N or W cell to also remove the wall
                        if (dijkstraMap[column, row - 1] < dijkstraMap[column - 1, row])
                            grid[column, row - 1].SetWall(false);

                        else
                            grid[column - 1, row].SetWall(false);
                    }
                    // SE
                    else if ((lowestWeightLocation.X == column + 1) && (lowestWeightLocation.Y == row + 1))
                    {
                        // Select the S or E cell to also remove the wall
                        if (dijkstraMap[column, row + 1] < dijkstraMap[column + 1, row])
                            grid[column, row + 1].SetWall(false);

                        else
                            grid[column + 1, row].SetWall(false);
                    }
                    // SW
                    else if ((lowestWeightLocation.X == column - 1) && (lowestWeightLocation.Y == row + 1))
                    {
                        // Select the S or W cell to also remove the wall
                        if (dijkstraMap[column, row + 1] < dijkstraMap[column - 1, row])
                            grid[column, row + 1].SetWall(false);

                        else
                            grid[column - 1, row].SetWall(false);
                    }
                }
            }

            return new LevelGrid(grid, regions.ToArray(), new RegionModel[] { });
        }

        #region (private) Layout Finishing

        /// <summary>
        /// Triangulate rooms, locate and remove small rooms, create corridors, add walls
        /// </summary>
        /// <returns></returns>
        private LevelGrid FinishLayout(Cell[,] grid, IEnumerable<RegionModel> regions, LayoutTemplate template)
        {
            // Triangulate room positions (Delaunay Triangulation)
            //
            var graph = GeometryUtility.PrimsMinimumSpanningTree(regions.Select(x =>
            {
                var topLeft = new VertexInt(x.Bounds.Left, x.Bounds.Top);
                var bottomRight = new VertexInt(x.Bounds.Right, x.Bounds.Bottom);

                return new ReferencedVertex<RectangleInt>(new RectangleInt(topLeft, bottomRight), new Vertex(x.Bounds.Center.Column, x.Bounds.Center.Row));

            }), Metric.MetricType.Roguian);

            // For each edge in the triangulation - create a corridor
            //
            foreach (var edge in graph.Edges)
            {
                var room1 = regions.First(x => x.Bounds.Center.Column == (int)edge.Point1.Vertex.X &&
                                               x.Bounds.Center.Row == (int)edge.Point1.Vertex.Y);

                var room2 = regions.First(x => x.Bounds.Center.Column == (int)edge.Point2.Vertex.X &&
                                               x.Bounds.Center.Row == (int)edge.Point2.Vertex.Y);

                // Calculate nearest neighbor cells
                //
                Cell cell1, cell2;
                CorridorLocationCalculator.CalculateNearestNeighborLocations(grid, template, room1, room2, out cell1, out cell2);

                // Create the corridor cells
                //
                CorridorLayoutRegionConnector.Connect(grid, cell1, cell2, template);
            }

            //Create walls
            CreateWalls(grid);

            return new LevelGrid(grid, regions.ToArray(), new RegionModel[] { });
        }

        private LevelGrid FinishLayoutRectilinear(Cell[,] grid, IEnumerable<RegionModel> regions, LayoutTemplate template)
        {
            // Calculate rectangles for use with Prim's algorithm to create MST
            var rectangles = regions.Select(region => new RectangleInt(new VertexInt(region.Bounds.TopLeft), new VertexInt(region.Bounds.BottomRight)));

            // Create MST
            var minimumSpanningTree = GeometryUtility.PrimsMinimumSpanningTree(rectangles.Select(rectangle => new ReferencedVertex<RectangleInt>(rectangle, new Vertex(rectangle.Center))), 
                                                                                                                  Metric.MetricType.Roguian);

            // Create connections by drawing wide linear connector
            foreach (var edge in minimumSpanningTree.Edges)
            {
                var includedPoints = new List<VertexInt>();

                var left = (int)System.Math.Min(edge.Point1.Vertex.X, edge.Point2.Vertex.X);
                var right = (int)System.Math.Max(edge.Point1.Vertex.X, edge.Point2.Vertex.X);
                var top = (int)System.Math.Min(edge.Point1.Vertex.Y, edge.Point2.Vertex.Y);
                var bottom = (int)System.Math.Max(edge.Point1.Vertex.Y, edge.Point2.Vertex.Y);
                var vertices = new VertexInt[]
                {
                    new VertexInt(left, top),
                    new VertexInt(right, top),
                    new VertexInt(right, bottom),
                    new VertexInt(left, bottom)
                };

                // Add points that are part of one of the rooms
                foreach (var vertex in vertices)
                {
                    if (edge.Point1.Reference.Contains(vertex) ||
                        edge.Point2.Reference.Contains(vertex))
                        includedPoints.Add(vertex);
                }

                // Check to see if any of the vertices lies outside one of the rooms
                var midPoint = _randomSequenceGenerator.Get() > 0.5 ? vertices.FirstOrDefault(vertex => !includedPoints.Contains(vertex)) :
                                                                      vertices.LastOrDefault(vertex => !includedPoints.Contains(vertex));

                // If there's an exterior point, then use it as the mid point for the corridor
                //
                // NOTE** This is a "null" check (for the struct)
                if (vertices.Contains(midPoint))
                {
                    TilingCorridorRegionConnector.CreateRectilinearRoutePoints(grid, 
                                                                               new VertexInt(edge.Point1.Vertex), 
                                                                               midPoint, 
                                                                               edge.Point1.Vertex.Y != midPoint.Y);

                    TilingCorridorRegionConnector.CreateRectilinearRoutePoints(grid, 
                                                                               midPoint, 
                                                                               new VertexInt(edge.Point2.Vertex), 
                                                                               edge.Point2.Vertex.Y != midPoint.Y);
                }

                // Otherwise, just draw a line from one region to the other
                else
                {
                    // NOTE*** Since all vertices lie within both regions - just draw a straight line connecting
                    //         one of the off-diagonal vertices to the opposing center
                    var northSouthOriented = edge.Point1.Reference.Bottom < edge.Point2.Reference.Top ||
                                             edge.Point1.Reference.Top > edge.Point2.Reference.Bottom;

                    // Point1 -> Point 2 (off-diangonal or the actual center)
                    TilingCorridorRegionConnector.CreateRectilinearRoutePoints(grid, 
                                                                               new VertexInt(edge.Point1.Vertex),
                                                                               new VertexInt(edge.Point2.Vertex), 
                                                                               northSouthOriented);
                }
            }

            CreateWalls(grid);

            //return new LevelGrid(grid, regions.ToArray(), tiledRegions.ToArray());
            return new LevelGrid(grid, regions.ToArray(), new RegionModel[] { });
        }

        // Credit to this fellow for the idea for maze corridors!
        //
        // https://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/
        // https://github.com/munificent/hauberk/blob/db360d9efa714efb6d937c31953ef849c7394a39/lib/src/content/dungeon.dart
        //
        private LevelGrid FinishLayoutWithMazeCorridors(Cell[,] grid, IEnumerable<RegionModel> regions, LayoutTemplate template)
        {
            // Fill in the empty cells with walls
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // Create wall cell here
                    if (grid[i, j] == null)
                        grid[i, j] = new Cell(i, j, true);
                }
            }

            // Create a recursive-backtrack corridor in cell that contains 8-way walls. Continue until entire map is
            // considered.

            // Find empty regions and fill them with recrusive-backtracked corridors
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // Create a corridor here
                    if (grid.GetAdjacentElementsUnsafe(i, j).Count(cell => cell.IsWall) == 8)
                        MazeRegionCreator.CreateMaze(grid, i, j, 0);
                }
            }

            // Finally, connect the regions

            var finalRegions = grid.IdentifyRegions();

            while (finalRegions.Count() > 1)
            {
                var region1 = finalRegions.ElementAt(0);
                var region2 = finalRegions.ElementAt(1);

                var distanceLocations = region1.EdgeCells.SelectMany(cell1 => region2.EdgeCells.Select(cell2 => new
                {
                    Distance = Metric.EuclideanSquareDistance(cell1, cell2),
                    Cell1 = grid[cell1.Column, cell1.Row],
                    Cell2 = grid[cell2.Column, cell2.Row]
                }));

                var distanceLocation = distanceLocations.MinBy(x => x.Distance);
                CorridorLayoutRegionConnector.Connect(grid, distanceLocation.Cell1, distanceLocation.Cell2, template);

                finalRegions = grid.IdentifyRegions();
            }

            return new LevelGrid(grid, regions.ToArray(), new RegionModel[] { });
        }

        /// <summary>
        /// Creates walls on the boundary of the regions and connectors by checking for null cells
        /// </summary>
        private void CreateWalls(Cell[,] grid)
        {
            var bounds = new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1));

            // Iterate - leaving a padding of 1 on the edge to create cells for walls
            //           without stepping out of bounds
            //
            for (int i = 1; i < bounds.CellWidth - 1; i++)
            {
                for (int j = 1; j < bounds.CellHeight - 1; j++)
                {
                    if (grid[i, j] == null)
                        continue;

                    if (grid[i, j].IsWall || grid[i, j].IsDoor)
                        continue;

                    // North wall
                    if (grid.Get(i, j - 1) == null)
                        grid[i, j - 1] = new Cell(new GridLocation(i, j - 1), true);

                    // South wall
                    if (grid.Get(i, j + 1) == null)
                        grid[i, j + 1] = new Cell(new GridLocation(i, j + 1), true);

                    // West wall
                    if (grid.Get(i - 1, j) == null)
                        grid[i - 1, j] = new Cell(new GridLocation(i - 1, j), true);

                    // East wall
                    if (grid.Get(i + 1, j) == null)
                        grid[i + 1, j] = new Cell(new GridLocation(i + 1, j), true);

                    // North-East wall
                    if (grid.Get(i + 1, j - 1) == null)
                        grid[i + 1, j - 1] = new Cell(new GridLocation(i + 1, j - 1), true);

                    // South-East wall
                    if (grid.Get(i + 1, j + 1) == null)
                        grid[i + 1, j + 1] = new Cell(new GridLocation(i + 1, j + 1), true);

                    // North-West wall
                    if (grid.Get(i - 1, j - 1) == null)
                        grid[i - 1, j - 1] = new Cell(new GridLocation(i - 1, j - 1), true);

                    // South-West wall
                    if (grid.Get(i - 1, j + 1) == null)
                        grid[i - 1, j + 1] = new Cell(new GridLocation(i - 1, j + 1), true);
                }
            }
        }
        #endregion
    }
}
