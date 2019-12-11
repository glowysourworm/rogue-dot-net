﻿using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Math.Algorithm;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
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
        readonly ICorridorCreator _corridorCreator;
        readonly IMazeRegionCreator _mazeRegionCreator;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IRegionValidator _regionValidator;

        [ImportingConstructor]
        public ConnectionBuilder(ICorridorCreator corridorCreator,
                               IMazeRegionCreator mazeRegionCreator,
                               IRandomSequenceGenerator randomSequenceGenerator,
                               IRegionValidator regionValidator)
        {
            _corridorCreator = corridorCreator;
            _mazeRegionCreator = mazeRegionCreator;
            _randomSequenceGenerator = randomSequenceGenerator;
            _regionValidator = regionValidator;
        }

        public void BuildConnections(GridCellInfo[,] grid, IEnumerable<Region> regions, LayoutTemplate template)
        {
            BuildConnectionsWithAvoidRegions(grid, regions, new Region[] { }, template);
        }

        public void BuildConnectionsWithAvoidRegions(GridCellInfo[,] grid, IEnumerable<Region> regions, IEnumerable<Region> avoidRegions, LayoutTemplate template)
        {
            if (!PreValidateRegions(regions))
                throw new Exception("Invalid region layout in the grid - ConnectionBuilder.BuildCorridorsWithAvoidRegions");

            switch (template.ConnectionType)
            {
                case LayoutConnectionType.Corridor:
                    ConnectUsingShortestPath(grid, regions, avoidRegions, template);
                    break;
                case LayoutConnectionType.Teleporter:
                    CreateConnectionPoints(grid, regions);
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
        private void CreateMazeCorridors(GridCellInfo[,] grid, IEnumerable<Region> regions, IEnumerable<Region> avoidRegions, MazeType mazeType, LayoutTemplate template)
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
                    grid[i, j] = new GridCellInfo(i, j) { IsWall = true };
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
                foreach (var edgeCell in region.EdgeCells)
                {
                    var adjacentCells = grid.GetAdjacentElements(edgeCell.Column, edgeCell.Row);

                    foreach (var cell in adjacentCells)
                    {
                        // Adjacent cell is not in region; but is on the edge. This should
                        // be a wall cell
                        if (region[cell.Location.Column, cell.Location.Row] == null)
                            cell.IsWall = true;
                    }
                }
            }

            // Re-identify regions to connect
            var newRegions = grid.IdentifyRegions();

            // TODO:TERRAIN - Validate resulting regions
            //if (!PreValidateRegions(grid, out newRegions))
            //    throw new Exception("Maze Generation left no valid regions");

            // Finally, connect the regions using MST -> Linear connectors
            ConnectUsingShortestPath(grid, newRegions, avoidRegions, template);
        }

        private void CreateConnectionPoints(GridCellInfo[,] grid, IEnumerable<Region> regions)
        {
            // Check for no regions
            if (regions.Count() == 0)
                throw new Exception("No regions found ConnectionBuilder.BuildConnectionPoints");

            // Check for a single region
            if (regions.Count() == 1)
                return;

            // Create mandatory connection points between rooms
            for (int i = 0; i < regions.Count() - 1; i++)
            {
                // Get random cells from the two regions
                var region1Index = _randomSequenceGenerator.Get(0, regions.ElementAt(i).Cells.Length);
                var region2Index = _randomSequenceGenerator.Get(0, regions.ElementAt(i + 1).Cells.Length);

                // Get cells from the array ~ O(1)
                var location1 = regions.ElementAt(i).Cells[region1Index];
                var location2 = regions.ElementAt(i + 1).Cells[region2Index];

                // Set cells to mandatory
                grid[location1.Column, location1.Row].IsMandatory = true;
                grid[location1.Column, location1.Row].MandatoryType = LayoutMandatoryLocationType.RoomConnector1;

                grid[location2.Column, location2.Row].IsMandatory = true;
                grid[location2.Column, location2.Row].MandatoryType = LayoutMandatoryLocationType.RoomConnector2;
            }

            // Connect the first and last regions
            var firstIndex = _randomSequenceGenerator.Get(0, regions.First().Cells.Length);
            var lastIndex = _randomSequenceGenerator.Get(0, regions.Last().Cells.Length);

            // Get cells from the array ~ O(1)
            var firstLocation = regions.First().Cells[firstIndex];
            var lastLocation = regions.Last().Cells[lastIndex];

            // Set cells to mandatory
            grid[firstLocation.Column, firstLocation.Row].IsMandatory = true;
            grid[firstLocation.Column, firstLocation.Row].MandatoryType = LayoutMandatoryLocationType.RoomConnector1;

            grid[lastLocation.Column, lastLocation.Row].IsMandatory = true;
            grid[lastLocation.Column, lastLocation.Row].MandatoryType = LayoutMandatoryLocationType.RoomConnector2;
        }

        private void ConnectUsingShortestPath(GridCellInfo[,] grid, IEnumerable<Region> regions, IEnumerable<Region> avoidRegions, LayoutTemplate template)
        {
            // Procedure
            //
            // 1) Identify Regions
            // 2) Create cost map of terrain 
            // 3) Create region triangulation
            // 4) Use Dijkstra's algorithm to connect the regions with the cost map
            //

            // Create cost map from the regions
            var costMap = CreateRegionCostMap(grid, regions.Union(avoidRegions));

            // Triangulate room positions
            var graph = GeometryUtility.PrimsMinimumSpanningTree(regions, Metric.MetricType.Euclidean);

            // For each edge in the triangulation - create a corridor
            foreach (var edge in graph.Edges)
            {
                var location1 = edge.Point1.Reference.GetConnectionPoint(edge.Point2.Reference, Metric.MetricType.Euclidean);
                var location2 = edge.Point1.Reference.GetAdjacentConnectionPoint(edge.Point2.Reference, Metric.MetricType.Euclidean);

                var dijkstraMap = new DijkstraMap(costMap, location1, location2, true);

                if (!dijkstraMap.Run())
                {
                    dijkstraMap.OutputCSV(ResourceConstants.DijkstraOutputDirectory,
                                          template.Name +
                                          " DijkstraMap_Edge_" +
                                          edge.Point1.Vertex.Column + "_" +
                                          edge.Point1.Vertex.Row + " to " +
                                          edge.Point2.Vertex.Column + "_" +
                                          edge.Point2.Vertex.Row);

                    throw new Exception("Invalid Dijkstra Map (see output directory)");
                }

                foreach (var location in dijkstraMap.GeneratePath())
                {
                    // Maze levels can have walls set up to punch through
                    if (grid[location.Column, location.Row] != null)
                    {
                        grid[location.Column, location.Row].IsWall = false;
                        grid[location.Column, location.Row].IsCorridor = true;
                    }

                    // Empty cells need to be filled in
                    else
                        grid[location.Column, location.Row] = new GridCellInfo(location)
                        {
                            IsCorridor = true
                        };
                }
            }
        }

        private bool PreValidateRegions(IEnumerable<Region> regions)
        {
            // Validate room regions
            var invalidRoomRegions = regions.Where(region => !_regionValidator.ValidateRoomRegion(region));

            // Check for a valid room region
            if (invalidRoomRegions.Count() > 0)
                return false;

            return true;
        }

        /// <summary>
        /// Creates cost map for the grid using pre-identified regions
        /// </summary>
        private double[,] CreateRegionCostMap(GridCellInfo[,] grid, IEnumerable<Region> regions)
        {
            var costMap = new double[grid.GetLength(0), grid.GetLength(1)];

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // Cell Costs:
                    //
                    // - If grid cell is empty (null) then it has zero cost as long as it doesn't belong to any region.
                    // - A cell could be a wall left over from generating a maze - so allow walls to be zero cost.
                    // - Impassible terrain or region features will have a high cost
                    //
                    if ((grid[i, j] == null || grid[i, j].IsWall) && 
                        !regions.Any(region => region[i, j] != null))
                        costMap[i, j] = 0;

                    else
                        costMap[i, j] = DijkstraMap.RegionFeatureConstant;
                }
            }

            return costMap;
        }
    }
}
