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

            return FinishLayout(grid, regions, template);
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

            return FinishLayout(grid, regions, template);
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

        #region (private) Layout Finishing

        /// <summary>
        /// Triangulate rooms, locate and remove small rooms, create corridors, add walls
        /// </summary>
        /// <returns></returns>
        private LevelGrid FinishLayout(Cell[,] grid, IEnumerable<RegionModel> regions, LayoutTemplate template)
        {
            // Triangulate their positions (Delaunay Triangulation)
            //
            var triangulation = GeometryUtility.PrimsMinimumSpanningTree(regions.Select(x => new Vertex(x.Bounds.Center.Column, x.Bounds.Center.Row))
                                                                                .Actualize());

            // For each edge in the triangulation - create a corridor
            //
            foreach (var edge in triangulation)
            {
                var room1 = regions.First(x => x.Bounds.Center.Column == (int)edge.Point1.X &&
                                               x.Bounds.Center.Row == (int)edge.Point1.Y);

                var room2 = regions.First(x => x.Bounds.Center.Column == (int)edge.Point2.X &&
                                               x.Bounds.Center.Row == (int)edge.Point2.Y);

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

            //var tiledRegions = new List<RegionModel>();

            //// REMOVE THIS:  Add Connecting Regions to see them laid out
            //var connectingRegions = TiledMeshRegionGeometryCreator.CreateConnectingRegions(grid.GetLength(0), grid.GetLength(1), regions.Select(x => x.Bounds));

            //// Add cells for each connecting region - but don't line with walls
            //foreach (var region in connectingRegions)
            //    tiledRegions.Add(GridUtility.CreateRectangularRegion(grid, region, true, false));

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
                    Distance = RogueCalculator.EuclideanSquareDistance(cell1, cell2),
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
