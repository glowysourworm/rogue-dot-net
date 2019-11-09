using Rogue.NET.Common.Extension;
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

        public LevelGrid CreateLayout(LayoutTemplate template)
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
            var grid = new GridCellInfo[template.Width, template.Height];

            // Create the room rectangles
            var roomBoundaries = RectangularGridRegionGeometryCreator.CreateRegionGeometry(template);

            // Create cells in the contiguous rectangle groups (TODO:TERRAIN - PROVIDE REGION NAME)
            var regions = roomBoundaries.Select(boundary => GridUtility.CreateRectangularRegion("Room " + Guid.NewGuid().ToString(), grid, boundary, true))
                                        .Actualize();

            return FinishLayoutRectilinear(grid, regions, template);
        }

        private LevelGrid CreateRandomRooms(LayoutTemplate template)
        {
            var grid = new GridCellInfo[template.Width, template.Height];

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
            var grid = new GridCellInfo[template.Width, template.Height];

            // Create cells in the contiguous rectangle groups
            var regions = CellularAutomataRegionCreator.CreateRegions(grid, template.CellularAutomataType == LayoutCellularAutomataType.Filled, template.CellularAutomataFillRatio.Clip(0.4, 0.4));

            return FinishLayout(grid, regions, template);
        }

        private LevelGrid CreateMaze(LayoutTemplate template)
        {
            var grid = new GridCellInfo[template.Width, template.Height];

            // TODO:TERRAIN - PROVIDE UNIQUE REGION NAME
            var region = GridUtility.CreateRectangularRegion("Region " + Guid.NewGuid().ToString(), grid, 0, 0, template.Width, template.Height, true);

            // Set region cells to be walls
            foreach (var cell in region.Cells)
                grid[cell.Column, cell.Row].IsWall = true;

            // Create maze by "punching out walls"
            //
            MazeRegionCreator.CreateMaze(grid, template.NumberExtraWallRemovals);

            return new LevelGrid(grid, new RegionModel[] { region }, new RegionModel[] { });
        }

        private LevelGrid CreateOpenWorldLayout(LayoutTemplate template)
        {
            template.Width = 80;
            template.Height = 50;

            var grid = new GridCellInfo[template.Width, template.Height];

            // To avoid extra iteration - use the callback to set up the grid cells
            var featureMap = NoiseGenerator.GeneratePerlinNoise(template.Width, template.Height, 0.06, new NoiseGenerator.PostProcessingFilterCallback((column, row, value) =>
            {
                // Use the loop to create the grid and save an iteration - mark valleys as "Walls" to be carved out later
                grid[column, row] = new GridCellInfo(column, row)
                {
                    IsWall = value >= 0
                };

                // Create "walls" for regions by weighting the result to prevent BFS from using these cells
                // return value > 0 ? 10000 : value;

                return value;
            }));

            var regions = grid.IdentifyRegions().Where(x => x.Cells.Length >= 4);

            // Create MST
            var minimumSpanningTree = GeometryUtility.PrimsMinimumSpanningTree(regions, Metric.MetricType.Roguian);

            // Create connections by trying to "follow the valleys"
            foreach (var edge in minimumSpanningTree.Edges)
            {
                var region1 = edge.Point1.Reference;
                var region2 = edge.Point2.Reference;

                var location1 = region1.GetConnectionPoint(region2, Metric.MetricType.Roguian);
                var location2 = region1.GetAdjacentConnectionPoint(region2, Metric.MetricType.Roguian);

                // Creates dijkstra
                var dijkstraMap = featureMap.CreateDijkstraMap(location1, location2);

                // Generate Path locations
                var path = dijkstraMap.GeneratePath(location1, location2, true);

                // Add path to the grid
                foreach (var location in path)
                    grid[location.Column, location.Row].IsWall = false;

            }

            return new LevelGrid(grid, regions.ToArray(), new RegionModel[] { });
        }

        #region (private) Layout Finishing

        /// <summary>
        /// Triangulate rooms, locate and remove small rooms, create corridors, add walls
        /// </summary>
        /// <returns></returns>
        private LevelGrid FinishLayout(GridCellInfo[,] grid, IEnumerable<RegionModel> regions, LayoutTemplate template)
        {
            // Triangulate room positions
            //
            var graph = GeometryUtility.PrimsMinimumSpanningTree(regions, Metric.MetricType.Roguian);

            // For each edge in the triangulation - create a corridor
            //
            foreach (var edge in graph.Edges)
            {
                var location1 = edge.Point1.Reference.GetConnectionPoint(edge.Point2.Reference, Metric.MetricType.Roguian);
                var location2 = edge.Point1.Reference.GetAdjacentConnectionPoint(edge.Point2.Reference, Metric.MetricType.Roguian);

                var cell1 = grid.Get(location1.Column, location1.Row);
                var cell2 = grid.Get(location2.Column, location2.Row);

                // Create the corridor cells
                //
                CorridorLayoutRegionConnector.Connect(grid, cell1, cell2, template);
            }

            //Create walls
            CreateWalls(grid);

            return new LevelGrid(grid, regions.ToArray(), new RegionModel[] { });
        }

        private LevelGrid FinishLayoutRectilinear(GridCellInfo[,] grid, IEnumerable<RegionModel> regions, LayoutTemplate template)
        {
            // Create MST
            var minimumSpanningTree = GeometryUtility.PrimsMinimumSpanningTree(regions, Metric.MetricType.Roguian);

            // Create connections by drawing wide linear connector
            foreach (var edge in minimumSpanningTree.Edges)
            {
                var includedPoints = new List<GridLocation>();

                var left = (int)System.Math.Min(edge.Point1.Vertex.Column, edge.Point2.Vertex.Column);
                var right = (int)System.Math.Max(edge.Point1.Vertex.Column, edge.Point2.Vertex.Column);
                var top = (int)System.Math.Min(edge.Point1.Vertex.Row, edge.Point2.Vertex.Row);
                var bottom = (int)System.Math.Max(edge.Point1.Vertex.Row, edge.Point2.Vertex.Row);
                var vertices = new GridLocation[]
                {
                    new GridLocation(left, top),
                    new GridLocation(right, top),
                    new GridLocation(right, bottom),
                    new GridLocation(left, bottom)
                };

                // Add points that are part of one of the rooms
                foreach (var vertex in vertices)
                {
                    if (edge.Point1.Reference.Bounds.Contains(vertex) ||
                        edge.Point2.Reference.Bounds.Contains(vertex))
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
                                                                               edge.Point1.Vertex, 
                                                                               midPoint, 
                                                                               edge.Point1.Vertex.Row != midPoint.Row);

                    TilingCorridorRegionConnector.CreateRectilinearRoutePoints(grid, 
                                                                               midPoint, 
                                                                               edge.Point2.Vertex, 
                                                                               edge.Point2.Vertex.Row != midPoint.Row);
                }

                // Otherwise, just draw a line from one region to the other
                else
                {
                    // NOTE*** Since all vertices lie within both regions - just draw a straight line connecting
                    //         one of the off-diagonal vertices to the opposing center
                    var northSouthOriented = edge.Point1.Reference.Bounds.Bottom < edge.Point2.Reference.Bounds.Top ||
                                             edge.Point1.Reference.Bounds.Top > edge.Point2.Reference.Bounds.Bottom;

                    // Point1 -> Point 2 (off-diangonal or the actual center)
                    TilingCorridorRegionConnector.CreateRectilinearRoutePoints(grid, 
                                                                               edge.Point1.Vertex,
                                                                               edge.Point2.Vertex, 
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
        private LevelGrid FinishLayoutWithMazeCorridors(GridCellInfo[,] grid, IEnumerable<RegionModel> regions, LayoutTemplate template)
        {
            // Fill in the empty cells with walls
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // Create wall cell here
                    if (grid[i, j] == null)
                        grid[i, j] = new GridCellInfo(i, j)
                        {
                            IsWall = true
                        };
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

        private LevelGrid FinishLayoutWithTerrain(GridCellInfo[,] grid, IEnumerable<RegionModel> regions, LayoutTemplate template)
        {
            // Create terrain grid to use to identify regions
            var terrainGrid = new GridCellInfo[grid.GetLength(0), grid.GetLength(1)];

            // Create terrain map to use in Dijkstra's algorithm for path routing
            var terrainMap = NoiseGenerator.GeneratePerlinNoise(grid.GetLength(0), grid.GetLength(1), 0.07, (column, row, value) =>
            {
                // Procedure
                //
                // 1) Create terrain cells using a threshold (value < 0) (INSIDE ROOMS)
                // 2) Set the noise value very high for terrain and region cells
                // 3) Set the grid for terrain cells to null

                if (value < 0 && grid[column, row] != null)
                {
                    // Replace grid cell for terrain cell
                    terrainGrid[column, row] = new GridCellInfo(column, row) { IsWall = false };
                    grid[column, row] = null;

                    return 1000;
                }

                return 0;
            });

            // Create terrain regions
            var terrainRegions = terrainGrid.IdentifyRegions();

            // Re-calculate grid regions
            var rooms = grid.IdentifyRegions();

            // Triangulate room positions
            var graph = GeometryUtility.PrimsMinimumSpanningTree(rooms, Metric.MetricType.Roguian);

            // For each edge in the triangulation - create a corridor
            foreach (var edge in graph.Edges)
            {
                var location1 = edge.Point1.Reference.GetConnectionPoint(edge.Point2.Reference, Metric.MetricType.Roguian);
                var location2 = edge.Point1.Reference.GetAdjacentConnectionPoint(edge.Point2.Reference, Metric.MetricType.Roguian);

                // Creates dijkstra
                var dijkstraMap = terrainMap.CreateDijkstraMap(location1, location2);

                //terrainMap.OutputCSV("c:\\test\\terrainMap.csv");
                //dijkstraMap.OutputCSV("c:\\test\\dijkstraMap.csv");


                // Generate Path locations
                var path = dijkstraMap.GeneratePath(location1, location2, true);

                // Add path to the grid
                foreach (var location in path)
                    grid[location.Column, location.Row] = new GridCellInfo(location) { IsWall = false };
            }

            //Create walls
            CreateWalls(grid);

            return new LevelGrid(grid, rooms.ToArray(), terrainRegions.ToArray());
        }

        /// <summary>
        /// Creates walls on the boundary of the regions and connectors by checking for null cells
        /// </summary>
        private void CreateWalls(GridCellInfo[,] grid)
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
                        grid[i, j - 1] = new GridCellInfo(new GridLocation(i, j - 1)) { IsWall = true };

                    // South wall
                    if (grid.Get(i, j + 1) == null)
                        grid[i, j + 1] = new GridCellInfo(new GridLocation(i, j + 1)) { IsWall = true };

                    // West wall
                    if (grid.Get(i - 1, j) == null)
                        grid[i - 1, j] = new GridCellInfo(new GridLocation(i - 1, j)) { IsWall = true };

                    // East wall
                    if (grid.Get(i + 1, j) == null)
                        grid[i + 1, j] = new GridCellInfo(new GridLocation(i + 1, j)) { IsWall = true };

                    // North-East wall
                    if (grid.Get(i + 1, j - 1) == null)
                        grid[i + 1, j - 1] = new GridCellInfo(new GridLocation(i + 1, j - 1)) { IsWall = true };

                    // South-East wall
                    if (grid.Get(i + 1, j + 1) == null)
                        grid[i + 1, j + 1] = new GridCellInfo(new GridLocation(i + 1, j + 1)) { IsWall = true };

                    // North-West wall
                    if (grid.Get(i - 1, j - 1) == null)
                        grid[i - 1, j - 1] = new GridCellInfo(new GridLocation(i - 1, j - 1)) { IsWall = true };

                    // South-West wall
                    if (grid.Get(i - 1, j + 1) == null)
                        grid[i - 1, j + 1] = new GridCellInfo(new GridLocation(i - 1, j + 1)) { IsWall = true };
                }
            }
        }
        #endregion
    }
}
