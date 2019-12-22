using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using static Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface.IMazeRegionCreator;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IConnectionBuilder))]
    public class ConnectionBuilder : IConnectionBuilder
    {
        readonly IMazeRegionCreator _mazeRegionCreator;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IRegionTriangulationCreator _regionTriangulationCreator;

        [ImportingConstructor]
        public ConnectionBuilder(IMazeRegionCreator mazeRegionCreator,
                                 IRandomSequenceGenerator randomSequenceGenerator,
                                 IRegionTriangulationCreator regionTriangulationCreator)
        {
            _mazeRegionCreator = mazeRegionCreator;
            _randomSequenceGenerator = randomSequenceGenerator;
            _regionTriangulationCreator = regionTriangulationCreator;
        }

        public void BuildConnections(GridCellInfo[,] grid, IEnumerable<Region<GridCellInfo>> regions, LayoutTemplate template)
        {
            BuildConnectionsWithAvoidRegions(grid, regions, new Region<GridCellInfo>[] { }, template);
        }

        public void BuildConnectionsWithAvoidRegions(GridCellInfo[,] grid, IEnumerable<Region<GridCellInfo>> regions, IEnumerable<Region<GridCellInfo>> avoidRegions, LayoutTemplate template)
        {
            if (!PreValidateRegions(regions))
                throw new Exception("Invalid region layout in the grid - ConnectionBuilder.BuildCorridorsWithAvoidRegions");

            switch (template.ConnectionType)
            {
                case LayoutConnectionType.Corridor:
                    ConnectUsingShortestPath(grid, regions, avoidRegions, template);
                    break;
                case LayoutConnectionType.ConnectionPoints:
                    CreateConnectionPoints(grid, regions, template);
                    break;
                case LayoutConnectionType.Maze:
                    {
                        // Use "Filled" rule for rectangular regions only. "Open" maze rule works better with non-rectangular regions.
                        switch (template.Type)
                        {
                            case LayoutType.RectangularRegion:
                            case LayoutType.RandomRectangularRegion:
                            case LayoutType.CellularAutomataMap:
                            case LayoutType.ElevationMap:
                            case LayoutType.RandomSmoothedRegion:
                                CreateMazeCorridors(grid, regions, avoidRegions, MazeType.Filled, template);
                                break;
                            case LayoutType.MazeMap:
                            case LayoutType.CellularAutomataMazeMap:
                            case LayoutType.ElevationMazeMap:
                            default:
                                throw new Exception("Unhandled or Unsupported Layout Type for maze connections");
                        }
                    }
                    break;
                default:
                    throw new Exception("Unhandled Connection Type");
            }
        }

        // Credit to this fellow for the idea for maze corridors!
        //
        // https://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/
        // https://github.com/munificent/hauberk/blob/db360d9efa714efb6d937c31953ef849c7394a39/lib/src/content/dungeon.dart
        //
        private void CreateMazeCorridors(GridCellInfo[,] grid, IEnumerable<Region<GridCellInfo>> regions, IEnumerable<Region<GridCellInfo>> avoidRegions, MazeType mazeType, LayoutTemplate template)
        {
            // Procedure
            //
            // - (Pre-Validated) Identify regions to pass to the maze generator (avoids these when removing walls)
            // - Fill in empty cells with walls
            // - Create mazes where there are 8-way walls surrounding a cell
            // - Since maze generator removes walls (up to the edge of the avoid regions)
            //   Must add back just those walls adjacent to the avoid regions
            //
            // - Then, BuildCorridors(...) will connect any mazes together that were interrupted
            //   (Also, by the adding back of walls)
            //

            // Leave room for a wall border around the outside
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // Skip region cells
                    if (grid[i, j] != null)
                        continue;

                    // Add walls in the negative space - leaving room for region cells
                    grid[i, j] = new GridCellInfo(i, j) { IsWall = true, IsCorridor = false };
                }
            }

            // Create a recursive-backtrack corridor in cell that contains 8-way walls. Continue until entire map is
            // considered.

            // Find empty regions and fill them with recrusive-backtracked corridors
            //
            // NOTE*** Avoiding edges that WERE created as walls - because the "Filled" rule won't touch those.
            //
            for (int i = 1; i < grid.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < grid.GetLength(1) - 1; j++)
                {
                    // Create a corridor where all adjacent cells are walls
                    if (grid.GetAdjacentElements(i, j).All(cell => cell.IsWall))
                    {
                        // Create the maze!
                        _mazeRegionCreator.CreateCellsStartingAt(grid, regions.Union(avoidRegions), grid[i, j].Location, mazeType, template.MazeWallRemovalRatio, template.MazeHorizontalVerticalBias);
                    }
                }
            }

            // Add back walls surrounding the regions
            foreach (var region in regions)
            {
                foreach (var edgeLocation in region.EdgeLocations)
                {
                    var adjacentCells = grid.GetAdjacentElements(edgeLocation.Column, edgeLocation.Row);

                    foreach (var cell in adjacentCells)
                    {
                        // Adjacent cell is not in region; but is on the edge. This should
                        // be a wall cell
                        if (region[cell.Location.Column, cell.Location.Row] == null)
                        {
                            cell.IsWall = true;
                            cell.IsCorridor = false;
                        }
                    }
                }
            }

            // Re-identify regions to connect
            var newRegions = grid.IdentifyRegions(cell => !cell.IsWall);

            // Finally, connect the regions using shortest path
            ConnectUsingShortestPath(grid, newRegions, avoidRegions, template);
        }

        private void CreateConnectionPoints(GridCellInfo[,] grid, IEnumerable<Region<GridCellInfo>> regions, LayoutTemplate template)
        {
            // Check for no regions
            if (regions.Count() == 0)
                throw new Exception("No regions found ConnectionBuilder.BuildConnectionPoints");

            // Check for a single region
            if (regions.Count() == 1)
                return;

            // Create triangulation of the regions for connecting them
            var graph = _regionTriangulationCreator.CreateTriangulation(regions, template);

            // Create mandatory connection points between rooms
            foreach (var edge in graph.Edges)
            {
                // Get random cells from the two regions
                var region1Index = _randomSequenceGenerator.Get(0, edge.Point1.Reference.Locations.Length);
                var region2Index = _randomSequenceGenerator.Get(0, edge.Point2.Reference.Locations.Length);

                // Get cells from the array ~ O(1)
                var location1 = edge.Point1.Reference.Locations[region1Index];
                var location2 = edge.Point2.Reference.Locations[region2Index];

                // Set cells to mandatory
                grid[location1.Column, location1.Row].IsMandatory = true;
                grid[location1.Column, location1.Row].MandatoryType = LayoutMandatoryLocationType.RoomConnector1;

                grid[location2.Column, location2.Row].IsMandatory = true;
                grid[location2.Column, location2.Row].MandatoryType = LayoutMandatoryLocationType.RoomConnector2;
            }
        }

        private void ConnectUsingShortestPath(GridCellInfo[,] grid, IEnumerable<Region<GridCellInfo>> regions, IEnumerable<Region<GridCellInfo>> avoidRegions, LayoutTemplate template)
        {
            // Procedure
            //
            // 1) Triangulate region positions
            // 2) Use the graph edges to create shortest paths using avoid regions
            //

            // Triangulate room positions - DOES NOT TAKE INTO ACCOUNT AVOID REGIONS
            //
            var graph = _regionTriangulationCreator.CreateTriangulation(regions, template);

            // For each edge in the triangulation - create a corridor
            foreach (var edge in graph.Edges)
            {
                var location1 = edge.Point1.Reference.GetConnectionPoint(edge.Point2.Reference);
                var location2 = edge.Point1.Reference.GetAdjacentConnectionPoint(edge.Point2.Reference);

                // Create a Dijkstra path generator to find paths for the edge
                var dijkstraMap = new DijkstraPathGenerator(grid, avoidRegions, location1, new GridCellInfo[] { location2 }, true);

                // Embed path cells using callback to set properties
                dijkstraMap.EmbedPaths(new DijkstraPathGenerator.DijkstraEmbedPathCallback(cell =>
                {
                    cell.IsWall = false;
                    cell.IsCorridor = true;
                }));
            }
        }

        private bool PreValidateRegions(IEnumerable<Region<GridCellInfo>> regions)
        {
            // Validate room regions
            var invalidRoomRegions = regions.Where(region => !RegionValidator.ValidateRoomRegion(region));

            // Check for a valid room region
            if (invalidRoomRegions.Count() > 0)
                return false;

            return true;
        }
    }
}
